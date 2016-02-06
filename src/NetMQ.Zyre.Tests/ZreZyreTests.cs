using NUnit.Framework;
using NetMQ.Zyre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NetMQ.Sockets;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class ZreZyreTest
    {
        /// <summary>
        /// Parallel to zeromq/zyre zyre.c zyre_test()
        /// </summary>
        [Test]
        public void ZyreTests()
        {
            //TODO using (var node = ZreNode.NewNode(new PairSocket(), new PairSocket()))
            //{
            //    node.Should().NotBeNull();
            //}
        }

    }
}