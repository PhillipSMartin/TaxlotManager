using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LumenWorks.Framework.IO.Csv;

namespace GHVHugoLib
{
    [CLSCompliant(false)]
    public class StockTradeCollection : CsvReader
    {
        public StockTradeCollection(string fileName)
            : base(new StreamReader(fileName), /*hasHeaders = */ true)
        {
            SupportsMultiline = false;
        }

        public StockTrade NextRecord
        {
            get
            {
                while (ReadNextRecord())
                {
                    try
                    {
                        StockTrade record = new StockTrade(this[0], this[3], this[4], this[5], this[6], this[9]);
                        if (record.IsValid)
                            return record;
                    }
                    catch (Exception e)
                    {
                        Utilities.FireOnError("Error reading trade record: " + GetCurrentRawData(), e);
                    }
                }

                return null;
            }
        }
    }
}
