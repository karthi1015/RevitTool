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
    class F_GetFamily
    {
        //----------------------------------------------------------
        public static void distint_family(ObservableCollection<data_category> my_category,ListView thong_tin_category,
            ObservableCollection<data_family> my_family, ListView thong_tin_family)
        {
            try
            {
                List<string> family_list = new List<string>();
                try
                {
                    family_list = CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource).Cast<data_family>().Where(x => x.check == true).Select(y => y.family_name).ToList();
                }
                catch (Exception)
                {

                }

                List<data_element> my_element = new List<data_element>();
                if (my_category.Count() > 0)
                {
                    foreach (List<Element> list in CollectionViewSource.GetDefaultView(thong_tin_category.ItemsSource).Cast<data_category>().Where(x => x.check == true).Select(x => x.elements))
                    {
                        foreach (Element element in list)
                        {
                            my_element.Add(new data_element()
                            {
                                family_name = element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString(),
                                element = element
                            });
                        }
                    }
                }

                my_element.GroupBy(x => new
                {
                    x.family_name
                }).Select(y => new data_family()
                {
                    family_name = y.Key.family_name,
                    elements = y.Select(z => z.element).ToList(),

                    check = false,
                    count = y.Count()
                }).ToList().ForEach(x => my_family.Add(x));

                my_family.Where(x => family_list.Contains(x.family_name)).ToList().ForEach(y => y.check = true);

                thong_tin_family.ItemsSource = my_family;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_family();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
