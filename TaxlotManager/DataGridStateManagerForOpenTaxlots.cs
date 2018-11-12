using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Threading;

namespace TaxlotManager
{
    internal class DataGridStateManagerForOpenTaxlots : DataGridStateManagerForUnrealizedPandL
    {
         private bool allSelectedRowsAreForSameUnderlying;
        private bool allSelectedRowsHaveLinkedTaxlots;
        private bool allSelectedRowsDoNotHaveLinkedTaxlots;

        public DataGridStateManagerForOpenTaxlots(string name,
            DataGridView dataGridView,
             EventHandler<RefreshBindingSourceEventArgs> refreshBindingSource,
              RefreshDataDelegate refreshDataDelegate,
           ToolStripStatusLabel filterStatusLabel,
             ToolStripStatusLabel showAllLabel,
             ToolStripStatusLabel totalSumLabel,
             ToolStripStatusLabel tradeSumLabel)
            : base(name, GHVHugoLib.InstrumentType.Stock, dataGridView, refreshBindingSource, refreshDataDelegate, filterStatusLabel, showAllLabel, totalSumLabel, tradeSumLabel)
        {
        }

  
        protected override void SetSelectionFlags()
        {
            base.SetSelectionFlags();

            allSelectedRowsAreForSameUnderlying = true;
            allSelectedRowsHaveLinkedTaxlots = true;
            allSelectedRowsDoNotHaveLinkedTaxlots = true;
            string ticker = null;

            foreach (GHVHugoLib.ITaxLot row in GetTaxlots(VisibleSelectedRows))
            {

                // check for same underlying
                if (ticker == null)
                {
                    ticker = row.Ticker ?? String.Empty;
                }
                else
                {
                    if (ticker != row.Ticker)
                        allSelectedRowsAreForSameUnderlying = false;
                }

                // check for all rows linked or all rows not linked
                if (String.IsNullOrEmpty(row.LinkedTaxLotId))
                {
                    allSelectedRowsHaveLinkedTaxlots = false;
                }
                else
                {
                    allSelectedRowsDoNotHaveLinkedTaxlots = false;
                }
            }
        }
 
        public IEnumerable<GHVHugoLib.ITaxLot> TaxlotsToProcess
        {
            get
            {
                return GetTaxlots(RowsToProcess);
            }
        }
        protected IEnumerable<GHVHugoLib.ITaxLot> GetTaxlots(IEnumerable<DataGridViewRow> rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                yield return ((DataRowView)row.DataBoundItem).Row as GHVHugoLib.ITaxLot;
            }
        }
         public bool AllSelectedRowsAreForSameUnderlying
        {
            get { return allSelectedRowsAreForSameUnderlying; }
        }
        public bool AllSelectedRowsHaveLinkedTaxlots
        {
            get { return allSelectedRowsHaveLinkedTaxlots; }
        }
        public bool AllSelectedRowsDoNotHaveLinkedTaxlots
        {
            get { return allSelectedRowsDoNotHaveLinkedTaxlots; }
        }
     }
}
