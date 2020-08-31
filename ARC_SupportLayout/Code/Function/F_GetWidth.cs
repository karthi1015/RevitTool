using ARC_SupportLayout.Data;
using ARC_SupportLayout.Data.Binding;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ARC_SupportLayout.Code.Function
{
    class F_GetWidth
    {
        public static void get_width(ObservableCollection<data_width> my_width, ListView data_width, List<ElementType> data_type)
        {
            try
            {

                ObservableCollection<data_width> my_width_support = new ObservableCollection<data_width>();
                foreach (ElementType type in data_type)
                {
                    //if (type.Name.Split('_').Count() > 0 && type.Name.Split('_')[0] == "A")
                    //{
                    if (type.Category != null && type.Category.AllowsBoundParameters == true && type.Category.CategoryType.ToString() == "Model")
                    {
                        if (type.LookupParameter(Source.share_para_hoan_thien) != null && type.LookupParameter(Source.share_para_hoan_thien).IsReadOnly == false)
                        {
                            my_width_support.Add(new data_width()
                            {
                                type = type,
                                width = type.LookupParameter(Source.share_para_hoan_thien).AsValueString()
                            });
                        }
                    }
                    //}
                }

                my_width = new ObservableCollection<data_width>(my_width_support.GroupBy(x => new
                {
                    x.width
                }).Select(x => new data_width()
                {
                    width = x.Key.width,
                    count = x.Count(),
                    types = x.Select(y => y.type).ToList()
                }).ToList());

                data_width.ItemsSource = my_width;
                ListCollectionView view = CollectionViewSource.GetDefaultView(data_width.ItemsSource) as ListCollectionView;
                view.SortDescriptions.Add(new System.ComponentModel.SortDescription("width", System.ComponentModel.ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
