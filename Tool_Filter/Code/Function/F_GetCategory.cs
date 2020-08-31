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
    class F_GetCategory
    {
        //----------------------------------------------------------
        public static void distint_category(Document doc, ObservableCollection<data_category> my_category, ListView thong_tin_category, List<Element> elements)
        {
            try
            {
                List<data_element> my_element = new List<data_element>();
                foreach (Element element in elements)
                {
                    if(element.Category != null)
                    {
                        my_element.Add(new data_element()
                        {
                            category_name = element.Category.Name,
                            element = element
                        });
                    }
                }
                my_element.GroupBy(x => new
                {
                    x.category_name
                }).Select(y => new data_category()
                {
                    category_name = y.Key.category_name,
                    elements = y.Select(z => z.element).ToList(),

                    check = false,
                    count = y.Count()
                }).ToList().ForEach(x => my_category.Add(x));

                thong_tin_category.ItemsSource = my_category;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_category.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_category();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
