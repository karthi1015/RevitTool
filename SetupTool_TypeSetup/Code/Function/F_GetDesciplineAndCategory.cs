using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_GetDesciplineAndCategory
    {
        public static void get_descipline_and_category(ComboBox descipline, ComboBox category)
        {
            try
            {
                descipline.ItemsSource = Source.list_discipline;
                CollectionView view_descipline = (CollectionView)CollectionViewSource.GetDefaultView(descipline.ItemsSource);
                view_descipline.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
                descipline.SelectedIndex = Source.list_discipline.Count() - 1;

                category.ItemsSource = Source.list_category;
                CollectionView view_category = (CollectionView)CollectionViewSource.GetDefaultView(category.ItemsSource);
                view_category.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Ascending));
                category.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
