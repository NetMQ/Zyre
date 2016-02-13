/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NetMQ.Sockets;
using NetMQ.Zyre.ZyreEvents;

namespace NetMQ.Zyre
{
    /// <summary>
    /// zyre - API wrapping one Zyre node
    /// </summary>
    public class Zyre : IDisposable
    {
        public const int ZyreVersionMajor = 1;
        public const int ZyreVersionMinor = 1;
        public const int ZyreVersionPatch = 0;

        /// <summary>
        /// A Zyre instance wraps the actor instance
        /// All node control is done through _actor
        /// </summary>
        private readonly NetMQActor _actor;

        /// <summary>
        /// Receives incoming cluster traffic (traffic into Zyre from the network)
        /// </summary>
        private readonly PairSocket _inbox;

        /// <summary>
        /// Copy of node UUID
        /// </summary>
        private Guid _uuid;

        /// <summary>
        /// Copy of node name
        /// </summary>
        private string _name;

        /// <summary>
        /// Copy of last endpoint bound to
        /// </summary>
        private string _endpoint;

        private readonly NetMQPoller _inboxPoller;

        /// <summary>
        /// Create a Zyre API that communicates with a node on the ZRE bus.
        /// </summary>
        /// <param name="name">The name of the node</param>
        /// <param name="loggerDelegate">An action to take for logging when _verbose is true. Default is null.</param>
        public Zyre (string name, Action<string> loggerDelegate = null)
        {
            // Create front-to-back pipe pair for data traffic
            // outbox is passed to ZyreNode for sending Zyre message traffic back to _inbox
            PairSocket outbox;
            PairSocket.CreateSocketPair(out outbox, out _inbox);

            // Start node engine and wait for it to be ready
            // All node control is done through _actor

            _actor = ZyreNode.Create(outbox, loggerDelegate);
            _inboxPoller = new NetMQPoller();
            _inbox.ReceiveReady += InboxReceiveReady;
            _inboxPoller.RunAsync();

            // Send name, if any, to node ending
            if (!string.IsNullOrEmpty(name))
            {
                _actor.SendMoreFrame("SET NAME").SendFrame(name);
            }
        }

        /// <summary>
        /// Return our node UUID string, after successful initialization
        /// </summary>
        public Guid Uuid()
        {
            _actor.SendFrame("UUID");
            var uuidBytes = _actor.ReceiveFrameBytes();
            Debug.Assert(uuidBytes.Length == 16);
            _uuid = new Guid(uuidBytes);
            return _uuid;
        }

        /// <summary>
        /// Return our node endpoint, after successful initialization.
        /// </summary>
        /// <returns></returns>
        public string EndPoint()
        {
            _actor.SendFrame("ENDPOINT");
            _endpoint = _actor.ReceiveFrameString();
            return _endpoint;
        }

        /// <summary>
        /// Return our node name, after successful initialization. By default
        /// is taken from the UUID and shortened.
        /// </summary>
        /// <returns></returns>
        public string Name()
        {
            _actor.SendFrame("NAME");
            _name = _actor.ReceiveFrameString();
            return _name;
        }

        /// <summary>
        /// Set Name
        /// </summary>
        /// <param name="name">the name to set</param>
        public void SetName(string name)
        {
            _actor.SendMoreFrame("SET NAME").SendFrame(name);
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
            _actor.SendMoreFrame("SET HEADER").SendMoreFrame(key).SendFrame(value);
        }

        /// <summary>
        /// Set UDP beacon discovery port; defaults to 5670, this call overrides
        /// that so you can create independent clusters on the same network, for
        /// e.g. development vs. production. Has no effect after Zzyre.Start().
        /// </summary>
        /// <param name="port">the UDP beacon discovery port override</param>
        public void SetPort(int port)
        {
            _actor.SendMoreFrame("SET PORT").SendFrame(port.ToString());
        }

        /// <summary>
        /// Set UDP beacon discovery interval. Default is instant
        /// beacon exploration followed by pinging every 1,000 msecs.
        /// </summary>
        /// <param name="interval">beacon discovery interval</param>
        public void SetInterval(TimeSpan interval)
        {
            _actor.SendMoreFrame("SET INTERVAL").SendFrame(interval.ToString());
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
            _inboxPoller.Add(_inbox);
            Thread.Sleep(100);
            _actor.SendFrame("START");
        }

