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
    public class ZreNodeTests
    {
        // TODO: Use the new NetMQActor approach
        //[Test]
        //public void NewNodeTest()
        //{
        //    using (var node = ZreNode.NewNode(new PairSocket(), new PairSocket()))
        //    {
        //        node.Should().NotBeNull();
        //    }
        //}

        //[Test]
        //public void StartStopTest()
        //{
        //    // TODO: Re-enable this test after NetMQBeacon is changed. And change Stop from TrySend back to Send
        //    using (var pipe = new PairSocket())
        //    using (var outbox = new PairSocket())
        //    using (var node = ZreNode.NewNode(pipe, outbox))
        //    {
        //        var success = node.Start();
        //        success.Should().BeTrue();
        //        node.Stop();
        //        node.Destroy();
        //    }
        //}
    }
}