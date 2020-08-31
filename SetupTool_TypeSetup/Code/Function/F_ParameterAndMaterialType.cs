using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_ParameterAndMaterialType
    {//----------------------------------------------------------
        public static void parameter_and_material_type(Document doc, ElementType type, ListView thong_tin_kich_thuoc, ObservableCollection<data_parameters> my_parameters,
            ListView thong_tin_cong_tac_vat_lieu, ObservableCollection<data_materials> my_materials, ObservableCollection<data_material> my_material, string unit_length)
        {
            try
            {
                var TypeOftype = type.GetType();
                CompoundStructure compound = null;
                if (TypeOftype.Name == "WallType")
                {
                    var wall = type as WallType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == "FloorType")
                {
                    var wall = type as FloorType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == "RoofType")
                {
                    var wall = type as RoofType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == "CeilingType")
                {
                    var wall = type as CeilingType;
                    compound = wall.GetCompoundStructure();
                }

                IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                foreach (var layer in getlayer)
                {
                    my_parameters.Add(new data_parameters()
                    {
                        ten_parameter = layer.Function.ToString(),
                        gia_tri_parameter = Math.Round(layer.Width * Source.units_document.First(x => x.name == unit_length).value, 0).ToString(),
                        group_parameter = "Dimensions",
                        layer = layer,
                        compound = compound
                    });
                }

                thong_tin_kich_thuoc.ItemsSource = my_parameters;

                ListCollectionView view_kich_thuoc = CollectionViewSource.GetDefaultView(thong_tin_kich_thuoc.ItemsSource) as ListCollectionView;
                PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("group_parameter");
                view_kich_thuoc.GroupDescriptions.Add(groupDescription1);
                view_kich_thuoc.CustomSort = new sort_data_parameters();
                //---------------------------------------------------------------------------------------------------------------------
                foreach (var layer in getlayer)
                {
                    string ten = "<By Category>";
                    string ma = "";
                    if (layer.MaterialId.IntegerValue != -1)
                    {
                        ten = doc.GetElement(layer.MaterialId).Name;
                        ma = F_GetSchema.Check_Para_And_Get_Para(doc.GetElement(layer.MaterialId) as Material, Source.MCT[1], Source.MCT[0]);
                    }
                    my_materials.Add(new data_materials()
                    {
                        ten_cong_tac = layer.Function.ToString(),
                        ten_vat_lieu_list = my_material,
                        ten_vat_lieu = my_material.First(x => x.name == ten),
                        layer = layer,
                        compound = compound
                    });
                }
                thong_tin_cong_tac_vat_lieu.ItemsSource = my_materials;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_cong_tac_vat_lieu.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("ten_cong_tac", ListSortDirection.Ascending));
            }
            catch (Exception)
            {
                thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<data_parameters>();
                thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<data_materials>();
            }
        }
    }
}
