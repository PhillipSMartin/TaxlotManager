using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoNotInitializedException : GHVHugoException
    {
        public GHVHugoNotInitializedException() : base() { }
        public GHVHugoNotInitializedException(string msg) : base(msg) { }
        public GHVHugoNotInitializedException(string msg, Exception e) : base(msg, e) { }
        protected GHVHugoNotInitializedException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
   }
}
