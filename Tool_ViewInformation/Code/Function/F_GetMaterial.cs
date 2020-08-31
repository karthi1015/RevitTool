using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using Tool_ViewInformation.Data.Binding;

namespace Tool_ViewInformation.Code.Function
{
    class F_GetMaterial
    {
        //----------------------------------------------------------
        public static void Get_All_Material_Of_Element_By_Category(Document doc, Element element, List<data_paint> materials)
        {
            try
            {
                if (element.Category.Name == "Railings")
                {
                    RailingType type = doc.GetElement(element.GetTypeId()) as RailingType;
                    if (type.RailStructure.GetNonContinuousRailCount() > 0)
                    {
                        for (var i = 0; i < type.RailStructure.GetNonContinuousRailCount(); i++)
                        {
                            var idRailing = type.RailStructure.GetNonContinuousRail(i).MaterialId;
                            Material m = doc.GetElement(idRailing) as Material;
                            if (m != null)
                            {
                                materials.Add(new data_paint()
                                {
                                    vat_lieu = m,
                                    is_paint = Source.no_paint
                                });
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
        public static void Get_All_Material_Of_Element(Document doc, List<data_paint> materials, ICollection<ElementId> IdMaterial, ICollection<ElementId> IdMaterialPaint)
        {
            try
            {
                // add material element
                foreach (ElementId id in IdMaterial)
                {
                    Material m = doc.GetElement(id) as Material;
                    if (m != null)
                    {
                        materials.Add(new data_paint()
                        {
                            vat_lieu = m,
                            is_paint = Source.no_paint
                        });
                    }
                }
                foreach (ElementId id in IdMaterialPaint)
                {
                    Material m = doc.GetElement(id) as Material;
                    if (m != null)
                    {
                        materials.Add(new data_paint()
                        {
                            vat_lieu = m,
                            is_paint = Source.paint
                        });
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
