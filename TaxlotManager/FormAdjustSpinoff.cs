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
    public partial class FormAdjustSpinoff : Form
    {
        double m_totalCost;
        double m_oldPostSpinoffCostOfOriginalStock;
        double m_oldCostOfSpunoffStock;
        double m_newPostSpinoffCostOfOriginalStock;
        double m_newCostOfSpunoffStock;
        double m_costPercentageOfOriginalStock;
        double m_costPercentageOfSpunoffStock;

        public FormAdjustSpinoff(string originalSymbol, double postSpinoffSharesOfOriginalStock, double postSpinoffCostOfOriginalStock,
            string spinoffSymbol, double sharesSpunOff, double costOfSpunoffStock)
        {
            InitializeComponent();
            m_totalCost = postSpinoffCostOfOriginalStock + costOfSpunoffStock;

            labelOriginalSymbol.Text = originalSymbol;
            NewPostSpinoffSharesOfOriginalStock = OldPostSpinoffSharesOfOriginalStock = postSpinoffSharesOfOriginalStock;
            NewPostSpinoffCostOfOriginalStock = OldPostSpinoffCostOfOriginalStock = postSpinoffCostOfOriginalStock;

            labelNewSymbol.Text = spinoffSymbol;
            NewSharesSpunOff = OldSharesSpunOff = sharesSpunOff;
            NewCostOfSpunoffStock = OldCostOfSpunoffStock = costOfSpunoffStock;

            ShowPercentages();
            EnableControls();
        }

        #region Properties
        public double OldPostSpinoffSharesOfOriginalStock
        {
            get
            {
                return Convert.ToDouble(labelPostSpinoffSharesOfOriginalStock.Text);
            }
            set
            {
                labelPostSpinoffSharesOfOriginalStock.Text = value.ToString();
            }
        }
        public double OldPostSpinoffCostOfOriginalStock
        {
            get
            {
                return m_oldPostSpinoffCostOfOriginalStock;
            }
            set
            {
                labelPostSpinoffCostOfOriginalStock.Text = (m_oldPostSpinoffCostOfOriginalStock = value).ToString("F2");
            }
        }
         public double OldSharesSpunOff
        {
            get
            {
                return Convert.ToDouble(labelSharesSpunOff.Text);
            }
            set
            {
                labelSharesSpunOff.Text = value.ToString();
            }
        }
         public double OldCostOfSpunoffStock
         {
             get
             {
                 return m_oldCostOfSpunoffStock;
             }
             set
             {
                 this.labelCostOfSpunoffStock.Text = (m_oldCostOfSpunoffStock = value).ToString("F2");
             }
         }
         public double NewPostSpinoffSharesOfOriginalStock
        {
            get
            {
                return Convert.ToDouble(textBoxPostSpinoffSharesOfOriginalStock.Text);
            }
            set
            {
                textBoxPostSpinoffSharesOfOriginalStock.Text = value.ToString();
            }
        }
         public double NewPostSpinoffCostOfOriginalStock
        {
            get
            {
                return m_newPostSpinoffCostOfOriginalStock;
            }
            set
            {
                textBoxPostSpinoffCostOfOriginalStock.Text = (m_newPostSpinoffCostOfOriginalStock = value).ToString("F2");
            }
        }
         public double NewSharesSpunOff
         {
             get
             {
                 return Convert.ToDouble(textBoxSharesSpunOff.Text);
             }
             set
             {
                 textBoxSharesSpunOff.Text = value.ToString();
             }
         }
         public double NewCostOfSpunoffStock
        {
            get
            {
                return m_newCostOfSpunoffStock;
            }
            set
            {
                textBoxCostOfSpunoffStock.Text = (m_newCostOfSpunoffStock = value).ToString("F2");
            }
        }
         public double CostPercentageOfOriginalStock
         {
             get
             {
                 return m_costPercentageOfOriginalStock;
             }
             set
             {
                 textBoxCostPercentageOfOriginalStock.Text = (m_costPercentageOfOriginalStock = value).ToString("F6");
             }
         }
         public double CostPercentageOfSpunoffStock
         {
             get
             {
                 return m_costPercentageOfSpunoffStock;
             }
             set
             {
                 textBoxCostPercentageOfSpunoffStock.Text = (m_costPercentageOfSpunoffStock = value).ToString("F6");
             }
         }
        #endregion

         private void FormAdjustSpinoff_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.Cancel)
            {
                try
                {
                    if (NewPostSpinoffCostOfOriginalStock < 0)
                    {
                        MessageBox.Show("New cost basis of original shares must be greater than or equal to zero", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("New cost basis of original shares must be numeric", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
                if (e.Cancel == false)
                {
                    try
                    {
                        if (NewCostOfSpunoffStock < 0)
                        {
                            MessageBox.Show("New cost basis of spun off shares must be greater than or equal to zero", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("New cost basis of spun off shares must be numeric", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                if (e.Cancel == false)
                {
                    try
                    {
                        if (NewSharesSpunOff <= 0)
                        {
                            MessageBox.Show("Number of spun off shares must be greater than zero", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Number of spun off shares must be numeric", "Adjust spinoff", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }

            }
        }

        private void checkBoxUsePercentage_CheckedChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void EnableControls()
        {
            textBoxPostSpinoffSharesOfOriginalStock.Enabled = true;
            textBoxPostSpinoffCostOfOriginalStock.Enabled = !checkBoxUsePercentage.Checked;
            textBoxCostPercentageOfOriginalStock.Enabled = checkBoxUsePercentage.Checked;

            textBoxSharesSpunOff.Enabled = true;
            textBoxCostOfSpunoffStock.Enabled = !checkBoxUsePercentage.Checked;
            textBoxCostPercentageOfSpunoffStock.Enabled = checkBoxUsePercentage.Checked;
        }

        private void textBoxPostSpinoffCostOfOriginalStock_TextChanged(object sender, EventArgs e)
        {
            if (textBoxPostSpinoffCostOfOriginalStock.Focused)
            {
                m_newPostSpinoffCostOfOriginalStock = TryConvertToDouble(textBoxPostSpinoffCostOfOriginalStock.Text);
                NewCostOfSpunoffStock = m_totalCost - m_newPostSpinoffCostOfOriginalStock;
                ShowPercentages();
            }
        }

        private void ShowPercentages()
        {
            CostPercentageOfOriginalStock = (m_totalCost == 0) ? 0 : 100.0 * m_newPostSpinoffCostOfOriginalStock / m_totalCost;
            CostPercentageOfSpunoffStock = (m_totalCost == 0) ? 0 : 100.0 * m_newCostOfSpunoffStock / m_totalCost;
        }

        private void textBoxCostOfSpunoffStock_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCostOfSpunoffStock.Focused)
            {
                m_newCostOfSpunoffStock = TryConvertToDouble(textBoxCostOfSpunoffStock.Text);
                NewPostSpinoffCostOfOriginalStock = m_totalCost - m_newCostOfSpunoffStock;
                ShowPercentages();
            }
        }

        private void textBoxCostPercentageOfOriginalStock_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCostPercentageOfOriginalStock.Focused)
            {
                m_costPercentageOfOriginalStock = TryConvertToDouble(textBoxCostPercentageOfOriginalStock.Text);
                CostPercentageOfSpunoffStock = 100 - m_costPercentageOfOriginalStock;
                ApplyPercentages();
            }
        }

        private void ApplyPercentages()
        {
            NewPostSpinoffCostOfOriginalStock = m_totalCost * m_costPercentageOfOriginalStock / 100.0;
            NewCostOfSpunoffStock = m_totalCost * m_costPercentageOfSpunoffStock / 100.0;
        }

        private void textBoxCostPercentageOfSpunoffStock_TextChanged(object sender, EventArgs e)
        {
            if (textBoxCostPercentageOfSpunoffStock.Focused)
            {
                m_costPercentageOfSpunoffStock = TryConvertToDouble(textBoxCostPercentageOfSpunoffStock.Text);
                CostPercentageOfOriginalStock = 100 - m_costPercentageOfSpunoffStock;
                ApplyPercentages();
            }
        }

        private double TryConvertToDouble(string str)
        {
            try
            {
                return Convert.ToDouble(str);
            }
            catch
            {
                return 0.0;
            }
        }


    }
}
