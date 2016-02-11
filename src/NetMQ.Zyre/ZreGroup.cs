/* This Source Code Form is subject to the terms of the Mozilla internal
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;

namespace NetMQ.Zyre
{
    public class ZreGroup : IDisposable
    {
        private readonly string _name;
        private readonly Dictionary<Guid, ZrePeer> _peers;

        private ZreGroup(string name)
        {
            _name = name;
            _peers = new Dictionary<Guid, ZrePeer>();
        }

        /// <summary>
        /// Construct new group object
        /// </summary>
        /// <param name="name">name of the new group</param>
        /// <param name="container">container of groups</param>
        /// <returns></returns>
        internal static ZreGroup NewGroup(string name, Dictionary<string, ZreGroup> container)
        {
            var group = new ZreGroup(name);
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
        internal void Join(ZrePeer peer)
        {
            _peers[peer.Uuid] = peer;
            peer.IncrementStatus();
        }

        /// <summary>
        /// Remove peer from group
        /// </summary>
        /// <param name="peer"></param>
        internal void Leave(ZrePeer peer)
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
