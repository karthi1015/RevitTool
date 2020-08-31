using Autodesk.Revit.DB;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace SetupTool_MaterialSetup.Code.Function.FunctionExcel
{
    class F_GetMaterialExcel
    {//----------------------------------------------------------
        public static void get_material(List<data_material_excel> data_create, string path, ComboBox pattern)
        {
            try
            {
                Excel.Application excel = new Excel.Application();
                excel.Visible = false;
                Excel.Workbook workbook = excel.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                Excel.Range range = sheet.UsedRange;

                List<data_fill_pattern> list_pattern = CollectionViewSource.GetDefaultView(pattern.ItemsSource).Cast<data_fill_pattern>().ToList();
                ElementId pattern_element = list_pattern.First(x => x.name == "<None>").pattern_id;
                int iRow = range.Rows.Count + 1;
                for (int i = 2; i < iRow; i++)
                {
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
                    //string range_USER = 11;
                    //string range_TIME = 12;

                    string ma_cong_tac = !string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_MCT].Value)) ? Convert.ToString(range.Cells[i, range_MCT].Value) : "";
                    string don_vi = !string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_DV].Value)) ? Convert.ToString(range.Cells[i, range_DV].Value) : "Default";

                    if (!string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_TVL].Value)))
                    {
                        string ten_vat_lieu = Convert.ToString(range.Cells[i, range_TVL].Value);
                        System.Drawing.Color color_MVL = System.Drawing.Color.FromArgb(Convert.ToInt32(range.Cells[i, range_MVL].Interior.Color));
                        System.Drawing.Color color_MSF = System.Drawing.Color.FromArgb(Convert.ToInt32(range.Cells[i, range_MSF].Interior.Color));
                        System.Drawing.Color color_MC = System.Drawing.Color.FromArgb(Convert.ToInt32(range.Cells[i, range_MC].Interior.Color));

                        int do_trong = 0;
                        if (!string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_DTS].Value)) && System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(range.Cells[i, range_DTS].Value), "[^0-9]") == false)
                            do_trong = Convert.ToInt32(range.Cells[i, range_DTS].Value);

                        ElementId id_surface = pattern_element;
                        if (!string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_TSF].Value))) id_surface = list_pattern.First(x => x.name == Convert.ToString(range.Cells[i, range_TSF].Value)).pattern_id;

                        ElementId id_cut = pattern_element;
                        if (!string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_TC].Value))) id_cut = list_pattern.First(x => x.name == Convert.ToString(range.Cells[i, range_TC].Value)).pattern_id;

                        data_create.Add(new data_material_excel()
                        {
                            ma_cong_tac_excel = ma_cong_tac,
                            ten_vat_lieu_excel = ten_vat_lieu,
                            don_vi_excel = don_vi,
                            mau_vat_lieu_excel = new Color(color_MVL.B, color_MVL.G, color_MVL.R),
                            do_trong_suot_vat_lieu_excel = do_trong,
                            mau_surface_excel = new Color(color_MSF.B, color_MSF.G, color_MSF.R),
                            id_surface_excel = id_surface,
                            mau_cut_excel = new Color(color_MC.B, color_MC.G, color_MC.R),
                            id_cut_excel = id_cut,
                            ton = !string.IsNullOrEmpty(Convert.ToString(range.Cells[i, range_TON].Value)) ? Convert.ToString(range.Cells[i, range_TON].Value) : "2.5"
                        });
                    }
                }

                workbook.Close(true, null, null);
                excel.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
