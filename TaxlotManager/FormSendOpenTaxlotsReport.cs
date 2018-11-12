using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using LoggingUtilitiesLib;

namespace TaxlotManager
{
    public partial class FormSendOpenTaxlotsReport : FormSendReport
    {
        public FormSendOpenTaxlotsReport(ILog logger)
            : base(logger, new OpenTaxlotsReportSender(GHVHugoLib.Utilities.OpenTaxlots))
        {
            Text = "Send Open Taxlot Report";
            SetTextboxBinding(TextBoxTo, "MailOpenTaxlotsTo");
            SetTextboxBinding(TextBoxSubject, "MailOpenTaxlotsSubject");
        }
    }
}
