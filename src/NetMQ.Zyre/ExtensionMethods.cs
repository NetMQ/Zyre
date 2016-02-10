using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMQ.Zyre
{
    public static class ExtensionMethods
    {
        public static string ToShortString6(this Guid guid)
        {
            var str = guid.ToString();
            return str.Substring(0, 6);
        }
    }
}
