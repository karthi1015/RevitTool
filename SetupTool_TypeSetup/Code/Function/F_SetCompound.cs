using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_SetCompound
    {
        //----------------------------------------------------------
        public static CompoundStructure set_compound_structure(CompoundStructure compound, ObservableCollection<data_materials> mydata_materials, ObservableCollection<data_parameters> mydata_parameters, string unit_length)
        {
            CompoundStructure compound_new = compound;
            try
            {
                IList<CompoundStructureLayer> cslayers = compound_new.GetLayers();

                for (int i = 0; i < cslayers.Count; i++)
                {
                    compound_new.SetLayerWidth(mydata_parameters[i].layer.LayerId, Convert.ToDouble(mydata_parameters[i].gia_tri_parameter) / Source.units_document.First(x => x.name == unit_length).value);
                    if (mydata_materials[i].ten_vat_lieu.name != "<By Category>")
                    {
                        compound_new.SetMaterialId(mydata_materials[i].layer.LayerId, mydata_materials[i].ten_vat_lieu.vat_lieu.Id);
                    }
                    else
                    {
                        compound_new.SetMaterialId(mydata_materials[i].layer.LayerId, new ElementId(-1));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return compound_new;
        }
    }
}
