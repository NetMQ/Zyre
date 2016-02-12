using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMQ.Zyre.ZyreEvents
{
    public interface IZyreEventHeader
    {
        Guid SenderUuid { get; }
        string SenderName { get; }
    }

    public enum ZyreEventType
    {
        // A new peer has entered the network
        Enter,

        // A peer has left the network
        Exit,

        // A peer has joined a specific group
        Join,

        // A peer has left a specific group
        Leave,

        // A peer has sent this node a message
        Whisper,

        // A peer has sent one of our groups a message
        Shout,

        // Our node has been stopped
        Stop,

        // A peer is being evasive (quiet for too long)
        Evasive
    }

    public class ZyreEventHeader : IZyreEventHeader
    {
        public Guid SenderUuid { get; }
        public string SenderName { get; }

        public ZyreEventHeader(Guid senderUuid, string senderName)
        {
            SenderUuid = senderUuid;
            SenderName = senderName;
        }
    }
}
