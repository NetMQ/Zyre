/* This Source Code Form is subject to the terms of the Mozilla Public
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
    public class ZreNode : IDisposable
    {
        private const int ZreDiscoveryPort = 5670; // IANA-assigned
        private const byte BeaconVersion = 0x1;
        private const byte UbyteMax = byte.MaxValue;

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
        /// API shut us down
        /// </summary>
        private bool _terminated;


        /// <summary>
        /// Beacon port number
        /// </summary>
        private int _beaconPort;

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
        /// Our UUID (guid), 16 bytes when transmitted
        /// </summary>
        private Guid _uuid;

        /// <summary>
        /// Our inbox socket (ROUTER)
        /// </summary>
        private RouterSocket _inbox;

        /// <summary>
        /// Our public name
        /// </summary>
        private string _name;

        /// <summary>
        /// Our public endpoint
        /// </summary>
        private string _endpoint;

        /// <summary>
        /// Our inbox port, if any
        /// </summary>
        private int _port;

        /// <summary>
        /// Our own change counter
        /// </summary>
        private byte _status;

        /// <summary>
        /// Hash of known peers, fast lookup. Key is _uuid
        /// </summary>
        private readonly Dictionary<Guid, ZrePeer> _peers;

        /// <summary>
        /// Groups that our peers are in. Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZreGroup> _peerGroups;

        /// <summary>
        /// Groups that we are in.  Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZreGroup> _ownGroups;

        /// <summary>
        /// Our header values
        /// </summary>
        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// The actor used to communicate all control messages to and from Zyre
        /// </summary>
        private readonly NetMQActor _actor;

        /// <summary>
        /// True when Start() has finished, False when Stop() has finished.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Create a new node and return the actor that controls it.
        /// All node control is done through _actor.
        /// outbox is passed to ZreNode for sending Zyre message traffic back to caller.
        /// </summary>
        /// <param name="outbox"></param>
        /// <returns></returns>
        public static NetMQActor Create(PairSocket outbox)
        {
            var node = new ZreNode(outbox);
            return node._actor;
        }

        private ZreNode(PairSocket outbox)
        {
            _inbox = new RouterSocket();

            //  Use ZMQ_ROUTER_HANDOVER so that when a peer disconnects and
            //  then reconnects, the new client connection is treated as the
            //  canonical one, and any old trailing commands are discarded.
            // NOTE: This RouterHandover option apparently doesn't exist in NetMQ 
            //      so I IGNORE it for now. DaleBrubaker Feb 1 2016

            _outbox = outbox;
            //_beaconPort = ZreDiscoveryPort;
            _interval = TimeSpan.Zero; // Use default
            _uuid = Guid.NewGuid();
            _peers = new Dictionary<Guid, ZrePeer>();
            _peerGroups = new Dictionary<string, ZreGroup>();
            _ownGroups = new Dictionary<string, ZreGroup>();
            _headers = new Dictionary<string, string>();

            //  Default name for node is first 6 characters of UUID:
            //  the shorter string is more readable in logs
            _name = _uuid.ToString().ToUpper().Substring(0, 6);

            _actor = NetMQActor.Create(RunActor);
        }

        /// <summary>
        /// Start node. Use beacon discovery
        /// </summary>
        /// <returns>true if OK, false if not possible</returns>
        public bool Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("ZreNode is already running");
            }
            Debug.Assert(_beacon == null);
            _beacon = new NetMQBeacon();

            _beacon.Configure(ZreDiscoveryPort);

            // listen to incoming beacons
            _beacon.ReceiveReady += OnBeaconReady;


            IPAddress bindTo = null; // TODO change this once this property comes thru from NuGet = _beacon.BoundTo;
            var interfaceCollection = new InterfaceCollection();
            foreach (var @interface in interfaceCollection)
            {
                    bindTo = @interface.Address;
                    break;
            }

            // Bind our router port to the host
            var address = string.Format("tcp://{0}", bindTo);
            _port = _inbox.BindRandomPort(address);
            if (_port <= 0)
            {
                // Die on bad interface or port exhaustion
                return false;
            }
            _endpoint = _inbox.Options.LastEndpoint;

            //  Set broadcast/listen beacon
            PublishBeacon(_port);
            _beacon.Subscribe("ZRE");
            _poller.Add(_beacon);

            // Start polling on inbox
            _inbox.ReceiveReady += OnInboxReady;
            _poller.Add(_inbox);
            IsRunning = true;
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
        public bool Stop()
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("ZreNode is not running");
            }
            // Stop broadcast/listen beacon
            PublishBeacon(0);
            Thread.Sleep(1); // Allow 1 msec for beacon to go out
            _beacon.ReceiveReady -= OnBeaconReady;
            _poller.Remove(_beacon);
            _beacon = null;

            // Stop polling on inbox
            _inbox.ReceiveReady -= OnInboxReady;
            _poller.Remove(_inbox);

            // Tell the application we are stopping
            var msg = new NetMQMessage(3);
            msg.Append("STOP");
            msg.Append(_uuid.ToString());
            msg.Append(_name);
            _outbox.TrySendMultipartMessage(TimeSpan.Zero, msg);
            IsRunning = false;
            return true;
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
            uuid = new Guid(bytes);
            var portBytes = new byte[2];
            Buffer.BlockCopy(bytes, 20, portBytes, 0, 2);
            port = NetworkOrderBitsConverter.ToInt16(portBytes);
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
            var portBytes = NetworkOrderBitsConverter.GetBytes((short) port);
            Buffer.BlockCopy(portBytes, 0, transmit, 20, 2);
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
        public void SendMessageToPeer(ZrePeer peer, ZreMsg msg)
        {
            peer.Send(msg);
        }

        /// <summary>
        /// Send message to all peers
        /// </summary>
        /// <param name="msg">the message to send</param>
        public void SendPeers(ZreMsg msg)
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
        public void ReceiveApi()
        {
            // Get the whole message off the pipe in one go
            var request = _pipe.ReceiveMultipartMessage();
            var command = request.Pop().ConvertToString();
            switch (command)
            {
                case "UUID":
                    _pipe.SendFrame(_uuid.ToString());
                    break;
                case "NAME":
                    _pipe.SendFrame(_name);
                    break;
                case "SET NAME":
                    _name = request.Pop().ConvertToString();
                    Debug.Assert(!string.IsNullOrEmpty(_name));
                    break;
                case "SET HEADER":
                    var name = request.Pop().ConvertToString();
                    var value = request.Pop().ConvertToString();
                    _headers[name] = value;
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
                    ZrePeer peer;
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
                    var groupName = request.Pop().ConvertToString();
                    ZreGroup group;
                    if (_ownGroups.TryGetValue(groupName, out group))
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
                    ZreGroup groupJoin;
                    if (!_ownGroups.TryGetValue(groupNameJoin, out groupJoin))
                    {
                        // Only send if we're not already in group
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Join,
                            Join = {Group = groupNameJoin}
                        };
                        // Update status before sending command
                        IncrementStatus();
                        foreach (var peerJoin in _peers.Values)
                        {
                            peerJoin.Send(msg);
                        }
                    }
                    break;
                case "LEAVE":
                    var groupNameLeave = request.Pop().ConvertToString();
                    ZreGroup groupLeave;
                    if (_ownGroups.TryGetValue(groupNameLeave, out groupLeave))
                    {
                        // Only send if we are actually in group
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Leave,
                            Join = {Group = groupNameLeave}
                        };
                        // Update status before sending command
                        IncrementStatus();
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
                    ZrePeer peerForHeader;
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
                case "endPipe":
                case "$TERM":
                    _terminated = true;
                    if (_beacon != null)
                    {
                        _beacon.Unsubscribe();
                        _beacon.Dispose();
                        _beacon = null;
                    }
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
        /// Increment status
        /// </summary>
        public void IncrementStatus()
        {
            _status = _status == UbyteMax ? (byte) 0 : _status++;
        }

        /// <summary>
        /// Delete peer for a given endpoint
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="endpoint"></param>
        public void PurgePeer(ZrePeer peer, string endpoint)
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
        public ZrePeer RequirePeer(Guid uuid, string endpoint)
        {
            Debug.Assert(!string.IsNullOrEmpty(endpoint));
            ZrePeer peer;
            if (_peers.TryGetValue(uuid, out peer))
            {
                return peer;
            }

            // Purge any previous peer on same endpoint
            foreach (var existingPeer in _peers.Values)
            {
                PurgePeer(existingPeer, endpoint);
            }
            peer = ZrePeer.NewPeer(_peers, uuid);
            peer.SetOrigin(_name);
            peer.Connect(_uuid, _endpoint);

            // Handshake discovery by sending HELLO as first message
            var helloMessage = new ZreMsg
            {
                Id = ZreMsg.MessageId.Hello,
                Hello =
                {
                    Endpoint = endpoint,
                    Groups = _ownGroups.Keys.ToList(),
                    Status = _status,
                    Name = _name,
                    Headers = _headers
                }
            };
            peer.Send(helloMessage);
            return peer;
        }

        /// <summary>
        /// Remove peer from group, if it's a member
        /// </summary>
        /// <param name="group"></param>
        /// <param name="peer"></param>
        public void DeletePeer(ZreGroup group, ZrePeer peer)
        {
            group.Leave(peer);
        }

        /// <summary>
        /// Remove a peer from our data structures
        /// </summary>
        /// <param name="peer"></param>
        public void RemovePeer(ZrePeer peer)
        {
            // Tell the calling application the peer has gone
            _outbox.SendMoreFrame("EXIT").SendMoreFrame(peer.Uuid.ToString()).SendFrame(peer.Name);

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
        public ZreGroup RequirePeerGroup(string groupName)
        {
            ZreGroup group;
            if (!_peerGroups.TryGetValue(groupName, out group))
            {
                group = new ZreGroup(groupName);
            }
            return group;
        }

        /// <summary>
        /// Join peer to group
        /// </summary>
        /// <param name="peer">The peer that is joining thie group</param>
        /// <param name="groupName">The name of the group to join</param>
        /// <returns>the group joined</returns>
        public ZreGroup JoinPeerGroup(ZrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Join(peer);

            // Now tell the caller about the peer joined group
            _outbox.SendMoreFrame("JOIN").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(_name);
            return group;
        }

        /// <summary>
        /// Have peer leave group
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ZreGroup LeavePeerGroup(ZrePeer peer, string groupName)
        {
            var group = RequirePeerGroup(groupName);
            group.Leave(peer);

            // Now tell the caller about the peer left group
            _outbox.SendMoreFrame("LEAVE").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(_name);
            return group;
        }

        /// <summary>
        /// Here we handle messages coming from other peers
        /// </summary>
        public void ReceivePeer()
        {
            Guid uuid;
            var msg = ZreMsg.ReceiveNew(_inbox, out uuid);
            if (msg == null)
            {
                // Ignore a bad message (header or message signature doesn't meet http://rfc.zeromq.org/spec:36)
                return;
            }
            ZrePeer peer;
            _peers.TryGetValue(uuid, out peer);
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
            if (peer.MessagesLost(msg))
            {
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
        public void ReceiveBeacon()
        {
            // Get IP address and beacon of peer
            string peerName;
            var bytes = _beacon.Receive(out peerName);

            // Ignore anything that isn't a valid beacon
            int port;
            Guid uuid;
            if (!IsValidBeacon(bytes, out uuid, out port))
            {
                return;
            }
            ZrePeer peer;
            if (port > 0)
            {
                var endPoint = string.Format("tcp://{0}:{1}", peerName, port);
                peer = RequirePeer(uuid, endPoint);
                peer.Refresh();
            }
            else
            {
                // Zero port means peer is going away; remove it if we had any knowledge of it already
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
        public void PingPeer(ZrePeer peer)
        {
            if (ZrePeer.CurrentTimeMilliseconds() >= peer.ExpiredAt)
            {
                RemovePeer(peer);
            }
            else if (ZrePeer.CurrentTimeMilliseconds() >= peer.EvasiveAt)
            {
                // If peer is being evasive, force a TCP ping.
                // ZeroMQTODO: do this only once for a peer in this state;
                // it would be nicer to use a proper state machine
                // for peer management.
                ZreMsg.SendPing(_outbox, 0);

                // Inform the calling application this peer is being evasive
                _outbox.SendMoreFrame("EVASIVE");
                _outbox.SendMoreFrame(peer.Uuid.ToByteArray());
                _outbox.SendFrame(peer.Name);
            }
        }

        /// <summary>
        /// We do this once a second:
        /// - if peer has gone quiet, send TCP ping and emit EVASIVE event
        /// - if peer has disappeared, expire it
        /// </summary>
        public void PingAllPeers()
        {
            foreach (var peer in _peers.Values)
            {
                PingPeer(peer);
            }
        }

        public override string ToString()
        {
            return string.Format("name:{0} router endpoint:{1} status:{2}", _name, _endpoint, _status);
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

            if (_outbox != null)
            {
                _outbox.Dispose();
                _outbox = null;
            }
            if (_poller != null)
            {
                _poller.Stop();
                _poller.Dispose();
                _poller = null;
            }
            if (_beacon != null)
            {
                _beacon.Dispose();
                _beacon = null;
            }
            if (_inbox != null)
            {
                _inbox.Dispose();
                _inbox = null;
            }
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
            PingAllPeers();
        }
    }

}
