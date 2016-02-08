using FluentAssertions;
using NUnit.Framework;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZreZyreTest
    {
        /// <summary>
        /// Parallel to zeromq/zyre zyre.c zyre_test()
        /// except it uses gossip, which we don't support
        /// </summary>
        [Test]
        public void ZyreTests()
        {

            // Create two nodes
            using (var node1 = new Zyre("node1"))
            using (var node2 = new Zyre("node2"))
            {
                int major, minor, patch;
                node1.Version(out major, out minor, out patch);
                major.Should().Be(Zyre.ZyreVersionMajor);

                node1.Should().NotBeNull();
                node1.Name().Should().Be("node1");
                node2.Should().NotBeNull();
                node2.Name().Should().Be("node2");

                node1.SetHeader("X-HELLO", "World");




            }
        }
    }
}