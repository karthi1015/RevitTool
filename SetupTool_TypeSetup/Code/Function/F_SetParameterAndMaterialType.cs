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
    class F_SetParameterAndMaterialType
    {
        public static ElementType set_elementtype(CompoundStructure compound, ObservableCollection<data_materials> mydata_materials, ObservableCollection<data_parameters> mydata_parameters, 
            ElementType elementType, string CreateOrUpdate, string name_type_new, string unit_length)
        {
            ElementType elementType_new = null;
            try
            {
                var TypeOftype = elementType.GetType();
                if (CreateOrUpdate == Source.Update)
                {
                    F_SetCompound.set_compound_structure(compound, mydata_materials, mydata_parameters, unit_length);

                    if (elementType.GetType().Name == "WallType")
                    {
                        var wall_Type = elementType as WallType;
                        wall_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == "FloorType")
                    {
                        var floor_Type = elementType as FloorType;
                        floor_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == "RoofType")
                    {
                        var roof_Type = elementType as RoofType;
                        roof_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == "CeilingType")
                    {
                        var ceiling_Type = elementType as CeilingType;
                        ceiling_Type.SetCompoundStructure(compound);
                    }
                }
                else if (CreateOrUpdate == Source.Duplicate)
                {
                    if (elementType.GetType().Name == "WallType")
                    {
                        var wall_Type = elementType.Duplicate(name_type_new) as WallType;
                        elementType_new = wall_Type;
                        wall_Type.SetCompoundStructure(F_SetCompound.set_compound_structure(wall_Type.GetCompoundStructure(), mydata_materials, mydata_parameters, unit_length));
                    }
                    if (elementType.GetType().Name == "FloorType")
                    {
                        var floor_Type = elementType.Duplicate(name_type_new) as FloorType;
                        elementType_new = floor_Type;
                        floor_Type.SetCompoundStructure(F_SetCompound.set_compound_structure(floor_Type.GetCompoundStructure(), mydata_materials, mydata_parameters, unit_length));
                    }
                    if (elementType.GetType().Name == "RoofType")
                    {
                        var roof_Type = elementType.Duplicate(name_type_new) as RoofType;
                        elementType_new = roof_Type;
                        roof_Type.SetCompoundStructure(F_SetCompound.set_compound_structure(roof_Type.GetCompoundStructure(), mydata_materials, mydata_parameters, unit_length));
                    }
                    if (elementType.GetType().Name == "CeilingType")
                    {
                        var ceiling_Type = elementType.Duplicate(name_type_new) as CeilingType;
                        elementType_new = ceiling_Type;
                        ceiling_Type.SetCompoundStructure(F_SetCompound.set_compound_structure(ceiling_Type.GetCompoundStructure(), mydata_materials, mydata_parameters, unit_length));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return elementType_new;
        }
    }
}
