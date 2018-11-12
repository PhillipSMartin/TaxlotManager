using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using Gargoyle.Configuration;
using Gargoyle.Utils.DataGridViewPrinterLib;
using LoggingUtilitiesLib;

namespace TaxlotManager
{
    public partial class Form1 : Form
    {
        #region Declarations
        private bool m_settingsChanged;
        private Gargoyle.Utils.DBAccess.DBAccess m_dbAccess;
        private string m_applicationName;
        private string m_databaseName;

        private static string s_selectedTaxlotId;
        private static EventHandler s_selectedTaxlotIdChanged;
        private static EventHandler s_currentDateChanged;
        private static ILog s_logger;     // log4net logger

        private DataGridStateManagerForOpenTaxlots openTaxlotsDataGridStateManager;
        private DataGridStateManagerForUnrealizedPandL unrealizedEquityOptionDataGridStateManager;
        private DataGridStateManagerForUnrealizedPandL unrealizedIndexOptionDataGridStateManager;
        private DataGridStateManagerForUnrealizedPandL unrealizedFuturesDataGridStateManager;
        private DataGridStateManagerForTaxlotHistory taxlotHistoryDataGridStateManager;
        private DataGridStateManager stockTradesDataGridStateManager;
        private DataGridStateManagerForRealizedPandL realizedGainLossDataGridStateManager;
        private DataGridStateManagerForRealizedPandL realizedFuturesDataGridStateManager;
        private DataGridStateManagerForRealizedPandL realizedEquityOptionDataGridStateManager;
        private DataGridStateManagerForRealizedPandL realizedIndexOptionDataGridStateManager;
        private DataGridStateManagerForTaxlotSummary taxlotSummaryDataGridStateManager;
        private DataGridStateManager stockDailyReturnsDataGridStateManager;
        private DataGridStateManager stockMonthlyReturnsDataGridStateManager;
        private DataGridViewPrinter dataGridViewPrinter = new DataGridViewPrinter();
 
        private ToolStripMenuItem adjustCostBasisCommand;
        private ToolStripMenuItem adjustSpinoffCommand;
        private ToolStripMenuItem deleteTradeCommand;
        private ToolStripMenuItem deleteOverridePriceCommand;
        private ToolStripMenuItem stockStrategyCommand;
        private ToolStripMenuItem shortCallStrategyCommand;
        private ToolStripMenuItem longPutStrategyCommand;
        #endregion

        #region Constructors
        public Form1()
        {
            Type classType = this.GetType();
            log4net.Config.XmlConfigurator.Configure();
            s_logger = LogManager.GetLogger(classType);

            InitializeComponent();

            InitializeDataGridViews();
            BuildContextMenuCommands();

            EnableControls();
        }
        #endregion

        #region Dispose methods
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GHVHugoLib.Utilities.Dispose();
                ReconciliationLib.Utilities.Dispose();
   
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }


