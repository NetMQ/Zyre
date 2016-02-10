/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetMQ.Sockets;

namespace NetMQ.Zyre
{
    public class ZrePeer : IDisposable
    {
        private const int PeerEvasive = 10000; // 10 seconds' silence is evasive
        private const int PeerExpired = 30000; // 30 seconds' silence is expired
        private const ushort UshortMax = ushort.MaxValue;
        private const byte UbyteMax = byte.MaxValue;

        private DealerSocket _mailbox;                  // Socket through to peer
        private readonly Guid _uuid;                    // Identity guid, 16 bytes
        private string _endpoint;                       // Endpoint connected to
        private string _name;                           // Peer's public name
        private string _origin;                         // Origin node's public name
        private long _evasiveAt;                        // Peer is being evasive
        private long _expiredAt;                        // Peer has expired by now
        private bool _connected;                        // Peer will send messages
        private bool _ready;                            // Peer has said Hello to us
        private byte _status;                           // Our status counter
        private ushort _sentSequence;                   // Outgoing message sequence
        private ushort _wantSequence;                   // Incoming message sequence
        private Dictionary<string, string> _headers;    // Peer headers 
        private bool _verbose;                          // Do we log traffic and failures? 
        private readonly Action<string> _verboseAction; // The action to take when _verbose
        
