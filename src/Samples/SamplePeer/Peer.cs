using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePeer
{
    public class Peer
    {
        public Guid SenderUuid { get;  }
        public string SenderName { get; }

        public Peer(string senderName, Guid senderUuid)
        {
            SenderName = senderName;
            SenderUuid = senderUuid;
        }
    }
}
