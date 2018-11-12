using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Gargoyle.Utils.DataGridViewAutoFilter;
using System.Threading;
using log4net;

namespace TaxlotManager
{
    internal delegate void CallbackDelegate(int n);
    internal delegate void RefreshDataDelegate();

    internal class DataGridStateManager
    {
        #region Declarations
        private string name;
        private static int updateNumber = 0;
        public static Form1 Form { get; set; }
        public static ILog Log { get; set; }
        public GHVHugoLib.InstrumentType InstrumentType { protected set;  get; }
        private bool updateInProgress = false;

        protected DataGridView dataGridView;
        protected BindingSource bindingSource;
        // private because it should always be called via Rebind method
        private EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource;
        private RefreshDataDelegate refreshDataDelegate;
        protected string key;

        private bool needToRefresh;     // the datasource has changed and the grid needs to be refreshed
        private bool isDirty;           // changes were made to the grid that have not yet been committed to the datasource

        private DataGridViewRow[] rowsToProcess;
        private DataGridViewCell[] cellsToProcess;

        protected ToolStripStatusLabel filterStatusLabel;
        protected ToolStripStatusLabel showAllLabel;
        
        #endregion

        #region Constructor
        public DataGridStateManager(string name,
            DataGridView dataGridView,
            EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
            RefreshDataDelegate refreshDataDelegate,
            ToolStripStatusLabel filterStatusLabel,
            ToolStripStatusLabel showAllLabel)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (dataGridView == null)
            {
                throw new ArgumentNullException("dataGridView");
            }
            if (refreshBindingSource == null)
            {
                throw new ArgumentNullException("refreshBindingSource");
            }
            if (filterStatusLabel == null)
            {
                throw new ArgumentNullException("filterStatusLabel");
            }

            this.name = name;
            this.dataGridView = dataGridView;
            this.bindingSource = dataGridView.DataSource as BindingSource;
            this.refreshBindingSource = refreshBindingSource;
            this.refreshDataDelegate = refreshDataDelegate;
            this.filterStatusLabel = filterStatusLabel;
            this.showAllLabel = showAllLabel;

            // allow access to this object from dataGridView
            dataGridView.Tag = this;

            // allow access to this object from toolstrip
            filterStatusLabel.Owner.Tag = this;
       }
        #endregion

        #region Public properties
        public BindingSource BindingSource
        {
            get { return bindingSource; }
        }
        public virtual string SubTitle
        {
            get { return null; }
        }
        public bool NeedToRefresh
        {
            get { return needToRefresh; }
            set 
            { 
                needToRefresh = value;
                if (needToRefresh == true)
                {
                    bindingSource.DataSource = null;
                    ShowLabelsWhenNotLoaded();
                }
            }
        }
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        // when user selects an item from the context menu of a row header, this array is filled with either
        //  the rows from dataGridView.SelectedRows or, if this is empty, the single
        //  row that the user right-clicked on to bring up the context menu
        public DataGridViewRow[] RowsToProcess
        {
            get { return rowsToProcess; }
        }
        // when user selects an item from the context menu of a cell, this array is filled with either
        //  the cells from dataGridView.SelectedCells within the clicked-on column or, if this is empty, the single
        //  cell that the user right-clicked on to bring up the context menu
        public DataGridViewCell[] CellsToProcess
        {
            get { return cellsToProcess; }
        }

        public virtual bool GroupByPrice
        {
            get { return false; }
        }
        #endregion

        #region Public methods
        public virtual void Refresh()
        {
            if (NeedToRefresh)
            {
                NeedToRefresh = false;

                // refresh table in background thread
                if (refreshDataDelegate != null)
                {
                    // don't refresh if previous refresh isn't finished yet
                    if (!updateInProgress)
                    {
                        updateInProgress = true;
                        Thread UpdateThread = new Thread(QueryDataBase);
                        UpdateThread.IsBackground = true;
                        UpdateThread.Start();
                    }
                }

                // refresh table inline
                else
                {
                    RefreshCallBack(0);
                }
            }
        }
 
        public virtual void RefreshStatusStrip()
        {
            ShowFilterStatus();
            ShowSelectedSum();
            ShowVisibleSum();
        }

        public virtual void SaveRowsToProcess(int rowIndex)
        {
            rowsToProcess = null;

            // make sure we have at least one selection
            dataGridView.Rows[rowIndex].Selected = true;

            List<DataGridViewRow> rowList = new List<DataGridViewRow>(VisibleSelectedRows);
            rowsToProcess = rowList.ToArray();
        }

        public virtual void ShowSelectedSum()
        {
        }
        public virtual void ShowVisibleSum()
        {
        }

