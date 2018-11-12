using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GHVHugoLib
{
    [Serializable]
    public class GHVHugoColumnNotFoundException : GHVHugoException
    {
        public GHVHugoColumnNotFoundException() : base() { }
        public GHVHugoColumnNotFoundException(string columnName) : base("Column " + columnName + " not found") { }
        public GHVHugoColumnNotFoundException(string columnName, Exception e) : base("Column " + columnName + " not found", e) { }
        protected GHVHugoColumnNotFoundException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
    }
}
