using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoException : Exception
    {
        public GHVHugoException() : base() { }
        public GHVHugoException(string msg) : base(msg) { }
        public GHVHugoException(string msg, Exception e) : base(msg, e) { }
        protected GHVHugoException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
    }
}
