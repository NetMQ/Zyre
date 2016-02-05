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
        private NetMQActor m_actor;     // A Zyre instance wraps the actor instance
        private PairSocket m_inbox;     // Receives incoming cluster traffic
        private Guid m_uuid;            // Copy of node UUID string
        private string m_name;          // Copy of node name
        private string m_endpoint;      // Copy of last endpoint bound to

        public Zyre NewZyre(string name)
        {
            var zyre = new Zyre();

            // Create front-to-back pipe pair for data traffic

            var outbox = new PairSocket();

            // Start node engine and wait for it to be ready
            m_actor = NetMQActor.Create(new Shim());

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
            var peers = Serialization.DeserializeListGuid(peersBuffer);
            return peers;
        }


        //TODO: Down to here













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

    public class Shim : IShimHandler
    {
        public void Run(PairSocket shim)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// This is the actor that runs a single node; it uses one thread, creates
        ///// a ZreNode at start and destroys that when finishing.
        ///// </summary>
        ///// <param name="pipe">Pipe back to application</param>
        ///// <param name="outbox">Outbox back to application</param>
        //public void RunActor(PairSocket pipe, NetMQSocket outbox)
        //{
        //    using (var node = ZreNode.NewNode(pipe, outbox))
        //    {
        //        //  Signal actor successfully initialized
        //        pipe.SignalOK();

        //        var items = new NetMQPoller { m_pipe, m_inbox, m_beacon };

        //        // Loop until the agent is terminated one way or another
        //        var reapAt = ZrePeer.CurrentTimeMilliseconds() + ReapInterval;
        //        while (!m_terminated)
        //        {
        //            var timeout = reapAt - ZrePeer.CurrentTimeMilliseconds();
        //            if (timeout > ReapInterval)
        //            {
        //                timeout = ReapInterval;
        //            }
        //            else if (timeout < 0)
        //            {
        //                timeout = 0;
        //            }
        //            if (ZrePeer.CurrentTimeMilliseconds() > reapAt)
        //            {
        //                reapAt = ZrePeer.CurrentTimeMilliseconds() + ReapInterval;
        //                node.PingAllPeers();
        //            }



        //        }
        //    }
        //}
    }
}
