using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Color = Autodesk.Revit.DB.Color;
using Color1 = System.Drawing.Color;
using Color2 = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Image = System.Windows.Controls.Image;
using System.Windows.Media.Imaging;
using ListView = System.Windows.Controls.ListView;

namespace SetupTool_MaterialSetup
{
    public class FunctionSupoort
    {
        ListSource mySource;
        //----------------------------------------------------------
        public string Get_Data_String(List<string> list, string key)
        {
            string value = "";
            try
            {
                value = list.First(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key);
            }
            catch (Exception)
            {

            }
            return value;
        }

        //----------------------------------------------------------
        public List<string> Get_Data_List(List<string> list, string key)
        {
            List<string> value = new List<string>();
            try
            {
                value = list.Where(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key).ToList();
            }
            catch (Exception)
            {

            }
            return value;
        }
        //----------------------------------------------------------
        public All_Data Get_Data_All(string path)
        {
            All_Data myAll_Data = new All_Data();
            try
            {
                List<string> du_lieu = File.ReadAllLines(path).ToList();

                myAll_Data = new All_Data()
                {
                    list_path_image_data = new ObservableCollection<list_image>(Get_Data_List(du_lieu, "path_image").Select(x => new list_image()
                    {
                        image_name = x.Split('\t')[2],
                        image_path = x.Split('\t')[3]
                    })),

                    list_unit_value_data = new ObservableCollection<double>(Get_Data_List(du_lieu, "unit_value").Select(x => Convert.ToDouble(x.Split('\t')[2]))),

                    list_color_UI_data = new ObservableCollection<Brush>(Get_Data_List(du_lieu, "color_UI").Select(x =>
                    (SolidColorBrush)new BrushConverter().ConvertFromString(x.Split('\t')[2]))),

                    list_procedure_data = new ObservableCollection<list_procedure>(Get_Data_List(du_lieu, "sp_quantity").Select(x => new list_procedure()
                    {
                        procedure_name = x.Split('\t')[2],
                        procedure_para = x.Split('\t')[3]
                    })),

                    list_path_foder_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_foder").Select(x => x.Split('\t')[2])),

                    list_path_connect_SQL_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_connect_SQL").Select(x => x.Split('\t')[2])),

                    list_material_para_data = new ObservableCollection<list_material_para>(Get_Data_List(du_lieu, "material_parameter").Select(x => new list_material_para()
                    {
                        material_para_guid = x.Split('\t')[2],
                        material_para_name = x.Split('\t')[3]
                    })),

                    list_mct_descipline_data = new ObservableCollection<list_mct_descipline>(Get_Data_List(du_lieu, "mct_descipline").Select(x => new list_mct_descipline()
                    {
                        mct = x.Split('\t')[2],
                        descipline = x.Split('\t')[3]
                    })),

                    list_ten_du_lieu_excel_data = new ObservableCollection<list_ten_du_lieu_excel>(Get_Data_List(du_lieu, "list_ten_du_lieu_excel").Select(x => new list_ten_du_lieu_excel()
                    {
                        excel_name = x.Split('\t')[2],
                        excel_ngang = x.Split('\t')[3],
                        excel_dung = x.Split('\t')[4]
                    })),

                    list_parameter_share_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "parameter_share").Select(x => x.Split('\t')[2]))
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myAll_Data;
        }

