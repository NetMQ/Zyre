using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NetMQ;
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
        private PairSocket m_pipe;

        /// <summary>
        /// Outbox back to application
        /// We send all Zyre messages to the API via the outbox, e.g. from ReceivePeer(), Start(), Stop(), 
        /// </summary>
        private PairSocket m_outbox;

        /// <summary>
        /// API shut us down
        /// </summary>
        private bool m_terminated;


        /// <summary>
        /// Beacon port number
        /// </summary>
        private int m_beaconPort;

        /// <summary>
        /// Beacon interval
        /// </summary>
        private TimeSpan m_interval;

        /// <summary>
        /// Socket poller
        /// </summary>
        private NetMQPoller m_poller;

        /// <summary>
        /// Beacon
        /// </summary>
        private NetMQBeacon m_beacon;

        /// <summary>
        /// Our UUID (guid), 16 bytes when transmitted
        /// </summary>
        private Guid m_uuid;

        /// <summary>
        /// Our inbox socket (ROUTER)
        /// </summary>
        private RouterSocket m_inbox;

        /// <summary>
        /// Our public name
        /// </summary>
        private string m_name;

        /// <summary>
        /// Our public endpoint
        /// </summary>
        private string m_endpoint;

        /// <summary>
        /// Our inbox port, if any
        /// </summary>
        private int m_port;

        /// <summary>
        /// Our own change counter
        /// </summary>
        private byte m_status;

        /// <summary>
        /// Hash of known peers, fast lookup. Key is m_uuid
        /// </summary>
        private readonly Dictionary<Guid, ZrePeer> m_peers;

        /// <summary>
        /// Groups that our peers are in. Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZreGroup> m_peerGroups;

        /// <summary>
        /// Groups that we are in.  Key is Group name
        /// </summary>
        private readonly Dictionary<string, ZreGroup> m_ownGroups;

        /// <summary>
        /// Our header values
        /// </summary>
        private readonly Dictionary<string, string> m_headers;

        /// <summary>
        /// The actor used to communicate all control messages to and from Zyre
        /// </summary>
        private readonly NetMQActor m_actor;

        //  Beacon frame has this format:
        //
        //  Z R E       3 bytes
        //  version     1 byte, %x01
        //  UUID        16 bytes
        //  port        2 bytes in network order

        //public static NetMQActor GetActor()
        //{
        //    return NetMQActor.Create(Run); 
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pipe"></param>
        /// <param name="outbox"></param>
        public ZreNode(PairSocket pipe, PairSocket outbox)
        {
            m_inbox = new RouterSocket();

            //  Use ZMQ_ROUTER_HANDOVER so that when a peer disconnects and
            //  then reconnects, the new client connection is treated as the
            //  canonical one, and any old trailing commands are discarded.
            // NOTE: This RouterHandover option apparently doesn't exist in NetMQ 
            //      so I IGNORE it for now. DaleBrubaker Feb 1 2016

            m_pipe = pipe;
            m_outbox = outbox;
            m_poller = new NetMQPoller {m_pipe};
            //m_beaconPort = ZreDiscoveryPort;
            m_interval = TimeSpan.Zero; // Use default
            m_uuid = Guid.NewGuid();
            m_peers = new Dictionary<Guid, ZrePeer>();
            m_peerGroups = new Dictionary<string, ZreGroup>();
            m_ownGroups = new Dictionary<string, ZreGroup>();
            m_headers = new Dictionary<string, string>();

            //  Default name for node is first 6 characters of UUID:
            //  the shorter string is more readable in logs
            m_name = m_uuid.ToString().ToUpper().Substring(0, 6);
        }

        /// <summary>
        /// Start node. Use beacon discovery
        /// </summary>
        /// <returns>true if OK, false if not possible</returns>
        public bool Start()
        {
            Debug.Assert(m_beacon == null);
            m_beacon = new NetMQBeacon();
            m_beacon.Configure(ZreDiscoveryPort);
            var hostIp = "TODO"; // m_beacon.BoundTo;

            // Bind our router port to the host
            var address = string.Format("tcp://{0}", hostIp);
            m_port = m_inbox.BindRandomPort(address);
            if (m_port <= 0)
            {
                // Die on bad interface or port exhaustion
                return false;
            }
            m_endpoint = m_inbox.Options.LastEndpoint;

            //  Set broadcast/listen beacon
            PublishBeacon(m_port);
            m_beacon.Subscribe("ZRE");
            m_poller.Add(m_beacon);
            // TODO: I'm not sure I need to do this, because NetMQBeacon does internal polling and has internal actor. Just hook to beacon ReceiveReady event?

            // Start polling on inbox
            m_poller.Add(m_inbox);
            return true;
        }

        /// <summary>
        /// Stop node discovery and interconnection
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            // Stop broadcast/listen beacon
            PublishBeacon(0);
            Thread.Sleep(1); // Allow 1 msec for beacon to go out
            m_poller.Remove(m_beacon);

            // Stop polling on inbox
            m_poller.Remove(m_inbox);

            // Tell the application we are stopping
            var msg = new NetMQMessage(3);
            msg.Append("STOP");
            msg.Append(m_uuid.ToString());
            msg.Append(m_name);
            m_outbox.TrySendMultipartMessage(TimeSpan.Zero, msg);
            return true;
        }

        /// <summary>
        /// Beacon 22-byte message per http://rfc.zeromq.org/spec:36
        /// </summary>
        /// <param name="port">the port can be m_port (normal) or 0 (stopping)</param>
        /// <returns></returns>
        private byte[] BeaconMessage(int port)
        {
            var transmit = new byte[22];
            transmit[0] = Convert.ToByte('Z');
            transmit[1] = Convert.ToByte('R');
            transmit[2] = Convert.ToByte('E');
            transmit[3] = BeaconVersion;
            var uuidBytes = m_uuid.ToByteArray();
            Buffer.BlockCopy(uuidBytes, 0, transmit, 4, 16);
            var portBytes = NetworkOrderBitsConverter.GetBytes((short) port);
            Buffer.BlockCopy(portBytes, 0, transmit, 20, 2);
            return transmit;
        }

        private void PublishBeacon(int port)
        {
            var transmit = BeaconMessage(port);
            if (m_interval == TimeSpan.Zero)
            {
                // Use default
                m_beacon.Publish(transmit);
            }
            else
            {
                m_beacon.Publish(transmit, m_interval);
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
            foreach (var peer in m_peers.Values)
            {
                SendMessageToPeer(peer, msg);
            }
        }

        /// <summary>
        /// Here we handle the different control messages from the front-end
        /// </summary>
        public void ReceiveApi()
        {
            // Get the whole message off the pipe in one go
            var request = m_pipe.ReceiveMultipartMessage();
            var command = request.Pop().ConvertToString();
            switch (command)
            {
                case "UUID":
                    m_pipe.SendFrame(m_uuid.ToString());
                    break;
                case "NAME":
                    m_pipe.SendFrame(m_name);
                    break;
                case "SET NAME":
                    m_name = request.Pop().ConvertToString();
                    Debug.Assert(!string.IsNullOrEmpty(m_name));
                    break;
                case "SET HEADER":
                    var name = request.Pop().ConvertToString();
                    var value = request.Pop().ConvertToString();
                    m_headers[name] = value;
                    break;
                case "SET PORT":
                    var str = request.Pop().ConvertToString();
                    int.TryParse(str, out m_port);
                    break;
                case "SET INTERVAL":
                    var intervalStr = request.Pop().ConvertToString();
                    TimeSpan.TryParse(intervalStr, out m_interval);
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
                    if (m_peers.TryGetValue(uuid, out peer))
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
                    if (m_ownGroups.TryGetValue(groupName, out group))
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
                    if (!m_ownGroups.TryGetValue(groupNameJoin, out groupJoin))
                    {
                        // Only send if we're not already in group
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Join,
                            Join = {Group = groupNameJoin}
                        };
                        // Update status before sending command
                        IncrementStatus();
                        foreach (var peerJoin in m_peers.Values)
                        {
                            peerJoin.Send(msg);
                        }
                    }
                    break;
                case "LEAVE":
                    var groupNameLeave = request.Pop().ConvertToString();
                    ZreGroup groupLeave;
                    if (m_ownGroups.TryGetValue(groupNameLeave, out groupLeave))
                    {
                        // Only send if we are actually in group
                        var msg = new ZreMsg
                        {
                            Id = ZreMsg.MessageId.Leave,
                            Join = {Group = groupNameLeave}
                        };
                        // Update status before sending command
                        IncrementStatus();
                        foreach (var peerLeave in m_peers.Values)
                        {
                            peerLeave.Send(msg);
                        }
                        m_ownGroups.Remove(groupNameLeave);
                    }
                    break;
                case "PEERS":
                    // Send the list of the m_peers keys
                    var peersKeyBuffer = Serialization.BinarySerialize(m_peers.Keys.ToList());
                    m_pipe.SendFrame(peersKeyBuffer);
                    break;
                case "PEER ENDPOINT":
                    var uuidForEndpoint = PopGuid(request);
                    var peerForEndpoint = m_peers[uuidForEndpoint]; // throw exception if not found
                    m_pipe.SendFrame(peerForEndpoint.Endpoint);
                    break;
                case "PEER NAME":
                    var uuidForName = PopGuid(request);
                    var peerForName = m_peers[uuidForName]; // throw exception if not found
                    m_pipe.SendFrame(peerForName.Name);
                    break;
                case "PEER HEADER":
                    var uuidForHeader = PopGuid(request);
                    var keyForHeader = request.Pop().ConvertToString();
                    ZrePeer peerForHeader;
                    if (m_peers.TryGetValue(uuidForHeader, out peerForHeader))
                    {
                        string header;
                        m_headers.TryGetValue(keyForHeader, out header);
                        m_pipe.SendFrame(header ?? "");
                    }
                    else
                    {
                        m_pipe.SendFrame("");
                    }
                    break;
                case "PEER GROUPS":
                    // Send a list of the m_peerGroups keys, comma-delimited
                    var peerGroupsKeyBuffer = Serialization.BinarySerialize(m_peerGroups.Keys.ToList());
                    m_pipe.SendFrame(peerGroupsKeyBuffer);
                    break;
                case "OWN GROUPS":
                    // Send a list of the m_ownGroups keys, comma-delimited
                    var ownGroupsKeyBuffer = Serialization.BinarySerialize(m_ownGroups.Keys.ToList());
                    m_pipe.SendFrame(ownGroupsKeyBuffer);
                    break;
                case "$TERM":
                    m_terminated = true;
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
            m_status = m_status == UbyteMax ? (byte) 0 : m_status++;
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
            if (m_peers.TryGetValue(uuid, out peer))
            {
                return peer;
            }

            // Purge any previous peer on same endpoint
            foreach (var existingPeer in m_peers.Values)
            {
                PurgePeer(existingPeer, endpoint);
            }
            peer = ZrePeer.NewPeer(m_peers, uuid);
            peer.SetOrigin(m_name);
            peer.Connect(m_uuid, m_endpoint);

            // Handshake discovery by sending HELLO as first message
            var helloMessage = new ZreMsg
            {
                Id = ZreMsg.MessageId.Hello,
                Hello =
                {
                    Endpoint = endpoint,
                    Groups = m_ownGroups.Keys.ToList(),
                    Status = m_status,
                    Name = m_name,
                    Headers = m_headers
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
            m_outbox.SendMoreFrame("EXIT").SendMoreFrame(peer.Uuid.ToString()).SendFrame(peer.Name);

            // Remove peer from any groups we've got it in
            foreach (var peerGroup in m_peerGroups.Values)
            {
                DeletePeer(peerGroup, peer);
            }

            // To destroy peer, we remove from peers hash table
            m_peers.Remove(peer.Uuid);
        }

        /// <summary>
        /// Find or create group via its name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public ZreGroup RequirePeerGroup(string groupName)
        {
            ZreGroup group;
            if (!m_peerGroups.TryGetValue(groupName, out group))
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
            m_outbox.SendMoreFrame("JOIN").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(m_name);
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
            m_outbox.SendMoreFrame("LEAVE").SendMoreFrame(peer.Uuid.ToString()).SendMoreFrame(peer.Name).SendFrame(m_name);
            return group;
        }

        /// <summary>
        /// Here we handle messages coming from other peers
        /// </summary>
        public void ReceivePeer()
        {
            Guid uuid;
            var msg = ZreMsg.ReceiveNew(m_inbox, out uuid);
            if (msg == null)
            {
                // Ignore a bad message (header or message signature doesn't meet http://rfc.zeromq.org/spec:36)
                return;
            }
            ZrePeer peer;
            m_peers.TryGetValue(uuid, out peer);
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
                        Debug.Assert(!m_peers.ContainsKey(uuid));
                    }
                    else if (peer.Endpoint == m_endpoint)
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
            NetMQMessage outMsg; // message we'll send to m_outbox
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
                    var headersBuffer = Serialization.BinarySerialize(m_headers);
                    outMsg.Append(headersBuffer);
                    outMsg.Append(helloMessage.Endpoint);
                    m_outbox.SendMultipartMessage(outMsg);

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
                    //m_outbox.SendMultipartMessage(outMsg);

                    // TODO Check this method instead
                    m_outbox.SendMoreFrame("WHISPER").SendMoreFrame(uuid.ToByteArray()).SendMoreFrame(peer.Name).SendMultipartMessage(msg.Whisper.Content);

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
                    m_outbox.SendMultipartMessage(outMsg);
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
                ZreMsg.SendPing(m_outbox, 0);

                // Inform the calling application this peer is being evasive
                m_outbox.SendMoreFrame("EVASIVE");
                m_outbox.SendMoreFrame(peer.Uuid.ToByteArray());
                m_outbox.SendFrame(peer.Name);
            }
        }

        /// <summary>
        /// We do this once a second:
        /// - if peer has gone quiet, send TCP ping and emit EVASIVE event
        /// - if peer has disappeared, expire it
        /// </summary>
        public void PingAllPeers()
        {
            foreach (var peer in m_peers.Values)
            {
                PingPeer(peer);
            }
        }

        public override string ToString()
        {
            return string.Format("name:{0} router endpoint:{1} status:{2}", m_name, m_endpoint, m_status);
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

            if (m_outbox != null)
            {
                m_outbox.Dispose();
                m_outbox = null;
            }
            if (m_poller != null)
            {
                m_poller.Dispose();
                m_poller = null;
            }
            if (m_beacon != null)
            {
                m_beacon.Dispose();
                m_beacon = null;
            }
            if (m_inbox != null)
            {
                m_inbox.Dispose();
                m_inbox = null;
            }
            foreach (var peer in m_peers.Values)
            {
                peer.Destroy();
            }
            foreach (var group in m_peerGroups.Values)
            {
                group.Dispose();
            }
            foreach (var group in m_ownGroups.Values)
            {
                group.Dispose();
            }
        }

        public class Shim : IShimHandler
        {
            private const int ReapInterval = 1000; // 1 second

            /// <summary>
            /// This is the actor that runs a single node; it uses one thread, creates
            /// a ZreNode at start and destroys that when finishing.
            /// </summary>
            /// <param name="pipe">Pipe back to application</param>
            /// <param name="outbox">Outbox back to application</param>
            public void Run(PairSocket shim)
            {
                using (var node = new ZreNode(pipe, outbox))
                {
                    //  Signal actor successfully initialized
                    pipe.SignalOK();

                    var items = new NetMQPoller {m_pipe, m_inbox, m_beacon};

                    // Loop until the agent is terminated one way or another
                    var reapAt = ZrePeer.CurrentTimeMilliseconds() + ReapInterval;
                    while (!node.m_terminated)
                    {
                        var timeout = reapAt - ZrePeer.CurrentTimeMilliseconds();
                        if (timeout > ReapInterval)
                        {
                            timeout = ReapInterval;
                        }
                        else if (timeout < 0)
                        {
                            timeout = 0;
                        }
                        if (ZrePeer.CurrentTimeMilliseconds() > reapAt)
                        {
                            reapAt = ZrePeer.CurrentTimeMilliseconds() + ReapInterval;
                            node.PingAllPeers();
                        }

                        // TODO stuff from zyre.c

                    }
                }
            }
        }
    }

}
