This is an implementation of the ZeroMQ RealMQ Realtime Exchange Protocol found at http://rfc.zeromq.org/spec:36
This is version 2, NOT the first version (from http://rfc.zeromq.org/spec:20)
This C# implementation is based primarily on the C zeromq/zyre implementation at https://github.com/zeromq/zyre
    ...but there is no implementation using the zgossip capability in czmq

Caveats:
1. The RFC30 spec says about WHISPER and SHOUT: "message content defined as one 0MQ frame. ZRE does not support multi-frame message contents."
	...but this C# implementation does NOT prevent sending and receiving multi-frame contents
2. The RFC30 spec says: Each node SHALL create a ZeroMQ ROUTER socket and bind this to an ephemeral TCP port (in the range %C000x - %FFFFx). 
    ...but this C# implementation does NOT guarantee the ephemeral port (chosen by the OS) will be in that range.





