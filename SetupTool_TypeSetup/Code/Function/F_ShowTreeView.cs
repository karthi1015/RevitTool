using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using SetupTool_TypeSetup.Data.BindingCompany;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_ShowTreeView
    {
        //-----------------------------------------------------------
        public static void show_data(Document doc, ComboBox descipline, ComboBox category, RadioButton outhome, RadioButton inhome, RadioButton kc, ObservableCollection<data_family> my_family)
        {
            try
            {
                data_name_key descipline_data = (data_name_key)descipline.SelectedItem;
                string position = Source.list_position.First(x => x.name == (outhome.IsChecked == true ? outhome.Content.ToString() : (inhome.IsChecked == true ? inhome.Content.ToString() : kc.Content.ToString()))).key;
                data_name_key category_data = (data_name_key)category.SelectedItem;

                var elements = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).ToElements();
                var familys = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToElements();
                List<string> family_check = new List<string>();
                foreach (ElementType ele_family in elements)
                {
                    var format = ele_family.Name.Split('_');
                    if (format.Count() > 2)
                    {
                        if (format[0] == descipline_data.key && format[1] == position && format[2] == category_data.key)
                        {
                            if (family_check.Contains(ele_family.FamilyName) == false)
                            {
                                ObservableCollection<data_type> my_type = new ObservableCollection<data_type>();
                                foreach (ElementType ele_type in elements)
                                {
                                    if (ele_family.FamilyName == ele_type.FamilyName && ele_type.Name.Split('_')[0] == descipline_data.key && ele_type.Name.Split('_')[1] == position && ele_type.Name.Split('_')[2] == category_data.key)
                                    {
                                        string type_type = Source.type_element;
                                        if (familys.Any(x => x.Id.IntegerValue == ele_type.Id.IntegerValue))
                                        {
                                            type_type = Source.type_symbol;
                                        }
                                        my_type.Add(new data_type()
                                        {
                                            ten_element_type = ele_type.Name,
                                            element_type = ele_type,
                                            type_type = type_type,
                                            delete_type = false,
                                            ValueIsSelect = false,
                                        });
                                    }
                                }
                                var a = my_type.OrderBy(x => x.ten_element_type).ToList();
                                my_type.Clear();
                                foreach (var b in a)
                                {
                                    my_type.Add(b);
                                }

                                my_family.Add(new data_family()
                                {
                                    ten_family_type = ele_family.FamilyName,
                                    image = F_ImageFamily.ConvertBitmapToBitmapSource(ele_family),
                                    ValueExpanded = true,
                                    Children = my_type
                                });
                            }
                            family_check.Add(ele_family.FamilyName);
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
