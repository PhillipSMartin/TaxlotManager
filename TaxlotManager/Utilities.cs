using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace TaxlotManager
{
    public sealed class Utilities
    {
        public static void InitializeComboBox(ComboBox comboBox, object[] items, string defaultItem)
        {
            comboBox.Items.Clear();
            comboBox.Items.AddRange(items);
            comboBox.SelectedIndex = comboBox.FindStringExact(defaultItem);
        }

    }
}
