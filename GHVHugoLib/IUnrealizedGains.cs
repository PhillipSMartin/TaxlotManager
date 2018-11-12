using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHVHugoLib
{
    public interface IUnrealizedGains
    {
        int InstrumentId { get; }
        InstrumentType InstrumentTypeId { get; }
        DateTime Open_Date { get; }
        double Unit_Cost { get; }
        double Open_Amount { get; set; }
        double TotalCost { get; }
        double CurrentPrice { get; set; }
        double MarketValue { get; }
        double GainOrLoss { get; }
        string PriceOverrideFlag { get; }
        DateTime PriceOverrideDate { get; }
        string PriceOverrideUser { get; }
        double OriginalPrice { get; }
    }
}
