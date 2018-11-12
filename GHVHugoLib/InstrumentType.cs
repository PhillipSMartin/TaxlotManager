using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHVHugoLib
{
    public enum InstrumentType
    {
        Stock = 0,
        Call = 1,
        Put = 2,
        Future = 3,
        Option = 4   // used for datagrids only, when items could be either a call or a put
    }
}