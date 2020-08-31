using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allplan_ParameterSupport.Code.Function
{
    class F_NumberElement
    {
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void add_number_element(UIDocument uidoc, Document doc)
        {
            try
            {
                var element = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList();
                List<Element> elements_1 = new List<Element>(); // cột, vách, móng, generic
                List<string> name_level_1 = new List<string>();

                List<Element> elements_2 = new List<Element>(); // dầm
                List<string> name_level_2 = new List<string>();

                List<Element> elements_3 = new List<Element>(); // sàn
                List<string> name_level_3 = new List<string>();

                foreach (Element ele in element)
                {
                    if (ele.LookupParameter(Source.share_para_name) != null && ele.LookupParameter(Source.share_para_level) != null)
                    {
                        if (ele.Category.Name == "Structural Framing")
                        {
                            elements_2.Add(ele);
                            name_level_2.Add(ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString());
                        }
                        else if (ele.Category.Name == "Floors")
                        {
                            elements_3.Add(ele);
                            name_level_3.Add(ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString());
                        }
                        else
                        {
                            elements_1.Add(ele);
                            name_level_1.Add(ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString());
                        }
                    }
                }
                List<string> name_level_unique_1 = name_level_1.Distinct().ToList();
                List<string> name_level_unique_2 = name_level_2.Distinct().ToList();
                List<string> name_level_unique_3 = name_level_3.Distinct().ToList();

                List<List<Element>> elements_list_1 = new List<List<Element>>();
                foreach (string level in name_level_unique_1)
                {
                    List<Element> elements_list_1_1 = new List<Element>();
                    foreach (Element ele in elements_1)
                    {
                        if (ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString() == level)
                        {
                            elements_list_1_1.Add(ele);
                        }
                    }
                    elements_list_1.Add(elements_list_1_1);
                }

                List<List<Element>> elements_list_2 = new List<List<Element>>();
                foreach (string level in name_level_unique_2)
                {
                    List<Element> elements_list_1_1 = new List<Element>();
                    foreach (Element ele in elements_2)
                    {
                        if (ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString() == level)
                        {
                            elements_list_1_1.Add(ele);
                        }
                    }
                    elements_list_2.Add(elements_list_1_1);
                }

                List<List<Element>> elements_list_3 = new List<List<Element>>();
                foreach (string level in name_level_unique_3)
                {
                    List<Element> elements_list_1_1 = new List<Element>();
                    foreach (Element ele in elements_3)
                    {
                        if (ele.LookupParameter(Source.share_para_name).AsString() + "|" + ele.LookupParameter(Source.share_para_level).AsString() == level)
                        {
                            elements_list_1_1.Add(ele);
                        }
                    }
                    elements_list_3.Add(elements_list_1_1);
                }

                foreach (List<Element> ele in elements_list_1)
                {
                    get_number_element_not_framing(uidoc, doc, ele);
                }

                foreach (List<Element> ele in elements_list_2)
                {
                    get_number_element_framing(uidoc, doc, ele);
                }

                foreach (List<Element> ele in elements_list_3)
                {
                    foreach (Element e in ele)
                    {
                        if (e.LookupParameter(Source.share_para_text4) != null)
                        {
                            e.LookupParameter(Source.share_para_text4).Set(1.ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static void get_number_element_framing(UIDocument uidoc, Document doc, List<Element> elements)
        {
            try
            {
                List<string> elementid_string = new List<string>();
                int i = 0;
                foreach (Element ele in elements)
                {
                    bool intersection = true;
                    if (elementid_string.Exists(x => x == ele.Id.ToString()) == false)
                    {
                        Element ele_new = ele;
                        elementid_string.Add(ele_new.Id.ToString());

                        List<ElementId> elementid1_string = new List<ElementId>();

                        List<ElementId> elementid2_string = new List<ElementId>();

                        while (intersection == true)
                        {
                            List<Element> list_element_detail_1 = new List<Element>();
                            if (ele_new.Category.Name == "Structural Framing")
                            {
                                list_element_detail_1 = intersection_framing(doc, ele_new);
                            }
                            elementid2_string.Add(ele_new.Id);

                            List<Element> elements_check = new List<Element>();
                            foreach (Element element_1 in list_element_detail_1)
                            {
                                List<Element> list_element_detail_1_1 = intersection_not_framing(doc, element_1);
                                foreach (Element element_list in list_element_detail_1_1)
                                {
                                    if (elementid1_string.Exists(x => x == element_list.Id) == false &&
                                        element_list.LookupParameter("Name").AsString() == ele.LookupParameter("Name").AsString() &&
                                        element_list.LookupParameter("LevelPart").AsString() == ele.LookupParameter("LevelPart").AsString())
                                    {
                                        elements_check.Add(element_list);

                                        elementid_string.Add(element_list.Id.ToString());

                                        elementid1_string.Add(element_list.Id);

                                    }
                                }
                            }
                            foreach (ElementId id in elementid1_string)
                            {
                                if (elementid2_string.Exists(x => x == id) == false)
                                {
                                    ele_new = doc.GetElement(id);
                                }
                            }
                            if (elements_check.Count == 0)
                            {
                                intersection = false;
                            }
                        }
                        i++;
                    }
                }
                foreach (Element ele in elements)
                {
                    if (ele.LookupParameter("Text4") != null)
                    {
                        ele.LookupParameter("Text4").Set(i.ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static void get_number_element_not_framing(UIDocument uidoc, Document doc, List<Element> elements)
        {
            try
            {
                List<string> elementid_string = new List<string>();
                int i = 0;
                foreach (Element ele in elements)
                {
                    bool intersection = true;
                    if (elementid_string.Exists(x => x == ele.Id.ToString()) == false)
                    {
                        Element ele_new = ele;
                        elementid_string.Add(ele_new.Id.ToString());

                        List<ElementId> elementid1_string = new List<ElementId>();

                        List<ElementId> elementid2_string = new List<ElementId>();

                        while (intersection == true)
                        {
                            List<Element> list_element_detail_1 = intersection_all(doc, ele_new);
                            if (elementid1_string.Exists(x => x == ele_new.Id) == false)
                            {
                                elementid1_string.Add(ele_new.Id);
                            }
                            elementid2_string.Add(ele_new.Id);

                            if (list_element_detail_1.Count > 0)
                            {
                                foreach (Element ele1 in list_element_detail_1)
                                {
                                    if (elementid1_string.Exists(x => x == ele1.Id) == false)
                                    {
                                        elementid1_string.Add(ele1.Id);
                                    }
                                    elementid_string.Add(ele1.Id.ToString());
                                }
                            }
                            foreach (ElementId id in elementid1_string)
                            {
                                if (elementid2_string.Exists(x => x == id) == false)
                                {
                                    ele_new = doc.GetElement(id);
                                }
                            }
                            if (elementid1_string.Count == elementid2_string.Count)
                            {
                                intersection = false;
                            }
                        }
                        i++;
                    }
                }
                foreach (Element ele in elements)
                {
                    if (ele.LookupParameter("Text4") != null)
                    {
                        ele.LookupParameter("Text4").Set(i.ToString());
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static List<Element> intersection_all(Document doc, Element element)
        {
            List<Element> elements = new List<Element>();
            try
            {
                BoundingBoxXYZ bb = element.get_BoundingBox(doc.ActiveView);
                Outline outline = new Outline(bb.Min, bb.Max);
                BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector collectors = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<ElementId> idsExclude = new List<ElementId>();
                idsExclude.Add(element.Id);
                var collector = collectors.Excluding(idsExclude).WherePasses(bbfilter).ToElements();

                foreach (Element ele in collector)
                {
                    if (ele.LookupParameter("Name") != null && ele.LookupParameter("LevelPart") != null)
                    {
                        if (ele.LookupParameter("Name").AsString() == element.LookupParameter("Name").AsString() && ele.LookupParameter("LevelPart").AsString() == element.LookupParameter("LevelPart").AsString())
                        {
                            elements.Add(ele);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return elements;
        }

        //----------------------------------------------------------
        public static List<Element> intersection_not_framing(Document doc, Element element)
        {
            List<Element> elements = new List<Element>();
            try
            {
                BoundingBoxXYZ bb = element.get_BoundingBox(doc.ActiveView);
                Outline outline = new Outline(bb.Min, bb.Max);
                BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector collectors = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<ElementId> idsExclude = new List<ElementId>();
                idsExclude.Add(element.Id);
                var collector = collectors.Excluding(idsExclude).WherePasses(bbfilter).ToElements();

                foreach (Element ele in collector)
                {
                    if (ele.LookupParameter("Name") != null && ele.LookupParameter("LevelPart") != null)
                    {
                        if (ele.Category.Name == "Structural Framing")
                        {
                            elements.Add(ele);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return elements;
        }

        //----------------------------------------------------------
        public static List<Element> intersection_framing(Document doc, Element element)
        {
            List<Element> elements = new List<Element>();
            try
            {
                BoundingBoxXYZ bb = element.get_BoundingBox(doc.ActiveView);
                Outline outline = new Outline(bb.Min, bb.Max);
                BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector collectors = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<ElementId> idsExclude = new List<ElementId>();
                idsExclude.Add(element.Id);
                var collector = collectors.Excluding(idsExclude).WherePasses(bbfilter).ToElements();

                foreach (Element ele in collector)
                {
                    if (ele.LookupParameter("Name") != null && ele.LookupParameter("LevelPart") != null)
                    {
                        if (ele.Category.Name != "Structural Framing" && ele.Category.Name != "Floors")
                        {
                            elements.Add(ele);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            return elements;
        }
    }
}
