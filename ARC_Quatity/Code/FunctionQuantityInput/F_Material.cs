using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ARC_Quatity.Code.Function;
using ARC_Quatity.Data;
using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;

namespace ARC_Quatity.Code.FunctionQuantityInput
{
    class F_Material
    {
        public static void get_material_combobox(Document doc, ComboBox input_material)
        {
            try
            {
                List<data_material> my_material = new List<data_material>();
                foreach (Material material in new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().ToList())
                {
                    string id = F_GetSchema.Check_Para_And_Get_Para(material, Source.MCT[1], Source.MCT[0]);
                    string unit = F_GetSchema.Check_Para_And_Get_Para(material, Source.DV[1], Source.DV[0]);
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(unit))
                    {
                        my_material.Add(new data_material()
                        {
                            material = material,
                            name = material.Name,
                            id = id,
                            unit = unit
                        });
                    }
                }
                input_material.ItemsSource = my_material;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(input_material.ItemsSource);
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("name", System.ComponentModel.ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
