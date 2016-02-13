/* This Source Code Form is subject to the terms of the Mozilla internal
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using NetMQ.Sockets;

namespace NetMQ.Zyre
{
    public class ZyreNode : IDisposable
    {
        private const int ZreDiscoveryPort = 5670; // IANA-assigned
        private const byte BeaconVersion = 0x1;
        private const byte UbyteMax = byte.MaxValue;

        /// <summary>
        /// Lock for the Dump() method so we get all of it at once.
        /// </summary>
        private static readonly object DumpLock = new object();

        #region Private Variables

        /// <summary>
        /// Pipe back to application
        /// ReceiveAPI() receives messages from the API and sends command replies and signals via the pipe
        /// </summary>
        private PairSocket _pipe;

        /// <summary>
        /// Outbox back to application
        /// We send all Zyre messages to the API via the outbox, e.g. from ReceivePeer(), Start(), Stop(), 
        /// </summary>
        private PairSocket _outbox;

        /// <summary>
        /// Beacon port number
        /// </summary>
        private readonly int _beaconPort;

        /// <summary>
        /// Beacon interval
        /// </summary>
        private TimeSpan _interval;

        /// <summary>
        /// Socket poller
        /// </summary>
        private NetMQPoller _poller;

        /// <summary>
        /// Beacon
        /// </summary>
        private NetMQBeacon _beacon;

        /// <summary>
        /// Our UUID (guid), 16 bytes when transmitted. 
        /// Used in the beacon message and as the Identity for the _inbox.
        /// Created in ctor for each new node.
        /// </summary>
        private Guid _uuid;

        /// <summary>
        /// Our _inbox socket (ROUTER).
        /// ReceivePeer() receives messages from peers into _inbox.
        /// ZRE messages are sent from this _mailbox for each peer
        ///    and they are received by the ZreNode's RouterSocket _inbox.
        /// </summary>
        private readonly RouterSocket _inbox;

        /// <summary>
        /// Our internal name. 
        /// Default is first 6 characters of _uuid.
        /// </summary>
        private string _name;

        /// <summary>
        /// Our internal endpoint, the endpoint corresponding to _inbox.
        /// </summary>
        private string _endpoint;

        /// <summary>
        /// Our _inbox port, if any. The port to which _inbox is bound.
        /// </summary>
        private int _port;

        /// <summary>
        /// Our own change counter
        /// </summary>
        private byte _status;

        /// <summary>
        /// Hash of known peers, fast lookup. Key is _uuid
        /// </summary>
        private readonly Dictionary<Guid, ZyrePeer> _peers;

        /// <summary>
        /// Groups that our peers are in. Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZyreGroup> _peerGroups;

        /// <summary>
        /// Groups that we are in.  Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZyreGroup> _ownGroups;

        /// <summary>
        /// Our header values
        /// </summary>
        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// The actor used to communicate all control messages to and from Zyre
        /// </summary>
        private readonly NetMQActor _actor;

        /// <summary>
        /// Optional logger action passed into ctor
        /// </summary>
        private readonly Action<string> _loggerDelegate;

        /// <summary>
        /// True when Start() has finished, False when Stop() has finished.
        /// </summary>
        private bool _isRunning;

        #endregion Private Variables

        /// <summary>
        /// Create a new node and return the actor that controls it.
        /// All node control is done through _actor.
        /// outbox is passed to ZreNode for sending Zyre message traffic back to caller.
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="loggerDelegate">An action to take for logging when _verbose is true. Default is null.</param>
        /// <returns></returns>
        internal static NetMQActor Create(PairSocket outbox, Action<string> loggerDelegate = null)
        {
            var node = new ZyreNode(outbox, loggerDelegate);
            return node._actor;
        }

        private ZyreNode(PairSocket outbox, Action<string> loggerDelegate = null)
        {
            _outbox = outbox;
            _loggerDelegate = loggerDelegate;

            _inbox = new RouterSocket();

            //  Use ZMQ_ROUTER_HANDOVER so that when a peer disconnects and
            //  then reconnects, the new client connection is treated as the
            //  canonical one, and any old trailing commands are discarded.
            // TODO: This RouterHandover option apparently doesn't exist in NetMQ 
            //      so I IGNORE it for now. DaleBrubaker Feb 1 2016

            _beaconPort = ZreDiscoveryPort;
            _interval = TimeSpan.Zero; // Use default
            _uuid = Guid.NewGuid();
            _peers = new Dictionary<Guid, ZyrePeer>();
            _peerGroups = new Dictionary<string, ZyreGroup>();
            _ownGroups = new Dictionary<string, ZyreGroup>();
            _headers = new Dictionary<string, string>();

            //  Default name for node is first 6 characters of UUID:
            //  the shorter string is more readable in logs
            _name = _uuid.ToShortString6();

            _actor = NetMQActor.Create(RunActor);
        }

        /// <summary>
        /// Start node. Use beacon discovery
        /// </summary>
        /// <returns>true if OK, false if not possible or if already running</returns>
        private bool Start()
        {
            if (_isRunning)
            {
                return false;
            }
            Debug.Assert(_beacon == null);
            _beacon = new NetMQBeacon();
            _beacon.Configure(_beaconPort);

            // listen to incoming beacons
            _beacon.ReceiveReady += OnBeaconReady;

            // Bind our router port to the host
            var address = $"tcp://{_beacon.BoundTo}";
            _port = _inbox.BindRandomPort(address);
            if (_port <= 0)
            {
                // Die on bad interface or port exhaustion
                return false;
            }
            _endpoint = $"{address}:{_port}";
            _loggerDelegate?.Invoke($"Beacon for {_uuid.ToShortString6()} is going out. The _inbox RouterSocket is bound to _endpoint={_endpoint}");

            //  Set broadcast/listen beacon
            PublishBeacon(_port);
            _beacon.Subscribe("ZRE");
            _poller.Add(_beacon);

            // Start polling on inbox
            _inbox.ReceiveReady += OnInboxReady;
            _poller.Add(_inbox);
            _isRunning = true;
            return true;
        }

        private void OnInboxReady(object sender, NetMQSocketEventArgs e)
        {
            ReceivePeer();
        }

        private void OnBeaconReady(object sender, NetMQBeaconEventArgs e)
        {
            ReceiveBeacon();
        }

        /// <summary>
        /// Stop node discovery and interconnection
        /// </summary>
        /// <returns></returns>
        private void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            // Stop broadcast/listen beacon
            PublishBeacon(0);
            Thread.Sleep(1); // Allow 1 millisecond for beacon to go out
            _beacon.ReceiveReady -= OnBeaconReady;
            _poller.Remove(_beacon);
            _beacon.Dispose();
            _beacon = null;

            // Stop polling on inbox
            _inbox.ReceiveReady -= OnInboxReady;
            _poller.Remove(_inbox);

            // Tell the application we are stopping
            var msg = new NetMQMessage(3);
            msg.Append("STOP");
            msg.Append(_uuid.ToByteArray());
            msg.Append(_name);
            _outbox.TrySendMultipartMessage(TimeSpan.Zero, msg);
            _isRunning = false;
        }

        /// <summary>
        /// Given a ZRE beacon header, return 
        /// </summary>
        /// <param name="bytes">the ZRE 22-byte beacon header</param>
        /// <param name="uuid">The peer's identity</param>
        /// <param name="port">The peer's port</param>
        /// <returns></returns>
        private bool IsValidBeacon(byte[] bytes, out Guid uuid, out int port)
        {
            uuid = Guid.Empty;
            port = int.MinValue;
            if (bytes.Length != 22)
            {
                return false;
            }
            if (bytes[0] != Convert.ToByte('Z') || bytes[1] != Convert.ToByte('R') || bytes[2] != Convert.ToByte('E') || bytes[3] != BeaconVersion)
            {
                return false;
            }
            var uuidBytes = new byte[16];
            Buffer.BlockCopy(bytes, 4, uuidBytes, 0, 16);
            uuid = new Guid(uuidBytes);
            var portBytes = new byte[2];
            Buffer.BlockCopy(bytes, 20, portBytes, 0, 2);
            port = (bytes[20] << 8) + bytes[21];
            return true;
        }

        /// <summary>
        /// Beacon 22-byte message per http://rfc.zeromq.org/spec:36
        /// </summary>
        /// <param name="port">the port can be _port (normal) or 0 (stopping)</param>
        /// <returns></returns>
        private byte[] BeaconMessage(int port)
        {
            var transmit = new byte[22];
            transmit[0] = Convert.ToByte('Z');
            transmit[1] = Convert.ToByte('R');
            transmit[2] = Convert.ToByte('E');
            transmit[3] = BeaconVersion;
            var uuidBytes = _uuid.ToByteArray();
            Buffer.BlockCopy(uuidBytes, 0, transmit, 4, 16);
            transmit[20] = (byte)((port >> 8) & 255);
            transmit[21] = (byte)(port & 255);
            return transmit;
        }

        private void PublishBeacon(int port)
        {
            var transmit = BeaconMessage(port);
            if (_interval == TimeSpan.Zero)
            {
                // Use default
                _beacon.Publish(transmit);
            }
            else
            {
                _beacon.Publish(transmit, _interval);
            }
        }

        /// <summary>
        /// Send message to one peer
        /// </summary>
        /// <param name="peer">The peer to get msg</param>
        /// <param name="msg">the message to send</param>
        private void SendMessageToPeer(ZyrePeer peer, ZreMsg msg)
        {
            peer.Send(msg);
        }

        /// <summary>
        /// Send message to all peers
        /// </summary>
        /// <param name="msg">the message to send</param>
        private void SendPeers(ZreMsg msg)
        {
            foreach (var peer in _peers.Values)
            {
                SendMessageToPeer(peer, msg);
            }
        }

        private void OnPipeReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            ReceiveApi();
        }

        /// <summary>
        /// Here we handle the different control messages from the front-end
        /// </summary>
        private void ReceiveApi()
        {
            // Get the whole message off the pipe in one go
            var request = _pipe.ReceiveMultipartMessage();
            var command = request.Pop().ConvertToString();
            switch (command)
            {
                case "UUID":
                    _pipe.SendFrame(_uuid.ToByteArray());
                    break;
                case "NAME":
                    _pipe.SendFrame(_name);
                    break;
                case "ENDPOINT":
                    _pipe.SendFrame(_endpoint ?? "");
                    break;
                case "SET NAME":
                    _name = request.Pop().ConvertToString();
                    Debug.Assert(!string.IsNullOrEmpty(_name));
                    break;
                case "SET HEADER":
                    var key = request.Pop().ConvertToString();
                    var value = request.Pop().ConvertToString();
                    _headers[key] = value;
                    break;
                case "SET PORT":
                    var str = request.Pop().ConvertToString();
                    int.TryParse(str, out _port);
                    break;
                case "SET INTERVAL":
                    var intervalStr = request.Pop().ConvertToString();
                    TimeSpan.TryParse(intervalStr, out _interval);
                    break;
                case "START":
                    Start();
                    break;
                case "STOP":
                    Stop();
                    break;
                case "WHISPER":
                    // Get peer to send message to
                    var uuid = PopGuid(request);
                    ZyrePeer peer;
                    if (_peers.TryGetValue(uuid, out peer))
                    {
                        //  Send frame on out to peer's mailbox, drop message
                        //  if peer doesn't exist (may have been destroyed)
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Whisper,
                            Whisper = {Content = request}
                        };
                        peer.Send(msg);
                    }
                    break;
                case "SHOUT":
                    // Get group to send message to
                    var groupNameShout = request.Pop().ConvertToString();
                    ZyreGroup group;
                    if (_ownGroups.TryGetValue(groupNameShout, out group))
                    {
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Shout,
                            Shout = {Content = request}
                        };
                        group.Send(msg);
                    }
                    break;
                case "JOIN":
                    var groupNameJoin = request.Pop().ConvertToString();
                    ZyreGroup groupJoin;
                    if (!_ownGroups.TryGetValue(groupNameJoin, out groupJoin))
                    {
                        // Only send if we're not already in group
                        // Update status before sending command
                        _status = _status == UbyteMax ? (byte)0 : ++_status;
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Join,
                            Join =
                            {
                                Group = groupNameJoin,
                                Status = _status
                            }
                        };
                        foreach (var peerJoin in _peers.Values)
                        {
                            peerJoin.Send(msg);
                        }
                    }
                    break;
                case "LEAVE":
                    var groupNameLeave = request.Pop().ConvertToString();
                    ZyreGroup groupLeave;
                    if (_ownGroups.TryGetValue(groupNameLeave, out groupLeave))
                    {
                        // Only send if we are actually in group
                        // Update status before sending command
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Leave,
                            Join =
                            {
                                Group = groupNameLeave,
                                Status = _status
                            }
                        };
                        foreach (var peerLeave in _peers.Values)
                        {
                            peerLeave.Send(msg);
                        }
                        _ownGroups.Remove(groupNameLeave);
                    }
                    break;
                case "PEERS":
                    // Send the list of the _peers keys
                    var peersKeyBuffer = Serialization.BinarySerialize(_peers.Keys.ToList());
                    _pipe.SendFrame(peersKeyBuffer);
                    break;
                case "PEER ENDPOINT":
                    var uuidForEndpoint = PopGuid(request);
                    var peerForEndpoint = _peers[uuidForEndpoint]; // throw exception if not found
                    _pipe.SendFrame(peerForEndpoint.Endpoint);
                    break;
                case "PEER NAME":
                    var uuidForName = PopGuid(request);
                    var peerForName = _peers[uuidForName]; // throw exception if not found
                    _pipe.SendFrame(peerForName.Name);
                    break;
                case "PEER HEADER":
                    var uuidForHeader = PopGuid(request);
                    var keyForHeader = request.Pop().ConvertToString();
                    ZyrePeer peerForHeader;
                    if (_peers.TryGetValue(uuidForHeader, out peerForHeader))
                    {
                        string header;
                        _headers.TryGetValue(keyForHeader, out header);
                        _pipe.SendFrame(header ?? "");
                    }
                    else
                    {
                        _pipe.SendFrame("");
                    }
                    break;
                case "PEER GROUPS":
                    // Send a list of the _peerGroups keys, comma-delimited
                    var peerGroupsKeyBuffer = Serialization.BinarySerialize(_peerGroups.Keys.ToList());
                    _pipe.SendFrame(peerGroupsKeyBuffer);
                    break;
                case "OWN GROUPS":
                    // Send a list of the _ownGroups keys, comma-delimited
                    var ownGroupsKeyBuffer = Serialization.BinarySerialize(_ownGroups.Keys.ToList());
                    _pipe.SendFrame(ownGroupsKeyBuffer);
                    break;
                case "DUMP":
                    Dump();
                    break;
                case NetMQActor.EndShimMessage:
                    // API shut us down
                    if (_poller != null)
                    {
                        _poller.Stop();
                    }
                    break;
                default:
                    throw new ArgumentException(command);
            }
        }

        /// <summary>
        /// Utility to read a Guid from a message.
        /// We transmit 16 bytes that define the Uuid.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static Guid PopGuid(NetMQMessage message)
        {
            var bytes = message.Pop().ToByteArray();
            Debug.Assert(bytes.Length == 16);
            var uuid = new Guid(bytes);
            return uuid;
        }

        /// <summary>
        /// Delete peer for a given endpoint
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="endpoint"></param>
        private void PurgePeer(ZyrePeer peer, string endpoint)
        {
            if (peer.Endpoint == endpoint)
            {
                peer.Disconnect();
            }
        }

        /// <summary>
        /// Find or create peer via its UUID
        /// </summary>
        /// <param name="uuid">the identity of peer</param>
        /// <param name="endpoint">the endpoint to which we will connect the new peer</param>
        /// <returns>A peer (existing, or new one connected to endpoint)</returns>
        private ZyrePeer RequirePeer(Guid uuid, string endpoint)
        {
            Debug.Assert(!string.IsNullOrEmpty(endpoint));
            ZyrePeer peer;
            if (_peers.TryGetValue(uuid, out peer))
            {
                return peer;
            }

            // Purge any previous peer on same endpoint
            foreach (var existingPeer in _peers.Values)
            {
                PurgePeer(existingPeer, endpoint);
            }
            peer = ZyrePeer.NewPeer(_peers, uuid, _loggerDelegate);
            peer.SetOrigin(_name);
            peer.Connect(_uuid, endpoint);

            // Handshake discovery by sending HELLO as first message
            var helloMessage = new ZreMsg
            {
                Id = ZreMsg.MessageId.Hello,
                Hello =
                {
                    Endpoint = _endpoint,
                    Groups = _ownGroups.Keys.ToList(),
                    Status = _status,
                    Name = _name,
                    Headers = _headers
                }
            };
            _loggerDelegate?.Invoke($"({_name}) RequirePeer() created new peer {peer}. Sending Hello message={helloMessage}");
            peer.Send(helloMessage);
            return peer;
        }

        /// <summary>
        /// Remove peer from group, if it's a member
        /// </summary>
        /// <param name="group"></param>
        /// <param name="peer"></param>
        private void DeletePeer(ZyreGroup group, ZyrePeer peer)
        {
            group.Leave(peer);
        }

        /// <summary>
        /// Remove a peer from our data structures
        /// </summary>
        /// <param name="peer"></param>
        private void RemovePeer(ZyrePeer peer)
        {
            // Tell the calling application the peer has gone
            _outbox.SendMoreFrame("EXIT").SendMoreFrame(peer.Uuid.ToByteArray()).SendFrame(peer.Name);
            _loggerDelegate?.Invoke($"({_name} EXIT name={peer.Name} endpoint={peer.Endpoint}");

            // Remove peer from any groups we've got it in
            foreach (var peerGroup in _peerGroups.Values)
            {
                DeletePeer(peerGroup, peer);
            }

            // To destroy peer, we remove from peers hash table
            _peers.Remove(peer.Uuid);
        }

        /// <summary>
        /// Find or create group via its name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private ZyreGroup RequirePeerGroup(string groupName)
        {
            ZyreGroup group;
            if (!_peerGroups.TryGetValue(groupName, out group))
            {
                group = ZyreGroup.NewGroup(groupName, _peerGroups);
            }
            return group;
        }

        /// <summary>
        /// Join peer to group
        /// </summary>
        /// <param name="peer">The peer that is joining thie group</param>
        /// <param name="groupName">The name of the group to join</param>
        /// <returns>the group joined</returns>
        private ZyreGroup JoinPeerGroup(ZyrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Join(peer);

            // Now tell the caller about the peer joined group
            _outbox.SendMoreFrame("JOIN").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(groupName);
            _loggerDelegate?.Invoke($"({_name} JOIN name={peer.Name} group={groupName}");
           return group;
        }

        /// <summary>
        /// Have peer leave group
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private ZyreGroup LeavePeerGroup(ZyrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Leave(peer);

            // Now tell the caller about the peer left group
            _outbox.SendMoreFrame("LEAVE").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(groupName);
            _loggerDelegate?.Invoke($"({_name} LEAVE name={peer.Name} group={groupName}");
            return group;
        }

        /// <summary>
        /// Here we handle messages coming from other peers
        /// </summary>
        private void ReceivePeer()
        {
            Guid uuid;
            var msg = ZreMsg.ReceiveNew(_inbox, out uuid);
            if (msg == null)
            {
                // Ignore a bad message (header or message signature doesn't meet http://rfc.zeromq.org/spec:36)
                return;
            }
            Debug.Assert(uuid != _uuid, $"({_name}) Our own message should not be coming back to us! {_uuid}");
            ZyrePeer peer;
            if (!_peers.TryGetValue(uuid, out peer))
            {
                _loggerDelegate?.Invoke($"Received Unknown Peer into _inbox uuid={uuid.ToShortString6()}");
                if (msg.Id == ZreMsg.MessageId.Hello)
                {
                    _loggerDelegate?.Invoke($"endPoint of the Unknown Peer={msg.Hello.Endpoint}");
                }
            }
            if (msg.Id == ZreMsg.MessageId.Hello)
            {
                // On HELLO we may create the peer if it's unknown
                // On other commands the peer must already exist
                if (peer != null)
                {
                    // Remove fake peers
                    if (peer.Ready)
                    {
                        RemovePeer(peer);
                        Debug.Assert(!_peers.ContainsKey(uuid));
                    }
                    else if (peer.Endpoint == _endpoint)
                    {
                        // We ignore HELLO, if peer has same endpoint as current node
                        return;
                    }
                }
                peer = RequirePeer(uuid, msg.Hello.Endpoint);
                peer.SetReady(true);
            }
            if (peer == null || !peer.Ready)
            {
                // Ignore command if peer isn't ready
                return;
            }
            _loggerDelegate?.Invoke($"({_name}) ZreNode.ReceivePeer() received message={msg} from peer={peer} ");
            if (peer.MessagesLost(msg))
            {
                _loggerDelegate?.Invoke($"({_name}) MessagesLost! ZreNode.ReceivePeer() ignoring message={msg} from peer={peer} ");
                RemovePeer(peer);
                return;
            }

            // Now process each command
            NetMQMessage outMsg; // message we'll send to _outbox
            switch (msg.Id)
            {
                case ZreMsg.MessageId.Hello:
                    // Store properties from HELLO command into peer
                    var helloMessage = msg.Hello;
                    peer.SetName(helloMessage.Name);
                    peer.SetHeaders(helloMessage.Headers);

                    // Tell the caller about the peer
                    outMsg = new NetMQMessage();
                    outMsg.Append("ENTER");
                    outMsg.Append(peer.Uuid.ToByteArray());
                    outMsg.Append(peer.Name);
                    var headersBuffer = Serialization.BinarySerialize(_headers);
                    outMsg.Append(headersBuffer);
                    outMsg.Append(helloMessage.Endpoint);
                    _outbox.SendMultipartMessage(outMsg);
                    _loggerDelegate?.Invoke($"({_name} ENTER name={peer.Name} endpoint={peer.Endpoint}");

                    // Join peer to listed groups
                    foreach (var groupName in helloMessage.Groups)
                    {
                        JoinPeerGroup(peer, groupName);
                    }

                    // Now take peer's status from HELLO, after joining groups
                    peer.SetStatus(helloMessage.Status);
                    break;
                case ZreMsg.MessageId.Whisper:
                    // Pass up to caller API as WHISPER event
                    //outMsg = new NetMQMessage();
                    //outMsg.Append("WHISPER");
                    //outMsg.Append(uuid.ToByteArray());
                    //outMsg.Append(peer.Name);
                    //for (int i = 0; i < msg.Whisper.Content.FrameCount; i++)
                    //{
                    //    outMsg.Append(msg.Whisper.Content[i]);
                    //}
                    //_outbox.SendMultipartMessage(outMsg);

                    // TODO Check this method instead
                    _outbox.SendMoreFrame("WHISPER").SendMoreFrame(uuid.ToByteArray()).SendMoreFrame(peer.Name).SendMultipartMessage(msg.Whisper.Content);

                    break;
                case ZreMsg.MessageId.Shout:
                    // Pass up to caller API as SHOUT event
                    outMsg = new NetMQMessage();
                    outMsg.Append("SHOUT");
                    outMsg.Append(uuid.ToByteArray());
                    outMsg.Append(peer.Name);
                    outMsg.Append(msg.Shout.Group);
                    for (int i = 0; i < msg.Shout.Content.FrameCount; i++)
                    {
                        outMsg.Append(msg.Shout.Content[i]);
                    }
                    _outbox.SendMultipartMessage(outMsg);
                    // TODO: DO this like Whisper above?
                    break;
                case ZreMsg.MessageId.Join:
                    JoinPeerGroup(peer, msg.Join.Group);
                    Debug.Assert(msg.Join.Status == peer.Status);
                    break;
                case ZreMsg.MessageId.Leave:
                    LeavePeerGroup(peer, msg.Leave.Group);
                    Debug.Assert(msg.Leave.Status == peer.Status);
                    break;
                case ZreMsg.MessageId.Ping:
                    break;
                case ZreMsg.MessageId.PingOk:
                    Debug.Fail("Unexpected");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Activity from peer resets peer timers
            peer.Refresh();
        }

        /// <summary>
        /// Handle beacon data
        /// </summary>
        private void ReceiveBeacon()
        {
            // Get IP address and beacon of peer
            var beaconMessage = _beacon.Receive();

            // Ignore anything that isn't a valid beacon
            int port;
            Guid uuid;
            if (!IsValidBeacon(beaconMessage.Bytes, out uuid, out port))
            {
                return;
            }
            var endPoint = $"tcp://{beaconMessage.PeerHost}:{port}";
            ZyrePeer peer;
            if (port > 0)
            {
                peer = RequirePeer(uuid, endPoint);
                peer.Refresh();
            }
            else
            {
                // Zero port means peer is going away; remove it if we had any knowledge of it already
                _loggerDelegate?.Invoke($"Removing peer {uuid.ToShortString6()} due to zero port received from {endPoint}");
                if (_peers.TryGetValue(uuid, out peer))
                {
                    RemovePeer(peer);
                }
            }
        }

        /// <summary>
        /// We do this once a second:
        /// - if peer has gone quiet, send TCP ping and emit EVASIVE event
        /// - if peer has disappeared, expire it
        /// </summary>
        /// <param name="peer">the peer to ping</param>
        /// <returns>true if this peer should be removed</returns>
        private bool PingPeer(ZyrePeer peer)
        {
            if (ZyrePeer.CurrentTimeMilliseconds() >= peer.ExpiredAt)
            {
                return true;
            }
            if (ZyrePeer.CurrentTimeMilliseconds() >= peer.EvasiveAt)
            {
                // If peer is being evasive, force a TCP ping.
                // ZeroMQTODO: do this only once for a peer in this state;
                // it would be nicer to use a proper state machine
                // for peer management.
                _loggerDelegate?.Invoke($"({_name} peer seems dead/slow name={peer.Name} endpoint={peer.Endpoint}");
                ZreMsg.SendPing(_outbox, 0);

                // Inform the calling application this peer is being evasive
                _outbox.SendMoreFrame("EVASIVE");
                _outbox.SendMoreFrame(peer.Uuid.ToByteArray());
                _outbox.SendFrame(peer.Name);
            }
            return false;
        }

        private void Dump()
        {
            if (_loggerDelegate == null)
            {
                return;
            }

            lock (DumpLock)
            {
                _loggerDelegate("zyre_node: dump state");
                _loggerDelegate($" - name={_name} uuidShort={_uuid.ToShortString6()} uuid={_uuid}");
                _loggerDelegate($" - endpoint={_endpoint}");
                _loggerDelegate($" - discovery=beacon port={_beaconPort} interval={_interval}");
                _loggerDelegate($" - headers={_headers.Count}");
                foreach (var header in _headers)
                {
                    _loggerDelegate($"key={header.Key} value={header.Value}");
                }
                _loggerDelegate($" - peers={_peers.Count}");
                foreach (var peer in _peers.Values)
                {
                    _loggerDelegate($"peer={peer}");
                }
                _loggerDelegate($" - ownGroups={_ownGroups.Count}");
                foreach (var group in _ownGroups.Values)
                {
                    _loggerDelegate($"ownGroup={@group}");
                }
                _loggerDelegate($" - peerGroups={_peerGroups.Count}");
                foreach (var group in _peerGroups.Values)
                {
                    _loggerDelegate($"peerGroup={@group}");
                }
            }
        }

        public override string ToString()
        {
            return $"name:{_name} router endpoint:{_endpoint} status:{_status}";
        }

        /// <summary>
        /// Release any contained resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release any contained resources.
        /// </summary>
        /// <param name="disposing">true if managed resources are to be released</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_isRunning)
            {
                Stop();
            }
            _poller?.Stop();
            _poller?.Dispose();
            _beacon?.Dispose();
            _inbox?.Dispose();
            _outbox?.Dispose();
            foreach (var peer in _peers.Values)
            {
                peer.Destroy();
            }
            foreach (var group in _peerGroups.Values)
            {
                group.Dispose();
            }
            foreach (var group in _ownGroups.Values)
            {
                group.Dispose();
            }
        }

        /// <summary>
        /// This method is being run asynchronously by m_actor.
        /// </summary>
        /// <param name="shim"></param>
        private void RunActor(PairSocket shim)
        {
            _pipe = shim;
            _pipe.ReceiveReady += OnPipeReceiveReady;

            var reapTimer = new NetMQTimer(TimeSpan.FromMilliseconds(1000));
            reapTimer.Elapsed += OnReapTimerElapsed;

            // Start poller, but poll only the _pipe. Start() and Stop() will add/remove other items to poll
            _poller = new NetMQPoller { _pipe, reapTimer };

            // Signal the actor that we're ready to work
            _pipe.SignalOK();

            // polling until cancelled
            _poller.Run();

            reapTimer.Enable = false;
            reapTimer.Elapsed -= OnReapTimerElapsed;
        }

        private void OnReapTimerElapsed(object sender, NetMQTimerEventArgs e)
        {
            // Ping all peers and reap any expired ones
            // Don't remove them during the foreach loop
            var peersToRemove = new List<ZyrePeer>();
            foreach (var peer in _peers.Values)
            {
                var isToBeRemoved = PingPeer(peer);
                if (isToBeRemoved)
                {
                    peersToRemove.Add(peer);
                }
            }
            foreach (var peer in peersToRemove)
            {
                RemovePeer(peer);
            }
        }
    }

}
