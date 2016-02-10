using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZreZyreTest
    {
        private void ConsoleWrite(string str)
        {
            Console.WriteLine(str);
        }

        /// <summary>
        /// Parallel to zeromq/zyre zyre.c zyre_test()
        /// except it uses gossip, which we don't support
        /// </summary>
        [Test]
        public void ZyreTests()
        {
            // Create two nodes
            using (var node1 = new Zyre("node1", ConsoleWrite))
            using (var node2 = new Zyre("node2", ConsoleWrite))
            {
                int major, minor, patch;
                node1.Version(out major, out minor, out patch);
                major.Should().Be(Zyre.ZyreVersionMajor);

                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");
                node2.Should().NotBeNull();
                node2.Name().Should().Be("node2");

                node1.SetHeader("X-HELLO", "World");
                node1.SetVerbose();

                // Start both nodes
                node1.Start();
                node2.Start();

                var uuid1 = node1.Uuid;
                uuid1.Should().NotBeEmpty();
                var uuid2 = node2.Uuid;
                uuid1.Should().NotBe(uuid2);

                node1.Join("GLOBAL");
                node2.Join("GLOBAL");

                //  Give time for them to interconnect
                Thread.Sleep(100);
                node1.Dump();

                var peers = node1.Peers();
                peers.Count.Should().Be(1);

                node1.Join("node1 group of one");
                node2.Join("node2 group of one");

                // Give them time to join their groups
                Thread.Sleep(100);

                var peerGroups = node1.PeerGroups();
                peerGroups.Count.Should().Be(2);

                var value = node1.PeerHeaderValue(node1.Uuid, "X-HELLO");
                value.Should().Be("World");

                // One node shouts to GLOBAL
                node1.Shouts("GLOBAL", "Hello, World");

                // Second node should receive ENTER, JOIN, and SHOUT
                var msg = node2.Receive();
                msg.Should().NotBeNull();
                var command = msg.Pop().ConvertToString();
                command.Should().Be("ENTER");
                msg.FrameCount.Should().Be(4);
                var peerId = msg.Pop().ConvertToString();
                var name = msg.Pop().ConvertToString();
                name.Should().Be("node1");
                var headersFrame = msg.Pop();
                var address = msg.Pop().ConvertToString();
                var endpoint = msg.Pop().ConvertToString();

                headersFrame.Should().NotBeNull();

                node2.Stop();
                node1.Stop();

                ConsoleWrite("OK");
            }
        }
    }
}