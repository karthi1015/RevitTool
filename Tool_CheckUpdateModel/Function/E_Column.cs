using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using RadioButton = System.Windows.Controls.RadioButton;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;
using System.Globalization;
using Tool_CheckUpdateModel.Data;
using Autodesk.Revit.DB.IFC;

namespace Tool_CheckUpdateModel.Function
{
    class E_Column : IExternalEventHandler
    {
        public ObservableCollection<Element_Change> element_Changes { get; set; }
        public ObservableCollection<Element_Change> my_element_change { get; set; }
        public Document doc_link { get; set; }

        public ListView data_update { get; set; }
        public ComboBox link_file { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc);
            transaction.Start("Column Change");

            Column_Change(uiapp, doc);
            transaction.Commit();
        }

        public string GetName()
        {
            return "Column Change";
        }

        public string Column_Change(UIApplication uiapp, Document doc)
        {
            string result = "S";
            try
            {
                if (element_Changes.Count() > 0)
                {
                    foreach (Element_Change change in element_Changes)
                    {
                        if (change.element == null)
                        {
                            ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { change.element_link.Id }, doc, Transform.Identity, new CopyPasteOptions()).First();
                        }
                        else if (change.element_link == null)
                        {
                            if (change.element.Pinned) change.element.Pinned = false;
                            doc.Delete(change.element.Id);
                        }
                        else
                        {
                            if (change.element.Pinned) change.element.Pinned = false;
                            if(change.type_change == "Profile")
                            {
                                var eles = ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { change.element_link.Id }, doc, Transform.Identity, new CopyPasteOptions());
                                // Change host
                                //................
                                ElementMulticategoryFilter filter = new ElementMulticategoryFilter(new List<BuiltInCategory>() { BuiltInCategory.OST_Windows, BuiltInCategory.OST_Doors});
                                List<Element_Group> support_all = new List<Element_Group>();
                                foreach (FamilyInstance familyInstance in new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType().Where(x => (x as FamilyInstance).Host.Id == change.element_link.Id).Cast<FamilyInstance>().ToList())
                                {
                                    support_all.Add(new Element_Group()
                                    {
                                        element = familyInstance,
                                        element_text2 = familyInstance.LookupParameter("Text2").AsString(),
                                        link = false
                                    });
                                }
                                foreach (FamilyInstance familyInstance in new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType().Where(x => (x as FamilyInstance).Host.Id == eles.First()).Cast<FamilyInstance>().ToList())
                                {
                                    support_all.Add(new Element_Group()
                                    {
                                        element = familyInstance,
                                        element_text2 = familyInstance.LookupParameter("Text2").AsString(),
                                        link = true
                                    });
                                }

                                List<List<Element_Group>> support_group = new List<List<Element_Group>>(support_all
                                    .GroupBy(x => new { x.element_text2 })
                                    .Select(y => new List<Element_Group>(y)));

                                foreach (List<Element_Group> familyInstances in support_group)
                                {
                                    if (familyInstances.Count() == 2)
                                    {
                                        familyInstances[1].element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).Set(familyInstances[0].element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId());
                                    }
                                }

                                doc.Delete(change.element.Id);
                                doc.GetElement(eles.First()).Pinned = true;
                            }
                            else
                            {
                                if (change.type_change == "Location")
                                {
                                    if (change.parameter_category_name == "column")
                                    {
                                        LocationPoint columnPoint = change.element.Location as LocationPoint;
                                        columnPoint.Point = (change.element_link.Location as LocationPoint).Point;
                                    }
                                    else if (change.parameter_category_name == "framing")
                                    {
                                        LocationCurve columnPoint = change.element.Location as LocationCurve;
                                        columnPoint.Curve = (change.element_link.Location as LocationCurve).Curve;
                                    }
                                    else if (change.parameter_category_name == "wall")
                                    {
                                        Wall wall = change.element as Wall;
                                        Wall wall_change = change.element_link as Wall;
                                        LocationCurve columnPoint = wall.Location as LocationCurve;
                                        columnPoint.Curve = (wall_change.Location as LocationCurve).Curve;
                                    }
                                }
                                foreach (Parameter_Change para in change.parameter_change)
                                {
                                    if (para.parameter.Definition.Name == "Type")
                                    {
                                        ElementId type_id = new ElementId(-1);
                                        ElementType link_symbol = doc_link.GetElement(change.element_link.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;
                                        if (change.parameter_category_name == "column")
                                        {
                                            var type = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsElementType().Where(x => x.Name == link_symbol.Name).ToList();
                                            if (type.Count() > 0) type_id = type.First().Id;
                                            else
                                            {
                                                var elment_copy = ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { link_symbol.Id }, doc, Transform.Identity, new CopyPasteOptions());
                                                type_id = elment_copy.First();
                                            }
                                        }
                                        else if (change.parameter_category_name == "framing")
                                        {
                                            var type = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsElementType().Where(x => x.Name == link_symbol.Name).ToList();
                                            if (type.Count() > 0) type_id = type.First().Id;
                                            else
                                            {
                                                var elment_copy = ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { link_symbol.Id }, doc, Transform.Identity, new CopyPasteOptions());
                                                type_id = elment_copy.First();
                                            }
                                        }
                                        else if (change.parameter_category_name == "wall")
                                        {
                                            var type = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().Where(x => x.Name == link_symbol.Name).ToList();
                                            if (type.Count() > 0) type_id = type.First().Id;
                                            else
                                            {
                                                var elment_copy = ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { link_symbol.Id }, doc, Transform.Identity, new CopyPasteOptions());
                                                type_id = elment_copy.First();
                                            }
                                        }
                                        else
                                        {
                                            var type = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().Where(x => x.Name == link_symbol.Name).ToList();
                                            if (type.Count() > 0) type_id = type.First().Id;
                                            else
                                            {
                                                var elment_copy = ElementTransformUtils.CopyElements(doc_link, new List<ElementId>() { link_symbol.Id }, doc, Transform.Identity, new CopyPasteOptions());
                                                type_id = elment_copy.First();
                                            }
                                        }
                                        change.element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).Set(type_id);
                                    }
                                    else
                                    {
                                        Support.GetParameterValue(para.parameter, para.parameter_link);
                                    }
                                }
                                if (!change.element.Pinned) change.element.Pinned = true;
                            }
                            
                        }
                        change.color = Source.color_used_change;
                        data_update.Items.Refresh();
                    }
                }
                F_ListBox.Save_Data_Check(my_element_change, link_file, uiapp, doc_link);
            }
            catch (Exception ex)
            {
                result = "F";
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
