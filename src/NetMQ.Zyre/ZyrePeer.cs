/* This Source Code Form is subject to the terms of the Mozilla internal
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NetMQ.Sockets;

namespace NetMQ.Zyre
{
    public class ZyrePeer : IDisposable
    {
        private const int PeerEvasive = 10000; // 10 seconds' silence is evasive
        private const int PeerExpired = 30000; // 30 seconds' silence is expired
        private const ushort UshortMax = ushort.MaxValue;
        private const byte UbyteMax = byte.MaxValue;

        #region Private Variables

        /// <summary>
        /// Socket through to peer.
        /// DealerSocket connected to the peer's RouterSocket.
        /// ZRE messages are sent from this _mailbox for each peer
        ///    to the peer's RouterSocket _inbox.
        /// </summary>
        private DealerSocket _mailbox;

        /// <summary>
        /// Peer's internal name
        /// </summary>
        private string _name;

        /// <summary>
        /// Origin node's internal name
        /// </summary>
        private string _origin;

        /// <summary>
        /// Outgoing message sequence counter used for detecting lost messages
        /// </summary>
        private ushort _sentSequence;

        /// <summary>
        /// Incoming message sequence counter used for detecting lost messages
        /// </summary>
        private ushort _wantSequence;

        /// <summary>
        /// Optional logger action passed into ctor
        /// </summary>
        private readonly Action<string> _loggerDelegate;

        #endregion Private Variables


        private ZyrePeer(Guid uuid, Action<string> loggerDelegate = null)
        {
            Uuid = uuid;
            _loggerDelegate = loggerDelegate;
            Ready = false;
            Connected = false;
            _sentSequence = 0;
            _wantSequence = 0;
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Construct new ZyrePeer object
        /// </summary>
        /// <param name="container">The dictionary of peers</param>
        /// <param name="guid">The identity for this peer</param>
        /// <param name="loggerDelegate">An action to take for logging when _verbose is true. Default is null.</param>
        /// <returns></returns>
        internal static ZyrePeer NewPeer(Dictionary<Guid, ZyrePeer> container, Guid guid, Action<string> loggerDelegate = null)
        {
            var peer = new ZyrePeer(guid, loggerDelegate);
            container[guid] = peer; // overwrite any existing entry for same uuid
            return peer;
        }

        /// <summary>
        /// Disconnect this peer and Dispose this class.
        /// </summary>
        internal void Destroy()
        {
            Disconnect();
            Dispose();
        }

        /// <summary>
        /// Connect peer mailbox
        /// Configures a DealerSocket mailbox connected to peer's router endpoint
        /// </summary>
        /// <param name="replyTo"></param>
        /// <param name="endpoint"></param>
        internal void Connect(Guid replyTo, string endpoint)
        {
            Debug.Assert(!Connected);

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

                    // SendTimeout = TimeSpan.Zero Instead of this, ZreMsg.Send() uses TrySend() with TimeSpan.Zero
                }
            };
            Endpoint = endpoint;
            Connected = true;
            Ready = false;
            _loggerDelegate?.Invoke($"{nameof(ZyrePeer)}.{nameof(Connect)}() has connected its DealerSocket mailbox to peer={this}");
        }

        internal static byte[] GetIdentity(Guid replyTo)
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
        internal void Disconnect()
        {
            if (Connected)
            {
                _mailbox.Dispose();
                _mailbox = null;
                Endpoint = null;
                Connected = false;
                Ready = false;
            }
        }

        /// <summary>
        /// Send message to peer
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>always true</returns>
        internal bool Send(ZreMsg msg)
        {
            if (Connected)
            {
                msg.Sequence = ++_sentSequence;
                _loggerDelegate?.Invoke($"{nameof(ZyrePeer)}.{nameof(Send)}() sending message={msg} to Endpoint={Endpoint}");
                var tmp = _mailbox.Options.Identity;
                var success = msg.Send(_mailbox);
                if (!success)
                {
                    _loggerDelegate?.Invoke($"{nameof(ZyrePeer)}.{nameof(Send)}() UNABLE to send message={msg} to Endpoint={Endpoint}");
                }
            }
            return true;
        }

        /// <summary>
        /// Return peer connected status. True when connected and peer will send messages.
        /// </summary>
        internal bool Connected { get; private set; }

        /// <summary>
        /// Identity guid of the peer, 16 bytes
        /// </summary>
        internal Guid Uuid { get; }

        /// <summary>
        /// Return peer connection endpoint. (The RouterSocket address listening to messages from this peer.)
        /// </summary>
        internal string Endpoint { get; private set; }

        /// <summary>
        /// Register activity at peer
        /// </summary>
        internal void Refresh()
        {
            EvasiveAt = CurrentTimeMilliseconds() + PeerEvasive;
            ExpiredAt = CurrentTimeMilliseconds() + PeerExpired;
        }

        /// <summary>
        /// Milliseconds since January 1, 1970 UTC
        /// </summary>
        /// <returns></returns>
        internal static long CurrentTimeMilliseconds()
        {
            return (DateTime.UtcNow.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Return peer future expired time
        /// </summary>
        internal long EvasiveAt { get; private set; }

        /// <summary>
        /// Return peer future evasive time
        /// </summary>
        internal long ExpiredAt { get; private set; }

        /// <summary>
        /// Return peer name
        /// </summary>
        internal string Name => _name ?? "";

        /// <summary>
        /// Return peer cycle
        /// This gives us a state change count for the peer, which we can
        /// check against its claimed status, to detect message loss.
        /// </summary>
        internal byte Status { get; private set; }

        /// <summary>
        /// Increment status
        /// </summary>
        internal void IncrementStatus()
        {
            Status = Status == UbyteMax ? (byte) 0 : ++Status;
        }

        /// <summary>
        /// Set peer name
        /// </summary>
        /// <param name="name"></param>
        internal void SetName(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Set current node name, for logging
        /// </summary>
        /// <param name="originNodeName"></param>
        internal void SetOrigin(string originNodeName)
        {
            _origin = originNodeName;
        }

        /// <summary>
        /// Set peer status
        /// </summary>
        /// <param name="status"></param>
        internal void SetStatus(byte status)
        {
            Status = status;
        }

        /// <summary>
        /// Return peer ready state. True when peer has said Hello to us
        /// </summary>
        internal bool Ready { get; set; }

        /// <summary>
        /// Get peer header value
        /// </summary>
        /// <param name="key">The he</param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal string Header(string key, string defaultValue)
        {
            string value;
            if (!Headers.TryGetValue(key, out value))
            {
                return defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Get or set peer headers table
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Check if messages were lost from peer, returns true if they were
        /// </summary>
        /// <param name="msg"></param>
        /// <returns>true if we have lost one or more message</returns>
        internal bool MessagesLost(ZreMsg msg)
        {
            Debug.Assert(msg != null);

            //  The sequence number set by the peer and our own calculated sequence number should be the same.
            if (msg.Id == ZreMsg.MessageId.Hello)
            {
                //  HELLO always MUST have sequence = 1
                _wantSequence = 1;
            }
            else
            {
                _wantSequence = _wantSequence == UshortMax ? (ushort) 0 : ++_wantSequence;
            }
            if (_wantSequence != msg.Sequence)
            {
                if (_loggerDelegate != null)
                {
                    _loggerDelegate($"Sequence error for peer={_name} expect={_wantSequence}, got={msg.Sequence}");
                }
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            var name = string.IsNullOrEmpty(_name) ? "NotSet" : _name;
            var origin = string.IsNullOrEmpty(_origin) ? "NotSet" : _origin;
            return
                $"[from origin:{origin} to name:{name} endpoint:{Endpoint} connected:{Connected} ready:{Ready} status:{Status} _sentSeq:{_sentSequence} _wantSeq:{_wantSequence} _ guidShort:{Uuid.ToShortString6()}]";
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

            _mailbox?.Dispose();
        }
    }
}
