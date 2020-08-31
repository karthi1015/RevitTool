using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
using System.Windows.Data;
using ComboBox = System.Windows.Controls.ComboBox;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_TypeDefault
    {//----------------------------------------------------------
        public static ElementType get_type_select(UIDocument uidoc, Document doc, ComboBox descipline, ComboBox category, RadioButton outhome, RadioButton inhome, RadioButton kc)
        {
            ElementType type = null;
            try
            {
                Selection selection = uidoc.Selection;
                var element_selects = selection.GetElementIds().ToList();
                if (element_selects.Count() == 1)
                {
                    Element element_select = doc.GetElement(element_selects[0]);
                    type = doc.GetElement(element_select.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;
                    var data = type.Name.Split('_').ToList();
                    if (data.Count() > 2)
                    {
                        string descipline_name = data[0];

                        string position = data[1];

                        string category_name = data[2];
                        
                        if (Source.list_discipline.Exists(x => x.key == descipline_name))
                        {
                            descipline.SelectedItem = Source.list_discipline.First(x => x.key == descipline_name);
                            CollectionViewSource.GetDefaultView(category.ItemsSource).Refresh();

                            var category_data = CollectionViewSource.GetDefaultView(category.ItemsSource).Cast<data_name_key>().ToList();
                            if(Source.list_category.Exists(x => x.key == category_name))
                            {
                                category.SelectedItem = category_data.First(x => x.key == category_name);

                                if (position == Source.list_position[0].key) outhome.IsChecked = true;
                                else if (position == Source.list_position[1].key) inhome.IsChecked = true;
                                else if (position == Source.list_position[2].key) kc.IsChecked = true;
                                else type = null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return type;
        }
    }
}
