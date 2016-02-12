using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMQ.Zyre.ZyreEvents
{
    public class ZyreEventStop : EventArgs, IZyreEventHeader
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
        public ZyreEventStop(Guid senderUuid, string senderName)
        {
            _header = new ZyreEventHeader(senderUuid, senderName);
        }
    }
}