                if (dataGridViewPrinter != null)
                {
                    dataGridViewPrinter.Dispose();
                    dataGridViewPrinter = null;
                }

            }
            base.Dispose(disposing);
        }
        #endregion

        #region Events
        public static event EventHandler SelectedTaxlotIdChanged
        {
            add { s_selectedTaxlotIdChanged += value; }
            remove { s_selectedTaxlotIdChanged -= value; }
        }
        public static event EventHandler CurrentDateChanged
        {
            add { s_currentDateChanged += value; }
            remove { s_currentDateChanged -= value; }
        }
        #endregion

        #region Event handlers

        #region Utilities events
        private void Utilities_OnError(object sender, ErrorEventArgs e)
        {
            OnError(e.Message, e.Exception);
        }
        private void Utilities_OnInfo(object sender, InfoEventArgs e)
        {
            s_logger.Info(e.Message);
        }
        private delegate void OnTablesUpdatedDelegate(GHVHugoLib.TablesUpdatedEventArgs e);
        private void Utilities_OnTablesUpdated(object sender, GHVHugoLib.TablesUpdatedEventArgs e)
        {
            BeginInvoke(new OnTablesUpdatedDelegate(OnTablesUpdatedCallBack), e);
        }
        private void OnTablesUpdatedCallBack(GHVHugoLib.TablesUpdatedEventArgs e)
        {
            if (e.AccountsTableUpdated)
            {
                FillAccountsComboBox();
            }
            openTaxlotsDataGridStateManager.NeedToRefresh |= e.OpenTaxlotsTableUpdated;
            unrealizedFuturesDataGridStateManager.NeedToRefresh |= e.UnrealizedFuturesGainsTableUpdated;
            unrealizedEquityOptionDataGridStateManager.NeedToRefresh |= e.UnrealizedEquityOptionGainsTableUpdated;
            unrealizedIndexOptionDataGridStateManager.NeedToRefresh |= e.UnrealizedIndexOptionGainsTableUpdated;

            realizedGainLossDataGridStateManager.NeedToRefresh |= e.RealizedStockGainsTableUpdated;
            realizedFuturesDataGridStateManager.NeedToRefresh |= e.RealizedFuturesGainsTableUpdated;
            realizedEquityOptionDataGridStateManager.NeedToRefresh |= e.RealizedEquityOptionGainsTableUpdated;
            realizedIndexOptionDataGridStateManager.NeedToRefresh |= e.RealizedIndexOptionGainsTableUpdated;

            taxlotHistoryDataGridStateManager.NeedToRefresh |= e.TaxlotHistoryTableUpdated;
            stockTradesDataGridStateManager.NeedToRefresh |= e.StockTradesTableUpdated;
            taxlotSummaryDataGridStateManager.NeedToRefresh |= e.TaxlotSummaryReportTableUpdated;
            stockDailyReturnsDataGridStateManager.NeedToRefresh |= e.StockDailyReturnsTableUpdated;
            stockMonthlyReturnsDataGridStateManager.NeedToRefresh |= e.StockMonthlyReturnsTableUpdated;
        }
        #endregion

        #region Form events
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                // wire events
                GHVHugoLib.Utilities.OnError += new EventHandler<ErrorEventArgs>(Utilities_OnError);
                GHVHugoLib.Utilities.OnInfo += new EventHandler<InfoEventArgs>(Utilities_OnInfo);
                GHVHugoLib.Utilities.OnTablesUpdated += new EventHandler<GHVHugoLib.TablesUpdatedEventArgs>(Utilities_OnTablesUpdated);
                GHVHugoLib.Utilities.SetCommandTimeouts(0);

                SetUpTabMenu();

                Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);

                if (Properties.Settings.Default.UpgradeSettings)
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.UpgradeSettings = false;
                }

                LoadSettings();
                EnableControls();

                // not sure why this is necessary, but Visual Studio keeps setting Visible to false
                //  pretty much at random
                this.statusStripOpenTaxlots.Visible = true;
                this.statusStripOpenTaxlotsSummary.Visible = true;
                this.statusStripOpenEquityOptions.Visible = true;
                this.statusStripOpenEquityOptionsSummary.Visible = true;
                this.statusStripOpenIndexOptions.Visible = true;
                this.statusStripOpenIndexOptionsSummary.Visible = true;
                this.statusStripRealizedGainLoss.Visible = true;
                this.statusStripRealizedGainLossSummary.Visible = true;
                this.statusStripRealizedEquityOption.Visible = true;
                this.statusStripRealizedEquityOptionSummary.Visible = true;
                this.statusStripRealizedIndexOption.Visible = true;
                this.statusStripRealizedIndexOptionSummary.Visible = true;
                this.statusStripStockTrades.Visible = true;
                this.statusStripTaxlotHistory.Visible = true;
                this.statusStripTaxlotSummary.Visible = true;
                this.statusStripStockDailyReturns.Visible = true;
                this.statusStripStockMonthlyReturns.Visible = true;

                RefreshCurrentDataGrid();

                Application.Idle +=new EventHandler(Application_Idle);
            }
            catch (Exception ex)
            {
                OnError("Error loading form", ex);
                Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // see if we have any dataset changes that haven't been committed
            if (!CanChangeSelection(null))
            {
                e.Cancel = true;
            }

            // see if we have any setting changes
            if (m_settingsChanged && (e.Cancel == false))
            {
                DialogResult result = MessageBox.Show("Do you want to save your settings?", "Taxlot manager", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        Properties.Settings.Default.Save();
                        break;

                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }


            if (e.Cancel == false)
            {
                Properties.Settings.Default.PropertyChanged -= new PropertyChangedEventHandler(Default_PropertyChanged);
            }
        }
        private void tabsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tabPage = (sender as ToolStripItem).Tag as TabPage;
            tabControl1.SelectTab(tabPage);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form formSettings = new FormSettings();
                formSettings.ShowDialog();
            }
            catch (Exception ex)
            {
                OnError("Error editing settings", ex);
            }
        }
        #endregion

        #region Control events
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            Enabled = false;
            try
            {
                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
            catch (Exception ex)
            {
                OnError("Error refreshing screen", ex);
            }
            finally
            {
                Enabled = true;
                EnableControls();
            }
        }

        private void buttonCommitChanges_Click(object sender, EventArgs e)
        {
            Enabled = false;
            try
            {
                DataTable changesTable = ((DataTable)SelectedDataGridStateManager.BindingSource.DataSource).GetChanges();

                if (changesTable != null)
                {
                    if (ShouldProceedWithUpdates(changesTable.Rows.Count))
                    {
                        ProceedWithUpdates(changesTable);
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("Error committing changes", ex);
                buttonCancelChanges_Click(sender, e);
            }
            finally
            {
                Enabled = true;
                EnableControls();
            }

        }


        private void buttonCancelChanges_Click(object sender, EventArgs e)
        {
            Enabled = false;
            try
            {
                openTaxlotsDataGridStateManager.IsDirty = false;
                unrealizedEquityOptionDataGridStateManager.IsDirty = false;
                unrealizedIndexOptionDataGridStateManager.IsDirty = false;
                unrealizedFuturesDataGridStateManager.IsDirty = false;

                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
            catch (Exception ex)
            {
                OnError("Error canceling changes", ex);
            }
            finally
            {
                Enabled = true;
                EnableControls();
            }

        }

        private void comboBoxAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                AccountSelected = false;

                if (Properties.Settings.Default.AccountName != AccountName)
                {
                    Properties.Settings.Default.AccountName = AccountName;
                }
                GHVHugoLib.Utilities.Account = Properties.Settings.Default.AccountName;
 
                AccountSelected = (AccountName != null);
                RefreshCurrentDataGrid();
            }
            catch (Exception ex)
            {
                OnError("Error selecting account", ex);
            }
        }

        private void dateTimePickerCurrentDate_CloseUp(object sender, EventArgs e)
        {
            try
            {
                SaveDates(dateTimePickerCurrentDate.Value);
                RefreshCurrentDataGrid();
            }
            catch (Exception ex)
            {
                OnError("Error changing date", ex);
            }

        }

        private void showAllLabel_Click(object sender, EventArgs e)
        {
            DataGridStateManager stateManager = GetStateManagerFromToolStripItem(sender);
            stateManager.RemoveFilter();
        }
        private void radioButtonGroupBy_CheckedChanged(object sender, EventArgs e)
        {
            if (GHVHugoLib.Utilities.GroupBy != SubtotalGroup)
            {
                GHVHugoLib.Utilities.GroupBy = SubtotalGroup;
                SelectedTaxlotId = null;
                RefreshCurrentDataGrid();
            }
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshCurrentDataGrid();
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = !CanChangeSelection(null);
        }
        #endregion

        #region Menu item events
         private void sendOpeningtaxlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormSendOpenTaxlotsReport formSend = new FormSendOpenTaxlotsReport(s_logger);
                formSend.ShowDialog();
            }
            catch (Exception ex)
            {
                OnError("Error sending open taxlots report", ex);
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveAccessibleName = String.Empty;

            try
            {
                if (SelectedDataGridView != null)
                {
                    saveAccessibleName = SelectedDataGridView.AccessibleName;
                    SelectedDataGridView.AccessibleName += " - " + AccountName;

                    dataGridViewPrinter.SubTitle = SelectedDataGridViewSubtitle;
                    dataGridViewPrinter.Print(SelectedDataGridView);
                }
            }
            catch (Exception ex)
            {
                OnError("Error printing report", ex);
            }
            finally
            {
                if (SelectedDataGridView != null)
                {
                    SelectedDataGridView.AccessibleName = saveAccessibleName;
                }
            }

        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string saveAccessibleName = String.Empty;

            try
            {
                if (SelectedDataGridView != null)
                {
                    saveAccessibleName = SelectedDataGridView.AccessibleName;
                    SelectedDataGridView.AccessibleName += " - " + AccountName;

                    dataGridViewPrinter.SubTitle = SelectedDataGridViewSubtitle;
                    dataGridViewPrinter.PrintPreview(SelectedDataGridView);
                }
            }
            catch (Exception ex)
            {
                OnError("Error printing report", ex);
            }
            finally
            {
                if (SelectedDataGridView != null)
                {
                    SelectedDataGridView.AccessibleName = saveAccessibleName;
                }
            }
        }
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "Excel files (*.xls)|*.xls|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = String.Format("Taxlot Reports {0:yyyyMMdd}.xls", GHVHugoLib.Utilities.ClosingDate);

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    SaveReports(saveFileDialog1.FileName);
                }
            }
            catch (Exception ex)
            {
                OnError("Error exporting reports", ex);
            }
        }
        #endregion

        #region Datagridview events

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            DataGridStateManager stateManager = GetStateManagerFromDataGridView(sender);
            if (e.RowIndex >= 0)
            {
                DataGridView dataGridView = (DataGridView)sender;
                DataGridViewCell cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (Convert.IsDBNull(cell.Value) && (cell.ValueType.Name == "Double"))
                {
                    cell.Value = 0.0;
                }
                stateManager.IsDirty = true;
                EnableControls();
            }

        }

        private void dataGridViewUnrealizedPandL_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {

            if (e.ColumnIndex < 0)
            {
                try
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.ShowImageMargin = false;
                    SelectedDataGridStateManager.SaveRowsToProcess(e.RowIndex);

                    if (((DataGridStateManagerForUnrealizedPandL)SelectedDataGridStateManager).AllSelectedRowsHaveOverriddenPrices)
                    {
                        menu.Items.Add(deleteOverridePriceCommand);
                    }

                    if (menu.Items.Count > 0)
                    {
                        e.ContextMenuStrip = menu;
                    }
                }
                catch (Exception ex)
                {
                    OnError("Error displaying context menu in open option taxlots grid", ex);
                }
            }
            else CheckIfStrategyContextMenuStripNeeded(sender, e);
        }
       private void CheckIfStrategyContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (GHVHugoLib.Utilities.GroupBy == GHVHugoLib.SubtotalGroup.Taxlot)
            {
                if ("Strategy" == ((DataGridView)sender).Columns[e.ColumnIndex].HeaderText)
                {
                    try
                    {
                        ContextMenuStrip menu = new ContextMenuStrip();
                        DataGridStateManager stateManager = GetStateManagerFromDataGridView(sender);
                        menu.Tag = stateManager;
                        menu.ShowImageMargin = false;
                        stateManager.SaveCellsToProcess(e.ColumnIndex, e.RowIndex);

                        menu.Items.Add(stockStrategyCommand);
                        menu.Items.Add(shortCallStrategyCommand);

                        if (stateManager.InstrumentType == GHVHugoLib.InstrumentType.Option)
                        {
                            menu.Items.Add(longPutStrategyCommand);
                        }

                        e.ContextMenuStrip = menu;
                    }
                    catch (Exception ex)
                    {
                        OnError("Error displaying context menu for strategy", ex);
                    }

                }
            }
        }

         private void dataGridViewOpenTaxlots_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {

            if (e.ColumnIndex < 0)
            {
                try
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.ShowImageMargin = false;
                    openTaxlotsDataGridStateManager.SaveRowsToProcess(e.RowIndex);

                    if (SubtotalGroup == GHVHugoLib.SubtotalGroup.Taxlot)
                    {
                        if (openTaxlotsDataGridStateManager.AllSelectedRowsAreForSameUnderlying
                            && openTaxlotsDataGridStateManager.AllSelectedRowsDoNotHaveLinkedTaxlots)
                        {
                            menu.Items.Add(adjustCostBasisCommand);
                        }
                        if (openTaxlotsDataGridStateManager.AllSelectedRowsAreForSameUnderlying
                           && openTaxlotsDataGridStateManager.AllSelectedRowsHaveLinkedTaxlots)
                        {
                            menu.Items.Add(adjustSpinoffCommand);
                        }
                        if (openTaxlotsDataGridStateManager.AllSelectedRowsHaveOverriddenPrices)
                        {
                            menu.Items.Add(deleteOverridePriceCommand);
                        }
                    }

                    else
                    {
                        if (openTaxlotsDataGridStateManager.AllSelectedRowsHaveOverriddenPrices)
                        {
                            menu.Items.Add(deleteOverridePriceCommand);
                        }
                    }

                    if (menu.Items.Count > 0)
                    {
                        e.ContextMenuStrip = menu;
                    }
                }
                catch (Exception ex)
                {
                    OnError("Error displaying context menu in open taxlots grid", ex);
                }
            }

            else CheckIfStrategyContextMenuStripNeeded(sender,  e);
        }

        private void dataGridViewTaxlotHistory_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            try
            {
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.ShowImageMargin = false;

                if (e.ColumnIndex < 0)
                {
                    taxlotHistoryDataGridStateManager.SaveRowsToProcess(e.RowIndex);
                    if (taxlotHistoryDataGridStateManager.AllSelectedRowsHaveZeroTradeVolume)
                    {
                        menu.Items.Add(deleteTradeCommand);
                    }
                }

                if (menu.Items.Count > 0)
                {
                    e.ContextMenuStrip = menu;
                }
            }
            catch (Exception ex)
            {
                OnError("Error displaying context menu in taxlot history grid", ex);
            }
        }
        private void dataGridViewOpenTaxlots_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if ((e.ColumnIndex >= 0) && (e.RowIndex >= 0))
            {
            if (SelectedDataGridView.Columns[e.ColumnIndex].DataPropertyName == "PriceOverrideFlag")
            {
                GHVHugoLib.IUnrealizedGains row = ((DataRowView)SelectedDataGridView.Rows[e.RowIndex].DataBoundItem).Row as
                    GHVHugoLib.IUnrealizedGains;
                if (row.PriceOverrideFlag == "*")
                {
                    e.ToolTipText = String.Format("Price changed from {0:f3} on {1:d} by {2}",
                        row.OriginalPrice, row.PriceOverrideDate, row.PriceOverrideUser);
                }
            }
            }
        }

         private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                DataGridStateManager stateManager = GetStateManagerFromDataGridView(sender);
                if (stateManager != null)
                {
                    stateManager.RefreshStatusStrip();
                }
            }
            catch (Exception ex)
            {
                OnError("Error binding data grid", ex);
            }
        }

        private void dataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                DataGridView dataGridView = sender as DataGridView;
                object obj = dataGridView.Rows[e.RowIndex].DataBoundItem;
                if (obj != null)
                {
                    if (CanChangeSelection(null))
                    {
                        SelectedTaxlotId = ((DataRowView)obj).Row["TaxlotId"] as string;
                        EnableControls();
                    }
                    else
                    {
                        dataGridView.Rows[e.RowIndex].Selected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("Error loading taxlot history", ex);
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                ((sender as DataGridView).Tag as DataGridStateManager).ShowSelectedSum();
            }
            catch (Exception ex)
            {
                OnError("Error selecting rows", ex);
            }
        }
        #endregion

        #region System events
        private void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            m_settingsChanged = true;
            LoadSettings();
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (AccountSelected)
            {
                if (taxlotSummaryDataGridStateManager.NeedToRefresh)
                {
                    taxlotSummaryDataGridStateManager.Refresh();
                }

                else if (realizedGainLossDataGridStateManager.NeedToRefresh)
                {
                    realizedGainLossDataGridStateManager.Refresh();
                }
                else if (realizedFuturesDataGridStateManager.NeedToRefresh)
                {
                    realizedFuturesDataGridStateManager.Refresh();
                }
                else if (realizedEquityOptionDataGridStateManager.NeedToRefresh)
                {
                    realizedEquityOptionDataGridStateManager.Refresh();
                }
                else if (realizedIndexOptionDataGridStateManager.NeedToRefresh)
                {
                    realizedIndexOptionDataGridStateManager.Refresh();
                }

                else if (openTaxlotsDataGridStateManager.NeedToRefresh)
                {
                    openTaxlotsDataGridStateManager.Refresh();
                }
                 else if (unrealizedFuturesDataGridStateManager.NeedToRefresh)
                {
                    unrealizedFuturesDataGridStateManager.Refresh();
                }
               else if (unrealizedEquityOptionDataGridStateManager.NeedToRefresh)
                {
                    unrealizedEquityOptionDataGridStateManager.Refresh();
                }
                else if (unrealizedIndexOptionDataGridStateManager.NeedToRefresh)
                {
                    unrealizedIndexOptionDataGridStateManager.Refresh();
                }

                else if (stockTradesDataGridStateManager.NeedToRefresh)
                {
                    stockTradesDataGridStateManager.Refresh();
                }
                else if (stockDailyReturnsDataGridStateManager.NeedToRefresh)
                {
                    stockDailyReturnsDataGridStateManager.Refresh();
                }
                else if (stockMonthlyReturnsDataGridStateManager.NeedToRefresh)
                {
                    stockMonthlyReturnsDataGridStateManager.Refresh();
                }
            }
            return;
        }
        #endregion

        #region Context menu commands
        private void OnAdjustSpinoff(object sender, EventArgs e)
        {
            try
            {
                // get taxlot of spinoff
                GHVHugoLib.ITaxLot taxlot = openTaxlotsDataGridStateManager.TaxlotsToProcess.First();

                // get date of spinoff from first trade in trade history
                SelectedTaxlotId = taxlot.TaxlotId;
                GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow taxlotHistory =
                    GHVHugoLib.Utilities.TaxlotHistoryTable.First();

                // use this data to get summary information about the spinoff
                GHVHugoLib.HugoDataSet.p_get_spinoff_infoRow spinoffInfo = GHVHugoLib.Utilities.GetSpinoffInfo(taxlot.Ticker, taxlotHistory.TradeDateTime);

                if (spinoffInfo == null)
                {
                    MessageBox.Show("Unable to get spinoff info for " + taxlot.Ticker, "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {

                    FormAdjustSpinoff form = new FormAdjustSpinoff(spinoffInfo.OriginalSymbol,
                        spinoffInfo.OriginalVolume,
                        spinoffInfo.OriginalCostBasis - spinoffInfo.SpinoffCostBasis,
                        taxlot.Ticker,
                        spinoffInfo.SpinoffVolume,
                    spinoffInfo.SpinoffCostBasis);
                    if (DialogResult.OK == form.ShowDialog())
                    {
                        if (0 == GHVHugoLib.Utilities.AdjustSpinoff(taxlot.Ticker,
                            form.NewCostOfSpunoffStock,
                            form.NewPostSpinoffCostOfOriginalStock,
                            form.NewSharesSpunOff,
                            taxlotHistory.TradeDateTime))
                        {
                            MessageBox.Show("Spinoff has been adjusted", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshCurrentDataGrid();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("Error trying to adjust spinoff", ex);
            }
        }
       
        private void OnAdjustCostBasis(object sender, EventArgs e)
        {
            try
            {
                double combinedCurrentCostBasis = GetCombinedCurrentCostBasis(openTaxlotsDataGridStateManager);
                double combinedNumberOfShares = GetCombinedNumberOfShares(openTaxlotsDataGridStateManager);

                FormAdjustCostBasis form = new FormAdjustCostBasis(combinedCurrentCostBasis);
                if (DialogResult.OK == form.ShowDialog())
                {
                    // this will always return a number greater than zero
                    foreach (GHVHugoLib.HugoDataSet.p_get_open_taxlotsRow row in openTaxlotsDataGridStateManager.TaxlotsToProcess)
                    {
                        switch (form.AdjustmentCriterion)
                        {
                            case AdjustmentCriterion.CurrentCostBasis:
                                GHVHugoLib.Utilities.AdjustCostBasis(row.TaxLotId,
                                    form.NewCostBasis * row.TotalCost / combinedCurrentCostBasis,
                                    form.AdjustmentDate,
                                    form.TradeNote);
                                break;
                            case AdjustmentCriterion.NumberOfShares:
                                GHVHugoLib.Utilities.AdjustCostBasis(row.TaxLotId,
                                    row.TotalCost + (form.NewCostBasis - combinedCurrentCostBasis) * row.Open_Amount / combinedNumberOfShares,
                                    form.AdjustmentDate,
                                    form.TradeNote);
                                break;

                        }
                    }

                    MessageBox.Show("Cost basis has been adjusted", "Adjust cost basis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                OnError("Error trying to adjust cost basis", ex);
            }
            finally
            {
                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
        }
        private void OnDeleteOverridePrice(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Delete selected price overrides?", "Delete price overrides", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    int numDeletes = 0;

                    foreach (GHVHugoLib.IUnrealizedGains row in ((DataGridStateManagerForUnrealizedPandL)SelectedDataGridStateManager).UnrealizedGainsToProcess)
                    {
                        if (0 == GHVHugoLib.Utilities.DeleteTaxlotPrice(row.InstrumentTypeId, row.InstrumentId))
                            numDeletes++;
                    }
                    MessageBox.Show(String.Format("{0} price(s) deleted", numDeletes), "Delete price overrides", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                OnError("Error trying to delete prices", ex);
            }
            finally
            {
                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
        }
        private void OnDeleteTrade(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.Yes == MessageBox.Show("Delete selected trades?", "Delete trades", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    int numDeletes = 0;

                    foreach (GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow row in taxlotHistoryDataGridStateManager.HistoryToProcess)
                    {
                        if (0 == GHVHugoLib.Utilities.DeleteStockTrade(row.TradeId))
                            numDeletes++;
                    }
                    MessageBox.Show(String.Format("{0} trade(s) deleted", numDeletes), "Delete trades", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                OnError("Error trying to adjust cost basis", ex);
            }
            finally
            {
                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
        }
        private void OnStockStrategy(object sender, EventArgs e)
        {
            ChangeStrategy(sender, "Stock Strategy");
        }
        private void OnShortCallStrategy(object sender, EventArgs e)
        {
            ChangeStrategy(sender, "Short Call Strategy");
        }
        private void OnLongPutStrategy(object sender, EventArgs e)
        {
            ChangeStrategy(sender, "Long Put Strategy");
        }
        #endregion
        #endregion

        #region Private properties
        private GHVHugoLib.SubtotalGroup SubtotalGroup
        {
            get
            {
                if (radioButtonGroupByInstrument.Checked == true)
                {
                    return GHVHugoLib.SubtotalGroup.Instrument;
                }
                return GHVHugoLib.SubtotalGroup.Taxlot;
            }
        }
        private string SelectedDataGridViewSubtitle
        {
            get
            {
                string subTitle = SelectedDataGridStateManager.SubTitle;
                return String.Format("{0:d}{1}{2}",
                                    GHVHugoLib.Utilities.ClosingDate,
                                    String.IsNullOrEmpty(subTitle) ? "" : " - ",
                                    subTitle);
           }
        }
        private DataGridView SelectedDataGridView
        {
            get
            {
                if (tabControl1.SelectedTab != null)
                {
                    return tabControl1.SelectedTab.Tag as DataGridView;
                }
                else
                {
                    return null;
                }
            }
        }

        private DataGridStateManager SelectedDataGridStateManager
        {
            get
            {
                if (SelectedDataGridView != null)
                {
                    return SelectedDataGridView.Tag as DataGridStateManager;
                }
                else
                {
                    return null;
                }
            }
        }
        private bool AccountSelected { get; set; }
        private string AccountName
        {
            get { return (comboBoxAccount.SelectedIndex >= 0) ? (string)comboBoxAccount.SelectedItem : null; }
        }
        public static string SelectedTaxlotId
        {
            get { return s_selectedTaxlotId; }
            set
            {
                if (s_selectedTaxlotId != value)
                {
                    s_selectedTaxlotId = value;

                    // if grouping by taxlotid, set the taxlotid
                    if (GHVHugoLib.Utilities.GroupBy == GHVHugoLib.SubtotalGroup.Taxlot)
                    {
                        GHVHugoLib.Utilities.TaxlotId = value;
                        GHVHugoLib.Utilities.UnderlyingSymbol = null;
                    }
                    // if grouping by instrument, set the underlyingsymbol
                    else if (GHVHugoLib.Utilities.GroupBy == GHVHugoLib.SubtotalGroup.Instrument)
                    {
                        GHVHugoLib.Utilities.UnderlyingSymbol = value;
                        GHVHugoLib.Utilities.TaxlotId = null;
                    }

                    if (s_selectedTaxlotIdChanged != null)
                    {
                        s_selectedTaxlotIdChanged(null, new EventArgs());
                    }
                }
            }
        }
        #endregion

        #region Private methods

        #region Initialization
        private void FillAccountsComboBox()
        {
            try
            {
                Utilities.InitializeComboBox(comboBoxAccount, GHVHugoLib.Utilities.AccountNames.ToArray(), Properties.Settings.Default.AccountName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error loading accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeDataGridViews()
        {
            DataGridStateManager.Form = this;
            DataGridStateManager.Log = s_logger;

            openTaxlotsDataGridStateManager = new DataGridStateManagerForOpenTaxlots("Unrealized stocks", dataGridViewOpenTaxlots,
                GetOpenTaxlots, GHVHugoLib.Utilities.RefreshOpenTaxlotsTable, filterStatusLabelOpenTaxlots, showAllLabelOpenTaxlots, toolStripStatusLabelSummary, tradeSumLabelOpenTaxlots);
            unrealizedFuturesDataGridStateManager = new DataGridStateManagerForUnrealizedPandL("Unrealized futures", GHVHugoLib.InstrumentType.Future, dataGridViewUnrealizedFutures,
                GetUnrealizedFutures, GHVHugoLib.Utilities.RefreshUnrealizedFuturesGainsTable, filterStatusLabelOpenFutures, showAllLabelOpenFutures, toolStripStatusLabelOpenFuturesSummary, tradeSumLabelOpenFutures);
            unrealizedEquityOptionDataGridStateManager = new DataGridStateManagerForUnrealizedPandL("Unrealized equity options", GHVHugoLib.InstrumentType.Option, dataGridViewUnrealizedEquityOption,
               GetUnrealizedEquityOption, GHVHugoLib.Utilities.RefreshUnrealizedEquityOptionGainsTable, filterStatusLabelOpenEquityOptions, showAllLabelOpenEquityOptions, toolStripStatusLabelOpenEquityOptionSummary, tradeSumLabelOpenEquityOptions);
            unrealizedIndexOptionDataGridStateManager = new DataGridStateManagerForUnrealizedPandL("Unrealized index options", GHVHugoLib.InstrumentType.Option, dataGridViewUnrealizedIndexOption,
                GetUnrealizedIndexOption, GHVHugoLib.Utilities.RefreshUnrealizedIndexOptionGainsTable, filterStatusLabelOpenIndexOptions, showAllLabelOpenIndexOptions, toolStripStatusLabelOpenIndexOptionSummary, tradeSumLabelOpenIndexOptions);

            realizedGainLossDataGridStateManager = new DataGridStateManagerForRealizedPandL("Realized stocks", GHVHugoLib.InstrumentType.Stock, dataGridViewRealizedPandL,
               GetRealizedPandL, GHVHugoLib.Utilities.RefreshRealizedStockGainsTable, filterStatusLabelRealizedGainLoss, showAllLabelRealizedGainLoss, toolStripStatusLabelRealizedSummary, tradeSumLabelRealizedGainLoss);
            realizedFuturesDataGridStateManager = new DataGridStateManagerForRealizedPandL("Realized futures", GHVHugoLib.InstrumentType.Future, dataGridViewRealizedFutures,
               GetRealizedFutures, GHVHugoLib.Utilities.RefreshRealizedFuturesGainsTable, filterStatusLabelRealizedFutures, showAllLabelRealizedFutures, toolStripStatusLabelRealizedFuturesSummary, tradeSumLabelRealizedFutures);
            realizedEquityOptionDataGridStateManager = new DataGridStateManagerForRealizedPandL("Realized equity options", GHVHugoLib.InstrumentType.Option, dataGridViewRealizedEquityOption,
                GetRealizedEquityOption, GHVHugoLib.Utilities.RefreshRealizedEquityOptionGainsTable, filterStatusLabelRealizedEquityOption, showAllLabelRealizedEquityOption, toolStripStatusLabelRealizedEquityOptionSummary, tradeSumLabelRealizedEquityOption);
            realizedIndexOptionDataGridStateManager = new DataGridStateManagerForRealizedPandL("Realized index options", GHVHugoLib.InstrumentType.Option, dataGridViewRealizedIndexOption,
                GetRealizedIndexOption, GHVHugoLib.Utilities.RefreshRealizedIndexOptionGainsTable, filterStatusLabelRealizedIndexOption, showAllLabelRealizedIndexOption, toolStripStatusLabelRealizedIndexOptionSummary, tradeSumLabelRealizedIndexOption);
          
            taxlotHistoryDataGridStateManager = new DataGridStateManagerForTaxlotHistory(dataGridViewTaxlotHistory,
                GetTaxlotHistory, GHVHugoLib.Utilities.RefreshTaxlotHistoryTable, filterStatusLabelTaxlotHistory, showAllLabelTaxlotHistory);
            stockTradesDataGridStateManager = new DataGridStateManager("Stock trades", dataGridViewStockTrades,
                GetStockTrades, GHVHugoLib.Utilities.RefreshStockTradesTable, filterStatusLabelStockTrades, showAllLabelStockTrades);
            taxlotSummaryDataGridStateManager = new DataGridStateManagerForTaxlotSummary(dataGridViewTaxlotSummary,
                GetTaxlotSummary, GHVHugoLib.Utilities.RefreshTaxlotSummaryReportTable, filterStatusLabelTaxlotSummary, showAllLabelTaxlotSummary, toolStripStatusLabelTaxlotSummarySummary, selectedSumLabelTaxlotSummary);
            stockDailyReturnsDataGridStateManager = new DataGridStateManager("Stock daily returns", dataGridViewStockDailyReturns,
                 GetStockDailyReturns, GHVHugoLib.Utilities.RefreshStockDailyReturnsTable, filterStatusLabelStockDailyReturns, showAllLabelStockDailyReturns);
            stockMonthlyReturnsDataGridStateManager = new DataGridStateManager("Stock monthly returns", dataGridViewStockMonthlyReturns,
                 GetStockMonthlyReturns, GHVHugoLib.Utilities.RefreshStockMonthlyReturnsTable, filterStatusLabelStockMonthlyReturns, showAllLabelStockMonthlyReturns);

            tabOpenTaxlots.Tag = dataGridViewOpenTaxlots;
            tabOpenFuturesTaxlots.Tag = dataGridViewUnrealizedFutures;
            tabOpenEquityOptions.Tag = dataGridViewUnrealizedEquityOption;
            tabOpenIndexOptions.Tag = dataGridViewUnrealizedIndexOption;

            tabRealizedPandL.Tag = dataGridViewRealizedPandL;
            tabRealizedFuturesPandL.Tag = dataGridViewRealizedFutures;
            tabRealizedEquityOptions.Tag = dataGridViewRealizedEquityOption;
            tabRealizedIndexOptions.Tag = dataGridViewRealizedIndexOption;

            tabTaxlotHistory.Tag = dataGridViewTaxlotHistory;
            tabTrades.Tag = dataGridViewStockTrades;
            tabTaxlotSummary.Tag = dataGridViewTaxlotSummary;
            tabStockDailyReturns.Tag = dataGridViewStockDailyReturns;
            tabStockMonthlyReturns.Tag = dataGridViewStockMonthlyReturns;
        }

        private void BuildContextMenuCommands()
        {
            adjustCostBasisCommand = new ToolStripMenuItem("&Adjust cost basis", null, new System.EventHandler(OnAdjustCostBasis));
            adjustSpinoffCommand = new ToolStripMenuItem("&Adjust spinoff", null, new System.EventHandler(OnAdjustSpinoff));
            deleteTradeCommand = new ToolStripMenuItem("&Delete trade", null, new System.EventHandler(OnDeleteTrade));
            deleteOverridePriceCommand = new ToolStripMenuItem("Delete &override price", null, new System.EventHandler(OnDeleteOverridePrice));
            stockStrategyCommand = new ToolStripMenuItem("&Stock strategy", null, new System.EventHandler(OnStockStrategy));
            shortCallStrategyCommand = new ToolStripMenuItem("Short &call strategy", null, new System.EventHandler(OnShortCallStrategy));
            longPutStrategyCommand = new ToolStripMenuItem("Long &put strategy", null, new System.EventHandler(OnLongPutStrategy));
        }

        private void LoadSettings()
        {
            // load settings
             if ((m_applicationName != Properties.Settings.Default.ApplicationName)
             || (m_databaseName != Properties.Settings.Default.Database))
            {
                GetHugoConnection();
            }
        }

        private void GetHugoConnection()
        {
            // get Hugo connection
            m_applicationName = Properties.Settings.Default.ApplicationName;
            m_databaseName = Properties.Settings.Default.Database;
            m_dbAccess = Gargoyle.Utils.DBAccess.DBAccess.GetDBAccessOfTheCurrentUser(m_applicationName);
            GHVHugoLib.Utilities.DBAccess = m_dbAccess;

            ReconciliationLib.Utilities.Init();
            ReconciliationLib.Utilities.Connection = m_dbAccess.GetConnection(m_databaseName);
            SaveDates(dateTimePickerCurrentDate.Value);

            // update title bar
            Text = String.Format("{0} {1} - {2}",
                 System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                ReconciliationLib.Utilities.Connection.DataSource.ToUpper());
        }

        #endregion

        private void ChangeStrategy(object sender, string strategy)
        {
            int changesMade = 0;
            try
            {
                DataGridStateManager stateManager = GetStateManagerFromToolStripItem(sender);
                DataGridViewCell[] cellsToProcess = stateManager.CellsToProcess;
                foreach (DataGridViewCell cell in cellsToProcess)
                {
                    if (stateManager.InstrumentType == GHVHugoLib.InstrumentType.Stock)
                    {
                        string taxlotId = stateManager.GetTaxlotIdForRow(cell.RowIndex);
                        if (!String.IsNullOrEmpty(taxlotId))
                        {
                            if (0 == GHVHugoLib.Utilities.ChangeStockTaxlotStrategyByTaxlotId(taxlotId, strategy))
                            {
                                changesMade++;
                            }
                        }
                    }
                    else
                    {
                        int tradeId = stateManager.GetTradeIdForRow(cell.RowIndex);
                        if (tradeId >= 0)
                        {
                            if (0 == GHVHugoLib.Utilities.ChangeStockTaxlotStrategyByTradeId(tradeId, strategy))
                            {
                                changesMade++;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnError("Error trying to change strategy to " + strategy, ex);
            }
            finally
            {
                if (changesMade > 0)
                {
                    MessageBox.Show(String.Format("Committed {0} strategy change(s)", changesMade), "Update",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
        }
        private void SaveDates(DateTime currentDate)
        {
            GHVHugoLib.Utilities.EndDate = currentDate;
            if (s_currentDateChanged != null)
            {
                s_currentDateChanged(currentDate, new EventArgs());
            }
        }

        private static double GetCombinedCurrentCostBasis(DataGridStateManagerForOpenTaxlots openTaxlotsDataGridStateManager)
        {
            double combinedCurrentCostBasis = 0f;
            foreach (GHVHugoLib.HugoDataSet.p_get_open_taxlotsRow row in openTaxlotsDataGridStateManager.TaxlotsToProcess)
            {
                combinedCurrentCostBasis += row.TotalCost;
            }
            if (combinedCurrentCostBasis <= 0)
            {
                throw new GHVHugoLib.GHVHugoException("Original cost basis must be greater than zero if selecting multiple taxlots");
            }
            return combinedCurrentCostBasis;
        }
        private static double GetCombinedNumberOfShares(DataGridStateManagerForOpenTaxlots openTaxlotsDataGridStateManager)
        {
            double combinedNumberOfShares = 0f;
            foreach (GHVHugoLib.HugoDataSet.p_get_open_taxlotsRow row in openTaxlotsDataGridStateManager.TaxlotsToProcess)
            {
                combinedNumberOfShares += row.Open_Amount;
            }
            return combinedNumberOfShares;
        }

        private static void OnError(string message, Exception e)
        {
            s_logger.Error(message, e);
            MessageBox.Show(String.Format("{0}: {1}", message, e.Message), "Taxlot Manager Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static DataGridStateManager GetStateManagerFromToolStripItem(object sender)
        {
            return (sender as ToolStripItem).Owner.Tag as DataGridStateManager;
        }

        private static DataGridStateManager GetStateManagerFromDataGridView(object sender)
        {
            return (sender as Control).Tag as DataGridStateManager;
        }

        private static void GetOpenTaxlots(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.OpenTaxlotsTable;

        }
        private static void GetRealizedPandL(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.RealizedStockGainsTable;

        }
        private static void GetRealizedFutures(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.RealizedFuturesGainsTable;

        }
        private static void GetRealizedEquityOption(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.RealizedEquityOptionGainsTable;

        }
        private static void GetRealizedIndexOption(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.RealizedIndexOptionGainsTable;

        }
        private static void GetUnrealizedEquityOption(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.UnrealizedEquityOptionGainsTable;

        }
        private static void GetUnrealizedIndexOption(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.UnrealizedIndexOptionGainsTable;

        }
        private static void GetUnrealizedFutures(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.UnrealizedFuturesGainsTable;

        }
        private static void GetTaxlotSummary(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.TaxlotSummaryReportTable;

        }
        private static void GetStockTrades(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.StockTradesTable;

        }
        private static void GetTaxlotHistory(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.TaxlotHistoryTable;
        }
        private static void GetStockDailyReturns(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.StockDailyReturnsTable;
        }
        private static void GetStockMonthlyReturns(object sender, RefreshBindingSourceEventArgs e)
        {
            e.DataTable = GHVHugoLib.Utilities.StockMonthlyReturnsTable;
        }
        private void SetUpTabMenu()
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                ToolStripMenuItem tabItem = new ToolStripMenuItem();
                tabItem.Name = tabPage.Name;
                tabItem.Size = new System.Drawing.Size(202, 22);
                tabItem.Text = tabPage.Text;
                tabItem.Tag = tabPage;
                tabItem.Click += new System.EventHandler(tabsToolStripMenuItem_Click);
                tabsToolStripMenuItem.DropDownItems.Add(tabItem);
            }
        }
        public void EnableControls()
        {
            this.Enabled = true;
            this.buttonImportTrades.Enabled = AccountSelected;
            this.buttonCommitChanges.Enabled = openTaxlotsDataGridStateManager.IsDirty
                || unrealizedEquityOptionDataGridStateManager.IsDirty
                || unrealizedIndexOptionDataGridStateManager.IsDirty
               || unrealizedFuturesDataGridStateManager.IsDirty;
            this.buttonCancelChanges.Enabled = this.buttonCommitChanges.Enabled;
            this.comboBoxAccount.Enabled = !this.buttonCommitChanges.Enabled;
        }

        private void RefreshAllDataGrids()
        {
            Enabled = false;
            try
            {
                // cannot refresh if we don't have an account 
                if (AccountSelected)
                {
                    // these refreshes will do nothing unless the NeedToRefresh property has been set
                    openTaxlotsDataGridStateManager.Refresh();
                    unrealizedFuturesDataGridStateManager.Refresh();
                    unrealizedEquityOptionDataGridStateManager.Refresh();
                    unrealizedIndexOptionDataGridStateManager.Refresh();

                    realizedGainLossDataGridStateManager.Refresh();
                    realizedFuturesDataGridStateManager.Refresh();
                    realizedEquityOptionDataGridStateManager.Refresh();
                    realizedIndexOptionDataGridStateManager.Refresh();
                    
                    taxlotHistoryDataGridStateManager.Refresh();
                    stockTradesDataGridStateManager.Refresh();
                    taxlotSummaryDataGridStateManager.Refresh();
                    stockDailyReturnsDataGridStateManager.Refresh();
                    stockMonthlyReturnsDataGridStateManager.Refresh();
                 }
            }
            catch (Exception ex)
            {
                OnError("Error refreshing datagrids", ex);
            }
            finally
            {
                Enabled = true;
                EnableControls();
            }
        }

        private void RefreshCurrentDataGrid()
        {
            Enabled = false;
            try
            {
                // cannot refresh if we don't have an account 
                if (AccountSelected)
                {
                    if (SelectedDataGridStateManager == null)
                    {
                        MessageBox.Show("No DataGridManager associated with this data grid - contact programmer", "Taxlot Manager Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        SelectedDataGridStateManager.Refresh();
                    }

                }
            }
            catch (Exception ex)
            {
                OnError("Error refreshing current datagrid", ex);
            }
            finally
            {
                Enabled = true;
                EnableControls();
            }
        }

        // returns true if succeeded
         private void SaveReports(string fileName)
        {
            ExcelWorkbookGenerator workbookGenerator = new ExcelWorkbookGenerator();

            RefreshAllDataGrids();
            workbookGenerator.AddWorksheet(dataGridViewOpenTaxlots);
            workbookGenerator.AddWorksheet(dataGridViewUnrealizedFutures);
            workbookGenerator.AddWorksheet(dataGridViewUnrealizedEquityOption);
            workbookGenerator.AddWorksheet(dataGridViewUnrealizedIndexOption);

            workbookGenerator.AddWorksheet(dataGridViewRealizedPandL);
            workbookGenerator.AddWorksheet(dataGridViewRealizedFutures);
            workbookGenerator.AddWorksheet(dataGridViewRealizedEquityOption);
            workbookGenerator.AddWorksheet(dataGridViewRealizedIndexOption);

            workbookGenerator.AddWorksheet(dataGridViewTaxlotSummary);
            workbookGenerator.AddWorksheet(dataGridViewStockDailyReturns);
            workbookGenerator.AddWorksheet(dataGridViewStockMonthlyReturns);
            workbookGenerator.SaveWorkbook(fileName);

            System.Diagnostics.Process.Start(fileName);
        }

        private bool CanChangeSelection(DataGridStateManager stateManager /*if null, will check all grids*/)
        {
            if ((stateManager == null) ? buttonCommitChanges.Enabled : stateManager.IsDirty)
            {
                switch (MessageBox.Show("Commit changes?", "Changing selection", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        buttonCommitChanges_Click(this, null);
                        return true;

                    case DialogResult.No:
                        buttonCancelChanges_Click(this, null);
                        return true;

                    default:
                        return false;
                }
            }

            return true;
        }
        private bool ShouldProceedWithUpdates(int changedRows)
        {
            if (changedRows <= 0)
            {
                MessageBox.Show("No changes to commit", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SelectedDataGridStateManager.IsDirty = false;
                return false;
            }
            else
            {
                string msg = String.Format("Commit {0} change(s) to Hugo?", changedRows);
                return DialogResult.Yes == MessageBox.Show(msg, "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
        }

        private void ProceedWithUpdates(DataTable changesTable)
        {
            int changesMade = 0;
            try
            {
                foreach (GHVHugoLib.IUnrealizedGains row in changesTable.Rows)
                {
                    if (0 == GHVHugoLib.Utilities.ChangeTaxlotPrice(row.InstrumentTypeId, row.InstrumentId, row.CurrentPrice))
                    {
                        changesMade++;
                    }
                }
            }
            finally
            {
                if (changesMade > 0)
                {
                    MessageBox.Show(String.Format("Committed {0} change(s)", changesMade), "Update",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                GHVHugoLib.Utilities.Refresh();
                RefreshCurrentDataGrid();
            }
        }


          #endregion

  
 
 
     }

 }
