using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoDataException : GHVHugoException
    {
        public GHVHugoDataException() : base() { }
        public GHVHugoDataException(string fieldName) : base("Invalid value specified for " + fieldName) { }
        public GHVHugoDataException(string fieldName, Exception e) : base("Invalid value specified for " + fieldName, e) { }
        protected GHVHugoDataException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
   }
}
