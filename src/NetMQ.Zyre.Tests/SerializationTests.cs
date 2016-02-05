using NUnit.Framework;
using NetMQ.Zyre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;

namespace NetMQ.Zyre.Tests
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void ListStringTest()
        {
            var stringsIn = new List<string> { "Item1", "Item2" };
            var buffer = Serialization.Serialize(stringsIn);
            var stringsOut = Serialization.DeserializeListString(buffer);
            stringsOut.Should().NotBeNull();
            stringsOut.Count.Should().Be(2);
            stringsOut[0].Should().Be("Item1");
            stringsOut[1].Should().Be("Item2");
        }

        [Test]
        public void DictionaryStringStringTest()
        {
            var dictIn = new Dictionary<string, string> { { "Key1", "Value1" }, {"Key2", "Value2"} };
            var buffer = Serialization.Serialize(dictIn);
            var dictOut = Serialization.DeserializeDictStringString(buffer);
            dictOut.Should().NotBeNull();
            dictOut.Count.Should().Be(2);
            dictOut.Keys.First().Should().Be("Key1");
            dictOut.Values.First().Should().Be("Value1");
            dictOut.Keys.Last().Should().Be("Key2");
            dictOut.Values.Last().Should().Be("Value2");
        }
    }
}