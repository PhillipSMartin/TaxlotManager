using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ReconciliationLib;

namespace GHVHugoLib
{
    public class StockTrade : IGHVStockTrade, IImportableTradeMedium
    {
        #region Declarations
        private static string tradeDateColumnName = "Trade Date";
        private static string tradeSideColumnName = "Side";
        private static string tradeVolumeColumnName = "Number of Shares";
        private static string tickerColumnName = "Symbol";
        private static string unitCostColumnName = "Average Price";
        private static string totalCostColumnName = "Net Amount";
        private static TimeSpan tradeTimeOfDay = new TimeSpan(16, 0, 0);
        private static double ticketCharge = 7.5;
        private static IImportableTradeMedium importableTradeMedium;

        private DateTime tradeDate = new DateTime(0);
        private string tradeSide;
        private double tradeVolume;
        private string ticker;
        private double unitCost;
        private double totalCost;
        private string taxLotId;
        private string tradeMedium;
        private string subAcctName;
        private string traderName;
        private string tradeNote = "imported by GHVHugoLib";
        private bool isValid;
        #endregion

        #region Constructors
        // instantiate from an excel datarow
        public StockTrade(DataRow dataRow)
        {
            if (!Convert.IsDBNull(dataRow[tickerColumnName]))
            {
                string currentColumnName = tickerColumnName;
                try
                {
                    ticker = Convert.ToString(dataRow[tickerColumnName]);


                    if (ticker[0] != '-')
                    {
                        currentColumnName = tradeDateColumnName;
                        tradeDate = DateTime.Parse(Convert.ToString(dataRow[tradeDateColumnName]));
                        tradeDate = tradeDate.Add(tradeTimeOfDay);

                        currentColumnName = tradeSideColumnName;
                        tradeSide = Convert.ToString(dataRow[tradeSideColumnName]);

                        currentColumnName = tradeVolumeColumnName;
                        tradeVolume = Double.Parse(Convert.ToString(dataRow[tradeVolumeColumnName]));

                        currentColumnName = unitCostColumnName;
                        unitCost = Double.Parse(Convert.ToString(dataRow[unitCostColumnName]));

                        currentColumnName = totalCostColumnName;
                        totalCost = Double.Parse(Convert.ToString(dataRow[totalCostColumnName]));

                        UpdateTotalCost();

                        isValid = true;
                    }
                }
                catch (Exception e)
                {
                    throw new GHVHugoDataException(currentColumnName, e);
                }
            }
        }

        private void UpdateTotalCost()
        {
            double grossCost = Math.Round(unitCost * tradeVolume, 2);

            // if we override the commission, we must recalculate the total cost from scratch
            if (CommissionPerShare != 0)
            {
                double commission = Math.Round(CommissionPerShare * TradeVolume, 2);
                if (TradeType == "Buy")
                {
                    totalCost = grossCost + commission;
                }
                else
                {
                    totalCost = grossCost - commission;
                }
            }

            // add SEC fee for a sell
            if (TradeType != "Buy")
            {
                double SECFee = Math.Round(SECFeePerDollar * grossCost, 2);
                totalCost -= SECFee;
            }

            // add ticket chard if necessary
            if (TicketChargeApplied)
            {
                totalCost += (TradeType == "Buy") ? ticketCharge : -ticketCharge;
            }
        }

