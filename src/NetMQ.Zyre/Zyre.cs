using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using NetMQ.Sockets;

/*
    Zyre does local area discovery and clustering. A Zyre node broadcasts
    UDP beacons, and connects to peers that it finds. This class wraps a
    Zyre node with a message-based API.

    All incoming events are zmsg_t messages delivered via the zyre_recv
    call. The first frame defines the type of the message, and following
    frames provide further values:

        ENTER fromnode name headers ipaddress:port
            a new peer has entered the network
        EVASIVE fromnode name
	        a peer is being evasive (quiet for too long)
	    EXIT fromnode name
            a peer has left the network
        JOIN fromnode name groupname
            a peer has joined a specific group
        LEAVE fromnode name groupname
            a peer has joined a specific group
        WHISPER fromnode name message
            a peer has sent this node a message
        SHOUT fromnode name groupname message
            a peer has sent one of our groups a message

    In SHOUT and WHISPER the message is zero or more frames, and can hold
    any ZeroMQ message. In ENTER, the headers frame contains a packed
    dictionary, see zhash_pack/unpack.

    To join or leave a group, use the zyre_join and zyre_leave methods.
    To set a header value, use the zyre_set_header method. To send a message
    to a single peer, use zyre_whisper. To send a message to a group, use
    zyre_shout.
*/

namespace NetMQ.Zyre
{
    /// <summary>
    /// zyre - API wrapping one Zyre node
    /// </summary>
    public class Zyre : IDisposable
    {
        const int ZyreVersionMajor = 1;
        const int ZyreVersionMinor = 1;
        const int ZyreVersionPatch = 0;

        /// <summary>
        /// A Zyre instance wraps the actor instance
        /// All node control is done through m_actor
        /// </summary>
        private NetMQActor m_actor;

        /// <summary>
        /// Receives incoming cluster traffic (traffic into Zyre from the network)
        /// </summary>
        private PairSocket m_inbox;

        /// <summary>
        /// Copy of node UUID
        /// </summary>
        private Guid m_uuid;

        /// <summary>
        /// Copy of node name
        /// </summary>
        private string m_name;

        /// <summary>
        /// Copy of last endpoint bound to
        /// </summary>
        private string m_endpoint;

        public Zyre NewZyre(string name)
        {
            var zyre = new Zyre();

            // Create front-to-back pipe pair for data traffic
            // outbox is passed to ZreNode for sending Zyre message traffic back to m_inbox
            PairSocket outbox;
            PairSocket.CreateSocketPair(out outbox, out m_inbox);

            // Start node engine and wait for it to be ready
            // All node control is done through m_actor
            var shim = new ZreNode.Shim(outbox);
            m_actor = NetMQActor.Create(shim);

            // Send name, if any, to node ending
            if (!string.IsNullOrEmpty(name))
            {
                m_actor.SendMoreFrame("SET NAME").SendFrame(name);
            }

            return zyre;
        }