        private ZrePeer(Guid uuid, Action<string> verboseAction = null)
        {
            _uuid = uuid;
            _verboseAction = verboseAction;
            _ready = false;
            _connected = false;
            _sentSequence = 0;
            _wantSequence = 0;
            _headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Construct new ZrePeer object
        /// </summary>
        /// <param name="container">The dictionary of peers</param>
        /// <param name="guid">The identity for this peer</param>
        /// <param name="verboseAction">An action to take for logging when _verbose is true. Default is null.</param>
        /// <returns></returns>
        public static ZrePeer NewPeer(Dictionary<Guid, ZrePeer> container, Guid guid, Action<string> verboseAction = null)
        {
            var peer = new ZrePeer(guid, verboseAction);
            container[guid] = peer; // overwrite any existing entry for same uuid
            return peer;
        }

        /// <summary>
        /// Disconnect this peer and Dispose this class.
        /// </summary>
        public void Destroy()
        {
            Debug.Assert(_mailbox != null, "Mailbox must not be null");
            Disconnect();
            Dispose();
        }

        /// <summary>
        /// Connect peer mailbox
        /// Configures a DealerSocket mailbox connected to peer's router endpoint
        /// </summary>
        /// <param name="replyTo"></param>
        /// <param name="endpoint"></param>
        public void Connect(Guid replyTo, string endpoint)
        {
            Debug.Assert(!_connected);
            //  Create new outgoing socket (drop any messages in transit)
            _mailbox = new DealerSocket(endpoint) // default action is to connect to the peer node
            {
                Options =
                {
                    //  Set our own identity on the socket so that receiving node
                    //  knows who each message came from. Note that we cannot use
                    //  the UUID directly as the identity since it may contain a
                    //  zero byte at the start, which libzmq does not like for
                    //  historical and arguably bogus reasons that it nonetheless
                    //  enforces.
                    Identity = GetIdentity(replyTo), 

                    //  Set a high-water mark that allows for reasonable activity
                    SendHighWatermark = PeerExpired * 100,

                    // SendTimeout = TimeSpan.Zero Instead of this, ZreMsg.Send() uses 
                }
            };
            if (_verbose)
            {
                _verboseAction(string.Format("({0}) connect to peer: {1}", _origin, _endpoint));
            }
            _endpoint = endpoint;
            _connected = true;
            _ready = false;
        }

        public static byte[] GetIdentity(Guid replyTo)
        {
            var result = new byte[17];
            result[0] = 1;
            var uuidBytes = replyTo.ToByteArray();
            Buffer.BlockCopy(uuidBytes, 0, result, 1, 16);
            return result;
        }

        /// <summary>
        /// Disconnect peer mailbox 
        /// No more messages will be sent to peer until connected again
        /// </summary>
        public void Disconnect()
        {
            _mailbox.Dispose();
            _mailbox = null;
            _endpoint = null;
            _connected = false;
            _ready = false;
        }

        /// <summary>
        /// Send message to peer
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>always true</returns>
        public bool Send(ZreMsg msg)
        {
            if (_connected)
            {
                msg.Sequence = ++_sentSequence;
                if (_verbose)
                {
                    _verboseAction(string.Format("({0}) send {1} to peer={2} sequence={3}", _origin, msg.Command, _name, _sentSequence));
                }
                msg.Send(_mailbox);
            }
            return true;
        }

        /// <summary>
        /// Return peer connected status
        /// </summary>
        public bool Connected
        {
            get { return _connected; }
        }

        /// <summary>
        /// Return peer identity string
        /// </summary>
        public Guid Uuid
        {
            get { return _uuid; }
        }

        /// <summary>
        /// Return peer connection endpoint
        /// </summary>
        public string Endpoint
        {
            get { return _endpoint; }
        }

        /// <summary>
        /// Register activity at peer
        /// </summary>
        public void Refresh()
        {
            _evasiveAt = CurrentTimeMilliseconds() + PeerEvasive;
            _expiredAt = CurrentTimeMilliseconds() + PeerExpired;
        }

        /// <summary>
        /// Milliseconds since January 1, 1970 UTC
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeMilliseconds()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Return peer future expired time
        /// </summary>
        public long EvasiveAt
        {
            get { return _evasiveAt; }
        }

        /// <summary>
        /// Return peer future evasive time
        /// </summary>
        public long ExpiredAt
        {
            get { return _expiredAt; }
        }

        /// <summary>
        /// Return peer name
        /// </summary>
        public string Name
        {
            get { return _name ?? ""; }
        }

        /// <summary>
        /// Return peer cycle
        /// This gives us a state change count for the peer, which we can
        /// check against its claimed status, to detect message loss.
        /// </summary>
        public byte Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Increment status
        /// </summary>
        public void IncrementStatus()
        {
            _status = _status == UbyteMax ? (byte)0 : ++_status;
        }

        /// <summary>
        /// Set peer name
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Set current node name, for logging
        /// </summary>
        /// <param name="originNodeName"></param>
        public void SetOrigin(string originNodeName)
        {
            _origin = originNodeName;
        }

        /// <summary>
        /// Set peer status
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(byte status)
        {
            _status = status;
        }

        /// <summary>
        /// Return peer ready state
        /// </summary>
        public bool Ready
        {
            get { return _ready; }
        }

        /// <summary>
        /// Set peer ready
        /// </summary>
        /// <param name="ready"></param>
        public void SetReady(bool ready)
        {
            _ready = ready;
        }

        /// <summary>
        /// Get peer header value
        /// </summary>
        /// <param name="key">The he</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string Header(string key, string defaultValue)
        {
            string value;
            if (!_headers.TryGetValue(key, out value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Get peer headers table
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetHeaders()
        {
            return _headers;
        }

        /// <summary>
        /// Set peer headers from provided dictionary
        /// </summary>
        /// <returns></returns>
        public void SetHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
        }

        /// <summary>
        /// Check if messages were lost from peer, returns true if they were
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>true if we have lost one or more message</returns>
        public bool MessagesLost(ZreMsg msg)
        {
            Debug.Assert(msg != null);

            //  The sequence number set by the peer, and our own calculated
            //  sequence number should be the same.
            if (_verbose)
            {
                _verboseAction(string.Format("({0}) recv {1} from peer={2} sequence={3}", _origin, msg.Command, _name, msg.Sequence));
            }
            if (msg.Id == ZreMsg.MessageId.Hello)
            {
                //  HELLO always MUST have sequence = 1
                _wantSequence = 1;
            }
            else
            {
                _wantSequence = _wantSequence == UshortMax ? (ushort)0 : _wantSequence++;
            }
            if (_wantSequence != msg.Sequence)
            {
                if (_verboseAction != null)
                {
                    _verboseAction(string.Format("({0}) seq error from peer={1} expect={2}, got={3}", _origin, _name, _wantSequence, msg.Sequence));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ask peer to log all traffic via _verboseAction passed into the constructor
        /// Ignored (verbose is always false) if _verboseAction is null
        /// </summary>
        /// <param name="verbose">true means log all traffic</param>
        public void SetVerbose(bool verbose)
        {
            _verbose = _verboseAction != null && verbose;
        }

        public override string ToString()
        {
            var name = string.IsNullOrEmpty(_name) ? "NotSet" : _name;
            return string.Format("name:{0} router endpoint:{1} connected:{2} ready:{3} status:{4}", name, _endpoint, _connected, _ready, _status);
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

            if (_mailbox != null)
            {
                _mailbox.Dispose();
                _mailbox = null;
            }
        }
    }
}
