using ARC_Quatity.Data;
using ARC_Quatity.Data.Binding;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Document = Autodesk.Revit.DB.Document;

namespace ARC_Quatity.Code.Function
{
    class F_UploadData
    {
        //----------------------------------------------------------
        public static string Upload(UIApplication uiapp, Document doc, ListView thong_tin_quantity_total_project, ListView thong_tin_quantity_project, List<string> material_of_element_in_project, string id_file, string user)
        {
            string result = "F";
            try
            {
                if (material_of_element_in_project.Count() > 0)
                {
                    List<string> color_check = new List<string>();
                    if (thong_tin_quantity_total_project.Items.Count > 0) color_check = CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Cast<data_quantity>().Select(x => x.color_sort).ToList();

                    if (color_check.Distinct().ToList().Count() == 1 || thong_tin_quantity_total_project.Items.Count == 0)
                    {
                        List<List<string>> values_material = new List<List<string>>();
                        List<List<string>> values_element = new List<List<string>>();

                        List<data_quantity> data_material = new List<data_quantity>();
                        List<data_quantity> data_element = new List<data_quantity>();
                        List<data_quantity> data_input = new List<data_quantity>();
                        if (thong_tin_quantity_total_project.Items.Count > 0) data_material = CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Cast<data_quantity>().ToList();
                        if (thong_tin_quantity_project.Items.Count > 0) data_element = CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Cast<data_quantity>().ToList();

                        data_material.ForEach(x =>
                        values_material.Add(new List<string>()
                        {
                        id_file,
                        x.block,
                        x.level,
                        x.ten_vat_lieu,
                        x.ma_cong_tac,
                        x.quantity.ToString(),
                        x.don_vi,
                        user,
                        DateTime.Now.ToString(),
                        ""
                        }));

                        data_element.ForEach(x =>
                        values_element.Add(new List<string>()
                        {
                            id_file,
                            x.block,
                            x.level,
                            x.id_cau_kien,
                            x.ten_cau_kien,
                            x.ten_vat_lieu,
                            x.ma_cong_tac,
                            x.quantity.ToString(),
                            x.don_vi,
                            user,
                            DateTime.Now.ToString()
                        }));

                        List<List<List<string>>> values = new List<List<List<string>>>() { values_material, values_element };
                        result = F_Upload.Upload(doc, values, material_of_element_in_project, id_file);
                    }
                    else
                    {
                        MessageBox.Show("Missing data. Upload fail!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Not data. Upload fail!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
