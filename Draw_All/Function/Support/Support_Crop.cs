using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Draw_All
{
    class Support_Crop
    {
        public ObservableCollection<view_plan_name_data> my_view_plan_name_data_tem { get; set; }
        public ObservableCollection<view_plan_name_data> my_view_plan_name_data_apply { get; set; }

        public TextBox search_view_tem { get; set; }
        public TextBox search_view_apply { get; set; }

        Support_All mySupport_All;
        ListSource mySource;
        FunctionSQL mySQL;

        //----------------------------------------------------------
        public void Get_View_Data(ComboBox view_tem, ComboBox view_apply, Document doc)
        {
            try
            {
                my_view_plan_name_data_tem = new ObservableCollection<view_plan_name_data>(new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewPlan))
                    .Cast<ViewPlan>()
                    .Select(x => new view_plan_name_data()
                    {
                        single_value = x.Name,
                        view_plan = x
                    }));

                view_tem.ItemsSource = my_view_plan_name_data_tem;
                view_tem.SelectedIndex = 0;
                CollectionView view_crop_tem = (CollectionView)CollectionViewSource.GetDefaultView(view_tem.ItemsSource);
                view_crop_tem.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
                view_crop_tem.Filter = Filter_view_tem;

                my_view_plan_name_data_apply = new ObservableCollection<view_plan_name_data>(new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewPlan))
                    .Cast<ViewPlan>()
                    .Select(x => new view_plan_name_data()
                    {
                        single_value = x.Name,
                        view_plan = x
                    }));

                view_apply.ItemsSource = my_view_plan_name_data_apply;
                view_apply.SelectedIndex = 0;
                CollectionView view_crop_apply = (CollectionView)CollectionViewSource.GetDefaultView(view_apply.ItemsSource);
                view_crop_apply.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
                view_crop_apply.Filter = Filter_view_apply;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_view_tem(object item)
        {
            if (string.IsNullOrEmpty(search_view_tem.Text) || search_view_tem.Text == "search")
                return true;
            else
                return ((item as view_plan_name_data).single_value.IndexOf(search_view_tem.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private bool Filter_view_apply(object item)
        {
            if (string.IsNullOrEmpty(search_view_apply.Text) || search_view_apply.Text == "search")
                return true;
            else
                return ((item as view_plan_name_data).single_value.IndexOf(search_view_apply.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
