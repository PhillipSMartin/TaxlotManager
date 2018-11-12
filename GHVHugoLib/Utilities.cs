using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using ReconciliationLib;
using LoggingUtilitiesLib;
using System.Threading;


namespace GHVHugoLib
{
    public sealed class Utilities
    {
        #region Constants
        private const string excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = {0};Extended Properties=Excel 8.0;Mode=Read";
        private const string selectAllFromExcelWorkSheetCommand = "select * from [{0}$]";
        private static DateTime firstCoveredDate = new DateTime(2011, 1, 1);
        #endregion

        #region Declarations
        private static HugoDataSet hugoDataSet = new HugoDataSet();
        private static bool openTaxlotsTableFilled;
        private static bool accountsTableFilled;
        private static bool subAccountsTableFilled;
        private static bool tradersTableFilled;
        private static bool taxlotHistoryTableFilled;
        private static bool stockTradesTableFilled;
        private static bool realizedStockGainsTableFilled;
        private static bool realizedIndexOptionGainsTableFilled;
        private static bool realizedEquityOptionGainsTableFilled;
        private static bool unrealizedIndexOptionGainsTableFilled;
        private static bool unrealizedEquityOptionGainsTableFilled;
        private static bool unrealizedFuturesGainsTableFilled;
        private static bool realizedFuturesGainsTableFilled;
        private static bool taxlotSummaryReportTableFilled;
        private static bool importableTradeMediaTableFilled;
        private static bool stockDailyReturnsTableFilled;
        private static bool stockMonthlyReturnsTableFilled;

        // parameters
        private static HugoDataSet.AccountsRow selectedAccountsRow;
        private static string underlyingSymbol;
        private static string taxlotId;
        private static DateTime? endDate;
        private static DateTime? scoreDate;
        private static DateTime? startingDate;
        private static DateTime? closingDate;
        private static SubtotalGroup subtotalGroup = SubtotalGroup.Taxlot;

        #region Table adapters
        private static HugoDataSetTableAdapters.QueriesTableAdapter queriesTableAdapter = new HugoDataSetTableAdapters.QueriesTableAdapter();
        private static HugoDataSetTableAdapters.p_get_open_taxlotsTableAdapter openTaxlotsAdapter = new HugoDataSetTableAdapters.p_get_open_taxlotsTableAdapter();
        private static HugoDataSetTableAdapters.AccountsTableAdapter accountsAdapter = new HugoDataSetTableAdapters.AccountsTableAdapter();
        private static HugoDataSetTableAdapters.p_get_taxlots_with_running_positions_and_unit_cost_basisTableAdapter taxlotHistoryAdapter = new HugoDataSetTableAdapters.p_get_taxlots_with_running_positions_and_unit_cost_basisTableAdapter();
        private static HugoDataSetTableAdapters.SubAccountsTableAdapter subAccountsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.SubAccountsTableAdapter();
        private static HugoDataSetTableAdapters.TradersTableAdapter tradersAdapter = new GHVHugoLib.HugoDataSetTableAdapters.TradersTableAdapter();
        private static HugoDataSetTableAdapters.p_get_stock_tradesTableAdapter stockTradesAdapter = new GHVHugoLib.HugoDataSetTableAdapters.p_get_stock_tradesTableAdapter();
        private static HugoDataSetTableAdapters.p_get_spinoff_infoTableAdapter spinoffInfoAdapter = new GHVHugoLib.HugoDataSetTableAdapters.p_get_spinoff_infoTableAdapter();
        private static HugoDataSetTableAdapters.RealizedStockGainsTableAdapter realizedStockGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.RealizedStockGainsTableAdapter();
        private static HugoDataSetTableAdapters.RealizedEquityOptionGainsTableAdapter realizedEquityOptionGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.RealizedEquityOptionGainsTableAdapter();
        private static HugoDataSetTableAdapters.RealizedIndexOptionGainsTableAdapter realizedIndexOptionGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.RealizedIndexOptionGainsTableAdapter();
        private static HugoDataSetTableAdapters.RealizedFuturesGainsTableAdapter realizedFuturesGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.RealizedFuturesGainsTableAdapter();
        private static HugoDataSetTableAdapters.UnrealizedEquityOptionGainsTableAdapter unrealizedEquityOptionGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.UnrealizedEquityOptionGainsTableAdapter();
        private static HugoDataSetTableAdapters.UnrealizedIndexOptionGainsTableAdapter unrealizedIndexOptionGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.UnrealizedIndexOptionGainsTableAdapter();
        private static HugoDataSetTableAdapters.UnrealizedFuturesGainsTableAdapter unrealizedFuturesGainsAdapter = new GHVHugoLib.HugoDataSetTableAdapters.UnrealizedFuturesGainsTableAdapter();
        private static HugoDataSetTableAdapters.TaxlotSummaryReportTableAdapter taxlotSummaryReportAdapter = new GHVHugoLib.HugoDataSetTableAdapters.TaxlotSummaryReportTableAdapter();
        private static HugoDataSetTableAdapters.p_get_importable_trademediaTableAdapter importableTradeMediaTableAdapter = new GHVHugoLib.HugoDataSetTableAdapters.p_get_importable_trademediaTableAdapter();
        private static HugoDataSetTableAdapters.p_generate_stock_daily_returns_reportTableAdapter generateStockDailyReturnsReportTableAdapter = new GHVHugoLib.HugoDataSetTableAdapters.p_generate_stock_daily_returns_reportTableAdapter();
        private static HugoDataSetTableAdapters.p_generate_stock_monthly_returns_reportTableAdapter generateStockMonthlyReturnsReportTableAdapter = new GHVHugoLib.HugoDataSetTableAdapters.p_generate_stock_monthly_returns_reportTableAdapter();
        #endregion

        #region event handlers
        // event fired when tables are updated
        private static EventHandler<TablesUpdatedEventArgs> tablesUpdatedEventHandler;
        #endregion
        #endregion

        private Utilities() { }

        #region Public methods
        public static void Refresh()
        {
            openTaxlotsTableFilled = false;
            unrealizedFuturesGainsTableFilled = false;
            unrealizedEquityOptionGainsTableFilled = false;
            unrealizedIndexOptionGainsTableFilled = false;

            realizedStockGainsTableFilled = false;
            realizedFuturesGainsTableFilled = false;
            realizedEquityOptionGainsTableFilled = false;
            realizedIndexOptionGainsTableFilled = false;
 
            taxlotHistoryTableFilled = false;
            stockTradesTableFilled = false;
            taxlotSummaryReportTableFilled = false;
            stockDailyReturnsTableFilled = false;
            stockMonthlyReturnsTableFilled = false;

            subAccountsTableFilled = false;
            importableTradeMediaTableFilled = false;

            FireOnTablesUpdated();
        }

        #region Methods to read data from misc sources
        public static DataRowCollection ReadExcelWorksheet(string excelFileName, string excelWorksheetName)
        {
            OleDbConnection cn = GetExcelConnection(excelFileName);
            OleDbDataAdapter cmd = GetSelectAllFromExcelWorksheetCommand(excelWorksheetName, cn);
            DataSet ds = new DataSet();

            try
            {
                cn.Open();
                cmd.Fill(ds);
                return ds.Tables[0].Rows;
            }
            finally
            {
                cn.Close();
            }
        }

