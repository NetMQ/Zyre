using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Zyre
    {
        private NetMQActor m_actor;     // A Zyre instance wraps the actor instance
        private PairSocket m_inbox;     // Receives incoming cluster traffic
        private Guid m_uuid;            // Copy of node UUID string
        private string m_name;          // Copy of node name
        private string m_endpoint;      // Copy of last endpoint bound to

        public Zyre NewZyre(string name)
        {
            var outbox = new PairSocket();
            //var inbox = new NetMQActor();

            return null;
        }

    }
}
