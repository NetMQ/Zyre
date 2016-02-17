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
        private const byte UbyteMax = Byte.MaxValue;

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
        private readonly PairSocket _outbox;

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
        /// ZRE messages are to this _mailbox by each peer
        /// </summary>
        private RouterSocket _inbox;

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
        /// outbox is passed to ZyreNode for sending Zyre message traffic back to caller.
        /// </summary>
        /// <param name="outbox"></param>
        /// <param name="loggerDelegate">An action to take for logging when _verbose is true. Default is null.</param>
        /// <returns>the _actor, or null if not successful</returns>
        internal static NetMQActor Create(PairSocket outbox, Action<string> loggerDelegate = null)
        {
            var node = new ZyreNode(outbox, loggerDelegate);

            return node._actor;
        }

        private ZyreNode(PairSocket outbox, Action<string> loggerDelegate = null)
        {
            _outbox = outbox;
            _loggerDelegate = loggerDelegate;

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

            //  Use ZMQ_ROUTER_HANDOVER so that when a peer disconnects and
            //  then reconnects, the new client connection is treated as the
            //  canonical one, and any old trailing commands are discarded.
            //  This RouterHandover option is currently not supported in NetMQ Feb 16 2016
            
            _actor = NetMQActor.Create(RunActor);
        }

        private void OnInboxReady(object sender, NetMQSocketEventArgs e)
        {
            try
            {
                ReceivePeer();
            }
            catch (Exception ex)
            {
                _loggerDelegate?.Invoke(ex.ToString());
            }
        }

        private void OnBeaconReady(object sender, NetMQBeaconEventArgs e)
        {
            ReceiveBeacon();
        }

        /// <summary>
        /// Start node. Use beacon discovery.
        /// We get a new _inbox (RouterSocket listening to peers) and a new _beacon on every Start().
        /// Also a new _port and _endpoint at each Start()
        /// </summary>
        /// <returns>true if OK, false if not possible or if already running</returns>
        private bool Start()
        {
            if (_isRunning)
            {
                return false;
            }

            // Create the _beacon and bind the _inbox
            _beacon = new NetMQBeacon();
            _beacon.ReceiveReady += OnBeaconReady;
            _beacon.Configure(_beaconPort);

            // Bind our router port to the host. Our hostName is provided by the beacon.
            var address = $"tcp://{_beacon.BoundTo}";
            _inbox = new RouterSocket();
            _inbox.ReceiveReady += OnInboxReady;
            _port = _inbox.BindRandomPort(address);
            if (_port <= 0)
            {
                // Die on bad interface or port exhaustion
                return false;
            }
            _endpoint = $"{address}:{_port}";
            _loggerDelegate?.Invoke($"The _inbox RouterSocket for node {_uuid.ToShortString6()} is bound to _endpoint={_endpoint}");

            _loggerDelegate?.Invoke($"Starting {_name} {_uuid.ToShortString6()}. Publishing beacon on port {_port}. Adding _beacon and _inbox to poller.");

            // Start polling on _inbox and _beacon
            _poller.Add(_inbox);
            _poller.Add(_beacon);
            Thread.Sleep(100); // wait for poller so we don't miss messages

            //  Set broadcast/listen beacon
            PublishBeacon(_port);
            _beacon.Subscribe("ZRE");

            _isRunning = true;
            return true;
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
            _loggerDelegate?.Invoke($"Stopping {_name} {_uuid.ToShortString6()}. Publishing beacon on port 0. Removing _beacon and _inbox from poller.");

            // Stop broadcast/listen beacon by broadcasting port 0
            PublishBeacon(0);
            Thread.Sleep(1); // Allow 1 millisecond for beacon to go out
            _poller.Remove(_beacon);
            _beacon.ReceiveReady -= OnBeaconReady;
            _beacon.Unsubscribe();
            _beacon.Dispose();
            _beacon = null;

            // Stop polling on inbox
            _poller.Remove(_inbox);
            _inbox.ReceiveReady -= OnInboxReady;
            _inbox.Dispose();


            // Tell the application we are stopping
            var msg = new NetMQMessage(3);
            msg.Append("STOP");
            msg.Append(_uuid.ToByteArray());
            msg.Append(_name);
            _outbox.SendMultipartMessage(msg);
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
            port = Int32.MinValue;
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
                    Debug.Assert(!String.IsNullOrEmpty(_name));
                    break;
                case "SET HEADER":
                    var key = request.Pop().ConvertToString();
                    var value = request.Pop().ConvertToString();
                    _headers[key] = value;
                    break;
                case "SET PORT":
                    var str = request.Pop().ConvertToString();
                    Int32.TryParse(str, out _port);
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
                    if (_peerGroups.TryGetValue(groupNameShout, out group))
                    {
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Shout,
                            Shout = {Group = groupNameShout, Content = request}
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
                        var groupJoined = ZyreGroup.NewGroup(groupNameJoin, _ownGroups);

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
                        _status = _status == UbyteMax ? (byte)0 : ++_status;
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Leave,
                            Leave =
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
                    _poller?.Stop();
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

        private readonly HashSet<ZyrePeer> _reportedKnownPeersTmp = new HashSet<ZyrePeer>();
        private bool _isRequirePeerRunning;

        /// <summary>
        /// Find or create peer via its UUID
        /// </summary>
        /// <param name="uuid">the identity of peer</param>
        /// <param name="endpoint">the endpoint to which we will connect the new peer</param>
        /// <returns>A peer (existing, or new one connected to endpoint)</returns>
        private ZyrePeer RequirePeer(Guid uuid, string endpoint)
        {
            if (_isRequirePeerRunning)
            {
                _loggerDelegate?.Invoke($"Entry into {nameof(ZyreNode)}.{nameof(RequirePeer)}() while it's running. uuid={uuid.ToShortString6()}");
            }
            _isRequirePeerRunning = true;
            Debug.Assert(!String.IsNullOrEmpty(endpoint));
            ZyrePeer peer;
            if (_peers.TryGetValue(uuid, out peer))
            {
                if (!_reportedKnownPeersTmp.Contains(peer))
                {
                    _loggerDelegate?.Invoke($"{nameof(ZyreNode)}.{nameof(RequirePeer)}() returning already-known peer={peer}");
                    _reportedKnownPeersTmp.Add(peer);
                }
                _isRequirePeerRunning = false;
                return peer;
            }

            // Purge any previous peer on same endpoint
            foreach (var existingPeer in _peers.Values)
            {
                PurgePeer(existingPeer, endpoint);
            }
            peer = ZyrePeer.NewPeer(_peers, uuid, _loggerDelegate);
            peer.Origin = _name;
            peer.Connect(_uuid, endpoint);
            Thread.Sleep(100); // allow some time so we don't lose messages

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
            _loggerDelegate?.Invoke($"{nameof(ZyreNode)}.{nameof(RequirePeer)}() created new peer uuid={uuid}. Sending Hello message...");
            peer.Send(helloMessage);
            _loggerDelegate?.Invoke($"TMP {nameof(ZyreNode)}.{nameof(RequirePeer)}() created new peer uuid={uuid}. SENT Hello message...");
            var callingMethod = MethodNameLevelAbove();
            _loggerDelegate?.Invoke($"TMP Called by {callingMethod}");
            _isRequirePeerRunning = false;
            return peer;
        }

        /// <summary>
        /// Return a string showing the class name and method of method calling the method calling this method, used for reporting errors
        /// </summary>
        /// <returns></returns>
        public static string MethodNameLevelAbove()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(2);
            var mb = stackFrame.GetMethod();

            // ReSharper disable once PossibleNullReferenceException
            return mb.ReflectedType.Name + ":" + mb.Name + "()";
        }

        /// <summary>
        /// Remove peer from group, if it's a member
        /// </summary>
        /// <param name="group"></param>
        /// <param name="peer"></param>
        private void RemovePeerFromGroup(ZyreGroup group, ZyrePeer peer)
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
            _loggerDelegate?.Invoke($"EXIT name={peer.Name} endpoint={peer.Endpoint}");

            // Remove peer from any groups we've got it in
            foreach (var peerGroup in _peerGroups.Values)
            {
                RemovePeerFromGroup(peerGroup, peer);
            }

            _peers.Remove(peer.Uuid);
            peer.Destroy();
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
        /// <param name="peer">The peer that is joining this group</param>
        /// <param name="groupName">The name of the group to join</param>
        /// <returns>the group joined</returns>
        private void JoinPeerGroup(ZyrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Join(peer);

            // Now tell the caller about the peer joined group
            _outbox.SendMoreFrame("JOIN").SendMoreFrame(peer.Uuid.ToByteArray()).SendMoreFrame(peer.Name).SendFrame(groupName);
            _loggerDelegate?.Invoke($"JOIN name={peer.Name} group={groupName}");
        }

        /// <summary>
        /// Have peer leave group
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private void LeavePeerGroup(ZyrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Leave(peer);

            // Now tell the caller about the peer left group
            _outbox.SendMoreFrame("LEAVE").SendMoreFrame(peer.Uuid.ToByteArray()).SendMoreFrame(peer.Name).SendFrame(groupName);
            _loggerDelegate?.Invoke($"LEAVE name={peer.Name} group={groupName}");
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
                _loggerDelegate?.Invoke("Ignoring a bad message (header or message signature doesn't meet http://rfc.zeromq.org/spec:36).");
                return;
            }
            if (uuid == _uuid)
            {
                var text = $"({_name}) Our own message should not be coming back to us! {_uuid}";
                _loggerDelegate?.Invoke(text);
                throw new InvalidOperationException(text);
            }
            _loggerDelegate?.Invoke($"{nameof(ZyreNode)}.{nameof(ReceivePeer)}() received message={msg}");
            Debug.Assert(uuid != _uuid, $"({_name}) Our own message should not be coming back to us! uuid={_uuid}");
            ZyrePeer peer;
            if (!_peers.TryGetValue(uuid, out peer))
            {
                _loggerDelegate?.Invoke($"Peer {uuid.ToShortString6()} is unknown.");
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
                        _loggerDelegate?.Invoke("Removing fake peer={peer} because we received another HELLO from the same uuid.");
                        RemovePeer(peer);
                        Debug.Assert(!_peers.ContainsKey(uuid));
                    }
                    else if (peer.Endpoint == _endpoint)
                    {
                        // We ignore HELLO, if peer has same endpoint as current node
                        _loggerDelegate?.Invoke("Ignoring HELLO for peer that has same endpoint as current node.");
                        return;
                    }
                }
                peer = RequirePeer(uuid, msg.Hello.Endpoint);
                //_loggerDelegate?.Invoke($"TMP Did {nameof(RequirePeer)}");
                peer.Ready = true;
            }
            if (peer == null)
            {
                _loggerDelegate?.Invoke("Ignoring null peer");
                return;
            }
            if (!peer.Ready)
            {
                // Ignore command if peer isn't ready
                _loggerDelegate?.Invoke($"Ignoring peer that isn't ready: {peer}");
                return;
            }
            if (peer.MessagesLost(msg))
            {
                _loggerDelegate?.Invoke($"MessagesLost! {nameof(ZyreNode)}.{nameof(ReceivePeer)}() ignoring message={msg} and removing peer={peer} ");
                RemovePeer(peer);
                return;
            }

            // Now process each command
            _loggerDelegate?.Invoke($"{nameof(ZyreNode)}.{nameof(ReceivePeer)}() is now ready to process this {msg.Id} command.");
            switch (msg.Id)
            {
                case ZreMsg.MessageId.Hello:
                    // Store properties from HELLO command into peer
                    var helloMessage = msg.Hello;
                    peer.Name = helloMessage.Name;
                    peer.Headers = helloMessage.Headers;

                    // Tell the caller about the peer
                    var headersBuffer = Serialization.BinarySerialize(peer.Headers);
                    _outbox.SendMoreFrame("ENTER").SendMoreFrame(peer.Uuid.ToByteArray()).SendMoreFrame(peer.Name).SendMoreFrame(headersBuffer).SendMoreFrame(helloMessage.Endpoint);
                    _loggerDelegate?.Invoke($"ENTER name={peer.Name} endpoint={peer.Endpoint}");

                    // Join peer to listed groups
                    foreach (var groupName in helloMessage.Groups)
                    {
                        JoinPeerGroup(peer, groupName);
                    }

                    // Now take peer's status from HELLO, after joining groups
                    peer.Status = helloMessage.Status;
                    _loggerDelegate?.Invoke($"Hello message has been processed for peer: {peer}");
                    break;
                case ZreMsg.MessageId.Whisper:
                    // Pass up to caller API as WHISPER event
                    _outbox.SendMoreFrame("WHISPER").SendMoreFrame(uuid.ToByteArray()).SendMoreFrame(peer.Name).SendMultipartMessage(msg.Whisper.Content);
                    break;
                case ZreMsg.MessageId.Shout:
                    // Pass up to caller API as SHOUT event
                    _outbox.SendMoreFrame("SHOUT").SendMoreFrame(uuid.ToByteArray()).SendMoreFrame(peer.Name).SendMoreFrame(msg.Shout.Group).SendMultipartMessage(msg.Shout.Content);
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
            //_loggerDelegate?.Invoke($"TMP Leaving {nameof(ReceivePeer)}");
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
            var endpoint = $"tcp://{beaconMessage.PeerHost}:{port}";
            ZyrePeer peer;
            if (port > 0)
            {
                //_loggerDelegate?.Invoke($"TMP {nameof(ReceiveBeacon)} is doing RequirePeer() for endpoint={endpoint}");
                peer = RequirePeer(uuid, endpoint);
                //_loggerDelegate?.Invoke($"TMP {nameof(ReceiveBeacon)} DID RequirePeer() for endpoint={endpoint}");
                peer.Refresh();
            }
            else
            {
                // Zero port means peer is going away; remove it if we had any knowledge of it already
                if (_peers.TryGetValue(uuid, out peer))
                {
                    _loggerDelegate?.Invoke($"Removing peer {uuid.ToShortString6()} due to zero port received from {endpoint}");
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
            if (!_isRunning)
            {
                // We have been stopped. We know longer can communicate to peers.
                return true;
            }
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
                _loggerDelegate?.Invoke($"Peer seems dead/slow: name={peer.Name} endpoint={peer.Endpoint}");
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
            if (_poller.IsRunning)
            {
                _poller?.Stop();
            }
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

            Dispose();
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
