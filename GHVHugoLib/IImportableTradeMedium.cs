using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHVHugoLib
{
    public interface IImportableTradeMedium
    {
        string TradeMediumName { get; }
        bool TicketChargeApplied { get; }
        double CommissionPerShare { get; }
        double SECFeePerDollar { get; }
    }
}
