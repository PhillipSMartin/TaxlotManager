using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace TaxlotManager
{
    internal class DataGridStateManagerForUnrealizedPandL : DataGridStateManager
    {
        protected ToolStripStatusLabel totalSumLabel;
        protected ToolStripStatusLabel tradeSumLabel;
        protected double selectedPositionSum;
        protected double selectedCostSum;
        protected double selectedMarketValueSum;
        protected double selectedGainLossSum;
        protected double visiblePositionSum;
        protected double visibleCostSum;
        protected double visibleMarketValueSum;
        protected double visibleGainLossSum;
        protected bool selectionMade;
        private bool allSelectedRowsHaveOverriddenPrices;

        public DataGridStateManagerForUnrealizedPandL(string name,
            GHVHugoLib.InstrumentType instrumentType,
            DataGridView dataGridView,
             EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
              RefreshDataDelegate refreshDataDelegate,
           ToolStripStatusLabel filterStatusLabel,
             ToolStripStatusLabel showAllLabel,
             ToolStripStatusLabel totalSumLabel,
             ToolStripStatusLabel tradeSumLabel)
            : base(name, dataGridView, refreshBindingSource,  refreshDataDelegate, filterStatusLabel, showAllLabel)
        {
            this.totalSumLabel = totalSumLabel;
            this.tradeSumLabel = tradeSumLabel;
            this.InstrumentType = instrumentType;
        }

        protected void CalculateSelectedSum()
        {
            selectedPositionSum = 0f;
            selectedCostSum = 0f;
            selectedMarketValueSum = 0f;
            selectedGainLossSum = 0f;
            selectionMade = false;

            foreach (DataGridViewRow row in VisibleSelectedRows)
            {
                GHVHugoLib.IUnrealizedGains dataRow = ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.IUnrealizedGains;

                selectionMade = true;
                selectedPositionSum += dataRow.Open_Amount;
                selectedCostSum += dataRow.TotalCost;
                selectedMarketValueSum += dataRow.MarketValue;
                selectedGainLossSum += dataRow.GainOrLoss;

            }
        }
        protected void CalculateVisibleSum()
        {
            visiblePositionSum = 0f;
            visibleCostSum = 0f;
            visibleMarketValueSum = 0f;
            visibleGainLossSum = 0f;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Visible)
                {
                    GHVHugoLib.IUnrealizedGains dataRow = ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.IUnrealizedGains;

                    visiblePositionSum += dataRow.Open_Amount;
                    visibleCostSum += dataRow.TotalCost;
                    visibleMarketValueSum += dataRow.MarketValue;
                    visibleGainLossSum += dataRow.GainOrLoss;
                }
            }
        }
        protected string SelectedSumMessage
        {
            get
            {
                if (selectionMade)
                    return String.Format("Selected position= {0} | Basis= {1:C2} | Market value= {2:C2} | P&&L= {3:C2}",
                        Math.Round(selectedPositionSum), selectedCostSum, selectedMarketValueSum, selectedGainLossSum);
                else
                    return String.Empty;
            }
        }
        protected string VisibleSumMessage
        {
            get
            {
                return String.Format("Position= {0} | Basis= {1:C2} | Market value= {2:C2} | P&&L= {3:C2}",
                visiblePositionSum, visibleCostSum, visibleMarketValueSum, visibleGainLossSum);
            }
        }
        public override void ShowSelectedSum()
        {
            SetSelectionFlags();
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
        public bool AllSelectedRowsHaveOverriddenPrices
        {
            get { return allSelectedRowsHaveOverriddenPrices; }
        }

        public override string SubTitle
        {
            get { return VisibleSumMessage.Replace("&&", "&"); }
        }
        public IEnumerable<GHVHugoLib.IUnrealizedGains> UnrealizedGainsToProcess
        {
            get
            {
                return GetUnrealizedGains(RowsToProcess);
            }
        }
        protected IEnumerable<GHVHugoLib.IUnrealizedGains> GetUnrealizedGains(IEnumerable<DataGridViewRow> rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                yield return ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.IUnrealizedGains;
            }
        }
        protected virtual void SetSelectionFlags()
        {
            allSelectedRowsHaveOverriddenPrices = true;

            foreach (GHVHugoLib.IUnrealizedGains row in GetUnrealizedGains(VisibleSelectedRows))
            {

                // check for price overrides
                if (row.PriceOverrideFlag != "*")
                {
                    allSelectedRowsHaveOverriddenPrices = false;
                }
            }
        }
    }
}
