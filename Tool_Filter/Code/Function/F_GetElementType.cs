using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tool_Filter.Data.Binding;

namespace Tool_Filter.Code.Function
{
    class F_GetElementType
    {
        //----------------------------------------------------------
        public static void distint_type(ObservableCollection<data_family> my_family, ListView thong_tin_family,
            ObservableCollection<data_type> my_type, ListView thong_tin_type)
        {
            try
            {
                List<string> type_list = new List<string>();
                try
                {
                    type_list = CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource).Cast<data_type>().Where(x => x.check == true).Select(y => y.type_name).ToList();
                }
                catch (Exception ex)
                {

                }

                List<data_element> my_element = new List<data_element>();
                if (my_family.Count() > 0)
                {
                    foreach (List<Element> list in CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource).Cast<data_family>().Where(x => x.check == true).Select(x => x.elements))
                    {
                        foreach (Element element in list)
                        {
                            my_element.Add(new data_element()
                            {
                                type_name = element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString(),
                                element = element
                            });
                        }
                    }
                }

                 my_element.GroupBy(x => new
                    {
                        x.type_name
                    }).Select(y => new data_type()
                    {
                        type_name = y.Key.type_name,
                        elements = y.Select(z => z.element).ToList(),

                        check = false,
                        count = y.Count()
                    }).ToList().ForEach(x => my_type.Add(x));

                my_type.Where(x => type_list.Contains(x.type_name)).ToList().ForEach(y => y.check = true);

                thong_tin_type.ItemsSource = my_type;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_type();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
