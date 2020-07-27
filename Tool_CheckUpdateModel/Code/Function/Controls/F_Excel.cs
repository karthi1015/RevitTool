using Autodesk.Revit.DB;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tool_CheckUpdateModel.Data;
using Tool_CheckUpdateModel.Data.Binding;
using Excel = Microsoft.Office.Interop.Excel;

namespace Tool_CheckUpdateModel.Function.Controls
{
    class F_Excel
    {
        public static void Export_Excel(Document doc, Document doc_link, string path_excel_source, ObservableCollection<Element_Change> my_element_change)
        {
            try
            {
                string name = doc_link.PathName.Split('\\').Last().Split('.')[0];
                string path_directory = Path.GetDirectoryName(doc_link.PathName) + "\\" + "Images";

                if (Directory.Exists(path_directory))
                {
                    Excel.Application app = new Excel.Application();
                    app.Visible = true;
                    Excel.Workbook wb = app.Workbooks.Open(path_excel_source, 0, false, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    Worksheet ws = wb.Worksheets.get_Item(1);
                    int iRow = 11;
                    Excel.Range range = ws.UsedRange;
                    float t = 0;
                    for (var i = 1; i < iRow; i++)
                    {
                        t += ws.Rows[i].Height;
                    }
                    ws.Columns[5].ColumnWidth = 264.75 / 6;
                    int index = 1;
                    range.Cells[1, 2].Value = doc.PathName.Split('\\').Last();
                    range.Cells[2, 2].Value = doc_link.PathName.Split('\\').Last();
                    range.Cells[3, 2].Value = doc.ProjectInformation.BuildingName;
                    range.Cells[4, 2].Value = doc.PathName.Split('\\').Last().Split('.')[0].Split('_')[1];
                    range.Cells[5, 2].Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    foreach (Element_Change item in my_element_change)
                    {
                        string id = "";
                        if (item.element_link != null) id = item.element_link.Id.ToString();
                        try
                        {
                            ws.Rows[iRow].RowHeight = 149;

                            float left = (float)(ws.Columns[1].Width + ws.Columns[2].Width + ws.Columns[3].Width + ws.Columns[4].Width);
                            float top = (float)(t);

                            range.Cells[iRow, 1].Value = index;
                            range.Cells[iRow, 2].Value = item.element_name;
                            range.Cells[iRow, 3].Value = id;
                            range.Cells[iRow, 4].Value = item.type_change;
                            ws.Shapes.AddPicture(Directory.GetFiles(path_directory).ToList().First(x => x.Contains(item.element_id)), MsoTriState.msoFalse, MsoTriState.msoCTrue, left + 1, top + 1, ws.Columns[5].Width - 2, ws.Rows[iRow].Height - 2);
                            range.Cells[iRow, 6].Value = item.preview;
                        }
                        catch (Exception)
                        {

                        }
                        t += ws.Rows[iRow].Height;
                        iRow++;
                        index++;
                    }
                    range = ws.Range[ws.Cells[11, 1], ws.Cells[iRow - 1, 7]];
                    foreach (Range cell in range.Cells)
                    {
                        cell.BorderAround2();
                    }
                    wb.SaveAs(doc_link.PathName.Split('.')[0]);
                    wb.Close(true);
                    app.Quit();
                }
                else
                {
                    MessageBox.Show("Images not found, Please export images and export excel again!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
