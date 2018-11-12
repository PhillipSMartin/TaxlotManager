using System;
using System.Data;
using System.Collections.Generic;
using LoggingUtilitiesLib;
using System.Data.SqlClient;

namespace GHVHugoLib.HugoDataSetTableAdapters
{

    partial class QueriesTableAdapter
    {
         [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
         internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand(string commandText)
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, commandText);
        }
        internal void SetConnection(string commandText, IDbConnection connection)
        {
            if (connection == null)
            {
                if (LoggingUtilities.FindSqlCommand(CommandCollection, commandText).Connection != null)
                {
                    LoggingUtilities.FindSqlCommand(CommandCollection, commandText).Connection.Close();
                }
            }
            LoggingUtilities.FindSqlCommand(CommandCollection, commandText).Connection = connection;
        }

    }
    public partial class p_get_open_taxlotsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class AccountsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_get_taxlots_with_running_positions_and_unit_cost_basisTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class SubAccountsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class TradersTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_get_stock_tradesTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_get_spinoff_infoTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class RealizedStockGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class RealizedFuturesGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class UnrealizedFuturesGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class RealizedEquityOptionGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class RealizedIndexOptionGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class UnrealizedEquityOptionGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class UnrealizedIndexOptionGainsTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class TaxlotSummaryReportTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_get_importable_trademediaTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_generate_stock_daily_returns_reportTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
    public partial class p_generate_stock_monthly_returns_reportTableAdapter
    {
        internal void SetCommandTimeout(int timeOut)
        {
            foreach (IDbCommand cmd in CommandCollection)
            {
                cmd.CommandTimeout = timeOut;
            }
        }
        internal int GetReturnValue(string commandText)
        {
            return LoggingUtilities.GetReturnCode(CommandCollection, commandText);
        }
        internal void LogCommand()
        {
            LoggingUtilities.LogSqlCommand("SqlLog", CommandCollection, 0);
        }
    }
}

namespace GHVHugoLib {
    
    public partial class HugoDataSet {
        partial class UnrealizedEquityOptionGainsDataTable
        {
        }
    
        public partial class RealizedStockGainsRow : IRealizedGains, ITaxlotId
        {
        }
        public partial class RealizedEquityOptionGainsRow : IRealizedGains, ITradeId
        {
            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }
        public partial class RealizedIndexOptionGainsRow : IRealizedGains, ITradeId
        {
            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }
        public partial class UnrealizedFuturesGainsRow : IUnrealizedGains, ITradeId
        {
            #region IUnerealizedGains Members
            public int InstrumentId
            {
                get { return FuturesId; }
            }
            public InstrumentType InstrumentTypeId
            {
                get { return InstrumentType.Future; }
            }
            public DateTime Open_Date
            {
                get { return OpeningTradeDate; }
            }

            public double Unit_Cost
            {
                get { return OpeningTradePrice; }
            }

