using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace TaxlotManager
{
    internal class DataGridStateManagerForRealizedPandL : DataGridStateManager
    {
        private ToolStripStatusLabel totalSumLabel;
        private ToolStripStatusLabel tradeSumLabel;
        private double selectedCostSum;
        private double selectedProceedsSum;
        private double selectedShortTermSum;
        private double selectedLongTermSum;
        private double visibleCostSum;
        private double visibleProceedsSum;
        private double visibleShortTermSum;
        private double visibleLongTermSum;
        private bool selectionMade;

        public DataGridStateManagerForRealizedPandL(string name,
            GHVHugoLib.InstrumentType instrumentType,
            DataGridView dataGridView,
             EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
             RefreshDataDelegate refreshDataDelegate,
            ToolStripStatusLabel filterStatusLabel,
             ToolStripStatusLabel showAllLabel,
             ToolStripStatusLabel totalSumLabel,
             ToolStripStatusLabel tradeSumLabel)
            : base(name, dataGridView, refreshBindingSource, refreshDataDelegate, filterStatusLabel, showAllLabel)
        {
            this.totalSumLabel = totalSumLabel;
            this.tradeSumLabel = tradeSumLabel;
            this.InstrumentType = instrumentType;
        }

        public void CalculateSelectedSum()
        {
            selectedCostSum = 0f;
            selectedProceedsSum = 0f;
            selectedShortTermSum = 0f;
            selectedLongTermSum = 0f;
            selectionMade = false;

            foreach (GHVHugoLib.IRealizedGains row in GetRealizedPandL(VisibleSelectedRows))
            {
                selectionMade = true;
                selectedCostSum += row.CostBasis;
                selectedProceedsSum += row.NetProceeds;
                selectedShortTermSum += row.ShortTermGainLoss;
                selectedLongTermSum += row.LongTermGainLoss;
            }
        }
        public void CalculateVisibleSum()
        {
            visibleCostSum = 0f;
            visibleProceedsSum = 0f;
            visibleShortTermSum = 0f;
            visibleLongTermSum = 0f;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Visible)
                {
                    GHVHugoLib.IRealizedGains dataRow = ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.IRealizedGains;

                    visibleCostSum += dataRow.CostBasis;
                    visibleProceedsSum += dataRow.NetProceeds;
                    visibleShortTermSum += dataRow.ShortTermGainLoss;
                    visibleLongTermSum += dataRow.LongTermGainLoss;
                }
            }
        }
        public string SelectedSumMessage
        {
            get
            {
                if (selectionMade)
                    return String.Format("Selected cost = {0:C2} | Proceeds = {1:C2} | Short-term = {2:C2} | Long-term  = {3:C2}",
                       selectedCostSum, selectedProceedsSum, selectedShortTermSum, selectedLongTermSum);
                else
                    return String.Empty;
            }
        }
        public string VisibleSumMessage
        {
            get
            {
                return String.Format("Cost = {0:C2} | Proceeds = {1:C2} | Short-term = {2:C2} | Long-term  = {3:C2}",
                   visibleCostSum, visibleProceedsSum, visibleShortTermSum, visibleLongTermSum);
            }
        }

        public override string SubTitle
        {
            get { return VisibleSumMessage.Replace("&&", "&"); }
        }

        public override void ShowSelectedSum()
        {
            CalculateSelectedSum();
            if ((dataGridView.Rows.Count > 0) && selectionMade)
            {
                tradeSumLabel.Visible = true;
                tradeSumLabel.Text = SelectedSumMessage;
            }
            else
            {
                tradeSumLabel.Visible = false;
            }
        }

        public override void ShowVisibleSum()
        {
            if (dataGridView.Rows.Count > 0)
            {
                CalculateVisibleSum();
                totalSumLabel.Visible = true;
                totalSumLabel.Text = VisibleSumMessage;
            }
            else
            {
                totalSumLabel.Visible = false;
            }
        }

        public IEnumerable<GHVHugoLib.IRealizedGains> RealizedPandLToProcess
        {
            get
            {
                return GetRealizedPandL(RowsToProcess);
            }
        }
        protected IEnumerable<GHVHugoLib.IRealizedGains> GetRealizedPandL(IEnumerable<DataGridViewRow> rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                yield return ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.IRealizedGains;
            }
        }
     }
}
