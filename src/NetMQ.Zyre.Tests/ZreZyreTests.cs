using System;
using System.Collections.Generic;
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
            using (var node1 = new Zyre("node1", false, node1Writer.ConsoleWrite))
            {
                node1.Start();
                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");
                node1.SetHeader("X-HELLO", "World");
                var uuid1 = node1.Uuid();
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
            using (var node1 = new Zyre("node1", false, node1Writer.ConsoleWrite))
            using (var node2 = new Zyre("node2", false, node2Writer.ConsoleWrite))
            {
                int major, minor, patch;
                node1.Version(out major, out minor, out patch);
                major.Should().Be(Zyre.ZyreVersionMajor);

                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");

                node1.SetHeader("X-HELLO", "World");

                // Start both nodes
                node1.Start();
                var uuid1 = node1.Uuid();
                uuid1.Should().NotBeEmpty();
                node1Writer.ConsoleWrite("After starting node1, Dump():");
                node1.Dump();

                node2.Should().NotBeNull();
                node2.Name().Should().Be("node2");

                node2.Start();
                var uuid2 = node2.Uuid();
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

                var node1Peers = node1.Peers();
                node1Peers.Count.Should().Be(1);

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

                var node2Peers = node2.Peers();
                node2Peers.Count.Should().Be(1);
                var value = node2.PeerHeaderValue(uuid1, "X-HELLO");
                value.Should().Be("World");

                // One node shouts to GLOBAL
                Console.WriteLine("node1 Shouting to Global");
                node1.Shouts("GLOBAL", "Hello, World");

                // Second node should receive ENTER, JOIN, JOIN and SHOUT
                var msg = node2.Receive();
                msg.Should().NotBeNull();
                var command = msg.Pop().ConvertToString();
                command.Should().Be("ENTER");
                msg.FrameCount.Should().Be(4);
                var peerIdBytes = msg.Pop().Buffer;
                var peerId = new Guid(peerIdBytes);
                peerId.Should().Be(uuid1);
                var name = msg.Pop().ConvertToString();
                name.Should().Be("node1");

                var headersBuffer = msg.Pop().Buffer;
                var headers = Serialization.BinaryDeserialize<Dictionary<string, string>>(headersBuffer);
                var address = msg.Pop().ConvertToString();
                headers.Count.Should().Be(1);
                headers["X-HELLO"].Should().Be("World");
                address.Should().NotBeNullOrEmpty();

                msg = node2.Receive();
                msg.Should().NotBeNull();
                command = msg.Pop().ConvertToString();
                command.Should().Be("JOIN");
                msg.FrameCount.Should().Be(3);

                msg = node2.Receive();
                msg.Should().NotBeNull();
                command = msg.Pop().ConvertToString();
                command.Should().Be("JOIN");
                msg.FrameCount.Should().Be(3);

                msg = node2.Receive();
                msg.Should().NotBeNull();
                command = msg.Pop().ConvertToString();
                command.Should().Be("SHOUT");

                Console.WriteLine("Stopping node2");
                node2.Stop();
                msg = node2.Receive();
                msg.Should().NotBeNull();
                command = msg.Pop().ConvertToString();
                command.Should().Be("STOP");

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