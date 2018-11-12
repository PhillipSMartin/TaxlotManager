using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Gargoyle.Utils.DataGridViewAutoFilter;

namespace TaxlotManager
{
    internal class DataGridStateManagerForTaxlotHistory : DataGridStateManager
    {
        public DataGridStateManagerForTaxlotHistory(DataGridView dataGridView,
           EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
            RefreshDataDelegate refreshDataDelegate,
           ToolStripStatusLabel filterStatusLabel,
           ToolStripStatusLabel showAllLabel)
            : base("Taxlot history",
            dataGridView,
            refreshBindingSource,
             refreshDataDelegate,
           filterStatusLabel,
            showAllLabel)
        {
            key = "TradeId";

            Form1.SelectedTaxlotIdChanged += new EventHandler(OnSelectedTaxlotIdChanged);
        }
        protected void OnSelectedTaxlotIdChanged(object sender, EventArgs e)
        {
            NeedToRefresh = true;
            RemoveFilter();
            Refresh();
        }
        public IEnumerable<GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow> HistoryToProcess
        {
            get
            {
                return GetHistory(RowsToProcess);
            }
        }
        protected IEnumerable<GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow> GetHistory(IEnumerable<DataGridViewRow> rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                yield return ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow;
            }
        }
        public bool AllSelectedRowsHaveZeroTradeVolume
        {
            get
            {
                foreach (GHVHugoLib.HugoDataSet.p_get_taxlots_with_running_positions_and_unit_cost_basisRow row in
                    GetHistory(VisibleSelectedRows))
                {
                    if (row.TradeVolume != 0)
                        return false;
                }
                return true;
            }
        }
    }
}
