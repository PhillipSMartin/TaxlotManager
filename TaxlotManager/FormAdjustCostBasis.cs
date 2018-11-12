using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TaxlotManager
{
    public partial class FormAdjustCostBasis : Form
    {
        public FormAdjustCostBasis(double currentCostBasis)
        {
            InitializeComponent();
            textBoxOldCostBasis.Text = currentCostBasis.ToString();
            DateTime adjustmentDate = GHVHugoLib.Utilities.EndDate ?? DateTime.Today;
            dateTimePickerAdjustmentDate.Value = adjustmentDate.Subtract(new TimeSpan(1,0,0,0));
        }

        public AdjustmentCriterion AdjustmentCriterion
        {
            get
            {
                if (radioButtonNumberOfShares.Checked)
                    return AdjustmentCriterion.NumberOfShares;
                if (radioButtonCurrentCostBasis.Checked)
                    return AdjustmentCriterion.CurrentCostBasis;

                throw new Exception("Invalid adjustment criterion");
            }
        }

        public double NewCostBasis
        {
            get
            {
                return Convert.ToDouble(textBoxNewCostBasis.Text);
            }
        }

        public string TradeNote
        {
            get
            {
                return Convert.ToString(textBoxTradeNote.Text);
            }
        }

        public DateTime AdjustmentDate
        {
            get
            {
                return dateTimePickerAdjustmentDate.Value;
            }
        }

        private void FormAdjustCostBasis_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Cancel)
            {
                try
                {
                    if (NewCostBasis < 0)
                    {
                        MessageBox.Show("New cost basis must be greater than or equal to zero", "Adjust cost basis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("New cost basis must be numeric", "Adjust cost basis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }

                if (e.Cancel == false)
                {
                    try
                    {
                        if (AdjustmentDate > DateTime.Now)
                        {
                            MessageBox.Show("Adjustment date cannot be in the future", "Adjust cost basis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Adjustment date must be a valid date", "Adjust cost basis", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }
        }
   }
}
