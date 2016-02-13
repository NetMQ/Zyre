using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMQ.Zyre.ZyreEvents
{
    public class ZyreEventJoin : EventArgs, IZyreEventHeader
    {
        /// <summary>
        /// The name of the group the sending peer is joining
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
        /// <param name="groupName">The name of the group the sending peer is joining</param>
        public ZyreEventJoin(Guid senderUuid, string senderName, string groupName)
        {
            GroupName = groupName;
            _header = new ZyreEventHeader(senderUuid, senderName);
        }
    }
}
