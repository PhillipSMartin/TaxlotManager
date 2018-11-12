using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoCommandNotFoundException : GHVHugoException
    {
        public GHVHugoCommandNotFoundException() : base() { }
        public GHVHugoCommandNotFoundException(string commandName) : base("Command " + commandName + " not found") { }
        public GHVHugoCommandNotFoundException(string commandName, Exception e) : base("Command " + commandName + " not found", e) { }
        protected GHVHugoCommandNotFoundException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
    }
}
