using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TaxlotManager
{
    internal class RefreshBindingSourceEventArgs : EventArgs
    {
        private DataTable dataTable;

        public RefreshBindingSourceEventArgs()
        {
        }

        public DataTable DataTable { get { return dataTable; } set { dataTable = value; } }
    }
}
