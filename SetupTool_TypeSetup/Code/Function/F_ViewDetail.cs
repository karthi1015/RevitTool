using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_ViewDetail
    {//----------------------------------------------------------
        public static void view_detail(Document doc,data_type type, ObservableCollection<data_parameters> my_parameters, ObservableCollection<data_materials> my_materials,
            ObservableCollection<data_material> my_material, ListView thong_tin_kich_thuoc, ListView thong_tin_cong_tac_vat_lieu, string unit_length)
        {
            try
            {
                var elements = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                my_material.Add(new data_material()
                {
                    name = "<By Category>",
                    vat_lieu = null
                });
                foreach (Material material in elements)
                {
                    my_material.Add(new data_material()
                    {
                        name = material.Name,
                        vat_lieu = material
                    });
                }
                var a = my_material.OrderBy(x => x.name).ToList();
                my_material.Clear();
                a.ForEach(x => my_material.Add(x));

                if (type.type_type == Source.type_symbol)
                {
                    F_ParameterSymbol.parameter_symbol(doc, type.element_type, thong_tin_kich_thuoc, my_parameters, unit_length);
                    F_MaterialSymbol.material_symbol(doc, type.element_type, thong_tin_cong_tac_vat_lieu, my_materials, my_material);
                }
                else
                {
                    F_ParameterAndMaterialType.parameter_and_material_type(doc, type.element_type, thong_tin_kich_thuoc, my_parameters,thong_tin_cong_tac_vat_lieu, my_materials, my_material, unit_length);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
