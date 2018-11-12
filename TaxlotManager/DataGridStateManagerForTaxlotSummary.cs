using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace TaxlotManager
{
    internal class DataGridStateManagerForTaxlotSummary : DataGridStateManager
    {
        // indices of columns with variable headers
       private const int startingDateColumnIndex = 5;
       private const int endingDateColumnIndex = 6;
       private const string unrealizedColumnHeading = "Unr Gain(Loss) - {0:d}";

        private ToolStripStatusLabel totalSumLabel;
        private ToolStripStatusLabel selectedSumLabel;
        private double selectedStartingUnrealizedSum;
        private double selectedEndingUnrealizedSum;
        private double selectedChangeInUnrealizedSum;
        private double selectedRealizedSum;
        private double selectedTotalSum;
        private double visibleStartingUnrealizedSum;
        private double visibleEndingUnrealizedSum;
        private double visibleChangeInUnrealizedSum;
        private double visibleRealizedSum;
        private double visibleTotalSum;
        private bool selectionMade;

        public DataGridStateManagerForTaxlotSummary(DataGridView dataGridView,
             EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
               RefreshDataDelegate refreshDataDelegate,
          ToolStripStatusLabel filterStatusLabel,
             ToolStripStatusLabel showAllLabel,
             ToolStripStatusLabel totalSumLabel,
             ToolStripStatusLabel selectedSumLabel)
            : base("Taxlot summary", dataGridView, refreshBindingSource, refreshDataDelegate, filterStatusLabel, showAllLabel)
        {
            this.totalSumLabel = totalSumLabel;
            this.selectedSumLabel = selectedSumLabel;

            Form1.CurrentDateChanged += new EventHandler(Form1_CurrentDateChanged);
        }

        public void CalculateSelectedSum()
        {
            selectedStartingUnrealizedSum = 0f;
            selectedEndingUnrealizedSum = 0f;
            selectedChangeInUnrealizedSum = 0f;
            selectedRealizedSum = 0f;
            selectedTotalSum = 0f;
            selectionMade = false;

            foreach (DataGridViewRow row in VisibleSelectedRows)
            {
                DataRowView dataRow = (DataRowView)row.DataBoundItem;
                selectionMade = true;
                selectedStartingUnrealizedSum += Convert.ToDouble(dataRow["StartingUnrealizedGainLoss"]);
                selectedEndingUnrealizedSum += Convert.ToDouble(dataRow["EndingUnrealizedGainLoss"]);
                selectedChangeInUnrealizedSum += Convert.ToDouble(dataRow["ChangeInUnrealizedGainLoss"]);
                selectedRealizedSum += Convert.ToDouble(dataRow["RealizedGainLoss"]);
                selectedTotalSum += Convert.ToDouble(dataRow["TotalGainLoss"]);
            }
        }
        public void CalculateVisibleSum()
        {
            visibleStartingUnrealizedSum = 0f;
            visibleEndingUnrealizedSum = 0f;
            visibleChangeInUnrealizedSum = 0f;
            visibleRealizedSum = 0f;
            visibleTotalSum = 0f;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Visible)
                {
                    DataRowView dataRow = (DataRowView)row.DataBoundItem;
                    visibleStartingUnrealizedSum += Convert.ToDouble(dataRow["StartingUnrealizedGainLoss"]);
                    visibleEndingUnrealizedSum += Convert.ToDouble(dataRow["EndingUnrealizedGainLoss"]);
                    visibleChangeInUnrealizedSum += Convert.ToDouble(dataRow["ChangeInUnrealizedGainLoss"]);
                    visibleRealizedSum += Convert.ToDouble(dataRow["RealizedGainLoss"]);
                    visibleTotalSum += Convert.ToDouble(dataRow["TotalGainLoss"]);
                }
            }
        }
        public string SelectedSumMessage
        {
            get
            {
                if (selectionMade)
                    return String.Format("Selected unrlzd start= {0:C2} | end= {1:C2} | change= {2:C2} | Rlzd= {3:C2} | Total= {4:C2}",
                       selectedStartingUnrealizedSum, selectedEndingUnrealizedSum, selectedChangeInUnrealizedSum, selectedRealizedSum, selectedTotalSum);
                else
                    return String.Empty;
            }
        }
        public string VisibleSumMessage
        {
            get
            {
                    return String.Format("Unrlzd start= {0:C2} | end= {1:C2} | change= {2:C2} | Rlzd= {3:C2} | Total= {4:C2}",
                       visibleStartingUnrealizedSum, visibleEndingUnrealizedSum,  visibleChangeInUnrealizedSum, visibleRealizedSum, visibleTotalSum);
            }
        }

        public override string SubTitle
        {
            get { return VisibleSumMessage; }
        }

        public override void ShowSelectedSum()
        {
            CalculateSelectedSum();
            if (selectedSumLabel != null)
            {
                if ((dataGridView.Rows.Count > 0) && selectionMade)
                {

                    selectedSumLabel.Visible = true;
                    selectedSumLabel.Text = SelectedSumMessage;
                }
                else
                {
                    selectedSumLabel.Visible = false;
                }
            }
        }

        public override void ShowVisibleSum()
        {
            if (dataGridView.Rows.Count > 0)
            {
                CalculateVisibleSum();

                if (totalSumLabel != null)
                {
                    totalSumLabel.Visible = true;
                    totalSumLabel.Text = VisibleSumMessage;
                }
            }
            else
            {
                if (totalSumLabel != null)
                {
                    totalSumLabel.Visible = false;
                }
            }
        }

        private void Form1_CurrentDateChanged(object sender, EventArgs e)
        {
            dataGridView.Columns[startingDateColumnIndex].HeaderText =
                String.Format(unrealizedColumnHeading, GHVHugoLib.Utilities.StartingDate);
            dataGridView.Columns[endingDateColumnIndex].HeaderText =
                String.Format(unrealizedColumnHeading, GHVHugoLib.Utilities.ClosingDate);
        }
    }
}
