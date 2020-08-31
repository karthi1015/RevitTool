using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Label = System.Windows.Controls.Label;

namespace Allplan_ParameterSupport.Code.Function
{
    class F_GetData
    {
        public static TextBox search_material_project { get; set; }
        //----------------------------------------------------------
        public static void Them_Data_Vao_List_View(ObservableCollection<Data> list_data, Document doc, Label number, ListView thong_tin_parameter)
        {
            try
            {
                list_data = new ObservableCollection<Data>();
                List<Element> elements = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList();
                List<Element> element_model = new List<Element>();
                foreach (Element ele in elements)
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
                foreach (Element model in element_model)
                {
                    if (model is FamilyInstance)
                    {
                        FamilyInstance familyInstance = model as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            Support.Add_Data_Listview(model, model, list_data);
                            List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                            foreach (ElementId id in elements_child)
                            {
                                FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                                Support.Add_Data_Listview(model, model_child, list_data);
                                Support_Foreach(model, model_child, doc, list_data);
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

                view.Filter = Filter;

                number.Content = list_data.Count().ToString() + " items";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public static void Support_Foreach(Element model, FamilyInstance model_child, Document doc, ObservableCollection<Data> list_data)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        Support.Add_Data_Listview(model, model_child, list_data);
                        Support_Foreach(model, model_child, doc, list_data);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static bool Filter(object item)
        {
            if (string.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as Data).level_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Data).ten_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Data).id_cau_kien.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
