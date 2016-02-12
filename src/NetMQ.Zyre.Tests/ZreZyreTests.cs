using System;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZreZyreTest
    {
        private class ConsoleLogger
        {
            private readonly string _nodeName;

            public ConsoleLogger(string nodeName)
            {
                _nodeName = nodeName;
            }

            public void ConsoleWrite(string str)
            {
                var msg = string.Format("{0} ({1}) {2}", DateTime.Now.ToString("h:mm:ss.fff"), _nodeName, str);
                Console.WriteLine(msg);
            }
        }

        [Test]
        public void SimpleStartStopOneNode()
        {
            var node1Writer = new ConsoleLogger("node1");
            using (var node1 = new Zyre("node1", node1Writer.ConsoleWrite))
            {
                node1.Start();
                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");
                node1.SetHeader("X-HELLO", "World");
                var uuid1 = node1.Uuid;
                uuid1.Should().NotBeEmpty();
                Thread.Sleep(100);
                node1Writer.ConsoleWrite("After starting node1, Dump():");
                node1.Dump();
                Thread.Sleep(100);

                Console.WriteLine("Stopping node1");
                node1.Stop();
                Thread.Sleep(100);
                node1.Dump();
            }
        }

        /// <summary>
        /// Parallel to zeromq/zyre zyre.c zyre_test()
        /// except it uses gossip, which we don't support
        /// </summary>
        [Test]
        public void FullTestLikeZeroMqZyre()
        {
            var node1Writer = new ConsoleLogger("node1");
            var node2Writer = new ConsoleLogger("node2");

            // Create two nodes
            using (var node1 = new Zyre("node1", node1Writer.ConsoleWrite))
            using (var node2 = new Zyre("node2", node2Writer.ConsoleWrite))
            {
                int major, minor, patch;
                node1.Version(out major, out minor, out patch);
                major.Should().Be(Zyre.ZyreVersionMajor);

                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");

                node1.SetHeader("X-HELLO", "World");

                // Start both nodes
                node1.Start();
                var uuid1 = node1.Uuid;
                uuid1.Should().NotBeEmpty();
                node1Writer.ConsoleWrite("After starting node1, Dump():");
                node1.Dump();

                node2.Should().NotBeNull();
                node2.Name().Should().Be("node2");

                node2.Start();
                var uuid2 = node2.Uuid;
                uuid1.Should().NotBe(uuid2);
                Console.WriteLine("After starting node2, Dump():");
                node2.Dump();

                //  Give time for them to interconnect. Our default is beacons every second
                Thread.Sleep(1100);
                Console.WriteLine("After Thread.Sleep(1100):");

                Console.WriteLine("node1 Joining Global");
                node1.Join("GLOBAL");
                Console.WriteLine("node2 Joining Global");
                node2.Join("GLOBAL");
                node1.Dump();
                node2.Dump();

                var peers = node1.Peers();
                peers.Count.Should().Be(1);

                Console.WriteLine("node1 Joining node1 group of one");
                node1.Join("node1 group of one");
                Console.WriteLine("node2 Joining node1 group of one");
                node2.Join("node2 group of one");

                // Give them time to join their groups
                Thread.Sleep(100);
                Console.WriteLine("After 2nd Thread.Sleep(100):");
                node1.Dump();
                node2.Dump();

                var peerGroups = node1.PeerGroups();
                peerGroups.Count.Should().Be(2);

                var value = node1.PeerHeaderValue(node1.Uuid, "X-HELLO");
                value.Should().Be("World");

                // One node shouts to GLOBAL
                Console.WriteLine("node1 Shouting to Global");
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

                Console.WriteLine("Stopping node2");
                node2.Stop();
                Console.WriteLine("Stopping node1");
                node1.Stop();
                Thread.Sleep(100);
                Console.WriteLine("After Stopping both:");
                node1.Dump();
                node2.Dump();

                Console.WriteLine("OK");
            }
        }
    }
}