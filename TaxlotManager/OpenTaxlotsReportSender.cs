using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GHVHugoLib;
using Gargoyle.Configuration;
using LoggingUtilitiesLib;

namespace TaxlotManager
{
    class OpenTaxlotsReportSender : ReportSender
    {
        #region Declarations
        private static StringBuilder s_reportBody;

        // header record
        private const string s_header = "Ticker,TaxlotId,OpenDate,OpenAmount";

        // open taxlots, used to create the body of the report
        private IEnumerable<ITaxLot> m_openTaxlots;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a class to send the Open Taxlots Report to Merrill.
        /// </summary>
        public OpenTaxlotsReportSender(IEnumerable<ITaxLot> openTaxlots)
            : base(new string[] { "OpenTaxlots.csv" } )
        {
            m_openTaxlots = openTaxlots;
        }
        #endregion


        #region Overridden methods
        protected override void WriteHeader(StreamWriter output)
        {
            // use StringBuilder in addition to output file
            s_reportBody = new StringBuilder();

            output.WriteLine("Account: " + GHVHugoLib.Utilities.Account);
            output.WriteLine(s_header);

            s_reportBody.AppendLine("Account: " + GHVHugoLib.Utilities.Account);
            s_reportBody.AppendLine(s_header);
        }

        protected override void WriteBody(StreamWriter output) 
        {
            // Write each data row
            foreach (ITaxLot taxlot in m_openTaxlots)
            {
                string outputLine = String.Format("\"{0}\",\"{1}\",\"{2:d}\",\"{3:f3}\"",
                   taxlot.Ticker,
                   taxlot.TaxlotId,
                   taxlot.Open_Date,
                   taxlot.Open_Amount);

                output.WriteLine(outputLine);
                s_reportBody.AppendLine(outputLine);
            }
        }

        protected override void TransmitReports(string[] reportFilenames)
        {
            EmailReports(reportFilenames, 
                s_reportBody.ToString(),
                Properties.Settings.Default.MailFrom,
                Properties.Settings.Default.MailOpenTaxlotsTo,
                Properties.Settings.Default.MailOpenTaxlotsSubject);
        }
        #endregion
    }
}