        // instantiate by specifying each property
        public StockTrade(
            string inputTradeDate,
            string inputTradeSide,
            string inputTradeVolume,
            string inputTicker,
            string inputUnitCost,
            string inputTotalCost)
        {
            string currentColumnName = tickerColumnName;

            try
            {
                if (!String.IsNullOrEmpty(inputTicker))
                {
                    if (inputTicker[0] != '-')
                    {
                        ticker = inputTicker.Replace(' ', '.');

                        currentColumnName = tradeDateColumnName;
                        tradeDate = DateTime.Parse(inputTradeDate);
                        tradeDate = tradeDate.Add(tradeTimeOfDay);

                        tradeSide = inputTradeSide;

                        currentColumnName = tradeVolumeColumnName;
                        tradeVolume = Double.Parse(inputTradeVolume);

                        currentColumnName = unitCostColumnName;
                        unitCost = Double.Parse(inputUnitCost);

                        currentColumnName = totalCostColumnName;
                        totalCost = Double.Parse(inputTotalCost);

                        UpdateTotalCost();

                        isValid = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw new GHVHugoDataException(currentColumnName, e);
            }
        }

      // copy constructor
        public StockTrade(IGHVStockTrade sourceRow)
        {
            UnderlyingSymbol = sourceRow.UnderlyingSymbol;
            TradeDate = sourceRow.TradeDate;
            TradeType = sourceRow.TradeType;
            FullVolume = sourceRow.FullVolume;
            UnitCost = sourceRow.UnitCost;
            TotalCost = sourceRow.TotalCost;
            TaxLotId = sourceRow.TaxLotId;
            SubAcctName = sourceRow.SubAcctName;
            TradeMedium = sourceRow.TradeMedium;
            TradeNote = sourceRow.TradeNote;
            TraderName = sourceRow.TraderName;
            isValid = sourceRow.IsValid;
        }
        #endregion

        #region Public Static Methods and Properties
        public static string TickerColumnName { get { return tickerColumnName; } set { tickerColumnName = value; } }
        public static string TradeSideColumnName { get { return tradeSideColumnName; } set { tradeSideColumnName = value; } }
        public static string TradeDateColumnName { get { return tradeDateColumnName; } set { tradeDateColumnName = value; } }
        public static string UnitCostColumnName { get { return unitCostColumnName; } set { unitCostColumnName = value; } }
        public static string TotalCostColumnName { get { return totalCostColumnName; } set { totalCostColumnName = value; } }
        public static string TradeVolumeColumnName { get { return tradeVolumeColumnName; } set { tradeVolumeColumnName = value; } }
        public static TimeSpan TradeTimeOfDay { get { return tradeTimeOfDay; } set { tradeTimeOfDay = value; } }
        public static double TicketCharge { get { return ticketCharge; } set { ticketCharge = value; } }
        public static IImportableTradeMedium IImportableTradeMedium { get { return importableTradeMedium; } set { importableTradeMedium = value; } }
        #endregion

        #region Public Instance Methods and Properties
        public bool IsValid { get { return isValid; } }

        public override string ToString()
        {
            return String.Format("{7}Ticker={0}|TaxLotId={1}|TradeDate={2:d}|TradeSide={3}|UnitCost={4:f2}|TradeVolume={5}|TotalCost={6:f2}",
                UnderlyingSymbol ?? "<NULL>", TaxLotId ?? "<NULL>", TradeDate, TradeType ?? "<NULL>", UnitCost, TradeVolume, TotalCost, IsValid ? "" : "***INVALID***");
        }
        #endregion

         #region IStockTrade Members
        public string TaxLotId
        {
            get { return taxLotId; }
            set { taxLotId = value; }
        }

        public DateTime TradeDate
        {
            get { return tradeDate; }
            set { tradeDate = value; }
        }

        public double UnitCost
        {
            get { return unitCost; }
            set { unitCost = value; }
        }

        public double TotalCost
        {
            get { return totalCost; }
            set { totalCost = value; }
        }

        public string BillingTypeDescr
        {
            get { return null; }
        }

        public string ContraName
        {
            get { return null; }
        }

        public long? OptionTradeId
        {
            get { return null; }
        }

        public bool ShortFlag
        {
            get { return false; }
        }

        public decimal SpecialRate
        {
            get { return 0; }
        }

        public bool TradeArchiveFlag
        {
            get { return false; }
        }

        public short TraderId
        {
            get { return -1; }
        }

        #endregion

        #region IUnderlying Members

        public string UnderlyingSymbol
        {
            get { return ticker; }
            set { ticker = value; }
        }

        #endregion

        #region ITrade Members
        public int? ConsolidationPackageId { get { return null; } }

        public int NumberOfTrades { get { return 1; } }

        public string BrokerName
        {
            get { return null; }
        }

        public string ExchangeName
        {
            get { return null; }
        }

        public double FractionalRemainder
        {
            get { return TradeVolume - tradeVolume; }
        }

        public string SubAcctName
        {
            get { return subAcctName; }
            set { subAcctName = value; }
        }

        public int? TradeId
        {
            get { return null; }
        }

        public string TradeMedium
        {
            get { return tradeMedium; }
            set { tradeMedium = value; }
        }

        public string TradeNote
        {
            get { return tradeNote; }
            set { tradeNote = value; }
        }

        public decimal TradePrice
        {
            get { return Convert.ToDecimal(unitCost); }
        }

        public string TradeReason
        {
            get { return null; }
        }

        public string TradeType
        {
            get
            {
                if (tradeSide == "B")
                    return "Buy";
                if (tradeSide == "S")
                    return "Sell";
                return null;
            }

            set
            {
                if (value == "Buy")
                    tradeSide = "B";
                else if (value == "Sell")
                    tradeSide = "S";
                else
                    throw new GHVHugoDataException("TradeType");
            }
        }

        public int TradeVolume
        {
            get { return Convert.ToInt32(Math.Truncate(tradeVolume)); }
        }
 
        public string TraderName
        {
            get { return traderName; }
            set { traderName = value; }
        }

        #endregion

        public double FullVolume
        {
            get { return tradeVolume; }
            set { tradeVolume = value; }
        }

        #region IGHVStockTrade Members


        public string MerrillAccountNumber
        {
            get { throw new NotImplementedException(); }
        }

        public string ClearingHouseTaxlotId
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime DateOpened
        {
            get { throw new NotImplementedException(); }
        }

        public decimal OpeningTradePrice
        {
            get { throw new NotImplementedException(); }
        }
        public string OpeningTradeType
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IImportableTradeMedium Members

        public string TradeMediumName
        {
            get 
            {
                if (importableTradeMedium == null)
                    return String.Empty;
                else
                    return importableTradeMedium.TradeMediumName;
            }
        }

        public bool TicketChargeApplied
        {
            get
            {
                if (importableTradeMedium == null)
                    return true;
                else
                    return importableTradeMedium.TicketChargeApplied;
            }
        }

        public double CommissionPerShare
        {
            get
            {
                if (importableTradeMedium == null)
                    return 0.0;
                else
                    return importableTradeMedium.CommissionPerShare;
            }
        }

        public double SECFeePerDollar
        {
            get
            {
                if (importableTradeMedium == null)
                    return 0.0;
                else
                    return importableTradeMedium.SECFeePerDollar;
            }
        }

        #endregion
    }
}