        /// <summary>
        /// Stop node; this signals to other peers that this node will go away.
        /// This is polite; however you can also just destroy the node without
        /// stopping it.
        /// </summary>
        public void Stop()
        {
            _actor.SendFrame("STOP");
            Thread.Sleep(1000); // wait for poller so we don't miss messages
            _inboxPoller.Remove(_inbox);
        }

        /// <summary>
        /// Join a named group; after joining a group you can send messages to
        /// the group and all Zyre nodes in that group will receive them.
        /// </summary>
        /// <param name="groupName">the name of the group</param>
        public void Join(string groupName)
        {
            _actor.SendMoreFrame("JOIN").SendFrame(groupName);
        }

        /// <summary>
        /// Leave a group.
        /// </summary>
        /// <param name="groupName">the name of the group</param>
        public void Leave(string groupName)
        {
            _actor.SendMoreFrame("LEAVE").SendFrame(groupName);
        }

        /// <summary>
        /// Receive next message from network; the message may be a control.
        /// message (ENTER, EXIT, JOIN, LEAVE) or data (WHISPER, SHOUT).
        /// </summary>
        /// <returns>message</returns>
        public NetMQMessage Receive()
        {
            return _inbox.ReceiveMultipartMessage();
        }

        /// <summary>
        /// Send message to single peer.
        /// </summary>
        /// <param name="peer">the peer who gets the message</param>
        /// <param name="message">the message</param>
        public void Whisper(Guid peer, NetMQMessage message)
        {
            _actor.SendMoreFrame("WHISPER").SendMoreFrame(peer.ToByteArray()).SendMultipartMessage(message);
        }

        /// <summary>
        /// Send message to a named group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        public void Shout(string groupName, NetMQMessage message)
        {
            _actor.SendMoreFrame("SHOUT").SendMoreFrame(groupName).SendMultipartMessage(message);
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
            _actor.SendMoreFrame("WHISPER").SendMoreFrame(peer.ToByteArray()).SendFrame(value);
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
            _actor.SendMoreFrame("WHISPER").SendMoreFrame(groupName).SendFrame(value);
        }

        /// <summary>
        /// Return list of current peers (their Uuids).
        /// </summary>
        /// <returns></returns>
        public List<Guid> Peers()
        {
            _actor.SendFrame("PEERS");
            var peersBuffer = _actor.ReceiveFrameBytes();
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
            _actor.SendMoreFrame("PEER ENDPOINT");
            _actor.SendFrame(peer.ToByteArray());
            return _actor.ReceiveFrameString();
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
            _actor.SendMoreFrame("PEER HEADER");
            _actor.SendMoreFrame(peer.ToByteArray());
            _actor.SendFrame(key);
            return _actor.ReceiveFrameString();
        }

        /// <summary>
        /// Return list of currently joined groups.
        /// </summary>
        /// <returns></returns>
        public List<string> OwnGroups()
        {
            _actor.SendFrame("OWN GROUPS");
            var result = Serialization.BinaryDeserialize<List<string>>(_actor.ReceiveFrameBytes());
            return result;
        }

        /// <summary>
        /// Return list of groups known through connected peers.
        /// </summary>
        /// <returns></returns>
        public List<string> PeerGroups()
        {
            _actor.SendFrame("PEER GROUPS");
            var bytes = _actor.ReceiveFrameBytes();
            var result = Serialization.BinaryDeserialize<List<string>>(bytes);
            return result;
        }

        /// <summary>
        /// Return the node socket, for direct polling of the socket
        /// </summary>
        public PairSocket Socket
        {
            get { return _inbox; }
        }

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

        public void Dump()
        {
            _actor.SendFrame("DUMP");
        }

        #region ZyreEvents
        // These events offer similar functionality to zeromq/zyre/zyre_event.c

