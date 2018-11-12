using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GHVHugoLib
{
    public class TablesUpdatedEventArgs : EventArgs
    {
        private bool openTaxlotsTableFilled;
        private bool unrealizedFuturesGainsTableFilled;
        private bool unrealizedEquityOptionGainsTableFilled;
        private bool unrealizedIndexOptionGainsTableFilled;

        private bool realizedStockGainsTableFilled;
        private bool realizedFuturesGainsTableFilled;
        private bool realizedEquityOptionGainsTableFilled;
        private bool realizedIndexOptionGainsTableFilled;
 
        private bool taxlotHistoryTableFilled;
        private bool stockTradesTableFilled;
        private bool taxlotSummaryReportTableFilled;
        private bool stockDailyReturnsTableFilled;
        private bool stockMonthlyReturnsTableFilled;

        private bool subAccountsTableFilled;
        private bool importableTradeMediaTableFilled;
        private bool accountsTableFilled;
        private bool tradersTableFilled;

        private string logMsg;

        private static bool s_oldopenTaxlotsTableFilled;
        private static bool s_oldunrealizedFuturesGainsTableFilled;
        private static bool s_oldunrealizedEquityOptionGainsTableFilled;
        private static bool s_oldunrealizedIndexOptionGainsTableFilled;

        private static bool s_oldrealizedStockGainsTableFilled;
        private static bool s_oldrealizedFuturesGainsTableFilled;
        private static bool s_oldrealizedEquityOptionGainsTableFilled;
        private static bool s_oldrealizedIndexOptionGainsTableFilled;

        private static bool s_oldtaxlotHistoryTableFilled;
        private static bool s_oldstockTradesTableFilled;
        private static bool s_oldtaxlotSummaryReportTableFilled;
        private static bool s_oldstockDailyReturnsTableFilled;
        private static bool s_oldstockMonthlyReturnsTableFilled;

        private static bool s_oldsubAccountsTableFilled;
        private static bool s_oldimportableTradeMediaTableFilled;
        private static bool s_oldaccountsTableFilled;
        private static bool s_oldtradersTableFilled;

        private static Semaphore s_tablesUpdatedSemaphore = new Semaphore(1, 1);

        public TablesUpdatedEventArgs()
        {
            openTaxlotsTableFilled = Utilities.OpenTaxlotsTableFilled;
            unrealizedFuturesGainsTableFilled = Utilities.UnrealizedFuturesGainsTableFilled;
            unrealizedEquityOptionGainsTableFilled = Utilities.UnrealizedEquityOptionGainsTableFilled;
            unrealizedIndexOptionGainsTableFilled = Utilities.UnrealizedIndexOptionGainsTableFilled;

            realizedStockGainsTableFilled = Utilities.RealizedStockGainsTableFilled;
            realizedFuturesGainsTableFilled = Utilities.RealizedFuturesGainsTableFilled;
            realizedEquityOptionGainsTableFilled = Utilities.RealizedEquityOptionGainsTableFilled;
            realizedIndexOptionGainsTableFilled = Utilities.RealizedIndexOptionGainsTableFilled;
 
            taxlotHistoryTableFilled = Utilities.TaxlotHistoryTableFilled;
            stockTradesTableFilled = Utilities.StockTradesTableFilled;
            taxlotSummaryReportTableFilled = Utilities.TaxlotSummaryReportTableFilled;
            stockDailyReturnsTableFilled = Utilities.StockDailyReturnsTableFilled;
            stockMonthlyReturnsTableFilled = Utilities.StockMonthlyReturnsTableFilled;

            subAccountsTableFilled = Utilities.SubAccountsTableFilled;
            importableTradeMediaTableFilled = Utilities.ImportableTradeMediaTableFilled;
            accountsTableFilled = Utilities.AccountsTableFilled;
            tradersTableFilled = Utilities.TradersTableFilled;

            s_tablesUpdatedSemaphore.WaitOne();

            logMsg = BuildLogMsg();
            s_oldopenTaxlotsTableFilled = openTaxlotsTableFilled;
            s_oldunrealizedFuturesGainsTableFilled = unrealizedFuturesGainsTableFilled;
            s_oldunrealizedEquityOptionGainsTableFilled = unrealizedEquityOptionGainsTableFilled;
            s_oldunrealizedIndexOptionGainsTableFilled = unrealizedIndexOptionGainsTableFilled;

            s_oldrealizedStockGainsTableFilled = realizedStockGainsTableFilled;
            s_oldrealizedFuturesGainsTableFilled = realizedFuturesGainsTableFilled;
            s_oldrealizedEquityOptionGainsTableFilled = realizedEquityOptionGainsTableFilled;
            s_oldrealizedIndexOptionGainsTableFilled = realizedIndexOptionGainsTableFilled;

            s_oldtaxlotHistoryTableFilled = taxlotHistoryTableFilled;
            s_oldstockTradesTableFilled = stockTradesTableFilled;
            s_oldtaxlotSummaryReportTableFilled = taxlotSummaryReportTableFilled;
            s_oldstockDailyReturnsTableFilled = stockDailyReturnsTableFilled;
            s_oldstockMonthlyReturnsTableFilled = stockMonthlyReturnsTableFilled;

            s_oldsubAccountsTableFilled = subAccountsTableFilled;
            s_oldimportableTradeMediaTableFilled = importableTradeMediaTableFilled;
            s_oldaccountsTableFilled = accountsTableFilled;
            s_oldtradersTableFilled = tradersTableFilled;

            s_tablesUpdatedSemaphore.Release();
        }

        public bool OpenTaxlotsTableUpdated { get { return !openTaxlotsTableFilled; } }
        public bool UnrealizedFuturesGainsTableUpdated { get { return !unrealizedFuturesGainsTableFilled; } }
        public bool UnrealizedEquityOptionGainsTableUpdated { get { return !unrealizedEquityOptionGainsTableFilled; } }
        public bool UnrealizedIndexOptionGainsTableUpdated { get { return !unrealizedIndexOptionGainsTableFilled; } }

        public bool RealizedStockGainsTableUpdated { get { return !realizedStockGainsTableFilled; } }
        public bool RealizedFuturesGainsTableUpdated { get { return !realizedFuturesGainsTableFilled; } }
        public bool RealizedEquityOptionGainsTableUpdated { get { return !realizedEquityOptionGainsTableFilled; } }
        public bool RealizedIndexOptionGainsTableUpdated { get { return !realizedIndexOptionGainsTableFilled; } }
        
        public bool TaxlotHistoryTableUpdated { get { return !taxlotHistoryTableFilled; } }
        public bool StockTradesTableUpdated { get { return !stockTradesTableFilled; } }
        public bool TaxlotSummaryReportTableUpdated { get { return !taxlotSummaryReportTableFilled; } }
        public bool StockDailyReturnsTableUpdated { get { return !stockDailyReturnsTableFilled; } }
        public bool StockMonthlyReturnsTableUpdated { get { return !stockMonthlyReturnsTableFilled; } }

        public bool SubAccountsTableUpdated { get { return !subAccountsTableFilled; } }
        public bool ImportableTradeMediaTableUpdated { get { return !importableTradeMediaTableFilled; } }
        public bool AccountsTableUpdated { get { return !accountsTableFilled; } }
        public bool TradersTableUpdated { get { return !tradersTableFilled; } }

        public override string ToString()
        {
            return logMsg;
        }

        private string BuildLogMsg()
        {
            string str = "Tables marked dirty:";

            if (OpenTaxlotsTableUpdated && s_oldopenTaxlotsTableFilled)
                str += " OpenTaxlots";
            if (UnrealizedEquityOptionGainsTableUpdated && s_oldunrealizedEquityOptionGainsTableFilled)
                str += " UnrealizedEquityOptionGains";
            if (UnrealizedIndexOptionGainsTableUpdated && s_oldunrealizedIndexOptionGainsTableFilled)
                str += " UnrealizedIndexOptionGains";
            if (UnrealizedFuturesGainsTableUpdated && s_oldunrealizedFuturesGainsTableFilled)
                str += " UnrealizedFuturesGains";

            if (RealizedStockGainsTableUpdated && s_oldrealizedStockGainsTableFilled)
                str += " RealizedStockGains";
            if (RealizedFuturesGainsTableUpdated && s_oldrealizedFuturesGainsTableFilled)
                str += " RealizedFuturesGains";
            if (RealizedEquityOptionGainsTableUpdated && s_oldrealizedEquityOptionGainsTableFilled)
                str += " RealizedEquityOptionGains";
            if (RealizedIndexOptionGainsTableUpdated && s_oldrealizedIndexOptionGainsTableFilled)
                str += " RealizedIndexOptionGains";

            if (TaxlotHistoryTableUpdated && s_oldtaxlotHistoryTableFilled)
                str += " TaxlotHistory";
            if (StockTradesTableUpdated && s_oldstockTradesTableFilled)
                str += " StockTrades";
            if (TaxlotSummaryReportTableUpdated && s_oldtaxlotSummaryReportTableFilled)
                str += " TaxlotSummaryReport";
            if (StockDailyReturnsTableUpdated && s_oldstockDailyReturnsTableFilled)
                str += " StockDailyReturns";
            if (StockMonthlyReturnsTableUpdated && s_oldstockMonthlyReturnsTableFilled)
                str += " StockMonthlyReturns";

            if (SubAccountsTableUpdated && s_oldsubAccountsTableFilled)
                str += " SubAccounts";
            if (ImportableTradeMediaTableUpdated && s_oldimportableTradeMediaTableFilled)
                str += " ImportableTradeMedia";
            if (AccountsTableUpdated && s_oldaccountsTableFilled)
                str += " Accounts";
            if (TradersTableUpdated && s_oldtradersTableFilled)
                str += " Traders";

            str += " - Tables filled:";

            if ((!OpenTaxlotsTableUpdated) && !s_oldopenTaxlotsTableFilled)
                str += " OpenTaxlots";
            if ((!UnrealizedEquityOptionGainsTableUpdated) && !s_oldunrealizedEquityOptionGainsTableFilled)
                str += " UnrealizedEquityOptionGains";
            if ((!UnrealizedIndexOptionGainsTableUpdated) && !s_oldunrealizedIndexOptionGainsTableFilled)
                str += " UnrealizedIndexOptionGains";
            if ((!UnrealizedFuturesGainsTableUpdated) && !s_oldunrealizedFuturesGainsTableFilled)
                str += " UnrealizedFuturesGains";

            if ((!RealizedStockGainsTableUpdated) && !s_oldrealizedStockGainsTableFilled)
                str += " RealizedStockGains";
            if ((!RealizedFuturesGainsTableUpdated) && !s_oldrealizedFuturesGainsTableFilled)
                str += " RealizedFuturesGains";
            if ((!RealizedEquityOptionGainsTableUpdated) && !s_oldrealizedEquityOptionGainsTableFilled)
                str += " RealizedEquityOptionGains";
            if ((!RealizedIndexOptionGainsTableUpdated) && !s_oldrealizedIndexOptionGainsTableFilled)
                str += " RealizedIndexOptionGains";

            if ((!TaxlotHistoryTableUpdated) && !s_oldtaxlotHistoryTableFilled)
                str += " TaxlotHistory";
            if ((!StockTradesTableUpdated) && !s_oldstockTradesTableFilled)
                str += " StockTrades";
            if ((!TaxlotSummaryReportTableUpdated) && !s_oldtaxlotSummaryReportTableFilled)
                str += " TaxlotSummaryReport";
            if ((!StockDailyReturnsTableUpdated) && !s_oldstockDailyReturnsTableFilled)
                str += " StockDailyReturns";
            if ((!StockMonthlyReturnsTableUpdated) && !s_oldstockMonthlyReturnsTableFilled)
                str += " StockMonthlyReturns";

            if ((!SubAccountsTableUpdated) && !s_oldsubAccountsTableFilled)
                str += " SubAccounts";
            if ((!ImportableTradeMediaTableUpdated) && !s_oldimportableTradeMediaTableFilled)
                str += " ImportableTradeMedia";
            if ((!AccountsTableUpdated) && !s_oldaccountsTableFilled)
                str += " Accounts";
            if ((!TradersTableUpdated) && !s_oldtradersTableFilled)
                str += " Traders";

            return str;
        }
    }
}
