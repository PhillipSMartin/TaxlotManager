using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReconciliationLib;

namespace GHVHugoLib
{
    public interface IGHVStockTrade : IStockTrade
    {
        // set accessors for IStockTrade properties
        new string SubAcctName { get;  set; }
        new string TaxLotId { get; set; }
        new double TotalCost { get; set; }
        new DateTime TradeDate { get; set; }
        new string TradeMedium { get; set; }
        new string TradeNote { get; set; }
        new string TraderName { get; set; }
        new string TradeType { get; set; }
        new double UnitCost { get; set; }
        new string UnderlyingSymbol { get; set; }

        // new properties
        string MerrillAccountNumber { get; }
        string ClearingHouseTaxlotId { get; } // Merrill's Taxlot Id
        DateTime DateOpened { get; } // date taxlot was opened
        decimal OpeningTradePrice { get; } // trade price of opening trade of the taxlot
        string OpeningTradeType { get; } // 'Buy' or 'Sell' for opening trade of the taxlot
        bool IsValid { get; }
    }
}
