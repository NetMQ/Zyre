/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;

namespace NetMQ.Zyre.ZyreEvents
{
    public interface IZyreEventHeader
    {
        Guid SenderUuid { get; }
        string SenderName { get; }
    }

    public class ZyreEventHeader : IZyreEventHeader
    {
        public Guid SenderUuid { get; }
        public string SenderName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="senderUuid">The sending peer's identity</param>
        /// <param name="senderName">The sending peer's public name</param>
        public ZyreEventHeader(Guid senderUuid, string senderName)
        {
            SenderUuid = senderUuid;
            SenderName = senderName;
        }
    }
}