        /// <summary>
        /// This receives a message relayed by ZyreNode.ReceivePeer()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InboxReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            var msg = Receive();
            if (msg.FrameCount < 3)
            {
                return;
            }
            var msgType = msg.Pop().ConvertToString();
            var senderBytes = msg.Pop().Buffer;
            Debug.Assert(senderBytes.Length == 16);
            var senderUuid = new Guid(senderBytes);
            var name = msg.Pop().ConvertToString();
            string groupName;
            switch (msgType)
            {
                case "ENTER":
                    var headersBuffer = msg.Pop().Buffer;
                    var headers = Serialization.BinaryDeserialize<Dictionary<string, string>>(headersBuffer);
                    var address = msg.Pop().ConvertToString();
                    var enterEvent = new ZyreEventEnter(senderUuid, name, headers, address);
                    OnEnterEvent(enterEvent);
                    break;
                case "WHISPER":
                    var whisperEvent = new ZyreEventWhisper(senderUuid, name, msg);
                    OnWhisperEvent(whisperEvent);
                    break;
                case "SHOUT":
                    groupName = msg.Pop().ConvertToString();
                    var shoutEvent = new ZyreEventShout(senderUuid, name, groupName, msg);
                    OnShoutEvent(shoutEvent);
                    break;
                case "JOIN":
                    groupName = msg.Pop().ConvertToString();
                    var joinEvent = new ZyreEventJoin(senderUuid, name, groupName);
                    OnJoinEvent(joinEvent);
                    break;
                case "LEAVE":
                    groupName = msg.Pop().ConvertToString();
                    var leaveEvent = new ZyreEventLeave(senderUuid, name, groupName);
                    OnLeaveEvent(leaveEvent);
                    break;
                case "EXIT":
                    OnExitEvent(new ZyreEventExit(senderUuid, name));
                    break;
                case "STOP":
                    OnStopEvent(new ZyreEventStop(senderUuid, name));
                    break;
                case "EVASIVE":
                    OnEvasiveEvent(new ZyreEventEvasive(senderUuid, name));
                    break;
                default:
                    throw new ArgumentException(msgType);
            }
        }

        public event EventHandler<ZyreEventEnter> EnterEvent;
        public event EventHandler<ZyreEventWhisper> WhisperEvent;
        public event EventHandler<ZyreEventShout> ShoutEvent;
        public event EventHandler<ZyreEventJoin> JoinEvent;
        public event EventHandler<ZyreEventLeave> LeaveEvent;
        public event EventHandler<ZyreEventExit> ExitEvent;
        public event EventHandler<ZyreEventStop> StopEvent;
        public event EventHandler<ZyreEventEvasive> EvasiveEvent;

        private void OnEvasiveEvent(ZyreEventEvasive evasiveEvent)
        {
            var temp = EvasiveEvent; // for thread safety
            temp?.Invoke(this, evasiveEvent);
        }

        private void OnExitEvent(ZyreEventExit exitEvent)
        {
            var temp = ExitEvent; // for thread safety
            temp?.Invoke(this, exitEvent);
        }

        private void OnStopEvent(ZyreEventStop stopEvent)
        {
            var temp = StopEvent; // for thread safety
            temp?.Invoke(this, stopEvent);
        }

        private void OnJoinEvent(ZyreEventJoin joinEvent)
        {
            var temp = JoinEvent; // for thread safety
            temp?.Invoke(this, joinEvent);
        }

        private void OnLeaveEvent(ZyreEventLeave leaveEvent)
        {
            var temp = LeaveEvent; // for thread safety
            temp?.Invoke(this, leaveEvent);
        }

        private void OnEnterEvent(ZyreEventEnter enterEvent)
        {
            var temp = EnterEvent; // for thread safety
            temp?.Invoke(this, enterEvent);
        }

        private void OnWhisperEvent(ZyreEventWhisper whisperEvent)
        {
            var temp = WhisperEvent; // for thread safety
            temp?.Invoke(this, whisperEvent);
        }

        private void OnShoutEvent(ZyreEventShout shoutEvent)
        {
            var temp = ShoutEvent; // for thread safety
            temp?.Invoke(this, shoutEvent);
        }

        #endregion ZyreEvents


        public override string ToString()
        {
            return $"name:{_name} endpoint:{_endpoint}";
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

            Stop();
            _inboxPoller?.StopAsync();
            _inboxPoller?.Dispose();
            _actor?.Dispose();
            _inbox?.Dispose();
        }
    }
}
