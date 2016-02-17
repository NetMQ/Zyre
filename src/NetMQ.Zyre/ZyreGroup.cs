/* This Source Code Form is subject to the terms of the Mozilla internal
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;

namespace NetMQ.Zyre
{
    public class ZyreGroup : IDisposable
    {
        private readonly string _name;
        private readonly Dictionary<Guid, ZyrePeer> _peers;

        private ZyreGroup(string name)
        {
            _name = name;
            _peers = new Dictionary<Guid, ZyrePeer>();
        }

        /// <summary>
        /// Construct new group object
        /// </summary>
        /// <param name="name">name of the new group</param>
        /// <param name="container">container of groups</param>
        /// <returns>the new group</returns>
        internal static ZyreGroup NewGroup(string name, Dictionary<string, ZyreGroup> container)
        {
            var group = new ZyreGroup(name);
            container[name] = group;
            return group;
        }

        /// <summary>
        /// Dispose this class and all objects it holds
        /// </summary>
        internal void Destroy()
        {
            Dispose();
        }

        /// <summary>
        /// Add peer to group
        /// Ignore duplicate joins
        /// </summary>
        /// <param name="peer"></param>
        internal void Join(ZyrePeer peer)
        {
            _peers[peer.Uuid] = peer;
            peer.IncrementStatus();
        }

        /// <summary>
        /// Remove peer from group
        /// </summary>
        /// <param name="peer"></param>
        internal void Leave(ZyrePeer peer)
        {
            _peers.Remove(peer.Uuid);
            peer.IncrementStatus();
        }

        /// <summary>
        /// Send message to all peers in group
        /// </summary>
        /// <param name="msg"></param>
        internal void Send(ZreMsg msg)
        {
            foreach (var peer in _peers.Values)
            {
                peer.Send(msg);
            }
        }

        public override string ToString()
        {
            return _name;
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

            foreach (var peer in _peers.Values)
            {
                peer.Dispose();
            }
        }
    }
}
