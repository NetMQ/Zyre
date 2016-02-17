/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
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
            using (var actor = ZyreNode.Create(new PairSocket()))
            {
                actor.Should().NotBeNull();
            }
        }
    }
}