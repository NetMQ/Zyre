using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZrePeerTests
    {
        private void ConsoleWrite(string str)
        {
            Console.WriteLine(str);
        }

        /// <summary>
        /// Parallel to zeromq/zyre zyre_peer.c zyre_peer_test()
        /// </summary>
        [Test]
        public void ZyrePeerTests()
        {
            var peers = new Dictionary<Guid, ZrePeer>();
            var me = Guid.NewGuid();
            var you = Guid.NewGuid();
            using (var peer = ZrePeer.NewPeer(peers, you, ConsoleWrite))
            {
                using (var mailbox = new RouterSocket("tcp://127.0.0.1:5551")) // RouterSocket default action binds to the address
                {
                    peer.Connected.Should().BeFalse();
                    peer.SetName("PeerYou");
                    peer.Name.Should().Be("PeerYou");
                    peer.Connect(me, "tcp://127.0.0.1:5551"); // create a DealerSocket connected to router on 5551
                    peer.Connected.Should().BeTrue();
                    var helloMsg = new ZreMsg
                    {
                        Id = ZreMsg.MessageId.Hello,
                        Hello =
                        {
                            Endpoint = "tcp://127.0.0.1:5552",
                            Name = "PeerMe"
                        },
                    };
                    var success = peer.Send(helloMsg);
                    success.Should().BeTrue();

                    var msg = new ZreMsg();
                    msg.Receive(mailbox);
                    var identityMe = ZrePeer.GetIdentity(me);
                    var routingEqual = msg.RoutingId.SequenceEqual(identityMe);
                    routingEqual.Should().BeTrue();
                    var hello = msg.Hello;
                    hello.Version.Should().Be(2);
                    hello.Sequence.Should().Be(1);
                    hello.Endpoint.Should().Be("tcp://127.0.0.1:5552");
                    hello.Status.Should().Be(0);
                    hello.Name.Should().Be("PeerMe");
                    hello.Headers.Count.Should().Be(0);
                }
            }
        }
    }
}
