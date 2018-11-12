using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace GHVHugoLib
{
    [CLSCompliant(false)]
    public class GHVCsvReader : CsvReader
    {
        public GHVCsvReader(string fileName, char delimeter)
            : base(new StreamReader(fileName), /*hasHeaders = */ true, delimeter)
        {
            SupportsMultiline = false;
        }


        protected string GetDataFromColumn(string columnName)
        {
            int columnIndex = GetFieldIndex(columnName);
            if (columnIndex < 0)
            {
                throw new GHVHugoColumnNotFoundException(columnName);
            }

            return this[columnIndex];
        }
    }
}
