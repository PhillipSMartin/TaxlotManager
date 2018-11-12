using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TaxlotManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                var dirDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Gargoyle Strategic Investments\\TaxlotManager";
                DirectoryInfo dInfo = new DirectoryInfo(dirDataPath);
                if (!dInfo.Exists)
                    dInfo.Create();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.InnerException.ToString());
            }
        }
    }
}
