using System.Threading;
using FluentAssertions;
using NetMQ.Sockets;
using NUnit.Framework;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZreNodeTests
    {
       [Test]
        public void NewNodeTest()
        {
            using (var actor = ZreNode.Create(new PairSocket()))
            {
                actor.Should().NotBeNull();
            }
        }
    }
}