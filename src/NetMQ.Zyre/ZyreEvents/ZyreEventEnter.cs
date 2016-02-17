/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
using System;
using System.Collections.Generic;

namespace NetMQ.Zyre.ZyreEvents
{
    public class ZyreEventEnter : EventArgs, IZyreEventHeader
    {
        /// <summary>
        /// The sending peer's identity.
        /// </summary>
        public Guid SenderUuid => _header.SenderUuid;


        /// <summary>
        /// The sending peer's public name.
        /// </summary>
        public string SenderName => _header.SenderName;

        /// <summary>
        /// The sending peer's headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// The sending peer's EndPoint.
        /// </summary>
        public string Address { get; set; }

        private readonly ZyreEventHeader _header;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="senderUuid">The sending peer's identity</param>
        /// <param name="senderName">The sending peer's public name</param>
        /// <param name="headers">The sending peer's headers</param>
        /// <param name="address">The full 0MQ address, or endpoint, e.g. "tcp://127.0.0.1:49520</param>
        public ZyreEventEnter(Guid senderUuid, string senderName, Dictionary<string, string> headers, string address)
        {
            Headers = headers;
            Address = address;
            _header = new ZyreEventHeader(senderUuid, senderName);
        }
    }
}
