using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Drawing.Color;
using Excel = Microsoft.Office.Interop.Excel;
using ComboBox = System.Windows.Controls.ComboBox;

namespace SetupTool_MaterialSetup.Code.Function.FunctionExcel
{
    class F_ExportExcel
    {
        //----------------------------------------------------------
        public static void Format_Range_Excel(Excel.Range range, int i, int position_range, string value, Color color)
        {
            try
            {
                range.Cells[i, position_range].Value = value;
                range.Cells[i, position_range].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.Cells[i, position_range].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                range.Cells[i, position_range].Interior.Color = color;
                range.Cells[i, position_range].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                range.Cells[i, position_range].Borders.Weight = Excel.XlBorderWeight.xlThin;
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static string Export_Excel(ObservableCollection<data_material_project> my_material_project, string path, string name_sheet, ComboBox pattern)
        {
            string result = "F";
            try
            {
                List<data_fill_pattern> data_pattern = CollectionViewSource.GetDefaultView(pattern.ItemsSource).Cast<data_fill_pattern>().ToList();

                Excel.Application excel = new Excel.Application();
                excel.Visible = true;
                Excel.Workbook workbook = excel.Workbooks.Add(Missing.Value);
                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                sheet.Name = name_sheet;
                Excel.Range range = sheet.UsedRange;

                int range_MCT = 1;
                int range_TVL = 2;
                int range_DV = 3;
                int range_MVL = 4;
                int range_DTS = 5;
                int range_MSF = 6;
                int range_TSF = 7;
                int range_MC = 8;
                int range_TC = 9;
                int range_TON = 10;

                int i = 2;
                foreach (data_material_project data in my_material_project)
                {
                    Format_Range_Excel(range, i, range_MCT, data.ma_cong_tac_project, Color.Transparent);
                    Format_Range_Excel(range, i, range_TVL, data.ten_vat_lieu_project, Color.Transparent);
                    Format_Range_Excel(range, i, range_DV, data.don_vi_project, Color.Transparent);
                    Format_Range_Excel(range, i, range_MVL, "", Color.FromArgb(255, data.mau_vat_lieu.Red, data.mau_vat_lieu.Green, data.mau_vat_lieu.Blue));
                    Format_Range_Excel(range, i, range_DTS, data.do_trong_suot_vat_lieu.ToString(), Color.Transparent);
                    Format_Range_Excel(range, i, range_MSF, "", Color.FromArgb(255, data.mau_surface.Red, data.mau_surface.Green, data.mau_surface.Blue));
                    Format_Range_Excel(range, i, range_TSF, data_pattern.First(x => x.pattern_id.IntegerValue == data.id_surface.IntegerValue).name, Color.Transparent);
                    Format_Range_Excel(range, i, range_MC, "", Color.FromArgb(255, data.mau_cut.Red, data.mau_cut.Green, data.mau_cut.Blue));
                    Format_Range_Excel(range, i, range_TC, data_pattern.First(x => x.pattern_id.IntegerValue == data.id_cut.IntegerValue).name, Color.Transparent);
                    Format_Range_Excel(range, i, range_TON, data.ton, Color.Transparent);

                    range.Cells[i, range_MCT].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    range.Cells[i, range_TVL].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    range.Cells[i, range_MCT].IndentLevel = 2;
                    range.Cells[i, range_TVL].IndentLevel = 2;

                    i++;
                }

                List<string> column_name = new List<string>() { "Material ID", "Material Name", "Unit", "Material Color", "Material Transparency", "Surface Color", "Surface Name", "Cut Color", "Cut Name", "Factor TON" };
                for (int a = 1; a < 11; a++)
                {
                    Format_Range_Excel(range, 1, a, column_name[a - 1], Color.Transparent);
                }
                int row = sheet.Rows.Count;
                sheet.get_Range("A1", "J" + row).Columns.AutoFit();

                workbook.SaveAs(path, Excel.XlFileFormat.xlWorkbookDefault);
                workbook.Close(true, null, null);
                excel.Quit();
                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = "F";
            }
            return result;
        }
    }
}
