using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePeer
{
    public class Header
    {
        public string Key { get; private set; }
        public string Value { get; private set; }

        public Header(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
