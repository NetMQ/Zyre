This is an implementation of the ZeroMQ RealMQ Realtime Exchange Protocol found at http://rfc.zeromq.org/spec:36
This is version 2, NOT the first version (from http://rfc.zeromq.org/spec:20)
This C# implementation is based primarily on the C zeromq/zyre implementation at https://github.com/zeromq/zyre
    ...but there is no implementation using the zgossip capability in czmq

Caveats:
1. The RFC30 spec says about WHISPER and SHOUT: "message content defined as one 0MQ frame. ZRE does not support multi-frame message contents."
	...but this C# implementation does NOT prevent sending and receiving multi-frame contents
	...on the other hand, it appears that zeromq/zyre also supports multi-frame contents, as per the top of zyre.c
2. The RFC30 spec says: Each node SHALL create a ZeroMQ ROUTER socket and bind this to an ephemeral TCP port (in the range %C000x - %FFFFx). 
    ...but this C# implementation does NOT guarantee the ephemeral port (chosen by the OS) will be in that range.

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