            public double Open_Amount
            {
                get
                {
                    return TradeVolume;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public double TotalCost
            {
                get { return OpeningTotalPrice; }
            }
            #endregion

            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }
        public partial class RealizedFuturesGainsRow : IRealizedGains, ITradeId
        {
            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }
        public partial class UnrealizedEquityOptionGainsRow : IUnrealizedGains, ITradeId
        {
            #region IUnrealizedGains Members
            public int InstrumentId
            {
                get
                {
                    return (int)OptionId;
                }
            }

            public InstrumentType InstrumentTypeId
            {
                get
                {
                    return (InstrumentType)OptionTypeId;
                }
            }

            public DateTime Open_Date
            {
                get { return OpeningTradeDate; }
            }

            public double Unit_Cost
            {
                get { return OpeningTradePrice; }
            }

            public double Open_Amount
            {
                get
                {
                    return TradeVolume;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public double TotalCost
            {
                get { return OpeningTotalPrice; }
            }

 
            #endregion

            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }
        public partial class UnrealizedIndexOptionGainsRow : IUnrealizedGains, ITradeId
        {
            #region IUnrealizedGains Members
            public int InstrumentId
            {
                get
                {
                    return (int)OptionId;
                }
            }

            public InstrumentType InstrumentTypeId
            {
                get
                {
                    return (InstrumentType)OptionTypeId;
                }
            }

            public DateTime Open_Date
            {
                get { return OpeningTradeDate; }
            }

            public double Unit_Cost
            {
                get { return OpeningTradePrice; }
            }

            public double Open_Amount
            {
                get
                {
                    return TradeVolume;
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public double TotalCost
            {
                get { return OpeningTotalPrice; }
            }


            #endregion

            #region ITrade Members

            public int TradeId
            {
                get { return OpeningTradeId; }
            }

            #endregion
        }

         public partial class p_get_open_taxlotsRow : ITaxLot
        {
             public string TaxlotId
             {
                 get { return TaxLotId; }
             }
            public double TotalCost
            {
                get { return Unit_Cost * Open_Amount; }
            }
            public override string ToString()
            {
                return String.Format("Ticker={0}|TaxLotId={1}|OpenDate={2:d}|UnitCost={3:f2}|OpenAmount={4}|TotalCost={5:f2}|CurrentPrice={6:f2}|Score={7}",
                    Ticker ?? "<NULL>", TaxLotId ?? "<NULL>", Open_Date, Unit_Cost, Open_Amount, TotalCost, CurrentPrice, Score);
            }

            #region IUnrealizedGains Members
            public int InstrumentId
            {
                get
                {
                    return UnderlyingId;
                }
            }

            public InstrumentType InstrumentTypeId
            {
                get
                {
                    return InstrumentType.Stock;
                }
            }
            #endregion
        }

        public partial class p_get_open_taxlotsDataTable
        {
            public IEnumerable<ITaxLot> Taxlots
            {
                get
                {
                    foreach (p_get_open_taxlotsRow row in DataTableExtensions.AsEnumerable(this))
                    {
                        yield return (ITaxLot)row;
                    }
                }
            }
        }

       public partial class p_get_stock_tradesRow : IGHVStockTrade
        {
            #region IStockTrade Members

            public string BillingTypeDescr
            {
                get { throw new NotImplementedException(); }
            }

            public string ContraName
            {
                get { throw new NotImplementedException(); }
            }

            public long? OptionTradeId
            {
                get { throw new NotImplementedException(); }
            }

            public bool ShortFlag
            {
                get { throw new NotImplementedException(); }
            }

            public decimal SpecialRate
            {
                get { throw new NotImplementedException(); }
            }

            public bool TradeArchiveFlag
            {
                get { throw new NotImplementedException(); }
            }

            public short TraderId
            {
                get { throw new NotImplementedException(); }
            }


            public double FullVolume
            {
                get { return FullTradeVolume; }
                set { FullTradeVolume = value; }
            }

            #endregion

            #region ITrade Members
            int? ReconciliationLib.ITrade.ConsolidationPackageId
            {
                get
                {
                    return IsConsolidationPackageIdNull() ? null : (int?)this.ConsolidationPackageId;
                }
            }

            public string BrokerName
            {
                get { throw new NotImplementedException(); }
            }

            public string ExchangeName
            {
                get { throw new NotImplementedException(); }
            }

            public double FractionalRemainder
            {
                get { throw new NotImplementedException(); }
            }

            public DateTime TradeDate
            {
                get { return TradeDatetime; }
                set { TradeDatetime = value; }
            }

            int? ReconciliationLib.ITrade.TradeId
            {
                get { return TradeId; }
            }

            public string TradeMedium
            {
                get { return TradeMediumName; }
                set { TradeMediumName = value; }
           }

            public string TradeReason
            {
                get { throw new NotImplementedException(); }
            }

            public string TradeType
            {
                get { return TradeTypeName; }
                set { TradeTypeName = value; }
            }

            int ReconciliationLib.ITrade.TradeVolume
            {
                get { return (int)TradeVolume; }
            }

            public string TraderName
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
           }

            #endregion

            #region IGHVStockTrade Members

            public bool IsValid
            {
                get { return true; }
            }

            #endregion

            #region IStockTrade Members
            #endregion
        }

       public partial class p_get_importable_trademediaRow : IImportableTradeMedium
       {
           public override string ToString()
           {
               return this.TradeMediumName;
           }
       }

     }
}
