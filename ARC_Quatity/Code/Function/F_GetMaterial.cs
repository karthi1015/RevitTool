using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using Excel = Microsoft.Office.Interop.Excel;
using ARC_Quatity.Data.Binding;
using ARC_Quatity.Data;

namespace ARC_Quatity.Code.Function
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
        public static void Get_All_Material_Of_Element(Document doc, ICollection<ElementId> IdMaterial, ICollection<ElementId> IdMaterialPaint, List<data_paint> materials)
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
