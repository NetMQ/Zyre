using System;

namespace SamplePeer
{
    public class Peer
    {
        public Guid SenderUuid { get;  }
        public string Address { get; set; }
        public string SenderName { get; }

        public Peer(string senderName, Guid senderUuid, string address)
        {
            SenderName = senderName;
            SenderUuid = senderUuid;
            Address = address;
        }
    }
}
