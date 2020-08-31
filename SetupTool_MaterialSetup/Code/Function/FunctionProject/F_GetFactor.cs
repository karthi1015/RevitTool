using SetupTool_MaterialSetup.Code.Function;
using Autodesk.Revit.DB;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_GetFactor
    {
        public static void get_material_factor(Document doc, ObservableCollection<data_material_factor> my_material_factor, ListView thong_tin_he_so_vat_lieu_project)
        {
            try
            {
                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                List<data_material_factor> support = new List<data_material_factor>();

                foreach (Material material in list_material)
                {
                    support.Add(new data_material_factor()
                    {
                        vat_lieu = material,
                        ton = F_GetSchema.Check_Para_And_Get_Para(material, Source.TON[1], Source.TON[0]),
                    });
                }

                my_material_factor = new ObservableCollection<data_material_factor>(support
                    .GroupBy(x => new
                    {
                        x.ton
                    }).Select(y => new data_material_factor()
                    {
                        ton = y.Key.ton,
                        list_vat_lieu = y.Select(z => z.vat_lieu).ToList(),
                        count = y.Select(z => z.vat_lieu).ToList().Count()
                    }));

                thong_tin_he_so_vat_lieu_project.ItemsSource = my_material_factor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
