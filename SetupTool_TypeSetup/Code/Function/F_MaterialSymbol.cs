using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_MaterialSymbol
    {
        //----------------------------------------------------------
        public static void material_symbol(Document doc, ElementType type, ListView thong_tin_cong_tac_vat_lieu, ObservableCollection<data_materials> my_materials,
            ObservableCollection<data_material> my_material)
        {
            try
            {
                List<Parameter> materialDefinition = new List<Parameter>();
                List<string> materialDefinitionName = new List<string>();
                foreach (Parameter para in type.Parameters)
                {
                    if (para.Definition.ParameterGroup ==  BuiltInParameterGroup.PG_MATERIALS)
                    {
                        string ten = "<By Category>";
                        string ma = "";
                        if (para.AsElementId().IntegerValue != -1)
                        {
                            ten = doc.GetElement(para.AsElementId()).Name;
                            ma = F_GetSchema.Check_Para_And_Get_Para(doc.GetElement(para.AsElementId()) as Material, Source.MCT[1], Source.MCT[0]);
                        }
                        my_materials.Add(new data_materials()
                        {
                            ten_cong_tac = para.Definition.Name,
                            ten_vat_lieu_list = my_material,
                            ten_vat_lieu = my_material.First(x => x.name == ten),
                            parameter = para
                        });
                    }
                }
                thong_tin_cong_tac_vat_lieu.ItemsSource = my_materials;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_cong_tac_vat_lieu.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_materials();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