        protected IEnumerable<DataGridViewRow> VisibleSelectedRows
        {
            get
            {
                foreach (DataGridViewRow row in dataGridView.SelectedRows)
                {
                    if (row.Visible)
                    {
                        yield return row;
                    }
                }
            }
        }
        public void SaveCellsToProcess(int columnIndex, int rowIndex)
        {
            cellsToProcess = null;

            // make sure we have at least one selection
            dataGridView[columnIndex, rowIndex].Selected = true;

            // save list of all selected cells
            List<DataGridViewCell> cells = new List<DataGridViewCell>();
            foreach (DataGridViewCell cell in dataGridView.SelectedCells)
            {
                if (cell.ColumnIndex == columnIndex)
                    cells.Add(cell);
            }
            cellsToProcess = cells.ToArray();
        }
        public virtual void RemoveFilter()
        {
            // Confirm that the data source is a BindingSource that supports filtering.
            if ((bindingSource.DataSource != null) && bindingSource.SupportsFiltering)
            {
                DataGridViewAutoFilterTextBoxColumn.RemoveFilter(dataGridView);
            }
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.Visible = true;
            }
            RefreshStatusStrip();
        }
        public void ReverseFilter()
        {
            // Confirm that the data source is a BindingSource that supports filtering.
            if ((bindingSource.DataSource != null) && bindingSource.SupportsFiltering)
            {
                if (!String.IsNullOrEmpty(bindingSource.Filter))
                {
                    StringBuilder newFilter = new StringBuilder();
                    for (int n = 0; n < bindingSource.Filter.Length; n++)
                    {
                        if (bindingSource.Filter[n] == '=')
                        {
                            newFilter.Append("<>");
                        }
                        else if ((bindingSource.Filter[n] == '<') && (bindingSource.Filter[n + 1] == '>'))
                        {
                            newFilter.Append("=");
                            n++;
                        }
                        else
                        {
                            newFilter.Append(bindingSource.Filter[n]);
                        }
                    }
                    bindingSource.Filter = newFilter.ToString();

                    RefreshStatusStrip();
                }
            }
        }

        public void EndEdit()
        {
            bindingSource.EndEdit();
        }

        public string GetTaxlotIdForRow(int row)
        {
            DataRowView dataRowView = dataGridView.Rows[row].DataBoundItem as DataRowView;
            if (dataRowView != null)
            {
                GHVHugoLib.ITaxlotId taxlot = dataRowView.Row as GHVHugoLib.ITaxlotId;
                if (taxlot != null)
                {
                    return taxlot.TaxlotId;
                }
            }
            return String.Empty;
        }
  
        public int GetTradeIdForRow(int row)
        {
            DataRowView dataRowView = dataGridView.Rows[row].DataBoundItem as DataRowView;
            if (dataRowView != null)
            {
                GHVHugoLib.ITradeId trade = dataRowView.Row as GHVHugoLib.ITradeId;
                if (trade != null)
                {
                    return trade.TradeId;
                }
            }
            return -1;
        }
        #endregion

        #region Private properties
        protected bool AllRowsAreVisible
        {
            get
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.Visible)
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region Private methods
        void QueryDataBase()
        {
            int currentUpdateNumber = Interlocked.Increment(ref updateNumber);
            Log.Info(String.Format("{0} reloading table, update number = {1}", name, currentUpdateNumber));
            refreshDataDelegate();
            Log.Info(String.Format("{0} finished reloading table, update number = {1}", name, currentUpdateNumber));
            Form.BeginInvoke(new CallbackDelegate(RefreshCallBack), currentUpdateNumber);
        }
        void RefreshCallBack(int updateNumber)
        {
           // save hidden keys
            List<object> hiddenKeys = GetHiddenKeys();

            // re-bind
            Log.Info(String.Format("{0} rebinding datasource, update number = {1}", name, updateNumber));
            bindingSource.DataSource = Rebind();
            updateInProgress = false;    // set this after Rebind so we don't compete with a new update thread

            NeedToRefresh = false;
            IsDirty = false;
            Form.EnableControls();
  
            // re-hide keys
            HideKeys(hiddenKeys);
            RefreshStatusStrip();
        }
        protected virtual DataTable Rebind()
        {
            return Rebind(new RefreshBindingSourceEventArgs());
        }

        protected DataTable Rebind(RefreshBindingSourceEventArgs e)
        {
            refreshBindingSource(this, e);
            if (dataGridView.Controls.Count > 0)
                dataGridView.Controls[0].Enabled = true;
            if (dataGridView.Controls.Count > 1)
                dataGridView.Controls[1].Enabled = true;
            if (dataGridView.AutoSizeColumnsMode != DataGridViewAutoSizeColumnsMode.None)
            {
                dataGridView.AutoResizeColumns();
            }

            return e.DataTable;
        }
        protected List<object> GetHiddenKeys()
        {
            List<object> hiddenKeys = new List<object>();
            if (key != null)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.Visible)
                        hiddenKeys.Add(((DataRowView)row.DataBoundItem)[key]);
                }
            }
            return hiddenKeys;
        }
        protected void HideKeys(List<object> hiddenKeys)
        {
            if (key != null)
            {
                dataGridView.CurrentCell = null;
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (hiddenKeys.Contains(((DataRowView)row.DataBoundItem)[key]))
                        row.Visible = false;
                }
            }
        }
        protected void ShowFilterStatus()
        {
            if (needToRefresh)
            {
                ShowLabelsWhenNotLoaded();
            }
            else
            {
                string filterStatus = DataGridViewAutoFilterColumnHeaderCell.GetFilterStatus(dataGridView);

                // set labels when not filtered
                if (string.IsNullOrEmpty(filterStatus) && AllRowsAreVisible)
                {
                    ShowLabelsWhenNotFiltered();
                }

                // set labels when filtered
                else
                {
                    ShowLabelsWhenFiltered();
                    filterStatusLabel.Text = filterStatus;
                }
            }
        }

        protected virtual void ShowLabelsWhenFiltered()
        {
            showAllLabel.Visible = true;
            filterStatusLabel.Visible = true;
        }

        protected virtual void ShowLabelsWhenNotFiltered()
        {
            showAllLabel.Visible = false;
            filterStatusLabel.Visible = false;
        }

        protected void ShowLabelsWhenNotLoaded()
        {
            showAllLabel.Visible = false;

            filterStatusLabel.Text = "Loading data...";
            filterStatusLabel.Visible = true;
        }

        #endregion
    }
}
