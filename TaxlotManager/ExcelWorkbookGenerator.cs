using System;
using System.Windows.Forms;
using CarlosAg.ExcelXmlWriter;

public class ExcelWorkbookGenerator
{
    Workbook m_workbook = new Workbook();

    public ExcelWorkbookGenerator()
    {
        CreateWorkbookStyles();
    }

    #region Public methods
    public void AddWorksheet(DataGridView dataGridView)
    {
        string cellValue;
        DataType cellDataType;
        string cellFormat;

        string worksheetName = dataGridView.AccessibleName + " - " + GHVHugoLib.Utilities.Account;
        if (worksheetName.Length > 31)
            worksheetName = worksheetName.Substring(0, 31);
        Worksheet worksheet = m_workbook.Worksheets.Add(worksheetName);

        WorksheetRow header = worksheet.Table.Rows.Add();
        foreach (DataGridViewColumn column in dataGridView.Columns)
        {
            header.Cells.Add(new WorksheetCell(column.HeaderText, DataType.String, "Header"));
            worksheet.Table.Columns.Add(new WorksheetColumn(column.HeaderText.Length * 7));
        }
        foreach (DataGridViewRow dataGridViewRow in dataGridView.Rows)
        {
            WorksheetRow worksheetRow = worksheet.Table.Rows.Add();
            foreach (DataGridViewCell dataGridViewCell in dataGridViewRow.Cells)
            {
                GetCellData(dataGridViewCell, out cellValue, out cellDataType, out cellFormat);
                worksheetRow.Cells.Add(new WorksheetCell(cellValue, cellDataType, cellFormat));
            }
        }
    }
    public void SaveWorkbook(string fileName)
    {
        m_workbook.Save(fileName);
    }
    #endregion

    #region Private methods
    private void CreateWorkbookStyles()
    {
        WorksheetStyle styleHeader = new WorksheetStyle("Header");
        styleHeader.Font.FontName = "Tahoma";
        styleHeader.Font.Bold = true;
        styleHeader.Alignment.Horizontal = StyleHorizontalAlignment.Center;
        m_workbook.Styles.Add(styleHeader);

        WorksheetStyle style0 = new WorksheetStyle("Default");
        m_workbook.Styles.Add(style0);

        WorksheetStyle style1 = new WorksheetStyle("DateTime");
        style1.NumberFormat = "mm/dd/yy";
        m_workbook.Styles.Add(style1);

        WorksheetStyle style2 = new WorksheetStyle("F2");
        style2.NumberFormat = "0.00";
        m_workbook.Styles.Add(style2);

        WorksheetStyle style3 = new WorksheetStyle("F3");
        style3.NumberFormat = "0.000";
        m_workbook.Styles.Add(style3);

        WorksheetStyle style4 = new WorksheetStyle("Currency");
        style4.NumberFormat = "Currency";
        m_workbook.Styles.Add(style4);
    }

    private static void GetCellData(DataGridViewCell dataGridViewCell, out string cellValue, out DataType cellDataType, out string cellFormat)
    {
        cellValue = ConvertCellValue(dataGridViewCell.Value, dataGridViewCell.ValueType);
        cellDataType = ConvertCellDataType(dataGridViewCell.ValueType);
        cellFormat = ConvertCellFormat(dataGridViewCell.ValueType, dataGridViewCell.OwningColumn.DefaultCellStyle.Format);
    }

    private static string ConvertCellValue(object cellValue, Type cellDataType)
    {
        if (cellDataType == typeof(DateTime))
            return Convert.ToDateTime(cellValue).ToOADate().ToString();
        else
            return Convert.ToString(cellValue);
    }
    private static DataType ConvertCellDataType(Type cellDataType)
    {
        if ((cellDataType == typeof(double)) || (cellDataType == typeof(int)) || (cellDataType == typeof(short)) || (cellDataType == typeof(DateTime)))
        {
            return DataType.Number;
        }
        else
        {
            return DataType.String;
        }
    }

    private static string ConvertCellFormat(Type cellDataType, string dataGridCellFormat)
    {
        string cellFormat = "Default";
        dataGridCellFormat = dataGridCellFormat.ToUpper();

        if (cellDataType == typeof(DateTime))
        {
            cellFormat = "DateTime";
        }
        else if (dataGridCellFormat.Length > 0)
        {
            if ((dataGridCellFormat == "N2") || (dataGridCellFormat == "F2"))
                cellFormat = "F2";
            else if ((dataGridCellFormat == "N3") || (dataGridCellFormat == "F3"))
                cellFormat = "F3";
            else if (dataGridCellFormat[0] == 'C')
                cellFormat = "Currency";
        }
        return cellFormat;
    }
    #endregion
}
