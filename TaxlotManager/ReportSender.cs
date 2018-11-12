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
    public class ReportSender
    {
        #region Declarations
        private string m_reportsDir;
        private string[] m_reportFileNames;

        public event EventHandler<LoggingUtilitiesLib.InfoEventArgs> Info;
        public event EventHandler<LoggingUtilitiesLib.ErrorEventArgs> Error;
        #endregion

        #region Constructor
        /// <summary>
        /// Instantiates a class which creates reports that we send to Merrill
        /// </summary>
        public ReportSender(string[] reportFileNames, string directory)
        {
            m_reportFileNames = reportFileNames;
            if (String.IsNullOrEmpty(directory))
            {
                m_reportsDir = Directory.GetCurrentDirectory();
            }
            else
            {
                m_reportsDir = directory;
            }
        }

        public ReportSender(string[] reportFileNames) : this(reportFileNames, Properties.Settings.Default.ReportsDirectory)
        {
        }

        public ReportSender()
            : this(null)
        {
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Calls various overridable methods to build and send the report
        /// </summary>
        /// <returns>True on success</returns>
        public bool SendReports()
        {
            bool success = false;
            string[] fullReportFilenames;

            // create reports and return their names in a string array
            try
            {
                fullReportFilenames = CreateArrayOfReports();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                FireError(String.Format("Could not find directory {0}.", Properties.Settings.Default.ReportsDirectory),
                    new GHVHugoException("Choose 'Edit\\Settings' from the menu and change 'ReportsDirectory' to an existing directory"));
                return success;
            }
            catch (Exception ex)
            {
                FireError("Could not create reports", ex);
                return success;
            }

            // transmit the reports
            try
            {
                TransmitReports(fullReportFilenames);
                success = true;
            }
            catch (Exception ex)
            {
                FireError("Could not transmit reports", ex);
                return success;
            }
            FireInfo("SendReports completed");
            return success;
        }
        #endregion

        #region Public properties
        public string[] ReportFileNames
        {
            get { return m_reportFileNames; }
            set { m_reportFileNames = value; }
        }
        #endregion

        #region Protected methods
      /// <summary>
        /// Creates the reports which should be sent to Pax
        /// </summary>
        /// <returns>An array containing the names of all of the report files</returns>
        protected string[] CreateArrayOfReports()
        {
            if (m_reportFileNames == null)
                throw new ArgumentNullException("reportFileNames");

            List<string> exportList = new List<string>();
            foreach (string reportFileName in m_reportFileNames)
            {
                string fullReportFileName = GetFullFilename(reportFileName);
                CreateSingleReport(fullReportFileName);
                exportList.Add(fullReportFileName);
            }

            return exportList.ToArray();
        }

        protected void CreateSingleReport(string fullReportFileName)
        {
            // delete file if it already exists
            if (File.Exists(fullReportFileName))
            {
                File.Delete(fullReportFileName);
            }

            // create a streamwriter
            StreamWriter output = new StreamWriter(fullReportFileName, false, System.Text.Encoding.ASCII);

            // call overridable methods to build report
            try
            {
                WriteHeader(output);
                WriteBody(output);
                WriteTrailer(output);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                output.Close();
            }
        }

        #region Overridable methods
        // these methods should be overridden to produce the various parts of the report
        protected virtual void WriteHeader(StreamWriter output) { }
        protected virtual void WriteBody(StreamWriter output) { }
        protected virtual void WriteTrailer(StreamWriter output) { }

        // this method should be overridden to transmit the report in the manner required
        protected virtual void TransmitReports(string[] reportFilenames) { }
        #endregion


        #region Methods available to transmit reports
        protected void EmailReports(string[] reportFilenames, string messageBody, string from, string to, string subject)
        {
            //load the config settings for the email client
            Gargoyle.Configuration.Configurator.LoadConfigurationFromService(Properties.Settings.Default.SMTPConfigurationModule, false);
            ConfigurationItem m_smtpSettings = Gargoyle.Configuration.Configurator.CurrentConfigurations[Properties.Settings.Default.SMTPConfigurationModule];

            FireInfo("Building email");

            //construct the Microsoft MailMessage
            System.Net.Mail.MailMessage mailToSend = new System.Net.Mail.MailMessage(
                from,
                to,
                subject,
                messageBody);

            try
            {
                //do not use HTML
                mailToSend.IsBodyHtml = false;

                //construct the smtp client that will send the message
                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(
                    m_smtpSettings["ServerAddress"].ValueAsStr,
                    m_smtpSettings["Port"].ValueAsInt);

                //create credentials that will be passed to the smtp server
                smtpClient.Credentials = new System.Net.NetworkCredential
                    (m_smtpSettings["UserName"].ValueAsStr,
                    m_smtpSettings["Password"].ValueAsStr);

                //for each file in  reportFileNames[],  add it to the MailMessage.Attachments collection
                if (reportFilenames != null)
                {
                    foreach (string filename in reportFilenames)
                    {
                        mailToSend.Attachments.Add(new System.Net.Mail.Attachment(filename));
                    }
                }

                string[] ccAddresses = from.Split(new char[] { ',' });

                //Now set the addresses that should be CC'd.
                foreach (string ccAddress in ccAddresses)
                {
                    mailToSend.CC.Add(ccAddress);
                }

                FireInfo("Sending reports");
                smtpClient.Send(mailToSend);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (mailToSend != null)
                {
                    mailToSend.Dispose();
                }
            }
        }
        #endregion

        protected string GetFullFilename(string filename)
        {
            return m_reportsDir + Path.DirectorySeparatorChar + filename;
        }

        protected void FireInfo(string message)
        {
            if (Info != null)
            {
                Info(this, new LoggingUtilitiesLib.InfoEventArgs("ReportSender", message));
            }
        }
        protected void FireError(string message, Exception e)
        {
            if (Error != null)
            {
                Error(this, new LoggingUtilitiesLib.ErrorEventArgs("ReportSender", message, e));
            }
        }
        #endregion
    }
}
