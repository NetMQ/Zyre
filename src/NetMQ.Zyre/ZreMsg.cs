//  =========================================================================
//    ZreMsg - work with ZRE messages
//
//    Codec class for ZreMsg.
//
//    Was generated using NetMQ.zproto and then modified extensively.

using System;
using System.Collections.Generic;
using System.Text;
using NetMQ.Sockets;

#pragma warning disable 168

namespace NetMQ.Zyre
{
    /// <summary>
    /// work with ZRE messages
    /// </summary>
    public class ZreMsg
    {
        public class MessageException : Exception
        {
            public MessageException(string message) : base(message)
            {
            }
        }

        public enum MessageId
        {
            Hello = 1,
            Whisper = 2,
            Shout = 3,
            Join = 4,
            Leave = 5,
            Ping = 6,
            PingOk = 7,
        }

        #region Hello

        /// <summary>
        /// Greet a peer so it can connect back to us
        /// </summary>
        public class HelloMessage
        {
            public HelloMessage()
            {
                Version = 2;
                Sequence = 1;
                Name = "";
                Headers = new Dictionary<string, string>();
                Groups = new List<string>();
            }

            /// <summary>
            /// Get/Set the Version field
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field. Always 1 for Hello.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }

            /// <summary>
            /// Get/Set the Endpoint field.
            /// Sender connect endpoint
            /// Endpoint includes transport, etc. "tcp://127.0.0.1:2225"
            /// </summary>
            public string Endpoint { get; set; }

            /// <summary>
            /// Get/Set the Groups list.
            /// List of groups sender is in.
            /// </summary>
            public List<string> Groups { get; set; }

            /// <summary>
            /// Get/Set the Status field.
            /// Sender groups status value.
            /// </summary>
            public byte Status { get; set; }

            /// <summary>
            /// Get/Set the Name field.
            /// Sender public name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Get/Set the Headers dictionary.
            /// Sender header properties.
            /// </summary>
            public Dictionary<string, string> Headers { get; set; }

            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                //  Endpoint
                frameSize += 1 + Endpoint.Length;

                //  Groups
                frameSize += 4; //  Size is 4 octets
                if (Groups != null)
                {
                    foreach (string s in Groups)
                    {
                        frameSize += 4 + s.Length;
                    }
                }

                //  Status
                frameSize += 1;

                //  Name
                frameSize += 1 + Name.Length;

                //  Headers
                frameSize += 4; //  Size is 4 octets
                if (Headers != null)
                {
                    int headersSize = 0;

                    foreach (var pair in Headers)
                    {
                        headersSize += 1 + pair.Key.Length;
                        headersSize += 4 + pair.Value.Length;
                    }

                    frameSize += headersSize;
                }


                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

                // Endpoint
                m.PutString(Endpoint);

                // Groups
                if (Groups != null)
                {
                    m.PutNumber4((UInt32) Groups.Count);

                    foreach (string s in Groups)
                    {
                        m.PutLongString(s);
                    }
                }
                else
                    m.PutNumber4(0); //  Empty string array

                // Status
                m.PutNumber1(Status);

                // Name
                m.PutString(Name);

                // Headers
                if (Headers != null)
                {
                    m.PutNumber4((UInt32) Headers.Count);

                    foreach (var pair in Headers)
                    {
                        m.PutString(pair.Key);
                        m.PutLongString(pair.Value);
                    }
                }
                else
                    m.PutNumber4(0); //  Empty dictionary

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

                // Endpoint
                Endpoint = m.GetString();

                // Groups
                listSize = (int) m.GetNumber4();
                Groups = new List<string>(listSize);
                while (listSize-- > 0)
                {
                    string s = m.GetLongString();
                    Groups.Add(s);
                }

                // Status
                Status = m.GetNumber1();

                // Name
                Name = m.GetString();

