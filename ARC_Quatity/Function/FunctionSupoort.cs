﻿using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using Excel = Microsoft.Office.Interop.Excel;

namespace WEB_SaveAs
{
    public class FunctionSupoort
    {
        ListSource mySource;
        FunctionSQL mySQL;
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
        public void Get_All_Material_Of_Element_By_Category(Document doc, Element element, List<Material_Paint_Or_NoPaint> materials)
        {
            try
            {
                mySource = new ListSource();
                if (element.Category.Name == "Railings")
                {
                    RailingType type = doc.GetElement(element.GetTypeId()) as RailingType;
                    if (type.RailStructure.GetNonContinuousRailCount() > 0)
                    {
                        for (var i = 0; i < type.RailStructure.GetNonContinuousRailCount(); i++)
                        {
                            var idRailing = type.RailStructure.GetNonContinuousRail(i).MaterialId;
                            Material m = doc.GetElement(idRailing) as Material;
                            if (m != null)
                            {
                                materials.Add(new Material_Paint_Or_NoPaint()
                                {
                                    vat_lieu = m,
                                    paint_or_nopaint = mySource.no_paint
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Get_All_Material_Of_Element(Document doc, ICollection<ElementId> IdMaterial, ICollection<ElementId> IdMaterialPaint, List<Material_Paint_Or_NoPaint> materials)
        {
            try
            {
                mySource = new ListSource();
                // add material element
                foreach (ElementId id in IdMaterial)
                {
                    Material m = doc.GetElement(id) as Material;
                    if (m != null)
                    {
                        materials.Add(new Material_Paint_Or_NoPaint()
                        {
                            vat_lieu = m,
                            paint_or_nopaint = mySource.no_paint
                        });
                    }
                }
                foreach (ElementId id in IdMaterialPaint)
                {
                    Material m = doc.GetElement(id) as Material;
                    if (m != null)
                    {
                        materials.Add(new Material_Paint_Or_NoPaint()
                        {
                            vat_lieu = m,
                            paint_or_nopaint = mySource.paint
                        });
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public double Get_Quantity(Document doc, string don_vi, double ton, Element element, Material_Paint_Or_NoPaint material, All_Data myAll_Data, List<Material> list_materials)
        {
            double quantity = 0;
            try
            {
                double factor = 1;
                if (don_vi.Contains("m3"))
                {
                    if (!string.IsNullOrEmpty(don_vi.Replace("m3", ""))) factor = Convert.ToDouble(don_vi.Replace("m3", ""));
                    quantity = element.GetMaterialVolume(material.vat_lieu.Id) * myAll_Data.list_unit_value_data[4] / factor;
                }

                else if (don_vi.Contains("m2"))
                {
                    if (!string.IsNullOrEmpty(don_vi.Replace("m2", ""))) factor = Convert.ToDouble(don_vi.Replace("m2", ""));
                    if (element.Category.Name == "Site")
                    {
                        Element materialSub = list_materials.Find(x => x.Name == "sub");
                        double valueSub = element.GetMaterialArea(materialSub.Id, false) * myAll_Data.list_unit_value_data[3];
                        quantity = (element.GetMaterialArea(material.vat_lieu.Id, material.paint_or_nopaint) * myAll_Data.list_unit_value_data[3] - valueSub) / factor;
                    }
                    else
                    {
                        quantity = element.GetMaterialArea(material.vat_lieu.Id, material.paint_or_nopaint) * myAll_Data.list_unit_value_data[3] / factor;
                    }
                }
                else if (don_vi.Contains("md") && don_vi.Contains("m2") == false && don_vi.Contains("m3") == false)
                {
                    if (element.LookupParameter("Chiều dài") != null && element.LookupParameter("Chiều dài").AsValueString() != null)
                    {
                        var valuecd = element.LookupParameter("Chiều dài").AsValueString();
                        quantity = Convert.ToDouble(valuecd) / 1000;
                    }
                }
                else if (don_vi.Contains("m") && don_vi.Contains("m2") == false && don_vi.Contains("m3") == false && don_vi.Contains("md") == false)
                {
                    if (element.LookupParameter("Length") != null && element.LookupParameter("Length").AsValueString() != null)
                    {
                        var valuecd = element.LookupParameter("Length").AsValueString();
                        quantity = Convert.ToDouble(valuecd) / 1000;
                    }
                }
                else if (don_vi.Contains("tấn"))
                {
                    if (!string.IsNullOrEmpty(don_vi.Replace("tấn", ""))) factor = Convert.ToDouble(don_vi.Replace("tấn", ""));
                    quantity = element.GetMaterialVolume(material.vat_lieu.Id) * myAll_Data.list_unit_value_data[4] * ton / factor;
                }
                else if (don_vi == "kg")
                {
                    if (!string.IsNullOrEmpty(don_vi.Replace("kg", ""))) factor = Convert.ToDouble(don_vi.Replace("kg", ""));
                    quantity = element.GetMaterialVolume(material.vat_lieu.Id) * myAll_Data.list_unit_value_data[4] * ton * 1000 / factor;
                }
                else if (don_vi == "cái")
                {
                    quantity = 1;
                }
                else if (string.IsNullOrEmpty(don_vi))
                {
                    quantity = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return quantity;
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
        public string Upload(Document doc, List<string> procedure_name, List<List<List<string>>> values, All_Data myAll_Data, string path)
        {
            string result = "F";
            try
            {
                mySQL = new FunctionSQL();
                mySource = new ListSource();
                List<List<string>> value_material = new List<List<string>>();

                DataTable dt = new DataTable();
                for (int i = 0; i < values[0][0].Count(); i++)
                {
                    dt.Columns.Add(new DataColumn(i.ToString(), typeof(string)));

                }
                foreach(string key in Get_Excel(path, myAll_Data))
                {
                    List<string> support = new List<string>();
                    for (int i = 0; i < values[0][0].Count(); i++)
                    {
                        if (i == 3) support.Add(key);
                        else support.Add("-1");
                    }
                    value_material.Add(support);
                }
                foreach (List<string> value in value_material)
                {
                    dt.Rows.Add(dt.NewRow().ItemArray = value.ToArray());
                }

                DataTable dt1 = new DataTable();
                for (int i = 0; i < values[0][0].Count(); i++)
                {
                    dt1.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                }
                foreach (List<string> value in values[0])
                {
                    dt1.Rows.Add(dt1.NewRow().ItemArray = value.ToArray());
                }

                DataTable dt2 = new DataTable();
                for (int i = 0; i < values[1][0].Count(); i++)
                {
                    dt2.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                }
                foreach (List<string> value in values[1])
                {
                    dt2.Rows.Add(dt2.NewRow().ItemArray = value.ToArray());
                }

                List<string> Para1 = new List<string>() { myAll_Data.list_procedure_data[1].procedure_para.Split('|')[0], myAll_Data.list_procedure_data[1].procedure_para.Split('|')[1],
                    myAll_Data.list_procedure_data[1].procedure_para.Split('|')[2], myAll_Data.list_procedure_data[1].procedure_para.Split('|')[3] };
                List<object> Para1_Values = new List<object>() { doc.ProjectInformation.Number, dt, dt1, dt2 };
                var result_sql_delete = mySQL.SQLDelete(myAll_Data.list_path_connect_SQL_data[5], procedure_name[1], mySource.type_Procedure, Para1, Para1_Values);

                List<string> Para2 = new List<string>() { myAll_Data.list_procedure_data[0].procedure_para.Split('|')[0], myAll_Data.list_procedure_data[0].procedure_para.Split('|')[1],
                myAll_Data.list_procedure_data[0].procedure_para.Split('|')[2]};
                List<object> Para2_Values = new List<object>() { doc.ProjectInformation.Number, dt1, dt2 };
                var result_sql_upload = mySQL.SQLWrite(myAll_Data.list_path_connect_SQL_data[5], procedure_name[0], mySource.type_Procedure, Para2, Para2_Values);

                if (result_sql_delete == "S" && result_sql_upload == "S")
                {
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public List<string> Get_Excel(string path, All_Data myAll_Data)
        {
            List<string> list_get_data = new List<string>();
            try
            {
                mySource = new ListSource();
                Excel.Application excel = new Excel.Application();
                excel.Visible = false;
                Excel.Workbook workbook = excel.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                Excel.Worksheet sheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                Excel.Range range = sheet.UsedRange;               

                int iRow = range.Rows.Count;
                for (int i = 0; i < iRow; i++)
                {
                    string range_row = "A" + (1 + i).ToString();                    

                    if (!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_row, range_row).Value)))
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(sheet.get_Range(range_row, range_row).Value))) list_get_data.Add(Convert.ToString(sheet.get_Range(range_row, range_row).Value));
                    }
                }

                workbook.Close(true, null, null);
                excel.Quit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list_get_data;
        }
    }
}