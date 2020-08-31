using Autodesk.Revit.DB;
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
using ARC_Quatity.Data.Binding;
using ARC_Quatity.Data;

namespace ARC_Quatity.Code.Function
{
    class F_GetQuantity
    {
        //----------------------------------------------------------
        public static void Support_Get_Khoi_Luong(Document doc, Element element, List<Level> levels, string Class, string unit_length, string block, ObservableCollection<data_quantity> my_quatity_item)
        {
            try
            {
                List<data_paint> materials = new List<data_paint>();

                F_GetMaterial.Get_All_Material_Of_Element_By_Category(doc, element, materials);
                F_GetMaterial.Get_All_Material_Of_Element(doc, materials, element.GetMaterialIds(false), element.GetMaterialIds(true));

                if (materials.Count() > 0)
                {
                    string name = "";
                    if (element.LookupParameter(Source.share_para_text1).AsString() != null) name = element.LookupParameter(Source.share_para_text1).AsString();

                    string id_cau_kien = "";
                    if (element.LookupParameter(Source.share_para_text2).AsString() != null) id_cau_kien = element.LookupParameter(Source.share_para_text2).AsString();

                    string level = "";
                    if (element.LookupParameter(Source.share_para_text3).AsString() != null) level = element.LookupParameter(Source.share_para_text3).AsString();

                    foreach (data_paint material in materials)
                    {
                        string don_vi = F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.DV[1], Source.DV[0]);
                        if (don_vi != "Default")
                        {
                            string ma_cong_tac = F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.MCT[1], Source.MCT[0]);

                            if ((ma_cong_tac.Split('.').Count() > 0 && ma_cong_tac.Split('.')[0] == Source.desciplines.First(x => x.name == Class).key) || ma_cong_tac == "")
                            {
                                double ton = Convert.ToDouble(F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.TON[1], Source.TON[0]));

                                double quantity = F_GetQuantity.Get_Quantity(don_vi, ton, element, material, unit_length);

                                Brush color = Source.color;
                                if (string.IsNullOrEmpty(ma_cong_tac) || string.IsNullOrEmpty(level) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id_cau_kien) || quantity <= 0) color = Source.color_error;

                                double elevation = 10000000000;
                                if (levels.Any(x => x.Name == level) == true) elevation = levels.First(x => x.Name == level).Elevation;

                                if (quantity != 0)
                                {
                                    my_quatity_item.Add(new data_quantity()
                                    {
                                        block = block,
                                        level = level,
                                        ten_cau_kien = name,
                                        id_cau_kien = id_cau_kien,
                                        ten_vat_lieu = material.vat_lieu.Name,
                                        ma_cong_tac = ma_cong_tac,
                                        quantity = quantity,
                                        don_vi = don_vi,
                                        color = color,
                                        color_sort = color.ToString(),

                                        cau_kien = element,
                                        vat_lieu = material,
                                        elevation = elevation,
                                        link_file_name = doc.Title
                                    });
                                }
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
        public static double Get_Quantity(string don_vi, double ton, Element element, data_paint material, string unit_length)
        {
            double quantity = -1;
            try
            {
                foreach (data_unit data in Source.units)
                {
                    if (don_vi == data.name)
                    {
                        if (don_vi.Contains("m3"))
                        {
                            quantity = element.GetMaterialVolume(material.vat_lieu.Id) * data.value;
                            break;
                        }
                        else if (don_vi.Contains("m2"))
                        {
                            quantity = quantity = element.GetMaterialArea(material.vat_lieu.Id, material.is_paint) * data.value;
                            break;
                        }
                        else if (don_vi.Contains("md"))
                        {
                            if (element.LookupParameter("Chiều dài") != null && element.LookupParameter("Chiều dài").AsValueString() != null)
                            {
                                var valuecd = element.LookupParameter("Chiều dài").AsDouble();
                                quantity = valuecd / Source.units_convert.First(x => x.name == "md").value;
                            }
                            break;
                        }
                        else if (don_vi.Contains("m"))
                        {
                            if (element.LookupParameter("Length") != null && element.LookupParameter("Length").AsValueString() != null)
                            {
                                var valuecd = element.LookupParameter("Length").AsDouble();
                                quantity = valuecd / Source.units_convert.First(x => x.name == "m").value;
                            }
                            break;
                        }
                        else if (don_vi.Contains("tấn") || don_vi.Contains("ton"))
                        {
                            quantity = element.GetMaterialVolume(material.vat_lieu.Id) * data.value * ton;
                            break;
                        }
                        else if (don_vi.Contains("kg"))
                        {
                            quantity = element.GetMaterialVolume(material.vat_lieu.Id) * ton * data.value;
                            break;
                        }
                        else
                        {
                            quantity = data.value;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return quantity;
        }
    }
}