                // Headers
                hashSize = (int) m.GetNumber4();
                Headers = new Dictionary<string, string>();
                while (hashSize-- > 0)
                {
                    string key = m.GetString();
                    string value = m.GetLongString();
                    Headers.Add(key, value);
                }
            }
        }

        #endregion

        #region Whisper

        /// <summary>
        /// Send a multi-part message to a peer
        /// </summary>
        public class WhisperMessage
        {
            public WhisperMessage()
            {
                Version = 2;
            }

            /// <summary>
            /// Get/Set the Version field
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }

            /// <summary>
            /// Get/Set the Content NetMQMessage.
            /// Wrapped message content.
            /// </summary>
            public NetMQMessage Content { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                //  Content
                //  A frame or a message with special handling in Receive() and Send()

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

                // Content

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

                // Content

            }
        }

        #endregion

        #region Shout

        /// <summary>
        /// Send a multi-part message to a group
        /// </summary>
        public class ShoutMessage
        {
            public ShoutMessage()
            {
                Version = 2;
                Group = "";
            }

            /// <summary>
            /// Get/Set the Version field
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }

            /// <summary>
            /// Get/Set the Group field.
            /// Group to send to.
            /// </summary>
            public string Group { get; set; }

            /// <summary>
            /// Get/Set the Content NetMQMessage.
            /// Wrapped message content.
            /// </summary>
            public NetMQMessage Content { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                //  Group
                frameSize += 1 + Group.Length;

                //  Content
                //  A frame or a message with special handling in Receive() and Send()

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

                // Group
                m.PutString(Group);

                // Content

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

                // Group
                Group = m.GetString();

                // Content

            }
        }

        #endregion

        #region Join

        /// <summary>
        /// Join a group
        /// </summary>
        public class JoinMessage
        {
            public JoinMessage()
            {
                Version = 2;
                Group = "";
            }

            /// <summary>
            /// Get/Set the Version field
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }

            /// <summary>
            /// Get/Set the Group field.
            /// Name of group.
            /// </summary>
            public string Group { get; set; }

            /// <summary>
            /// Get/Set the Status field.
            /// Sender groups status value.
            /// </summary>
            public byte Status { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                //  Group
                frameSize += 1 + Group.Length;

                //  Status
                frameSize += 1;

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

                // Group
                m.PutString(Group);

                // Status
                m.PutNumber1(Status);

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

                // Group
                Group = m.GetString();

                // Status
                Status = m.GetNumber1();

            }
        }

        #endregion

        #region Leave

        /// <summary>
        /// Leave a group
        /// </summary>
        public class LeaveMessage
        {
            public LeaveMessage()
            {
                Version = 2;
                Group = "";
            }

            /// <summary>
            /// Get/Set the Version field.
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }

            /// <summary>
            /// Get/Set the Group field.
            /// Name of group.
            /// </summary>
            public string Group { get; set; }

            /// <summary>
            /// Get/Set the Status field.
            /// Sender groups status value.
            /// </summary>
            public byte Status { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                //  Group
                frameSize += 1 + Group.Length;

                //  Status
                frameSize += 1;

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

                // Group
                m.PutString(Group);

                // Status
                m.PutNumber1(Status);

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

                // Group
                Group = m.GetString();

                // Status
                Status = m.GetNumber1();

            }
        }

        #endregion

        #region Ping

        /// <summary>
        /// Ping a peer that has gone silent
        /// </summary>
        public class PingMessage
        {
            public PingMessage()
            {
                Version = 2;
            }

            /// <summary>
            /// Get/Set the Version field.
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

            }
        }

        #endregion

        #region PingOk

        /// <summary>
        /// Reply to a peer's ping
        /// </summary>
        public class PingOkMessage
        {
            public PingOkMessage()
            {
                Version = 2;
            }

            /// <summary>
            /// Get/Set the Version field.
            /// </summary>
            public byte Version { get; set; }

            /// <summary>
            /// Get/Set the Sequence field.
            /// Cyclic sequence number.
            /// </summary>
            public UInt16 Sequence { get; set; }


            internal int GetFrameSize()
            {
                int frameSize = 0;

                //  Version
                frameSize += 1;

                //  Sequence
                frameSize += 2;

                return frameSize;
            }

            internal void Write(ZreMsg m)
            {
                // Version
                m.PutNumber1(2); // Version

                // Sequence
                m.PutNumber2(Sequence);

            }

            internal void Read(ZreMsg m)
            {
                int listSize;
                int hashSize;
                int chunkSize;
                byte[] guidBytes;
                byte version;

                // Version
                version = m.GetNumber1();
                if (version != 2)
                {
                    throw new MessageException("Version is invalid");
                }

                // Sequence
                Sequence = m.GetNumber2();

            }
        }

        #endregion


        private byte[] _buffer; //  Read/write buffer for serialization    
        private int _offset;
        private byte[] _routingId;

        /// <summary>
        /// Create a new ZreMsg
        /// </summary>
        public ZreMsg()
        {
            Hello = new HelloMessage();
            Whisper = new WhisperMessage();
            Shout = new ShoutMessage();
            Join = new JoinMessage();
            Leave = new LeaveMessage();
            Ping = new PingMessage();
            PingOk = new PingOkMessage();
        }

        public HelloMessage Hello { get; private set; }

        public WhisperMessage Whisper { get; private set; }

        public ShoutMessage Shout { get; private set; }

        public JoinMessage Join { get; private set; }

        public LeaveMessage Leave { get; private set; }

        public PingMessage Ping { get; private set; }

        public PingOkMessage PingOk { get; private set; }

        /// <summary>
        /// Get/set the message RoutingId.
        /// </summary>
        public byte[] RoutingId
        {
            get { return _routingId; }
            set
            {
                if (value == null)
                    _routingId = null;
                else
                {
                    if (_routingId == null || _routingId.Length != value.Length)
                        _routingId = new byte[value.Length];

                    Buffer.BlockCopy(value, 0, _routingId, 0, value.Length);
                }
            }
        }

        /// <summary>
        /// Get/Set the ZreMsg id
        /// </summary>
        public MessageId Id { get; set; }

        /// <summary>
        /// Return a printable command string
        /// </summary>
        public string Command
        {
            get { return Id.ToString(); }
        }

        public ushort Sequence
        {
            get
            {
                switch (Id)
                {
                    case MessageId.Hello:
                        return Hello.Sequence;
                    case MessageId.Whisper:
                        return Whisper.Sequence;
                    case MessageId.Shout:
                        return Shout.Sequence;
                    case MessageId.Join:
                        return Join.Sequence;
                    case MessageId.Leave:
                        return Leave.Sequence;
                    case MessageId.Ping:
                        return Ping.Sequence;
                    case MessageId.PingOk:
                        return PingOk.Sequence;
                    default:
                        throw new ArgumentException(Id.ToString());
                }
            }
            set
            {
                switch (Id)
                {
                    case MessageId.Hello:
                        Hello.Sequence = value;
                        break;
                    case MessageId.Whisper:
                        Whisper.Sequence = value;
                        break;
                    case MessageId.Shout:
                        Shout.Sequence = value;
                        break;
                    case MessageId.Join:
                        Join.Sequence = value;
                        break;
                    case MessageId.Leave:
                        Leave.Sequence = value;
                        break;
                    case MessageId.Ping:
                        Ping.Sequence = value;
                        break;
                    case MessageId.PingOk:
                        PingOk.Sequence = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Return a new ZreMsg based received from the input socket.
        /// Message is ignored if input is a RouterSocket and the message header doesn't meet the http://rfc.zeromq.org/spec:36 spec.
        /// Message is ignored if the message signature doesn't start with %xAA %xA1.
        /// </summary>
        /// <param name="input">the socket</param>
        /// <param name="uuid">The identity Guid received into the RoutingId, or Guid.Empty</param>
        /// <returns>null if not a valid message (message to be ignored)</returns>
        public static ZreMsg ReceiveNew(RouterSocket input, out Guid uuid)
        {
            var msg = new ZreMsg();
            if (msg.Receive(input))
            {
                if (msg.RoutingId.Length != 17 || msg.RoutingId[0] != 0x1)
                {
                    uuid = Guid.Empty;
                    return null;
                }
                uuid = GetUuid(msg.RoutingId, 1);
                return msg;
            }
            uuid = Guid.Empty;
            return null;
        }

        /// <summary>
        /// Receive a ZreMsg from the socket.                
        /// Message is ignored if the message signature doesn't start with %xAA %xA1 (returns false).
        /// It appears "real" input will always be a RouterSocket, but DealerSocket is used during unit testing by zeromq/zyre and by this implementation.
        /// </summary>
        /// <param name="input">the socket</param>
        /// <returns>true if successful</returns>
        public bool Receive(IReceivingSocket input)
        {
            if (input is RouterSocket)
            {
                Msg routingIdMsg = new Msg();
                routingIdMsg.InitEmpty();
                try
                {
                    input.Receive(ref routingIdMsg);

                    if (!routingIdMsg.HasMore)
                    {
                        throw new MessageException("No routing id");
                    }

                    if (_routingId == null || _routingId.Length == routingIdMsg.Size)
                        _routingId = new byte[routingIdMsg.Size];

                    Buffer.BlockCopy(routingIdMsg.Data, 0, _routingId, 0, _routingId.Length);
                }
                finally
                {
                    routingIdMsg.Close();
                }
            }
            else
            {
                RoutingId = null;
            }
            Msg msg = new Msg();
            msg.InitEmpty();

            try
            {
                input.Receive(ref msg);

                _offset = 0;
                _buffer = msg.Data;

                UInt16 signature = GetNumber2();

                if (signature != (0xAAA0 | 1))
                {
                    // spec:36 A node SHALL silently discard any message received that does not start with these two octets.
                    return false;
                }

                //  Get message id and parse per message type
                Id = (MessageId) GetNumber1();

                switch (Id)
                {
                    case MessageId.Hello:
                        Hello.Read(this);
                        break;
                    case MessageId.Whisper:
                        Whisper.Read(this);
                        break;
                    case MessageId.Shout:
                        Shout.Read(this);
                        break;
                    case MessageId.Join:
                        Join.Read(this);
                        break;
                    case MessageId.Leave:
                        Leave.Read(this);
                        break;
                    case MessageId.Ping:
                        Ping.Read(this);
                        break;
                    case MessageId.PingOk:
                        PingOk.Read(this);
                        break;
                    default:
                        throw new MessageException("Bad message id");
                }

                // Receive message content for types with content
                switch (Id)
                {
                    case MessageId.Whisper:
                        Whisper.Content = input.ReceiveMultipartMessage();
                        break;
                    case MessageId.Shout:
                        Shout.Content = input.ReceiveMultipartMessage();
                        break;
                }
            }
            finally
            {
                _buffer = null;
                msg.Close();
            }
            return true;
        }

        /// <summary>
        /// Send the ZreMsg to the socket.
        /// Warning re WHISPER and SHOUT: The 0MQ spec http://rfc.zeromq.org/spec:36 
        ///     says "message content defined as one 0MQ frame. ZRE does not support multi-frame message contents."
        /// 	...on the other hand, it appears that zeromq/zyre also supports multi-frame contents, as per the top of zyre.c
        ///     This C# implementation allows multi-frame contents.
        /// </summary>
        /// <returns>true if message was successfully sent</returns>
        public bool Send(IOutgoingSocket output)
        {
            if (output is RouterSocket)
                output.SendMoreFrame(RoutingId);

            int frameSize = 2 + 1; //  Signature and message ID
            switch (Id)
            {
                case MessageId.Hello:
                    frameSize += Hello.GetFrameSize();
                    break;
                case MessageId.Whisper:
                    frameSize += Whisper.GetFrameSize();
                    break;
                case MessageId.Shout:
                    frameSize += Shout.GetFrameSize();
                    break;
                case MessageId.Join:
                    frameSize += Join.GetFrameSize();
                    break;
                case MessageId.Leave:
                    frameSize += Leave.GetFrameSize();
                    break;
                case MessageId.Ping:
                    frameSize += Ping.GetFrameSize();
                    break;
                case MessageId.PingOk:
                    frameSize += PingOk.GetFrameSize();
                    break;
            }

            //  Now serialize message into the buffer    
            Msg msg = new Msg();
            msg.InitPool(frameSize);

            try
            {
                _offset = 0;
                _buffer = msg.Data;

                // put signature
                PutNumber2(0xAAA0 | 1);

                // put message id
                PutNumber1((byte) Id);

                switch (Id)
                {
                    case MessageId.Hello:
                        Hello.Write(this);
                        break;
                    case MessageId.Whisper:
                        Whisper.Write(this);
                        break;
                    case MessageId.Shout:
                        Shout.Write(this);
                        break;
                    case MessageId.Join:
                        Join.Write(this);
                        break;
                    case MessageId.Leave:
                        Leave.Write(this);
                        break;
                    case MessageId.Ping:
                        Ping.Write(this);
                        break;
                    case MessageId.PingOk:
                        PingOk.Write(this);
                        break;
                }

                //  Send the data frame				
                var more = Id == MessageId.Whisper || Id == MessageId.Shout;
                if (!output.TrySend(ref msg, TimeSpan.Zero, more))
                {
                    return false;
                }

                // Send message content for types with content
                switch (Id)
                {
                    case MessageId.Whisper:
                        if (Whisper.Content == null)
                        {
                            Whisper.Content = new NetMQMessage();
                            Whisper.Content.PushEmptyFrame();
                        }
                        output.TrySendMultipartMessage(Whisper.Content);
                        break;
                    case MessageId.Shout:
                        if (Shout.Content == null)
                        {
                            Shout.Content = new NetMQMessage();
                            Shout.Content.PushEmptyFrame();
                        }
                        output.TrySendMultipartMessage(Shout.Content);
                        break;
                }
                return true;
            }
            finally
            {
                _buffer = null;
                msg.Close();
            }
        }
        public static Guid GetUuid(byte[] buffer, int offset)
        {
            var guidBytes = new byte[16];
            Buffer.BlockCopy(buffer, offset, guidBytes, 0, 16);
            var result = new Guid(guidBytes);
            return result;
        }

        #region SendHelpers

        /// <summary>
        /// Send a Hello message to the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="endpoint"></param>
        /// <param name="groups"></param>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="headers"></param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendHello(IOutgoingSocket socket, string endpoint, List<string> groups, byte status, string name, Dictionary<string, string> headers)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Hello,
                Hello =
                {
                    Endpoint = endpoint,
                    Groups = groups,
                    Status = status,
                    Name = name,
                    Headers = headers
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a Whisper message to the socket
        /// Warning re WHISPER and SHOUT: The 0MQ spec http://rfc.zeromq.org/spec:36 
        ///     says "message content defined as one 0MQ frame. ZRE does not support multi-frame message contents."
        /// 	...on the other hand, it appears that zeromq/zyre also supports multi-frame contents, as per the top of zyre.c
        ///     This C# implementation allows multi-frame contents.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <param name="content">See warning above</param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendWhisper(IOutgoingSocket socket, ushort sequence, NetMQMessage content)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Whisper,
                Whisper =
                {
                    Version = 2,
                    Sequence = sequence,
                    Content = content
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a Shout message to the socket
        /// Warning re WHISPER and SHOUT: The 0MQ spec http://rfc.zeromq.org/spec:36 
        ///     says "message content defined as one 0MQ frame. ZRE does not support multi-frame message contents."
        /// 	...on the other hand, it appears that zeromq/zyre also supports multi-frame contents, as per the top of zyre.c
        ///     This C# implementation allows multi-frame contents.
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <param name="content">See warning above</param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendShout(IOutgoingSocket socket, ushort sequence, NetMQMessage content)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Shout,
                Shout =
                {
                    Version = 2,
                    Sequence = sequence,
                    Content = content
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a Join message to the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <param name="group"></param>
        /// <param name="status"></param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendJoin(IOutgoingSocket socket, ushort sequence, string group, byte status)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Join,
                Join =
                {
                    Version = 2,
                    Sequence = sequence,
                    Group = group,
                    Status = status,
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a Leave message to the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <param name="group"></param>
        /// <param name="status"></param>
        public static bool SendLeave(IOutgoingSocket socket, ushort sequence, string group, byte status)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Leave,
                Leave =
                {
                    Version = 2,
                    Sequence = sequence,
                    Group = group,
                    Status = status,
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a Ping message to the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendPing(IOutgoingSocket socket, ushort sequence)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.Ping,
                Ping =
                {
                    Version = 2,
                    Sequence = sequence,
                }
            };
            return msg.Send(socket);
        }

        /// <summary>
        /// Send a PingOk message to the socket
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="sequence"></param>
        /// <returns>true if message was successfully sent</returns>
        public static bool SendPingOk(IOutgoingSocket socket, ushort sequence)
        {
            var msg = new ZreMsg
            {
                Id = MessageId.PingOk,
                PingOk =
                {
                    Version = 2,
                    Sequence = sequence,
                }
            };
            return msg.Send(socket);
        }

        #endregion

        #region Network data encoding methods

        //  Put a block of octets to the frame
        private void PutOctets(byte[] host, int size)
        {
            Buffer.BlockCopy(host, 0, _buffer, _offset, size);
            _offset += size;
        }

        //  Get a block of octets from the frame
        private void GetOctets(byte[] host, int size)
        {
            if (_offset + size > _buffer.Length)
            {
                throw new MessageException("Malformed message");
            }

            Buffer.BlockCopy(_buffer, _offset, host, 0, size);
            _offset += size;
        }

        //  Put a 1-byte number to the frame
        private void PutNumber1(byte host)
        {
            _buffer[_offset] = host;
            _offset++;
        }

        //  Put a 2-byte number to the frame
        private void PutNumber2(UInt16 host)
        {
            _buffer[_offset] = (byte) (((host) >> 8) & 255);
            _buffer[_offset + 1] = (byte) (((host)) & 255);

            _offset += 2;
        }

        //  Put a 4-byte number to the frame
        private void PutNumber4(UInt32 host)
        {
            _buffer[_offset] = (byte) (((host) >> 24) & 255);
            _buffer[_offset + 1] = (byte) (((host) >> 16) & 255);
            _buffer[_offset + 2] = (byte) (((host) >> 8) & 255);
            _buffer[_offset + 3] = (byte) (((host)) & 255);

            _offset += 4;
        }

        //  Put a 8-byte number to the frame
        private void PutNumber8(UInt64 host)
        {
            _buffer[_offset] = (byte) (((host) >> 56) & 255);
            _buffer[_offset + 1] = (byte) (((host) >> 48) & 255);
            _buffer[_offset + 2] = (byte) (((host) >> 40) & 255);
            _buffer[_offset + 3] = (byte) (((host) >> 32) & 255);
            _buffer[_offset + 4] = (byte) (((host) >> 24) & 255);
            _buffer[_offset + 5] = (byte) (((host) >> 16) & 255);
            _buffer[_offset + 6] = (byte) (((host) >> 8) & 255);
            _buffer[_offset + 7] = (byte) (((host)) & 255);

            _offset += 8;
        }

        //  Get a 1-byte number from the frame
        private byte GetNumber1()
        {
            if (_offset + 1 > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            byte b = _buffer[_offset];

            _offset++;

            return b;
        }

        //  Get a 2-byte number from the frame
        private UInt16 GetNumber2()
        {
            if (_offset + 2 > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            UInt16 number = (UInt16) ((_buffer[_offset] << 8) + _buffer[_offset + 1]);

            _offset += 2;

            return number;
        }

        //  Get a 4-byte number from the frame
        private UInt32 GetNumber4()
        {
            if (_offset + 4 > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            UInt32 number = (((UInt32) _buffer[_offset]) << 24) + (((UInt32) _buffer[_offset + 1]) << 16) + (((UInt32) _buffer[_offset + 2]) << 8) +
                            (UInt32) _buffer[_offset + 3];

            _offset += 4;

            return number;
        }

        //  Get a 8byte number from the frame
        private UInt64 GetNumber8()
        {
            if (_offset + 8 > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            UInt64 number = (((UInt64) _buffer[_offset]) << 56) + (((UInt64) _buffer[_offset + 1]) << 48) + (((UInt64) _buffer[_offset + 2]) << 40) +
                            (((UInt64) _buffer[_offset + 3]) << 32) + (((UInt64) _buffer[_offset + 4]) << 24) + (((UInt64) _buffer[_offset + 5]) << 16) +
                            (((UInt64) _buffer[_offset + 6]) << 8) + (UInt64) _buffer[_offset + 7];

            _offset += 8;

            return number;
        }

        //  Put a string to the frame
        private void PutString(string host)
        {
            int length = Encoding.UTF8.GetByteCount(host);

            if (length > 255)
                length = 255;

            PutNumber1((byte) length);

            Encoding.UTF8.GetBytes(host, 0, length, _buffer, _offset);

            _offset += length;
        }

        //  Get a string from the frame
        private string GetString()
        {
            int length = GetNumber1();
            if (_offset + length > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            string s = Encoding.UTF8.GetString(_buffer, _offset, length);

            _offset += length;

            return s;
        }

        //  Put a long string to the frame
        private void PutLongString(string host)
        {
            PutNumber4((UInt32) Encoding.UTF8.GetByteCount(host));

            Encoding.UTF8.GetBytes(host, 0, host.Length, _buffer, _offset);

            _offset += host.Length;
        }

        //  Get a long string from the frame
        private string GetLongString()
        {
            int length = (int) GetNumber4();
            if (_offset + length > _buffer.Length)
            {
                throw new MessageException("Malformed message.");
            }

            string s = Encoding.UTF8.GetString(_buffer, _offset, length);

            _offset += length;

            return s;
        }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder("[");
            sb.Append(Command);
            sb.Append(' ');
            switch (Id)
            {
                case MessageId.Hello:
                    sb.Append("from ");
                    sb.Append(Hello.Name);
                    sb.Append(" Seq:");
                    sb.Append(Hello.Sequence);
                    sb.Append(" at EndPoint:");
                    sb.Append(Hello.Endpoint);
                    break;
                case MessageId.Whisper:
                    sb.Append("Seq:");
                    sb.Append(Whisper.Sequence);
                    break;
                case MessageId.Shout:
                    sb.Append("Seq:");
                    sb.Append(Shout.Sequence);
                    break;
                case MessageId.Join:
                    sb.Append("Seq:");
                    sb.Append(Join.Sequence);
                    sb.Append(" Group:");
                    sb.Append(Join.Group);
                    break;
                case MessageId.Leave:
                    sb.Append("Seq:");
                    sb.Append(Leave.Sequence);
                    sb.Append(" Group:");
                    sb.Append(Leave.Group);
                    break;
                case MessageId.Ping:
                    sb.Append("Seq:");
                    sb.Append(Ping.Sequence);
                    break;
                case MessageId.PingOk:
                    sb.Append("Seq:");
                    sb.Append(PingOk.Sequence);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            sb.Append(']');
            return sb.ToString();
        }
    }
}
