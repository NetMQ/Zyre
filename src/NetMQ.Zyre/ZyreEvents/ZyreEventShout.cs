/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;

namespace NetMQ.Zyre.ZyreEvents
{
    public class ZyreEventShout : EventArgs, IZyreEventHeader
    {
        /// <summary>
        /// The message being shouted.
        /// </summary>
        public NetMQMessage Content { get; set; }

        /// <summary>
        /// The name of the group to which the sending peer is shouting
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The sending peer's identity.
        /// </summary>
        public Guid SenderUuid => _header.SenderUuid;


        /// <summary>
        /// The sending peer's public name.
        /// </summary>
        public string SenderName => _header.SenderName;

        private readonly ZyreEventHeader _header;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="senderUuid">The sending peer's identity</param>
        /// <param name="senderName">The sending peer's public name</param>
        /// <param name="groupName">The name of the group to which the sending peer is shouting</param>
        /// <param name="content">The message being shouted</param>
        public ZyreEventShout(Guid senderUuid, string senderName, string groupName, NetMQMessage content)
        {
            Content = content;
            GroupName = groupName;
            _header = new ZyreEventHeader(senderUuid, senderName);
        }
    }
}