        //----------------------------------------------------------
        public void Default_Image(All_Data myAll_Data, List<Image> list_control_image)
        {
            try
            {
                for (int i = 0; i < list_control_image.Count; i++)
                {
                    list_control_image[i].Source = new BitmapImage(new Uri(myAll_Data.list_path_image_data.First(x => x.image_name == list_control_image[i].Name).image_path));
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Get_SurfacePatternName_And_CutPatternName(Document doc, List<Data> myData_fillPattern, ComboBox name_cut, ComboBox name_surface)
        {
            try
            {
                var fillPatternElements = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).ToElements();
                
                foreach (FillPatternElement f in fillPatternElements)
                {
                    myData_fillPattern.Add(new Data()
                    {
                        value_image = f.GetFillPattern().Name,
                        image = @"D:\00 CuaKhanh\03 Revit\00 Ribbon_Mr.W\Image\Logo\Logo1.png",
                        fillPatternElement = f
                    });
                }
                var a = myData_fillPattern.OrderBy(x => x.value_image).ToList();
                myData_fillPattern.Clear();
                foreach (var b in a)
                {
                    myData_fillPattern.Add(b);
                }
                myData_fillPattern.OrderBy(x => x.value_image);
                myData_fillPattern.Insert(0, new Data()
                {
                    value_image = "None",
                    image = @"D:\00 CuaKhanh\03 Revit\00 Ribbon_Mr.W\Image\Logo\Logo1.png"
                });
                name_cut.ItemsSource = myData_fillPattern;
                name_surface.ItemsSource = myData_fillPattern;
                name_cut.SelectedIndex = 0;
                name_surface.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public string Check_Para_And_Get_Para(Material material, string guid, string name_para)
        {
            string value = null;
            try
            {
                Schema getSchema = Schema.Lookup(new Guid(guid));
                if (getSchema != null)
                {
                    Entity ent = material.GetEntity(getSchema);
                    if (ent.Schema != null)
                    {
                        value = ent.Get<string>(getSchema.GetField(name_para));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //----------------------------------------------------------
        public void SetDataStorage(Material material, string guid, string name_para, string value)
        {
            try
            {
                SchemaBuilder sb1 = new SchemaBuilder(new Guid(guid));
                sb1.SetReadAccessLevel(AccessLevel.Public);
                sb1.SetWriteAccessLevel(AccessLevel.Public);
                sb1.SetVendorId("pentacons");
                sb1.SetSchemaName(name_para);

                FieldBuilder fieldUser = sb1.AddSimpleField(name_para, typeof(string));
                fieldUser.SetDocumentation("Set");
                Schema schema = sb1.Finish();
                Entity entity = new Entity(schema);

                Field field = schema.GetField(name_para);
                entity.Set(field, value);
                material.SetEntity(entity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public Color Choose_Color(Button button_color)
        {
            Color color = null;
            try
            {
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    color = new Autodesk.Revit.DB.Color(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                    button_color.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return color;
        }

        //----------------------------------------------------------
        public FillPatternElement Choose_Fill_Pattern(Document doc,string name)
        {
            FillPatternElement fillPattern = null;
            try
            {
                if (name != "None")
                {
                    var fillPatternElements = new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).ToElements();
                    foreach (FillPatternElement fillPatternElement in fillPatternElements)
                    {
                        if (fillPatternElement.GetFillPattern().Name == name)
                        {
                            fillPattern = fillPatternElement;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fillPattern;
        }

        //----------------------------------------------------------
        public void SetColorAppearanceAsset(Document doc, Material material)
        {
            try
            {
                var list = new FilteredElementCollector(doc).OfClass(typeof(AppearanceAssetElement)).ToList();
                List<string> name = new List<string>();
                foreach (AppearanceAssetElement e in list)
                {
                    name.Add(e.Name);
                }
                string newName = "";
                for (var i = 0; i < 10000000; i++)
                {
                    if (name.Any(x => x == material.Name + "(" + i.ToString() + ")") == false)
                    {
                        newName = material.Name + "(" + i.ToString() + ")";
                        break;
                    }
                }

                AppearanceAssetElement assetElem = list[0] as AppearanceAssetElement;

                AppearanceAssetElement assetElemNew = assetElem.Duplicate(newName);

                AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(assetElemNew.Document);
                Asset editableAsset = editScope.Start(assetElemNew.Id);
                AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset["generic_diffuse"] as AssetPropertyDoubleArray4d;
                genericDiffuseProperty.SetValueAsColor(material.Color);
                editScope.Commit(true);
                material.AppearanceAssetId = assetElemNew.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public string RemoveUnicode(string text)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = text.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(text[index]);
                text = text.Replace(text[index], ReplText[index2]);
            }
            return text;
        }

        //----------------------------------------------------------
        public List<Material_Excel> Get_Excel(ObservableCollection<list_ten_du_lieu_excel> list_position_excel, string path)
        {
            List<Material_Excel> list_set_data = new List<Material_Excel>();
            try
            {
                mySource = new ListSource();
                Excel.Application excel = new Excel.Application();
                excel.Visible = false;
                Excel.Workbook workbook = excel.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                Excel.Range range = sheet.UsedRange;

                List<int> index_start = new List<int>();
                foreach (list_ten_du_lieu_excel position in list_position_excel)
                {
                    index_start.Add(Convert.ToInt32(position.excel_ngang));
                }

                int iRow = range.Rows.Count;
                for(int i = 0; i < iRow; i++)
                {
                    string range_MCT = list_position_excel[0].excel_dung + (index_start[0] + i).ToString();
                    string range_TVL = list_position_excel[1].excel_dung + (index_start[1] + i).ToString();
                    string range_DV = list_position_excel[2].excel_dung + (index_start[2] + i).ToString();
                    string range_MVL = list_position_excel[3].excel_dung + (index_start[3] + i).ToString();
                    string range_DTS = list_position_excel[4].excel_dung + (index_start[4] + i).ToString();
                    string range_MSF = list_position_excel[5].excel_dung + (index_start[5] + i).ToString();
                    string range_TSF = list_position_excel[6].excel_dung + (index_start[6] + i).ToString();
                    string range_MC = list_position_excel[7].excel_dung + (index_start[7] + i).ToString();
                    string range_TC = list_position_excel[8].excel_dung + (index_start[8] + i).ToString();
                    //string range_M3T = list_position_excel[9].phuong_dung + (index_start[9] + i).ToString();
                    //string range_USER = list_position_excel[10].phuong_dung + (index_start[10] + i).ToString();
                    //string range_TIME = list_position_excel[11].phuong_dung + (index_start[11] + i).ToString();

                    if(!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_MCT, range_MCT).Value)) &&
                        !string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_TVL, range_TVL).Value)) &&
                        !string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_DV, range_DV).Value)) &&
                        Convert.ToString(sheet.get_Range(range_MCT, range_MCT).Value) != mySource.error)
                    {
                        Color1 color_MVL = Color1.FromArgb(System.Convert.ToInt32(sheet.get_Range(range_MVL, range_MVL).Interior.Color));
                        Color1 color_MSF = Color1.FromArgb(System.Convert.ToInt32(sheet.get_Range(range_MSF, range_MSF).Interior.Color));
                        Color1 color_MC = Color1.FromArgb(System.Convert.ToInt32(sheet.get_Range(range_MC, range_MC).Interior.Color));
                        int do_trong = 0;
                        if (!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_DTS, range_DTS).Value)) && System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(sheet.get_Range(range_DTS, range_DTS).Value), "[^0-9]") == false)
                            do_trong = Convert.ToInt32(sheet.get_Range(range_DTS, range_DTS).Value);
                        string name_surface = "None";
                        if (!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_TSF, range_TSF).Value))) name_surface = Convert.ToString(sheet.get_Range(range_TSF, range_TSF).Value);
                        string name_cut = "None";
                        if (!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_TC, range_TC).Value))) name_cut = Convert.ToString(sheet.get_Range(range_TC, range_TC).Value);

                        list_set_data.Add(new Material_Excel()
                        {
                            ma_cong_tac_excel = Convert.ToString(sheet.get_Range(range_MCT, range_MCT).Value),
                            ten_vat_lieu_excel = Convert.ToString(sheet.get_Range(range_TVL, range_TVL).Value),
                            don_vi_excel = Convert.ToString(sheet.get_Range(range_DV, range_DV).Value),
                            mau_vat_lieu_excel = new Color(color_MVL.B, color_MVL.G, color_MVL.R),
                            do_trong_suot_vat_lieu_excel = do_trong,
                            mau_surface_excel = new Color(color_MSF.B, color_MSF.G, color_MSF.R),
                            name_surface_excel = name_surface,
                            mau_cut_excel = new Color(color_MC.B, color_MC.G, color_MC.R),
                            name_cut_excel = name_cut
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
            return list_set_data;
        }

        //----------------------------------------------------------
        public void Format_Range_Excel(Excel.Worksheet sheet, string range, string value, Color1 color)
        {
            try
            {
                sheet.get_Range(range, range).Value = value;
                sheet.get_Range(range, range).VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                sheet.get_Range(range, range).HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                sheet.get_Range(range, range).Interior.Color = color;
                sheet.get_Range(range, range).Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.get_Range(range, range).Borders.Weight = Excel.XlBorderWeight.xlThin;
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Export_Excel(ObservableCollection<list_ten_du_lieu_excel> list_position_excel, List<Material_Project> list_data_export, string path, string name_sheet, Document doc)
        {
            try
            {
                mySource = new ListSource();
                Excel.Application excel = new Excel.Application();
                excel.Visible = true;
                Excel.Workbook workbook = excel.Workbooks.Add(Missing.Value);
                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                sheet.Name = name_sheet;
                int x = 0;
                List<int> index_start = new List<int>();
                foreach(list_ten_du_lieu_excel position in list_position_excel)
                {
                    string range = position.excel_dung + "1";
                    string value = position.excel_name;
                    Format_Range_Excel(sheet, range, value, Color1.Yellow);
                    sheet.get_Range(range, range).Font.Bold = true;

                    if (x == 0 || x == 6 || x== 8 || x == 10 || x == 11) { sheet.get_Range(range, range).ColumnWidth = 20; }
                    else if (x == 1) { sheet.get_Range(range, range).ColumnWidth = 100; }
                    else if (x == 2) { sheet.get_Range(range, range).ColumnWidth = 15; }
                    else if (x == 3 || x == 4 || x == 5 || x == 7 || x == 9) { sheet.get_Range(range, range).Columns.AutoFit(); }
                    x++;

                    index_start.Add(Convert.ToInt32(position.excel_ngang));
                }

                int i = 0;
                foreach (Material_Project data in list_data_export)
                {
                    string range_MCT    = list_position_excel[0].excel_dung + (index_start[0]   + i).ToString();
                    string range_TVL    = list_position_excel[1].excel_dung + (index_start[1]   + i).ToString();
                    string range_DV     = list_position_excel[2].excel_dung + (index_start[2]   + i).ToString();
                    string range_MVL    = list_position_excel[3].excel_dung + (index_start[3]   + i).ToString();
                    string range_DTS    = list_position_excel[4].excel_dung + (index_start[4]   + i).ToString();
                    string range_MSF    = list_position_excel[5].excel_dung + (index_start[5]   + i).ToString();
                    string range_TSF    = list_position_excel[6].excel_dung + (index_start[6]   + i).ToString();
                    string range_MC     = list_position_excel[7].excel_dung + (index_start[7]   + i).ToString();
                    string range_TC     = list_position_excel[8].excel_dung + (index_start[8]   + i).ToString();
                    string range_M3T    = list_position_excel[9].excel_dung + (index_start[9]   + i).ToString();
                    string range_USER   = list_position_excel[10].excel_dung + (index_start[10]  + i).ToString();
                    string range_TIME   = list_position_excel[11].excel_dung + (index_start[11]  + i).ToString();

                    string fillPattern_Surface_name = "None";
                    if (data.id_surface.IntegerValue != -1)
                    {
                        FillPatternElement fillPattern_Surface_Check = doc.GetElement(data.id_surface) as FillPatternElement;
                        fillPattern_Surface_name = fillPattern_Surface_Check.GetFillPattern().Name;
                    }

                    string fillPattern_Cut_name = "None";
                    if (data.id_cut.IntegerValue != -1)
                    {
                        FillPatternElement fillPattern_Cut_Check = doc.GetElement(data.id_cut) as FillPatternElement;
                        fillPattern_Cut_name = fillPattern_Cut_Check.GetFillPattern().Name;
                    }

                    try
                    {
                        Format_Range_Excel(sheet, range_MCT, data.ma_cong_tac_project, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TVL, data.ten_vat_lieu_project, Color1.Transparent);
                        Format_Range_Excel(sheet, range_DV, data.don_vi_project, Color1.Transparent);
                        Format_Range_Excel(sheet, range_MVL, "", Color1.FromArgb(data.mau_vat_lieu.Red, data.mau_vat_lieu.Green, data.mau_vat_lieu.Blue));
                        Format_Range_Excel(sheet, range_DTS, data.do_trong_suot_vat_lieu.ToString(), Color1.Transparent);
                        Format_Range_Excel(sheet, range_MSF, "", Color1.FromArgb(data.mau_surface.Red, data.mau_surface.Green, data.mau_surface.Blue));
                        Format_Range_Excel(sheet, range_TSF, fillPattern_Surface_name, Color1.Transparent);
                        Format_Range_Excel(sheet, range_MC, "", Color1.FromArgb(data.mau_cut.Red, data.mau_cut.Green, data.mau_cut.Blue));
                        Format_Range_Excel(sheet, range_TC, fillPattern_Cut_name, Color1.Transparent);
                        Format_Range_Excel(sheet, range_M3T, data.ton, Color1.Transparent);
                        Format_Range_Excel(sheet, range_USER, data.user, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TIME, data.time, Color1.Transparent);
                    }
                    catch (Exception)
                    {
                        Format_Range_Excel(sheet, range_MCT, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TVL, data.ten_vat_lieu_project, Color1.Transparent);
                        Format_Range_Excel(sheet, range_DV, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_MVL, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_DTS, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_MSF, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TSF, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_MC, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TC, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_M3T, mySource.error, Color1.Transparent);
                        Format_Range_Excel(sheet, range_USER, data.user, Color1.Transparent);
                        Format_Range_Excel(sheet, range_TIME, data.time, Color1.Transparent);
                    }
                    sheet.get_Range(range_MCT, range_MCT).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    sheet.get_Range(range_TVL, range_TVL).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    sheet.get_Range(range_MCT, range_MCT).IndentLevel = 2;
                    sheet.get_Range(range_TVL, range_TVL).IndentLevel = 2;

                    i++;
                }

                workbook.SaveAs(path, Excel.XlFileFormat.xlWorkbookDefault);
                workbook.Close(true, null, null);
                excel.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public Brush Check_Color(string data_check, All_Data myAll_Data)
        {
            Brush color = myAll_Data.list_color_UI_data[0];
            try
            {
                mySource = new ListSource();
                var key_company = data_check.Split('.');
                if (string.IsNullOrEmpty(data_check) || (key_company.Count() > 0 && myAll_Data.list_mct_descipline_data.Any(x => x.mct == key_company[0]) == false))
                {
                    color = myAll_Data.list_color_UI_data[1];
                }
            }
            catch (Exception)
            {

            }
            return color;
        }
    }
}
