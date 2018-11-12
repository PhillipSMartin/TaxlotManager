using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHVHugoLib
{
    public interface IRealizedGains
    {
        double CostBasis { get; }
        double NetProceeds { get; }
        double ShortTermGainLoss { get; }
        double LongTermGainLoss { get; }
    }
}
