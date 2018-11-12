using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHVHugoLib
{
    public interface ITaxLot : IUnrealizedGains, ITaxlotId
    {
        string Ticker { get; }
        int Score { get; set; }
        string LinkedTaxLotId { get; set;  }
    }
}
