using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Tool_ViewInformation.Data.Binding;

namespace Tool_ViewInformation.Code.Function
{
    class F_GetQuantity
    {
        //----------------------------------------------------------
        public static void Support_Get_Khoi_Luong(Document doc, Element element, List<Level> levels, string Class, string block, ObservableCollection<data_quantity> my_quatity_item)
        {
            try
            {
                List<data_paint> materials = new List<data_paint>();

                F_GetMaterial.Get_All_Material_Of_Element_By_Category(doc, element, materials);
                F_GetMaterial.Get_All_Material_Of_Element(doc, materials, element.GetMaterialIds(false), element.GetMaterialIds(true));

                if (materials.Count() > 0)
                {
                    foreach (data_paint material in materials)
                    {
                        string don_vi = F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.DV[1], Source.DV[0]);
                        if (don_vi != "Default")
                        {
                            string id_cau_kien = element.LookupParameter(Source.share_para_text2).AsString() != null ?
                            element.LookupParameter(Source.share_para_text2).AsString() :
                            element.UniqueId;

                            string ma_cong_tac = F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.MCT[1], Source.MCT[0]);

                            double ton = Convert.ToDouble(F_GetSchema.Check_Para_And_Get_Para(material.vat_lieu, Source.TON[1], Source.TON[0]));

                            double quantity = Get_Quantity(don_vi, ton, element, material);

                            if (quantity != 0)
                            {
                                my_quatity_item.Add(new data_quantity()
                                {
                                    ten_vat_lieu = material.vat_lieu.Name,
                                    ma_cong_tac = ma_cong_tac,
                                    quantity = quantity,
                                    don_vi = don_vi,

                                    id_cau_kien = id_cau_kien,
                                    cau_kien = element,
                                    vat_lieu = material,
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public static double Get_Quantity(string don_vi, double ton, Element element, data_paint material)
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
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
            return quantity;
        }
    }
}
