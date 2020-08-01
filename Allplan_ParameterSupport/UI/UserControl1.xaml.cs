using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Allplan_ParameterSupport
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        E_AllplanData my_allplan_data;
        ExternalEvent e_allplan_data;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<Data> list_data { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_allplan_data = new E_AllplanData();
            e_allplan_data = ExternalEvent.Create(my_allplan_data);

            Them_Data_Vao_List_View();
        }
        //----------------------------------------------------------
        public void Them_Data_Vao_List_View()
        {
            try
            {
                list_data = new ObservableCollection<Data>();
                List<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList();
                List<Element> element_model = new List<Element>();
                foreach(Element ele in elements)
                {
                    try
                    {
                        Category cate = ele.Category;
                        if (cate.AllowsBoundParameters == true && cate.CategoryType.ToString() == "Model" && cate.Name != "Materials")
                        {
                            element_model.Add(ele);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                foreach(Element model in element_model)
                {
                    if(model is FamilyInstance)
                    {
                        FamilyInstance familyInstance = model as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            Support.Add_Data_Listview(model, model, list_data);
                            List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                            foreach(ElementId id in elements_child)
                            {
                                FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                                Support.Add_Data_Listview(model, model_child, list_data);
                                Support_Foreach(model, model_child);
                            }
                        }
                    }
                    else
                    {
                        Support.Add_Data_Listview(model, model, list_data);
                    }
                }
                thong_tin_parameter.ItemsSource = list_data;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("level_cau_kien", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_cau_kien", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("id_cau_kien", ListSortDirection.Ascending));

                view.Filter = Filter_ten_vat_lieu;

                number.Content = list_data.Count().ToString() + " items";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Support_Foreach(Element model, FamilyInstance model_child)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        Support.Add_Data_Listview(model, model_child, list_data);
                        Support_Foreach(model, model_child);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            if (string.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as Data).level_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Data).ten_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Data).id_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Hight_Light_Cau_Kien_Duoc_Chon(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(thong_tin_parameter.SelectedItems != null)
                {
                    Selection selection = uidoc.Selection;
                    List<ElementId> ids = new List<ElementId>();
                    for (int i = 0; i < thong_tin_parameter.SelectedItems.Count; i++)
                    {
                        Data item = (Data)thong_tin_parameter.SelectedItems[i];
                        ids.Add(item.cau_kien.Id);
                    }
                    selection.SetElementIds(ids);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_Data(object sender, RoutedEventArgs e)
        {
            Them_Data_Vao_List_View();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Them_Thong_tin(object sender, RoutedEventArgs e)
        {
            try
            {
                my_allplan_data.thong_tin_parameter = thong_tin_parameter;
                e_allplan_data.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void add_number_element(UIDocument uidoc, Document doc)
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
        public void get_number_element_framing(UIDocument uidoc, Document doc, List<Element> elements)
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
        public void get_number_element_not_framing(UIDocument uidoc, Document doc, List<Element> elements)
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
        public List<Element> intersection_all(Document doc, Element element)
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
        public List<Element> intersection_not_framing(Document doc, Element element)
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
        public List<Element> intersection_framing(Document doc, Element element)
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