        /// <summary>
        /// Return our node UUID string, after successful initialization
        /// </summary>
        public Guid Uuid
        {
            get
            {
                // Hold uuid string in zyre object so caller gets a safe reference
                m_actor.SendFrame("UUID");
                var uuidBytes = m_actor.ReceiveFrameBytes();
                Debug.Assert(uuidBytes.Length == 16);
                m_uuid = new Guid(uuidBytes);
                return m_uuid;
            }
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">the name to set</param>
        public void SetName(string name)
        {
            m_actor.SendMoreFrame("SET HEADER").SendFrame(name);
        }

        /// <summary>
        /// Set node header; these are provided to other nodes during discovery
        /// and come in each ENTER message.
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="format">the format string for the value</param>
        /// <param name="args">the arguments for the format string for the value</param>
        public void SetHeader(string key, string format, params object[] args)
        {
            var value = string.Format(format, args);
            m_actor.SendMoreFrame(key).SendFrame(value);
        }

        /// <summary>
        /// Set UDP beacon discovery port; defaults to 5670, this call overrides
        /// that so you can create independent clusters on the same network, for
        /// e.g. development vs. production. Has no effect after Zzyre.Start().
        /// </summary>
        /// <param name="port">the UDP beacon discovery port override</param>
        public void SetPort(int port)
        {
            m_actor.SendMoreFrame("SET PORT").SendFrame(port.ToString());
        }

        /// <summary>
        /// Set UDP beacon discovery interval. Default is instant
        /// beacon exploration followed by pinging every 1,000 msecs.
        /// </summary>
        /// <param name="interval">beacon discovery interval</param>
        public void SetInterval(TimeSpan interval)
        {
            m_actor.SendMoreFrame("SET INTERVAL").SendFrame(interval.ToString());
        }

        /// <summary>
        /// Set network interface for UDP beacons. If you do not set this, NetMQ will
        /// choose an interface for you. On boxes with several interfaces you should
        /// specify which one you want to use, or strange things can happen.
        /// </summary>
        /// <param name="value">the interface</param>
        public void SetInterface(string value)
        {
            // TODO
            //  Implemented by zsys global for now
            // zsys_set_interface(value);
        }

        /// <summary>
        /// Start node, after setting header values. When you start a node it
        /// begins discovery and connection.
        /// </summary>
        public void Start()
        {
            m_actor.SendFrame("START");
        }

        /// <summary>
        /// Stop node; this signals to other peers that this node will go away.
        /// This is polite; however you can also just destroy the node without
        /// stopping it.
        /// </summary>
        public void Stop()
        {
            m_actor.SendFrame("STOP");
        }

        /// <summary>
        /// Join a named group; after joining a group you can send messages to
        /// the group and all Zyre nodes in that group will receive them.
        /// </summary>
        /// <param name="groupName">the name of the group</param>
        public void Join(string groupName)
        {
            m_actor.SendMoreFrame("JOIN").SendFrame(groupName);
        }

        /// <summary>
        /// Leave a group.
        /// </summary>
        /// <param name="groupName">the name of the group</param>
        public void Leave(string groupName)
        {
            m_actor.SendMoreFrame("LEAVE").SendFrame(groupName);
        }

        /// <summary>
        /// Receive next message from network; the message may be a control.
        /// message (ENTER, EXIT, JOIN, LEAVE) or data (WHISPER, SHOUT).
        /// </summary>
        /// <returns>message</returns>
        public NetMQMessage Recv()
        {
            return m_inbox.ReceiveMultipartMessage();
        }

        /// <summary>
        /// Send message to single peer.
        /// </summary>
        /// <param name="peer">the peer who gets the message</param>
        /// <param name="message">the message</param>
        public void Whisper(Guid peer, NetMQMessage message)
        {
            m_actor.SendMoreFrame("WHISPER").SendMoreFrame(peer.ToByteArray()).SendMultipartMessage(message);
        }

        /// <summary>
        /// Send message to a named group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        public void Shout(string groupName, NetMQMessage message)
        {
            m_actor.SendMoreFrame("SHOUT").SendMoreFrame(groupName).SendMultipartMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="peer">the peer who gets the message</param>
        /// <param name="format">the format string for the value</param>
        /// <param name="args">the arguments for the format string for the value</param>
        public void Whispers(Guid peer, string format, params object[] args)
        {
            var value = string.Format(format, args);
            m_actor.SendMoreFrame("WHISPER").SendMoreFrame(peer.ToByteArray()).SendFrame(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="format">the format string for the value</param>
        /// <param name="args">the arguments for the format string for the value</param>
        public void Shouts(string groupName, string format, params object[] args)
        {
            var value = string.Format(format, args);
            m_actor.SendMoreFrame("WHISPER").SendMoreFrame(groupName).SendFrame(value);
        }

        /// <summary>
        /// Return list of current peers (their Uuids).
        /// </summary>
        /// <returns></returns>
        public List<Guid> Peers()
        {
            m_actor.SendFrame("PEERS");
            var peersBuffer = m_actor.ReceiveFrameBytes();
            var peers = Serialization.BinaryDeserialize<List<Guid>>(peersBuffer);
            return peers;
        }

        /// <summary>
        /// Return the endpoint of a connected peer.
        /// </summary>
        /// <param name="peer">The peer identity</param>
        /// <returns>the endpoint of a connected peer</returns>
        public string PeerAddress(Guid peer)
        {
            m_actor.SendMoreFrame("PEER ENDPOINT");
            m_actor.SendFrame(peer.ToByteArray());
            return m_actor.ReceiveFrameString();
        }

        /// <summary>
        /// Return the value of a header of a connected peer.  Returns String.Empty if peer
        /// or key doesn't exist.
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="key"></param>
        /// <returns>the value of a header of a connected peer, or String.Empty if peer or key doesn't exist</returns>
        public string PeerHeaderValue(Guid peer, string key)
        {
            m_actor.SendMoreFrame("PEER HEADER");
            m_actor.SendMoreFrame(peer.ToByteArray());
            m_actor.SendFrame(key);
            return m_actor.ReceiveFrameString();
        }

        /// <summary>
        /// Return list of currently joined groups.
        /// </summary>
        /// <returns></returns>
        public List<string> OwnGroups()
        {
            m_actor.SendMoreFrame("OWN GROUPS");
            var result = Serialization.BinaryDeserialize<List<string>>(m_actor.ReceiveFrameBytes());
            return result;
        }

        /// <summary>
        /// Return list of groups known through connected peers.
        /// </summary>
        /// <returns></returns>
        public List<string> PeerGroups()
        {
            m_actor.SendMoreFrame("PEER GROUPS");
            var result = Serialization.BinaryDeserialize<List<string>>(m_actor.ReceiveFrameBytes());
            return result;
        }

        /// <summary>
        /// Return the node socket, for direct polling of the socket
        /// </summary>
        public PairSocket Socket { get { return m_inbox; } }

        /// <summary>
        /// Return the Zyre version for run-time API detection
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="patch"></param>
        public void Version(out int major, out int minor, out int patch)
        {
            major = ZyreVersionMajor;
            minor = ZyreVersionMinor;
            patch = ZyreVersionPatch;
        }










        public override string ToString()
        {
            return string.Format("name:{0} endpoint:{1}", m_name, m_endpoint);
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

            if (m_actor != null)
            {
                m_actor.Dispose();
                m_actor = null;
            }
            if (m_inbox != null)
            {
                m_inbox.Dispose();
                m_inbox = null;
            }
        }
    }
}
