using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SamplePeer
{
    public class ConsoleLogger
    {
        private readonly string _nodeName;

        public ConsoleLogger(string nodeName)
        {
            _nodeName = nodeName;
        }

        public void ConsoleWrite(string str)
        {
            var msg = $"{DateTime.Now.ToString("h:mm:ss.fff")} ({_nodeName}) {str}";
            Console.WriteLine(msg);
        }
    }
}