        private static IEnumerable<IGHVStockTrade> GetStockTradesFromExcelWorksheet(string excelFileName, string excelWorksheetName)
        {
            foreach (DataRow dataRow in ReadExcelWorksheet(excelFileName, excelWorksheetName))
            {
                StockTrade stockTrade = new StockTrade(dataRow);
                if (stockTrade.IsValid)
                    yield return stockTrade;
            }
        }
        private static IEnumerable<IGHVStockTrade> GetStockTradesFromCsvFile(string csvFileName)
        {
            StockTradeCollection stockTradeCollection = new StockTradeCollection(csvFileName);
            StockTrade stockTrade = stockTradeCollection.NextRecord;
            while (stockTrade != null)
            {
                if (stockTrade.IsValid)
                    yield return stockTrade;
                stockTrade = stockTradeCollection.NextRecord;
            }
        }
        public static IEnumerable<IGHVStockTrade> GetStockTrades(string fileName, string excelWorksheetName)
        {
            if (fileName.Length >= 3)
            {
                if (fileName.EndsWith("xls"))
                    return GetStockTradesFromExcelWorksheet(fileName, excelWorksheetName);
                else if (fileName.EndsWith("csv"))
                    return GetStockTradesFromCsvFile(fileName);
            }
            throw new GHVHugoException(String.Format("Cannot read file {0} - unsupported file type"));
        }
        public static HugoDataSet.p_get_spinoff_infoRow GetSpinoffInfo(string spinoffSymbol, DateTime spinoffDate)
        {
            int count = 0;
            bool bExecuted = false;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                bExecuted = true;
                spinoffInfoAdapter.Connection = Connection;
                count = spinoffInfoAdapter.Fill(hugoDataSet.p_get_spinoff_info, Account, spinoffSymbol, spinoffDate);
                if (count > 1)
                {
                    throw new GHVHugoException("Spinoff info is corrupted - spinoff is tied to more than one original symbol");
                }
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error getting spinoff info, account={0}, symbol={1}, spinoffDate={2:d}",
                    Account ?? "<NULL>",
                    spinoffSymbol,
                    spinoffDate), e);
            }
            finally
            {

                if (bExecuted)
                    spinoffInfoAdapter.LogCommand();
                FireOnInfo(String.Format("Retreived {0} rows of spinoff info, account={1}, symbol={2}, spinoffDate={3:d}",
                    count,
                    Account ?? "<NULL>",
                    spinoffSymbol,
                    spinoffDate));

                if (spinoffInfoAdapter.Connection != null)
                {
                    spinoffInfoAdapter.Connection.Close();
                    spinoffInfoAdapter.Connection = null;
                }
            }
            if (count == 1)
            {
                return hugoDataSet.p_get_spinoff_info[0];
            }
            else
            {
                return null;
            }
        }
        public static DateTime? GetStartOfYear(DateTime? currentDate)
        {
            DateTime? startDate = null;

            try
            {
                VerifyInitialization(/*needAccount =*/ false);

                queriesTableAdapter.SetConnection("dbo.p_get_start_of_year", Connection);
                queriesTableAdapter.p_get_start_of_year(currentDate, ref startDate);
                queriesTableAdapter.LogCommand("dbo.p_get_start_of_year");
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error getting start of year, currentDate={0}",
                  ReconciliationConvert.ToNullableString(currentDate)), e);
            }
            finally
            {
                queriesTableAdapter.SetConnection("dbo.p_get_start_of_year", null);
            }
            FireOnInfo(String.Format("Start of year = {0}, currentDate={1}",
                ReconciliationConvert.ToNullableString(startDate),
                ReconciliationConvert.ToNullableString(currentDate)));

            return startDate;
        }
        #endregion

        #region Methods to write data to Hugo
        // returns 0 on success
        public static int InsertTaxLotScore(ITaxLot taxLot, DateTime scoreDate)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                if (taxLot.Score > 0)
                {
                    queriesTableAdapter.SetConnection("GHV.p_insert_taxlot_score", Connection);
                    queriesTableAdapter.p_insert_taxlot_score(Account,
                        taxLot.TaxlotId,
                        scoreDate,
                        taxLot.CurrentPrice,
                        taxLot.Score);
                    openTaxlotsTableFilled = false;
                    taxlotSummaryReportTableFilled = false;
                    FireOnTablesUpdated();
                    queriesTableAdapter.LogCommand("GHV.p_insert_taxlot_score");

                    FireOnInfo("Added taxlot score: " + taxLot.ToString());
                }
                else
                {
                    FireOnInfo("Invalid current price or score for: " + taxLot.ToString());
                    return -1;
                }

            }
            catch (Exception e)
            {
                FireOnError("Error adding taxlot score: " + taxLot.ToString(), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.SetConnection("GHV.p_insert_taxlot_score", null);
            }

            return 0;
        }

        // returns 0 on success
        public static int CalculateTaxLotScore(ITaxLot taxLot, double currentPrice)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_calculate_taxlot_score", Connection);
                queriesTableAdapter.p_calculate_taxlot_score(AccountId,
                    taxLot.TaxlotId,
                    EndDate,
                    currentPrice,
                    taxLot.Unit_Cost,
                    taxLot.Open_Date);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //openTaxlotsTableFilled = false;
                //FireOnTablesUpdated();
                queriesTableAdapter.LogCommand("GHV.p_calculate_taxlot_score");

                FireOnInfo(String.Format("Calculated taxlot score for {0} at price {1}", taxLot.ToString(), currentPrice));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error calculating taxlot score for {0} at price {1}", taxLot.ToString(), currentPrice), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.SetConnection("GHV.p_calculate_taxlot_score", null);
            }

            return 0;
        }

        // no automatic refresh - caller must call refresh when he's through using this method
        public static int AdjustCostBasis(string taxlotId, double newCostBasis, DateTime adjustmentDate, string tradeNote)
        {
            int rc = -1;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_adjustCostBasis", Connection);
                queriesTableAdapter.p_adjustCostBasis(Account, taxlotId, newCostBasis, null, adjustmentDate, tradeNote);
                queriesTableAdapter.LogCommand("GHV.p_adjustCostBasis");

                rc = queriesTableAdapter.GetReturnValue("GHV.p_adjustCostBasis");
                if (rc == 0)
                {
                    // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                    //  this methods for each row, we cannot invalidate the table in this method

                    //openTaxlotsTableFilled = false;
                    //taxlotHistoryTableFilled = false;
                    //realizedStockGainsTableFilled = false;
                    //taxlotSummaryReportTableFilled = false;
                    //FireOnTablesUpdated();

                    FireOnInfo(String.Format("Adjusted cost basis of taxlot {0} to {1} effective on {2:d} ",
                        taxlotId, newCostBasis, adjustmentDate));
                }
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error adjusting cost basis of taxlot {0} to {1} effective on {2:d} ",
                            taxlotId, newCostBasis, adjustmentDate), e);
            }
            finally
            {
                queriesTableAdapter.SetConnection("GHV.p_adjustCostBasis", null);
            }

            return rc;
        }

        public static int AdjustSpinoff(string spinoffSymbol,
                    double newSpinoffCostBasis, double newOriginalCostBasis, double newSpinoffVolume, DateTime spinoffDate)
        {
            int rc = -1;
            bool bExecuted = false;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                bExecuted = true;
                queriesTableAdapter.SetConnection("GHV.p_adjustSpinoff", Connection);
                queriesTableAdapter.p_adjustSpinoff(Account, spinoffSymbol,
                    newSpinoffCostBasis, newOriginalCostBasis, newSpinoffVolume, spinoffDate);

                rc = queriesTableAdapter.GetReturnValue("GHV.p_adjustSpinoff");
                if (rc == 0)
                {
                    openTaxlotsTableFilled = false;
                    taxlotHistoryTableFilled = false;
                    realizedStockGainsTableFilled = false;
                    taxlotSummaryReportTableFilled = false;
                    FireOnTablesUpdated();

                    FireOnInfo(String.Format("Adjusted spinoff of {0}, spinoff cost basis={1:f2}, original cost basis={2:f2}, volume={3:f0} on {4:d}",
                        spinoffSymbol, newSpinoffCostBasis, newOriginalCostBasis, newSpinoffVolume, spinoffDate));
                }
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error adjusting spinoff of {0} ",
                            spinoffSymbol), e);
            }
            finally
            {
                if (bExecuted)
                    queriesTableAdapter.LogCommand("GHV.p_adjustSpinoff");
                queriesTableAdapter.SetConnection("GHV.p_adjustSpinoff", null);
            }

            return rc;
        }

        // returns 0 on success
        public static int DeleteStockTrade(int tradeId)
        {
            int rc = -1;
            try
            {
                VerifyInitialization(/*needAccount =*/ false);

                queriesTableAdapter.SetConnection("dbo.delete_trade", Connection);
                queriesTableAdapter.p_delete_trade(tradeId);
                queriesTableAdapter.LogCommand("dbo.delete_trade");

                rc = queriesTableAdapter.GetReturnValue("dbo.delete_trade");
                if (rc == 0)
                {
                    // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                    //  this methods for each row, we cannot invalidate the table in this method

                    //openTaxlotsTableFilled = false;
                    //taxlotHistoryTableFilled = false;
                    //stockTradesTableFilled = false;
                    //realizedStockGainsTableFilled = false;
                    //taxlotSummaryReportTableFilled = false;
                    //FireOnTablesUpdated();

                    FireOnInfo(String.Format("Deleted tradeid {0} ", tradeId));
                }
                else
                {
                    throw new GHVHugoTradeException(rc);
                }
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error deleting tradeid {0} ", tradeId), e);
            }
            finally
            {
                queriesTableAdapter.SetConnection("dbo.delete_trade", null);
            }


            return rc;
        }

        // return trade id on success, null on failure
        public static int? AddStockTrade(IStockTrade stockTrade)
        {
            int? tradeId = null;
            try
            {
                queriesTableAdapter.add_stocktrade(ref tradeId,
                    stockTrade.TradeType,
                    stockTrade.TradeVolume,
                    stockTrade.UnderlyingSymbol,
                    stockTrade.TradePrice,
                    stockTrade.SubAcctName,
                    stockTrade.TradeMedium,
                    stockTrade.ContraName,
                    stockTrade.TradeReason,
                    stockTrade.BrokerName,
                    stockTrade.ExchangeName,
                    stockTrade.TradeNote,
                    null,
                    null,
                    null,
                    stockTrade.TradeArchiveFlag,
                    stockTrade.TraderName,
                    stockTrade.BillingTypeDescr,
                    stockTrade.SpecialRate,
                    stockTrade.TraderId,
                    stockTrade.TradeDate,
                    null,
                    stockTrade.ShortFlag,
                    stockTrade.TaxLotId,
                    stockTrade.TotalCost,
                    stockTrade.FractionalRemainder,
                    null);
                queriesTableAdapter.LogCommand("dbo.add_stocktrade");

                int rc = queriesTableAdapter.GetReturnValue("dbo.add_stocktrade");
                if (rc == 0)
                {
                    openTaxlotsTableFilled = false;
                    taxlotHistoryTableFilled = false;
                    stockTradesTableFilled = false;
                    realizedStockGainsTableFilled = false;
                    taxlotSummaryReportTableFilled = false;
                    FireOnTablesUpdated();

                    FireOnInfo("Added trade: " + stockTrade.ToString());
                }
                else
                {
                    throw new GHVHugoTradeException(rc);
                }
            }
            catch (Exception e)
            {
                FireOnError("Error adding trade: " + stockTrade.ToString(), e);
            }
            return tradeId;
        }

        public static int ChangeTaxlotPrice(InstrumentType instrumentTypeId, int instrumentId, double currentPrice)
        {
            switch (instrumentTypeId)
            {
                case InstrumentType.Stock:
                    return ChangeStockTaxlotPrice((short)instrumentId, currentPrice);
                case InstrumentType.Future:
                    return ChangeFuturesTaxlotPrice((short)instrumentId, currentPrice);
                default: // Call or Put
                    return ChangeOptionTaxlotPrice(instrumentId, (short)instrumentTypeId, currentPrice);
            }
        }
        public static int DeleteTaxlotPrice(InstrumentType instrumentTypeId, int instrumentId)
        {
            switch (instrumentTypeId)
            {
                case InstrumentType.Stock:
                    return DeleteStockTaxlotPrice((short)instrumentId);
                case InstrumentType.Future:
                    return DeleteFuturesTaxlotPrice((short)instrumentId);
                default: // Call or Put
                    return DeleteOptionTaxlotPrice(instrumentId, (short)instrumentTypeId);
            }
        }
        public static int ChangeStockTaxlotStrategyByTaxlotId(string taxlotId, string strategy)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_strategy", Connection);
                queriesTableAdapter.p_update_taxlot_strategy(AccountId,
                    taxlotId,
                    strategy);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //openTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Updated acctId {0}, taxlotId {1} to strategy {2}", AccountId, taxlotId, strategy));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error updating acctId {0}, taxlotId {1} to strategy {2}", AccountId, taxlotId, strategy), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_update_taxlot_strategy");
                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_strategy", null);
            }

            return 0;
        }
        public static int ChangeStockTaxlotStrategyByTradeId(int tradeId, string strategy)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_strategy_by_tradeId", Connection);
                queriesTableAdapter.p_update_taxlot_strategy_by_tradeId(
                    tradeId,
                    strategy);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //openTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Updated tradeId {0} to strategy {1}", tradeId, strategy));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error updating tradeId {0} to strategy {1}", tradeId, strategy), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_update_taxlot_strategy_by_tradeId");
                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_strategy_by_tradeId", null);
            }

            return 0;
        }
        #endregion

        #region Tables
        private static Semaphore stockDailyReturnsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.p_generate_stock_daily_returns_reportDataTable StockDailyReturnsTable
        {
            get
            {
                if (!stockDailyReturnsTableFilled)
                    RefreshStockDailyReturnsTable();
                return hugoDataSet.p_generate_stock_daily_returns_report;
            }
        }
        public static void RefreshStockDailyReturnsTable()
        {
            stockDailyReturnsTableSemaphore.WaitOne();
            try
            {
                stockDailyReturnsTableFilled = false;
                FillStockDailyReturnsTable(Account, null, null);
            }
            finally
            {
                stockDailyReturnsTableSemaphore.Release();
            }
        }
        private static Semaphore stockMonthlyReturnsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.p_generate_stock_monthly_returns_reportDataTable StockMonthlyReturnsTable
        {
            get
            {
                if (!stockMonthlyReturnsTableFilled)
                    RefreshStockMonthlyReturnsTable();
                return hugoDataSet.p_generate_stock_monthly_returns_report;
            }
        }
        public static void RefreshStockMonthlyReturnsTable()
        {
            stockMonthlyReturnsTableSemaphore.WaitOne();
            try
            {
                stockMonthlyReturnsTableFilled = false;
                FillStockMonthlyReturnsTable(Account, null, null);
            }
            finally
            {
                stockMonthlyReturnsTableSemaphore.Release();
            }
        }
        private static Semaphore openTaxlotsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.p_get_open_taxlotsDataTable OpenTaxlotsTable
        {
            get
            {
                if (!openTaxlotsTableFilled)
                    RefreshOpenTaxlotsTable();
                return hugoDataSet.p_get_open_taxlots;
            }
        }
        public static void RefreshOpenTaxlotsTable()
        {
            openTaxlotsTableSemaphore.WaitOne();
            try
            {
                openTaxlotsTableFilled = false;
                FillOpenTaxlotsTable(Account, null, null, EndDate, ScoreDate);
            }
            finally
            {
                openTaxlotsTableSemaphore.Release();
            }
        }
        private static Semaphore stockTradesTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.p_get_stock_tradesDataTable StockTradesTable
        {
            get
            {
                if (!stockTradesTableFilled)
                    RefreshStockTradesTable();
                return hugoDataSet.p_get_stock_trades;
            }
        }
        public static void RefreshStockTradesTable()
        {
            stockTradesTableSemaphore.WaitOne();
            try
            {
                stockTradesTableFilled = false;
                FillStockTradesTable(Account, null, endDate ?? DateTime.Today);
            }
            finally
            {
                stockTradesTableSemaphore.Release();
            }
        }
        private static Semaphore taxlotHistoryTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisDataTable TaxlotHistoryTable
        {
            get
            {
                if (!taxlotHistoryTableFilled)
                    RefreshTaxlotHistoryTable();
                return hugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basis;
            }
        }
        public static void RefreshTaxlotHistoryTable()
        {
            taxlotHistoryTableSemaphore.WaitOne();
            try
            {
                taxlotHistoryTableFilled = false;
                FillTaxlotHistoryTable(Account, underlyingSymbol, taxlotId, endDate ?? DateTime.Today);
            }
            finally
            {
                taxlotHistoryTableSemaphore.Release();
            }
        }
        public static IEnumerable<ITaxLot> OpenTaxlots
        {
            get
            {
                return OpenTaxlotsTable.Taxlots;
            }
        }
        private static Semaphore realizedStockGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.RealizedStockGainsDataTable RealizedStockGainsTable
        {
            get
            {
                if (!realizedStockGainsTableFilled)
                    RefreshRealizedStockGainsTable();
                return hugoDataSet.RealizedStockGains;
            }
        }
        public static void RefreshRealizedStockGainsTable()
        {
            realizedStockGainsTableSemaphore.WaitOne();
            try
            {
                realizedStockGainsTableFilled = false;
                FillRealizedStockGainsTable(Account, null, null, null, EndDate);
            }
            finally
            {
                realizedStockGainsTableSemaphore.Release();
            }
        }
        private static Semaphore realizedEquityOptionGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.RealizedEquityOptionGainsDataTable RealizedEquityOptionGainsTable
        {
            get
            {
                if (!realizedEquityOptionGainsTableFilled)
                {
                    RefreshRealizedEquityOptionGainsTable();
                }
                return hugoDataSet.RealizedEquityOptionGains;
            }
        }
        public static void RefreshRealizedEquityOptionGainsTable()
        {
            realizedEquityOptionGainsTableSemaphore.WaitOne();
            try
            {
                realizedEquityOptionGainsTableFilled = false;
                FillRealizedEquityOptionGainsTable(Account, null, null, null, null, EndDate);
            }
            finally
            {
                realizedEquityOptionGainsTableSemaphore.Release();
            }
        }
        private static Semaphore realizedIndexOptionGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.RealizedIndexOptionGainsDataTable RealizedIndexOptionGainsTable
        {
            get
            {
                if (!realizedIndexOptionGainsTableFilled)
                {
                    RefreshRealizedIndexOptionGainsTable();
                }
                return hugoDataSet.RealizedIndexOptionGains;
            }
        }
        public static void RefreshRealizedIndexOptionGainsTable()
        {
            realizedIndexOptionGainsTableSemaphore.WaitOne();
            try
            {
                realizedIndexOptionGainsTableFilled = false;
                FillRealizedIndexOptionGainsTable(Account, null, null, null, null, EndDate);
            }
            finally
            {
                realizedIndexOptionGainsTableSemaphore.Release();
            }
        }
        private static Semaphore unrealizedEquityOptionGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.UnrealizedEquityOptionGainsDataTable UnrealizedEquityOptionGainsTable
        {
            get
            {
                if (!unrealizedEquityOptionGainsTableFilled)
                {
                    RefreshUnrealizedEquityOptionGainsTable();
                }
                return hugoDataSet.UnrealizedEquityOptionGains;
            }
        }
        public static void RefreshUnrealizedEquityOptionGainsTable()
        {
            unrealizedEquityOptionGainsTableSemaphore.WaitOne();
            try
            {
                unrealizedEquityOptionGainsTableFilled = false;
                FillUnrealizedEquityOptionGainsTable(Account, null, null, null, EndDate);
            }
            finally
            {
                unrealizedEquityOptionGainsTableSemaphore.Release();
            }
        }
        private static Semaphore unrealizedIndexOptionGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.UnrealizedIndexOptionGainsDataTable UnrealizedIndexOptionGainsTable
        {
            get
            {
                if (!unrealizedIndexOptionGainsTableFilled)
                {
                    RefreshUnrealizedIndexOptionGainsTable();
                }
                return hugoDataSet.UnrealizedIndexOptionGains;
            }
        }
        private static Semaphore unrealizedFuturesGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.UnrealizedFuturesGainsDataTable UnrealizedFuturesGainsTable
        {
            get
            {
                if (!unrealizedFuturesGainsTableFilled)
                {
                    RefreshUnrealizedFuturesGainsTable();
                }
                return hugoDataSet.UnrealizedFuturesGains;
            }
        }
        private static Semaphore realizedFuturesGainsTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.RealizedFuturesGainsDataTable RealizedFuturesGainsTable
        {
            get
            {
                if (!realizedFuturesGainsTableFilled)
                {
                    RefreshRealizedFuturesGainsTable();
                }
                return hugoDataSet.RealizedFuturesGains;
            }
        }
        public static void RefreshUnrealizedIndexOptionGainsTable()
        {
            unrealizedIndexOptionGainsTableSemaphore.WaitOne();
            try
            {
                unrealizedIndexOptionGainsTableFilled = false;
                FillUnrealizedIndexOptionGainsTable(Account, null, null, null, EndDate);
            }
            finally
            {
                unrealizedIndexOptionGainsTableSemaphore.Release();
            }
        }
        public static void RefreshUnrealizedFuturesGainsTable()
        {
            unrealizedFuturesGainsTableSemaphore.WaitOne();
            try
            {
                unrealizedFuturesGainsTableFilled = false;
                FillUnrealizedFuturesGainsTable(Account, null, EndDate);
            }
            finally
            {
                unrealizedFuturesGainsTableSemaphore.Release();
            }
        }
        public static void RefreshRealizedFuturesGainsTable()
        {
            realizedFuturesGainsTableSemaphore.WaitOne();
            try
            {
                realizedFuturesGainsTableFilled = false;
                FillRealizedFuturesGainsTable(Account, null, null, EndDate);
            }
            finally
            {
                realizedFuturesGainsTableSemaphore.Release();
            }
        }
        private static Semaphore taxlotSummaryReportTableSemaphore = new Semaphore(1, 1);
        public static HugoDataSet.TaxlotSummaryReportDataTable TaxlotSummaryReportTable
        {
            get
            {
                if (!taxlotSummaryReportTableFilled)
                {
                    RefreshTaxlotSummaryReportTable();
                }
                return hugoDataSet.TaxlotSummaryReport;
            }
        }
        public static void RefreshTaxlotSummaryReportTable()
        {
            taxlotSummaryReportTableSemaphore.WaitOne();
            try
            {
                taxlotSummaryReportTableFilled = false;
                FillTaxlotSummaryReportTable(Account, null, EndDate);
            }
            finally
            {
                taxlotSummaryReportTableSemaphore.Release();
            }
        }
        public static HugoDataSet.AccountsDataTable AccountsTable
        {
            get
            {
                if (!accountsTableFilled)
                    FillAccountsTable();
                return hugoDataSet.Accounts;
            }
        }
        public static HugoDataSet.p_get_importable_trademediaDataTable ImportableTradeMediaTable
        {
            get
            {
                if (!importableTradeMediaTableFilled)
                    FillImportableTradeMediaTable();
                return hugoDataSet.p_get_importable_trademedia;
            }
        }

        public static HugoDataSet.SubAccountsDataTable SubAccountsTable
        {
            get
            {
                if (!subAccountsTableFilled)
                    FillSubAccountsTable();
                return hugoDataSet.SubAccounts;
            }
        }
        public static HugoDataSet.TradersDataTable TradersTable
        {
            get
            {
                if (!tradersTableFilled)
                    FillTradersTable();
                return hugoDataSet.Traders;
            }
        }
        public static IEnumerable<IImportableTradeMedium> ImportableTradeMedia
        {
            get
            {
                foreach (IImportableTradeMedium row in ImportableTradeMediaTable)
                {
                    yield return row;
                }
            }
        }
        public static IEnumerable<string> AccountNames
        {
            get
            {
                foreach (HugoDataSet.AccountsRow row in AccountsTable)
                {
                    yield return row.AcctName;
                }
            }
        }
        public static IEnumerable<string> SubAccountNames
        {
            get
            {
                foreach (HugoDataSet.SubAccountsRow row in SubAccountsTable)
                {
                    yield return row.SubAcctName;
                }
            }
        }
        public static IEnumerable<string> TraderNames
        {
            get
            {
                foreach (HugoDataSet.TradersRow row in TradersTable)
                {
                    yield return row.TraderName;
                }
            }
        }
        #endregion

        #region Properties
        public static string Account
        {
            get { return (selectedAccountsRow == null) ? null : selectedAccountsRow.AcctName; }
            set
            {
                selectedAccountsRow = null;
                foreach (HugoDataSet.AccountsRow row in AccountsTable)
                {
                    if (row.AcctName == value)
                    {
                        selectedAccountsRow = row;
                        break;
                    }
                }
                Refresh();
            }
        }
        public static short AccountId
        {
            get { return (selectedAccountsRow == null) ? (short)-1 : selectedAccountsRow.AcctId; }
            set
            {
                selectedAccountsRow = AccountsTable.FindByAcctId(value);
                Refresh();
            }
        }
        public static string AccountTaxId
        {
            get { return (selectedAccountsRow == null) ? String.Empty : selectedAccountsRow.TaxId; }
        }
        public static string AccountClientName
        {
            get { return (selectedAccountsRow == null) ? String.Empty : selectedAccountsRow.ClientName; }
        }
        public static string UnderlyingSymbol
        {
            get { return underlyingSymbol; }
            set
            {
                if (underlyingSymbol != value)
                {
                    underlyingSymbol = value;
                    taxlotHistoryTableFilled = false;
                    FireOnTablesUpdated();
                }
            }
        }
        public static string TaxlotId
        {
            get { return taxlotId; }
            set
            {
                if (taxlotId != value)
                {
                    taxlotId = value;
                    taxlotHistoryTableFilled = false;
                    FireOnTablesUpdated();
                }
            }
        }
        public static SubtotalGroup GroupBy
        {
            get { return subtotalGroup; }
            set
            {
                subtotalGroup = value;

                openTaxlotsTableFilled = false;
                unrealizedFuturesGainsTableFilled = false;
                unrealizedEquityOptionGainsTableFilled = false;
                unrealizedIndexOptionGainsTableFilled = false;
 
                realizedStockGainsTableFilled = false;
                realizedFuturesGainsTableFilled = false;
                realizedEquityOptionGainsTableFilled = false;
                realizedIndexOptionGainsTableFilled = false;
                FireOnTablesUpdated();
            }
        }

        // the date on which we are interested in our taxlot positions
        //  usually today, but may be set to any date by the user
        public static DateTime? EndDate
        {
            get { return endDate; }
            set
            {
                if (value.HasValue)
                    value -= value.Value.TimeOfDay;
                if (endDate != value)
                {
                    endDate = value;

                    if (endDate.HasValue)
                    {
                        //set StartDate and ClosingDate
                        closingDate = ReconciliationLib.Utilities.CalculatePreviousBusinessDay(endDate.Value);
                        DateTime? startingDatePlusOne = GHVHugoLib.Utilities.GetStartOfYear(endDate.Value);
                        startingDate = ReconciliationLib.Utilities.CalculatePreviousBusinessDay(startingDatePlusOne.Value);
                    }
                    Refresh();
                }
            }
        }
        // the date on which we calculate the score (for purposes of choosing which taxlot to close)
        //  in the current implementation, this is always null, so it is set by the stored procedure
        //  (where it defaults to EndDate)
        public static DateTime? ScoreDate
        {
            get { return scoreDate; }
            set
            {
                if (value.HasValue)
                    value -= value.Value.TimeOfDay;
                if (scoreDate != value)
                {
                    scoreDate = value;
                    Refresh();
                }
            }
        }
        // the date range for which we examine closed taxlots
        // by default, StartingDate is the business day immediately preceding Jan 1 of the current year
        //  and ClosingDate is the business day immediately preceding EndDate
        public static DateTime? StartingDate
        {
            get { return startingDate; }
            set
            {
                if (value.HasValue)
                    value -= value.Value.TimeOfDay;
                if (startingDate != value)
                {
                    startingDate = value;
                    Refresh();
                }
            }
        }
        public static DateTime? ClosingDate
        {
            get { return closingDate; }
            set
            {
                if (value.HasValue)
                    value -= value.Value.TimeOfDay;
                if (closingDate != value)
                {
                    closingDate = value;
                    Refresh();
                }
            }
        }
        internal static bool AccountsTableFilled { get { return accountsTableFilled; } }
        internal static bool SubAccountsTableFilled { get { return subAccountsTableFilled; } }
        internal static bool TradersTableFilled { get { return tradersTableFilled; } }
        internal static bool OpenTaxlotsTableFilled { get { return openTaxlotsTableFilled; } }
        internal static bool TaxlotHistoryTableFilled { get { return taxlotHistoryTableFilled; } }
        internal static bool StockTradesTableFilled { get { return stockTradesTableFilled; } }
        internal static bool RealizedStockGainsTableFilled { get { return realizedStockGainsTableFilled; } }
        internal static bool RealizedEquityOptionGainsTableFilled { get { return realizedEquityOptionGainsTableFilled; } }
        internal static bool RealizedIndexOptionGainsTableFilled { get { return realizedIndexOptionGainsTableFilled; } }
        internal static bool UnrealizedEquityOptionGainsTableFilled { get { return unrealizedEquityOptionGainsTableFilled; } }
        internal static bool UnrealizedIndexOptionGainsTableFilled { get { return unrealizedIndexOptionGainsTableFilled; } }
        internal static bool UnrealizedFuturesGainsTableFilled { get { return unrealizedFuturesGainsTableFilled; } }
        internal static bool RealizedFuturesGainsTableFilled { get { return realizedFuturesGainsTableFilled; } }
        internal static bool TaxlotSummaryReportTableFilled { get { return taxlotSummaryReportTableFilled; } }
        internal static bool StockDailyReturnsTableFilled { get { return stockDailyReturnsTableFilled; } }
        internal static bool StockMonthlyReturnsTableFilled { get { return stockMonthlyReturnsTableFilled; } }
        internal static bool ImportableTradeMediaTableFilled { get { return importableTradeMediaTableFilled; } }
        #endregion


        private static Gargoyle.Utils.DBAccess.DBAccess dbAccess;
        public static Gargoyle.Utils.DBAccess.DBAccess DBAccess
        {
            set
            {

                FireOnInfo(String.Format("Connection changed from {0} to {1}",
                    (dbAccess == null) ? "<NULL>" : dbAccess.ConnectionString("Hugo"),
                    (value == null) ? "<NULL>" : value.ConnectionString("Hugo")));

                dbAccess = value;

                openTaxlotsTableFilled = false;
                accountsTableFilled = false;
                subAccountsTableFilled = false;
                tradersTableFilled = false;
                taxlotHistoryTableFilled = false;
                stockTradesTableFilled = false;
                realizedStockGainsTableFilled = false;
                realizedEquityOptionGainsTableFilled = false;
                realizedIndexOptionGainsTableFilled = false;
                unrealizedEquityOptionGainsTableFilled = false;
                unrealizedIndexOptionGainsTableFilled = false;
                unrealizedFuturesGainsTableFilled = false;
                realizedFuturesGainsTableFilled = false;
                taxlotSummaryReportTableFilled = false;
                importableTradeMediaTableFilled = false;
                stockDailyReturnsTableFilled = false;
                stockMonthlyReturnsTableFilled = false;
                FireOnTablesUpdated();
            }
            get
            {
                return dbAccess;
            }
        }

        internal static SqlConnection Connection
        {
            get { return DBAccess.GetConnection("Hugo"); }
        }


        public static void SetCommandTimeouts(int timeout)
        {
            queriesTableAdapter.SetCommandTimeout(timeout);
            openTaxlotsAdapter.SetCommandTimeout(timeout);
            accountsAdapter.SetCommandTimeout(timeout);
            subAccountsAdapter.SetCommandTimeout(timeout);
            tradersAdapter.SetCommandTimeout(timeout);
            stockTradesAdapter.SetCommandTimeout(timeout);
            spinoffInfoAdapter.SetCommandTimeout(timeout);
            taxlotHistoryAdapter.SetCommandTimeout(timeout);
            realizedStockGainsAdapter.SetCommandTimeout(timeout);
            realizedEquityOptionGainsAdapter.SetCommandTimeout(timeout);
            realizedIndexOptionGainsAdapter.SetCommandTimeout(timeout);
            unrealizedEquityOptionGainsAdapter.SetCommandTimeout(timeout);
            unrealizedIndexOptionGainsAdapter.SetCommandTimeout(timeout);
            unrealizedFuturesGainsAdapter.SetCommandTimeout(timeout);
            realizedFuturesGainsAdapter.SetCommandTimeout(timeout);
            taxlotSummaryReportAdapter.SetCommandTimeout(timeout);
            importableTradeMediaTableAdapter.SetCommandTimeout(timeout);
            generateStockDailyReturnsReportTableAdapter.SetCommandTimeout(timeout);
            generateStockMonthlyReturnsReportTableAdapter.SetCommandTimeout(timeout);
        }

        #region event handlers
        // event fired when an exception occurs
        public static event EventHandler<LoggingUtilitiesLib.ErrorEventArgs> OnError
        {
            add { LoggingUtilities.OnError += value; }
            remove { LoggingUtilities.OnError -= value; }
        }
        // event fired for logging
        public static event EventHandler<LoggingUtilitiesLib.InfoEventArgs> OnInfo
        {
            add { LoggingUtilities.OnInfo += value; }
            remove { LoggingUtilities.OnInfo -= value; }
        }
        // event fired when tables are updated
        public static event EventHandler<TablesUpdatedEventArgs> OnTablesUpdated
        {
            add { tablesUpdatedEventHandler += value; }
            remove { tablesUpdatedEventHandler -= value; }
        }
        #endregion

        public static void Dispose()
        {
            if (hugoDataSet != null)
            {
                hugoDataSet.Dispose();
                hugoDataSet = null;
            }
            if (queriesTableAdapter != null)
            {
                queriesTableAdapter.Dispose();
                queriesTableAdapter = null;
            }

        }
        #endregion

        #region Internal helper functions
        internal static int ParseCommaInfestedInteger(string input)
        {
            return (int)ParseCommaInfestedDouble(input);
        }
        internal static double ParseCommaInfestedDouble(string input)
        {
            if (String.IsNullOrEmpty(input))
                return 0.0;
            else
                return Double.Parse(input.Replace(",", ""));
        }
        internal static DateTime? ParseDateRange(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                string[] dates = input.Split(new char[] { ' ' });
                return ParseNullableDate(dates[dates.Length - 1]);
            }
            return null;
        }
        internal static DateTime? ParseNullableDate(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(input);
            }
        }
        #endregion
        #region Private helper functions
        private static int ChangeStockTaxlotPrice(short underlyingId, double currentPrice)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_price", Connection);
                queriesTableAdapter.p_update_taxlot_price(Account,
                    null,
                    underlyingId,
                    EndDate,
                    currentPrice);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //openTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Updated taxlots for underlying {0} to price {1}", underlyingId, currentPrice));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error updating taxlots for underlying {0} to price {1}", underlyingId, currentPrice), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_update_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_update_taxlot_price", null);
            }

            return 0;
        }
        private static int DeleteStockTaxlotPrice(short underlyingId)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_delete_taxlot_price", Connection);
                queriesTableAdapter.p_delete_taxlot_price(Account,
                    null,
                    underlyingId,
                    EndDate);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //openTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Deleted override price for underlying {0}", underlyingId));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error deleting override price for underlying {0}", underlyingId), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_delete_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_delete_taxlot_price", null);
            }

            return 0;
        }
        private static int ChangeFuturesTaxlotPrice(short futuresId, double currentPrice)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_update_futures_taxlot_price", Connection);
                queriesTableAdapter.p_update_futures_taxlot_price(Account,
                    null,
                    futuresId,
                    EndDate,
                    currentPrice);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //optionTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Updated taxlots for futures id={0} to price {1}", futuresId, currentPrice));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error updating taxlots for futures id={0} to price {1}", futuresId, currentPrice), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_update_futures_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_update_futures_taxlot_price", null);
            }

            return 0;
        }
        private static int DeleteFuturesTaxlotPrice(short futuresId)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_delete_futures_taxlot_price", Connection);
                queriesTableAdapter.p_delete_futures_taxlot_price(Account,
                    null,
                    futuresId,
                    EndDate);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //optionTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Deleted override price for futures id={0}", futuresId));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error deleting override price for futures id={0}", futuresId), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_delete_futures_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_delete_futures_taxlot_price", null);
            }

            return 0;
        }
        private static int ChangeOptionTaxlotPrice(int optionId, short optionTypeId, double currentPrice)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_update_option_taxlot_price", Connection);
                queriesTableAdapter.p_update_option_taxlot_price(Account,
                    null,
                    optionId,
                    optionTypeId,
                    EndDate,
                    currentPrice);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //optionTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Updated taxlots for option {0}-{1} to price {2}", optionId, optionTypeId, currentPrice));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error updating taxlots for option {0}-{1} to price {2}", optionId, optionTypeId, currentPrice), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_update_option_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_update_option_taxlot_price", null);
            }

            return 0;
        }
        private static int DeleteOptionTaxlotPrice(int optionId, short optionTypeId)
        {
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                queriesTableAdapter.SetConnection("GHV.p_delete_option_taxlot_price", Connection);
                queriesTableAdapter.p_delete_option_taxlot_price(Account,
                    null,
                    optionId,
                    optionTypeId,
                    EndDate);

                // DO NOT DO THIS - caller needs to be able to iterate through table rows, calling
                //  this methods for each row, we cannot invalidate the table in this method

                //optionTaxlotsTableFilled = false;
                //FireOnTablesUpdated();

                FireOnInfo(String.Format("Deleted override price for option {0}-{1}", optionId, optionTypeId));

            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error deleting override price for option {0}-{1}", optionId, optionTypeId), e);
                return -1;
            }
            finally
            {
                queriesTableAdapter.LogCommand("GHV.p_delete_option_taxlot_price");
                queriesTableAdapter.SetConnection("GHV.p_delete_option_taxlot_price", null);
            }

            return 0;
        }

        private static void VerifyInitialization(bool needAccount)
        {
            if (DBAccess == null)
            {
                throw new GHVHugoNotInitializedException("DBAccess has not been set");
            }
            if (needAccount && String.IsNullOrEmpty(Account))
            {
                throw new GHVHugoNotInitializedException("Account has not been set");
            }
        }

        #region Fire events
        internal static void FireOnInfo(string msg)
        {
            LoggingUtilities.Info("GHVHugoLib", msg);
        }
        internal static void FireOnError(string msg, Exception e)
        {
            LoggingUtilities.Error("GHVHugoLib", msg, e);
        }
        private static void FireOnTablesUpdated()
        {
            if (tablesUpdatedEventHandler != null)
            {
                try
                {
                    TablesUpdatedEventArgs eventArgs = new TablesUpdatedEventArgs();
                    tablesUpdatedEventHandler(null, eventArgs);

                    FireOnInfo(eventArgs.ToString());
                }
                catch (Exception e)
                {
                    FireOnError("Error in TablesUpdated event handler", e);
                }
            }
        }
        #endregion

        #region Fill tables
        private static int FillStockDailyReturnsTable(string inputAcctName, DateTime? inputStartDate, DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                generateStockDailyReturnsReportTableAdapter.Connection = Connection;
                count = generateStockDailyReturnsReportTableAdapter.Fill(hugoDataSet.p_generate_stock_daily_returns_report, inputAcctName, inputStartDate, inputEndDate);
                stockDailyReturnsTableFilled = true;
                FireOnTablesUpdated();

                generateStockDailyReturnsReportTableAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling stock daily returns table, account={0}, startdate={1}, endDate={2}",
                    inputAcctName ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate)), e);
            }
            finally
            {
                if (generateStockDailyReturnsReportTableAdapter.Connection != null)
                {
                    generateStockDailyReturnsReportTableAdapter.Connection.Close();
                    generateStockDailyReturnsReportTableAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} stock daily return rowns, account={1}, startdate={2}, endDate={3}",
                count,
                inputAcctName ?? "<NULL>",
                 ReconciliationConvert.ToNullableString(inputStartDate),
                ReconciliationConvert.ToNullableString(inputEndDate)));
            return count;
        }
        private static int FillStockMonthlyReturnsTable(string inputAcctName, DateTime? inputStartDate, DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                generateStockMonthlyReturnsReportTableAdapter.Connection = Connection;
                count = generateStockMonthlyReturnsReportTableAdapter.Fill(hugoDataSet.p_generate_stock_monthly_returns_report, inputAcctName, inputStartDate, inputEndDate);
                stockMonthlyReturnsTableFilled = true;
                FireOnTablesUpdated();

                generateStockMonthlyReturnsReportTableAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling stock Monthly returns table, account={0}, startdate={1}, endDate={2}",
                    inputAcctName ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate)), e);
            }
            finally
            {
                if (generateStockMonthlyReturnsReportTableAdapter.Connection != null)
                {
                    generateStockMonthlyReturnsReportTableAdapter.Connection.Close();
                    generateStockMonthlyReturnsReportTableAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} stock Monthly return rowns, account={1}, startdate={2}, endDate={3}",
                count,
                inputAcctName ?? "<NULL>",
                 ReconciliationConvert.ToNullableString(inputStartDate),
                ReconciliationConvert.ToNullableString(inputEndDate)));
            return count;
        }

        private static int FillOpenTaxlotsTable(string inputAcctName, string inputUnderlyingSymbol, string inputTaxlotId, DateTime? inputEndDate, DateTime? inputScoreDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                openTaxlotsAdapter.Connection = Connection;
                count = openTaxlotsAdapter.Fill(hugoDataSet.p_get_open_taxlots, inputAcctName, inputUnderlyingSymbol, inputTaxlotId, inputEndDate, inputScoreDate, GroupBy == SubtotalGroup.Instrument);
                openTaxlotsTableFilled = true;
                FireOnTablesUpdated();

                openTaxlotsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling open taxlots table, account={0}, underlyingsymbol={1}, taxlotid={2}, enddate={3}, scoreDate={4}, groupbyticker={5}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    inputTaxlotId ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    ReconciliationConvert.ToNullableString(inputScoreDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (openTaxlotsAdapter.Connection != null)
                {
                    openTaxlotsAdapter.Connection.Close();
                    openTaxlotsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} open taxlots, account={1}, underlyingsymbol={2}, taxlotid={3}, enddate={4}, scoreDate={5}, groupbyticker={6}",
                count,
                inputAcctName ?? "<NULL>",
                inputUnderlyingSymbol ?? "<NULL>",
                inputTaxlotId ?? "<NULL>",
                ReconciliationConvert.ToNullableString(inputEndDate),
                ReconciliationConvert.ToNullableString(inputScoreDate),
                GroupBy == SubtotalGroup.Instrument));
            return count;
        }

        private static int FillRealizedStockGainsTable(string inputAcctName, string inputUnderlyingSymbol, string inputTaxlotId, DateTime? inputStartDate, DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                realizedStockGainsAdapter.Connection = Connection;
                count = realizedStockGainsAdapter.Fill(hugoDataSet.RealizedStockGains, inputAcctName, inputUnderlyingSymbol, inputTaxlotId, inputStartDate, inputEndDate, GroupBy == SubtotalGroup.Instrument);
                realizedStockGainsTableFilled = true;
                FireOnTablesUpdated();

                realizedStockGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling realized gains table, account={0}, underlyingsymbol={1}, taxlotid={2}, startDate = {3}, enddate={4}, groupbyticker={5}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    inputTaxlotId ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (realizedStockGainsAdapter.Connection != null)
                {
                    realizedStockGainsAdapter.Connection.Close();
                    realizedStockGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} realized gains records, account={1}, underlyingsymbol={2}, taxlotid={3}, startdate={4}, enddate={5}, groupbyticker={6}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    inputTaxlotId ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }

        private static int FillRealizedEquityOptionGainsTable(string inputAcctName,
                string inputUnderlyingSymbol,
                int? inputOptionId,
                short? inputOptionTypeId,
                DateTime? inputStartDate,
                DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                realizedEquityOptionGainsAdapter.Connection = Connection;
                count = realizedEquityOptionGainsAdapter.Fill(hugoDataSet.RealizedEquityOptionGains,
                    inputAcctName,
                    inputUnderlyingSymbol,
                    inputOptionId,
                    inputOptionTypeId,
                    inputStartDate,
                    inputEndDate,
                    false,
                    GroupBy == SubtotalGroup.Instrument);
                realizedEquityOptionGainsTableFilled = true;
                FireOnTablesUpdated();

                realizedEquityOptionGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling realized equity option gains table, account={0}, underlyingsymbol={1}, optionid={2}, optiontypeid={3}, startDate={4}, enddate={5}, indexflag=false, groupbyoption={6}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (realizedEquityOptionGainsAdapter.Connection != null)
                {
                    realizedEquityOptionGainsAdapter.Connection.Close();
                    realizedEquityOptionGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} realized equity option gains records, account={1}, underlyingsymbol={2}, optionid={3}, optiontypeid={4}, startDate={5}, enddate={6}, indexflag=false, groupbyoption={7}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }
        private static int FillRealizedIndexOptionGainsTable(string inputAcctName,
                string inputUnderlyingSymbol,
                int? inputOptionId,
                short? inputOptionTypeId,
                DateTime? inputStartDate,
                DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                realizedIndexOptionGainsAdapter.Connection = Connection;
                count = realizedIndexOptionGainsAdapter.Fill(hugoDataSet.RealizedIndexOptionGains,
                    inputAcctName,
                    inputUnderlyingSymbol,
                    inputOptionId,
                    inputOptionTypeId,
                    inputStartDate,
                    inputEndDate,
                    true,
                    GroupBy == SubtotalGroup.Instrument);
                realizedIndexOptionGainsTableFilled = true;
                FireOnTablesUpdated();

                realizedIndexOptionGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling realized index option gains table, account={0}, underlyingsymbol={1}, optionid={2}, optiontypeid={3}, startDate={4}, enddate={5}, indexflag=true, groupbyoption={6}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (realizedIndexOptionGainsAdapter.Connection != null)
                {
                    realizedIndexOptionGainsAdapter.Connection.Close();
                    realizedIndexOptionGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} realized index option gains records, account={1}, underlyingsymbol={2}, optionid={3}, optiontypeid={4}, startDate={5}, enddate={6}, indexflag=true, groupbyoption={7}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }
        private static int FillUnrealizedEquityOptionGainsTable(string inputAcctName,
              string inputUnderlyingSymbol,
              int? inputOptionId,
              short? inputOptionTypeId,
              DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                unrealizedEquityOptionGainsAdapter.Connection = Connection;
                count = unrealizedEquityOptionGainsAdapter.Fill(hugoDataSet.UnrealizedEquityOptionGains,
                    inputAcctName,
                    inputUnderlyingSymbol,
                    inputOptionId,
                    inputOptionTypeId,
                    inputEndDate,
                    false,
                    GroupBy == SubtotalGroup.Instrument);
                unrealizedEquityOptionGainsTableFilled = true;
                FireOnTablesUpdated();

                unrealizedEquityOptionGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling unrealized equity option gains table, account={0}, underlyingsymbol={1}, optionid={2}, optiontypeid={3}, enddate={4}, indexflag=false, groupbyoption={5}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (unrealizedEquityOptionGainsAdapter.Connection != null)
                {
                    unrealizedEquityOptionGainsAdapter.Connection.Close();
                    unrealizedEquityOptionGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} unrealized equity option gains records, account={1}, underlyingsymbol={2}, optionid={3}, optiontypeid={4}, enddate={5}, indexflag=false, groupbyoption={6}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }

        private static int FillUnrealizedIndexOptionGainsTable(string inputAcctName,
              string inputUnderlyingSymbol,
              int? inputOptionId,
              short? inputOptionTypeId,
              DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                unrealizedIndexOptionGainsAdapter.Connection = Connection;
                count = unrealizedIndexOptionGainsAdapter.Fill(hugoDataSet.UnrealizedIndexOptionGains,
                    inputAcctName,
                    inputUnderlyingSymbol,
                    inputOptionId,
                    inputOptionTypeId,
                    inputEndDate,
                    true,
                    (GroupBy == SubtotalGroup.Instrument));
                unrealizedIndexOptionGainsTableFilled = true;
                FireOnTablesUpdated();

                unrealizedIndexOptionGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling unrealized index option gains table, account={0}, underlyingsymbol={1}, optionid={2}, optiontypeid={3}, enddate={4}, indexflag=false, groupbyoption={5}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (unrealizedIndexOptionGainsAdapter.Connection != null)
                {
                    unrealizedIndexOptionGainsAdapter.Connection.Close();
                    unrealizedIndexOptionGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} unrealized index option gains records, account={1}, underlyingsymbol={2}, optionid={3}, optiontypeid={4}, enddate={5}, indexflag=false, groupbyoption={6}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputOptionId),
                    ReconciliationConvert.ToNullableString(inputOptionTypeId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }
        private static int FillUnrealizedFuturesGainsTable(string inputAcctName,
                short? inputFuturesId,
                DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                unrealizedFuturesGainsAdapter.Connection = Connection;
                count = unrealizedFuturesGainsAdapter.Fill(hugoDataSet.UnrealizedFuturesGains,
                    inputAcctName,
                     inputFuturesId,
                     inputEndDate,
                    GroupBy == SubtotalGroup.Instrument);
                unrealizedFuturesGainsTableFilled = true;
                FireOnTablesUpdated();

                unrealizedFuturesGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling unrealized futures gains table, account={0}, futuresid={1}, enddate={2}, groupbyoption={3}",
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputFuturesId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (unrealizedFuturesGainsAdapter.Connection != null)
                {
                    unrealizedFuturesGainsAdapter.Connection.Close();
                    unrealizedFuturesGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} unrealized futures records, account={0}, futuresid={1}, enddate={2}, groupbyoption={3}",
                    count,
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputFuturesId),
                     ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }
        private static int FillRealizedFuturesGainsTable(string inputAcctName,
                 short? inputFuturesId,
                 DateTime? inputStartDate,
                DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                realizedFuturesGainsAdapter.Connection = Connection;
                count = realizedFuturesGainsAdapter.Fill(hugoDataSet.RealizedFuturesGains,
                    inputAcctName,
                     inputFuturesId,
                     inputStartDate,
                    inputEndDate,
                    GroupBy == SubtotalGroup.Instrument);
                realizedFuturesGainsTableFilled = true;
                FireOnTablesUpdated();

                realizedFuturesGainsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling realized futures gains table, account={0}, futuresid={1}, enddate={2}, groupbyoption={3}",
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputFuturesId),
                    ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument), e);
            }
            finally
            {
                if (realizedFuturesGainsAdapter.Connection != null)
                {
                    realizedFuturesGainsAdapter.Connection.Close();
                    realizedFuturesGainsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} realized futures records, account={0}, futuresid={1}, enddate={2}, groupbyoption={3}",
                    count,
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputFuturesId),
                     ReconciliationConvert.ToNullableString(inputEndDate),
                    GroupBy == SubtotalGroup.Instrument));
            return count;
        }

        private static int FillTaxlotSummaryReportTable(string inputAcctName,
                           DateTime? inputStartDate,
                           DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                taxlotSummaryReportAdapter.Connection = Connection;
                count = taxlotSummaryReportAdapter.Fill(hugoDataSet.TaxlotSummaryReport,
                    inputAcctName,
                    inputStartDate,
                    inputEndDate);
                taxlotSummaryReportTableFilled = true;
                FireOnTablesUpdated();

                taxlotSummaryReportAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling taxlot summary report table, account={0}, startdate={1}, enddate={2}",
                    inputAcctName ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate)), e);
            }
            finally
            {
                if (taxlotSummaryReportAdapter.Connection != null)
                {
                    taxlotSummaryReportAdapter.Connection.Close();
                    taxlotSummaryReportAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} taxlot summary report records, account={1}, startdate={2}, enddate={3}",
                    count,
                    inputAcctName ?? "<NULL>",
                    ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate)));
            return count;
        }
        private static int FillStockTradesTable(string inputAcctName, DateTime? inputStartDate, DateTime? inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                stockTradesAdapter.Connection = Connection;
                count = stockTradesAdapter.Fill(hugoDataSet.p_get_stock_trades, null, inputAcctName, inputStartDate, inputEndDate);
                stockTradesTableFilled = true;
                FireOnTablesUpdated();

                stockTradesAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling stock table, account={0}, startdate={1}, enddate={2}",
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputStartDate),
                     ReconciliationConvert.ToNullableString(inputEndDate)), e);
            }
            finally
            {
                if (stockTradesAdapter.Connection != null)
                {
                    stockTradesAdapter.Connection.Close();
                    stockTradesAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} stock trades, account={1}, startdate={2}, enddate={3}",
                    count,
                    inputAcctName ?? "<NULL>",
                     ReconciliationConvert.ToNullableString(inputStartDate),
                    ReconciliationConvert.ToNullableString(inputEndDate)));
            return count;
        }

        private static int FillTaxlotHistoryTable(string inputAcctName, string inputUnderlyingSymbol, string inputTaxlotId, DateTime inputEndDate)
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                if ((taxlotId == null) && (inputUnderlyingSymbol == null))
                {
                    hugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basis.Clear();
                }
                else
                {
                    taxlotHistoryAdapter.Connection = Connection;
                    count = taxlotHistoryAdapter.Fill(hugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basis, inputAcctName, inputUnderlyingSymbol, inputTaxlotId, inputEndDate);
                    taxlotHistoryAdapter.LogCommand();
                }
                taxlotHistoryTableFilled = true;
                FireOnTablesUpdated();
            }
            catch (Exception e)
            {
                FireOnError(String.Format("Error filling taxlot history table, account={0}, underlyingsymbol={1}, taxlotid={2}, enddate={3}",
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    inputTaxlotId ?? "<NULL>",
                    inputEndDate), e);
            }
            finally
            {
                if (taxlotHistoryAdapter.Connection != null)
                {
                    taxlotHistoryAdapter.Connection.Close();
                    taxlotHistoryAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} taxlot history records, account={1}, underlyingsymbol={2}, taxlotid={3}, enddate={4}",
                    count,
                    inputAcctName ?? "<NULL>",
                    inputUnderlyingSymbol ?? "<NULL>",
                    inputTaxlotId ?? "<NULL>",
                    inputEndDate));
            return count;
        }
        private static int FillAccountsTable()
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ false);

                accountsAdapter.Connection = Connection;
                count = accountsAdapter.Fill(hugoDataSet.Accounts);
                accountsTableFilled = true;
                subAccountsTableFilled = false;
                FireOnTablesUpdated();

                accountsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError("Error filling accounts table", e);
            }
            finally
            {
                if (accountsAdapter.Connection != null)
                {
                    accountsAdapter.Connection.Close();
                    accountsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} accounts", count));
            return count;
        }
        private static int FillImportableTradeMediaTable()
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ false);

                importableTradeMediaTableAdapter.Connection = Connection;
                count = importableTradeMediaTableAdapter.Fill(hugoDataSet.p_get_importable_trademedia);
                importableTradeMediaTableFilled = true;
                FireOnTablesUpdated();

                importableTradeMediaTableAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError("Error filling accounts table", e);
            }
            finally
            {
                if (importableTradeMediaTableAdapter.Connection != null)
                {
                    importableTradeMediaTableAdapter.Connection.Close();
                    importableTradeMediaTableAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} accounts", count));
            return count;
        }
        private static int FillSubAccountsTable()
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ true);

                subAccountsAdapter.Connection = Connection;
                count = subAccountsAdapter.Fill(hugoDataSet.SubAccounts, Account);
                subAccountsTableFilled = true;
                FireOnTablesUpdated();

                subAccountsAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError("Error filling subaccounts table", e);
            }
            finally
            {
                if (subAccountsAdapter.Connection != null)
                {
                    subAccountsAdapter.Connection.Close();
                    subAccountsAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} subaccounts", count));
            return count;
        }
        private static int FillTradersTable()
        {
            int count = 0;
            try
            {
                VerifyInitialization(/*needAccount =*/ false);

                tradersAdapter.Connection = Connection;
                count = tradersAdapter.Fill(hugoDataSet.Traders);
                tradersTableFilled = true;
                FireOnTablesUpdated();

                tradersAdapter.LogCommand();
            }
            catch (Exception e)
            {
                FireOnError("Error filling traders table", e);
            }
            finally
            {
                if (tradersAdapter.Connection != null)
                {
                    tradersAdapter.Connection.Close();
                    tradersAdapter.Connection = null;
                }
            }
            FireOnInfo(String.Format("Retreived {0} traders", count));
            return count;
        }
        #endregion

        private static OleDbDataAdapter GetSelectAllFromExcelWorksheetCommand(string excelWorksheetName, OleDbConnection cn)
        {
            return new System.Data.OleDb.OleDbDataAdapter(String.Format(selectAllFromExcelWorkSheetCommand, excelWorksheetName), cn);
        }

        private static OleDbConnection GetExcelConnection(string excelFileName)
        {
            return new OleDbConnection(String.Format(excelConnectionString, excelFileName));
        }
        #endregion
    }
}
