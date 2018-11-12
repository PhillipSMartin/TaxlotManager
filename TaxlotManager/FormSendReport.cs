using System;
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
    public partial class FormSendReport : Form
    {
        private ILog m_logger;
        private ReportSender m_reportSender;

        public FormSendReport(ILog logger, ReportSender reportSender)
        {
            InitializeComponent();
            m_logger = logger;
            m_reportSender = reportSender;
        }

        protected void SetTextboxBinding(TextBox textBox, string settingName)
        {
            textBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TaxlotManager.Properties.Settings.Default, settingName, true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
        }

        protected TextBox TextBoxFrom { get { return textBoxFrom; } }
        protected TextBox TextBoxTo { get { return textBoxTo; } }
        protected TextBox TextBoxSubject { get { return textBoxSubject; } }


        private void buttonSend_Click(object sender, EventArgs e)
        {
            Enabled = false;

            try
            {
                m_reportSender.Info += new EventHandler<InfoEventArgs>(m_reportSender_Info);
                m_reportSender.Error += new EventHandler<ErrorEventArgs>(m_reportSender_Error);

                // On success make the OK button visible
                if (m_reportSender.SendReports())
                {
                    buttonCancel.Visible = false;
                    buttonSend.Visible = false;
                    buttonOK.Visible = true;
                }
                m_reportSender.Info -= new EventHandler<InfoEventArgs>(m_reportSender_Info);
                m_reportSender.Error -= new EventHandler<ErrorEventArgs>(m_reportSender_Error);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                Enabled = true;
            }
        }

        private void m_reportSender_Info(object sender, InfoEventArgs e)
        {
            labelInfo.Text += e.Message + "\n";
            if (m_logger != null)
                m_logger.Info(e.Message);
        }

        private void m_reportSender_Error(object sender, ErrorEventArgs e)
        {
            labelInfo.Text += String.Format("{0}: {1}\n", e.Message, e.Exception.Message);
            if (m_logger != null)
                m_logger.Error(e.Message, e.Exception);
        }
    }
}
