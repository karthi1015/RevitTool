using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tool_CheckUpdateModel.Data;
using Document = Autodesk.Revit.DB.Document;
using Autodesk.Revit.DB.IFC;
using System.IO;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Tool_CheckUpdateModel.Function
{
    class F_Column
    {
        //----------------------------------------------------------
        public static void Check_Element(Document doc, Document doc_link, ObservableCollection<Parameter_Settings> my_parameter_settings, ObservableCollection<Element_Change> my_element_change)
        {
            try
            {
                List<Element_Group> support_all = new List<Element_Group>();
                Get_Element(doc, support_all, false);
                Get_Element(doc_link, support_all, true);

                List<List<Element_Group>> support_group = new List<List<Element_Group>>(support_all
                    .GroupBy(x => new { x.element_text2 })
                    .Select(y => new List<Element_Group>(y)));

                List<string> list_data = new List<string>();
                if(File.Exists(doc_link.PathName.Split('.')[0]))
                {
                    list_data = File.ReadAllLines(doc_link.PathName.Split('.')[0]).ToList();
                }
                foreach (List<Element_Group> familyInstances in support_group)
                {
                    bool changeORignore = true;
                    Brush color = Source.color_not_change;
                    foreach(string data in list_data)
                    {
                        if (familyInstances[0].element_text2 == data.Split('\t')[0])
                        {
                            changeORignore = Support.String_To_Bool(data.Split('\t')[1]);
                            Color c = (Color)ColorConverter.ConvertFromString(data.Split('\t')[2]);
                            color = new SolidColorBrush(c);
                        }
                    }

                    if (familyInstances.Count() == 1)
                    {
                        ObservableCollection<Parameter_Change> para_change = new ObservableCollection<Parameter_Change>();
                        if (familyInstances[0].link)
                        {
                            my_element_change.Add(new Element_Change()
                            {
                                element = null,
                                element_link = familyInstances[0].element,
                                parameter_category_name = familyInstances[0].parameter_category_name,
                                parameter_change = para_change,
                                type_change = "New",
                                element_name = familyInstances[0].element.LookupParameter("Text1").AsString(),
                                element_id = familyInstances[0].element_text2,
                                changeORignore = changeORignore,
                                color = color
                            });
                        }
                        else
                        {
                            my_element_change.Add(new Element_Change()
                            {
                                element = familyInstances[0].element,
                                element_link = null,
                                parameter_category_name = familyInstances[0].parameter_category_name,
                                parameter_change = para_change,
                                type_change = "Exist",
                                element_name = familyInstances[0].element.LookupParameter("Text1").AsString(),
                                element_id = familyInstances[0].element_text2,
                                changeORignore = changeORignore,
                                color = color
                            });
                        }
                    }
                    if (familyInstances.Count() == 2)
                    {
                        Element element = familyInstances[0].element;
                        Element element_link = familyInstances[1].element;

                        List<Point_Change> point_Location = new List<Point_Change>();

                        ObservableCollection<Parameter_Change> para_change = Element_Change(doc, doc_link, familyInstances, my_parameter_settings, familyInstances[0].parameter_category_name);
                        Check_Location(element, element_link, point_Location, doc);
                        bool Profile_change = Check_Profile(element, element_link, doc, doc_link);

                        if (para_change.Count() > 0 || point_Location.Where(x => x.point.ToString() != x.point_link.ToString()).Count() > 0 || Profile_change == true)
                        {
                            if (point_Location.Count() > 0)
                            {
                                var xx = point_Location[0].point.ToString();
                            }
                            string preview = "";
                            string type = "";
                            if (Profile_change == true) type = "Profile";
                            else if (point_Location.Where(x => x.point.ToString() != x.point_link.ToString()).Count() > 0) type = "Location";
                            else type = "Parameter";
                               
                            if((point_Location.Where(x => x.point.ToString() != x.point_link.ToString()).Count() > 0 && para_change.Count() > 0) || para_change.Count() > 0)
                            {
                                foreach (Parameter_Change para in para_change)
                                {
                                    preview += para.parameter.Definition.Name + ":          " + Support.GetParameterInformation(para.parameter, doc) + "          " + Support.GetParameterInformation(para.parameter_link, doc_link) + "\n";
                                }
                            }

                            my_element_change.Add(new Element_Change()
                            {
                                element = element,
                                element_link = element_link,
                                parameter_category_name = familyInstances[0].parameter_category_name,
                                parameter_change = para_change,
                                type_change = type,
                                preview = preview,
                                element_name = element.LookupParameter("Text1").AsString(),
                                element_id = familyInstances[0].element_text2,
                                changeORignore = changeORignore,
                                color = color
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Get data element with Text2----------------------------------------------------------
        public static void Get_Element(Document doc, List<Element_Group> support_all, bool link)
        {
            try
            {
                ElementMulticategoryFilter filter = new ElementMulticategoryFilter(Source.Category_Check);
                foreach (Element instance in new FilteredElementCollector(doc).WherePasses(filter).WhereElementIsNotElementType().ToList())
                {
                    if (instance.LookupParameter("Text2") != null)
                    {
                        if (!string.IsNullOrEmpty(instance.LookupParameter("Text2").AsString()))
                        {
                            string cate = "";
                            if (instance.Category.Name == Category.GetCategory(doc, Source.Category_Check[0]).Name) cate = "column";
                            else if (instance.Category.Name == Category.GetCategory(doc, Source.Category_Check[1]).Name) cate = "framing";
                            else if (instance.Category.Name == Category.GetCategory(doc, Source.Category_Check[2]).Name) cate = "floor";
                            else cate = "wall";
                            support_all.Add(new Element_Group()
                            {
                                element = instance,
                                element_text2 = instance.LookupParameter("Text2").AsString(),
                                link = link,
                                parameter_category_name = cate
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //  Check para change----------------------------------------------------------
        public static ObservableCollection<Parameter_Change> Element_Change(Document doc, Document doc_link, List<Element_Group> familyInstances, ObservableCollection<Parameter_Settings> my_parameter_settings, string cate)
        {
            ObservableCollection<Parameter_Change> para_change = new ObservableCollection<Parameter_Change>();
            try
            {
                foreach (Parameter_Settings parameter in my_parameter_settings)
                {
                    if (parameter.isCheck == true &&
                        parameter.parameter_category_name == cate &&
                        Support.GetParameterInformation(familyInstances[0].element.get_Parameter(parameter.parameter.Definition), doc) != Support.GetParameterInformation(familyInstances[1].element.get_Parameter(parameter.parameter.Definition), doc_link))
                    {
                        para_change.Add(new Parameter_Change()
                        {
                            parameter = familyInstances[0].element.get_Parameter(parameter.parameter.Definition),
                            parameter_link = familyInstances[1].element.get_Parameter(parameter.parameter.Definition)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return para_change;
        }

        //  Check Location----------------------------------------------------------
        public static void Check_Location(Element element , Element element_link, List<Point_Change> point_Location, Document doc)
        {
            try
            {
                if (element.Category.Name == Category.GetCategory(doc, Source.Category_Check[0]).Name)
                {
                    point_Location.Add(new Point_Change()
                    {
                        point = (element.Location as LocationPoint).Point,
                        point_link = (element_link.Location as LocationPoint).Point,
                    });
                }
                else if (element.Category.Name == Category.GetCategory(doc, Source.Category_Check[1]).Name)
                {
                    point_Location.Add(new Point_Change()
                    {
                        point = (element.Location as LocationCurve).Curve.GetEndPoint(0),
                        point_link = (element_link.Location as LocationCurve).Curve.GetEndPoint(0),
                    });
                    point_Location.Add(new Point_Change()
                    {
                        point = (element.Location as LocationCurve).Curve.GetEndPoint(1),
                        point_link = (element_link.Location as LocationCurve).Curve.GetEndPoint(1),
                    });
                }
                else if (element.Category.Name == Category.GetCategory(doc, Source.Category_Check[2]).Name)
                {
                    point_Location.Add(new Point_Change()
                    {
                        point = new XYZ(),
                        point_link = new XYZ(),
                    });
                }
                else 
                {
                    point_Location.Add(new Point_Change()
                    {
                        point = (element.Location as LocationCurve).Curve.GetEndPoint(0),
                        point_link = (element_link.Location as LocationCurve).Curve.GetEndPoint(0),
                    });
                    point_Location.Add(new Point_Change()
                    {
                        point = (element.Location as LocationCurve).Curve.GetEndPoint(1),
                        point_link = (element_link.Location as LocationCurve).Curve.GetEndPoint(1),
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //  Check Profile-----------------------------------------------------------
        public static bool Check_Profile(Element element, Element element_link, Document doc, Document doc_link)
        {
            bool Profile_change = false;
            try
            {
                if (element.Category.Name == Category.GetCategory(doc, Source.Category_Check[3]).Name)
                {
                    if (ExporterIFCUtils.HasElevationProfile(element as Wall) || ExporterIFCUtils.HasElevationProfile(element_link as Wall))
                    {
                        if (string.Join(",", Support.Get_Profile(doc, element).Select(x => x.ToString())) != string.Join(",", Support.Get_Profile(doc_link, element_link).Select(x => x.ToString())))
                        {
                            Profile_change = true;
                        }
                    }
                }
                else if (element.Category.Name == Category.GetCategory(doc, Source.Category_Check[2]).Name)
                {
                    if (string.Join(",", Support.Get_Profile(doc, element).Select(x => x.ToString())) != string.Join(",", Support.Get_Profile(doc_link, element_link).Select(x => x.ToString())))
                    {
                        Profile_change = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Profile_change;
        }
    
        // Check Type---------------------------------------------------------------
        public static bool Check_Type(Element element, Element element_link, Document doc, Document doc_link)
        {
            bool check_type = true;
            try
            {
                ElementType symbol = doc_link.GetElement(element.GetTypeId()) as ElementType;
                ElementType symbol_link = doc_link.GetElement(element_link.GetTypeId()) as ElementType;
                for (int i = 0; i < symbol.Parameters.Size; i++)
                {
                    if(Support.GetParameterInformation(symbol.Parameters.Cast<Parameter>().ToList()[i], doc) != Support.GetParameterInformation(symbol_link.Parameters.Cast<Parameter>().ToList()[i], doc_link))
                    {
                        check_type = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return check_type;
        }
    }
}
