using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SetupTool_ParameterSetup.Code.Function;
using SetupTool_ParameterSetup.Data.Binding;
using SetupTool_ParameterSetup.Data.BindingWEB;
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

namespace SetupTool_ParameterSetup
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;
        string Class;

        E_BindingShareParameter my_binding_share_parameter;
        ExternalEvent e_binding_share_parameter;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_group_share_parameter> my_group_share_parameter { get; set; } // Lay_Du_Lieu_Share_Parameter

        ObservableCollection<data_item_share_parameter> my_item_share_parameter { get; set; } // Lay_Du_Lieu_Share_Parameter

        ObservableCollection<data_parameter> my_data_parameter_need { get; set; } // Lay_Du_Lieu_Share_Parameter

        ObservableCollection<data_parameter> my_data_parameter_current { get; set; } // Lay_Du_Lieu_Share_Parameter

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_binding_share_parameter = new E_BindingShareParameter();
            e_binding_share_parameter = ExternalEvent.Create(my_binding_share_parameter);

            Function_Dau_Vao();
        }
        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                List<string> file_name = doc.Title.Split('_').ToList();
                if (file_name.Count() > 3)
                {
                    Class = file_name[3];
                    block.Text = string.IsNullOrEmpty(doc.ProjectInformation.BuildingName) ? "xx" : doc.ProjectInformation.BuildingName;
                    F_GetInformation.get_information(doc, number);
                    Lay_Share_Parameter_Da_Ton_Tai();
                    Lay_Du_Lieu();
                }
                else
                {
                    MessageBox.Show("Format file name is incorrect. Please check and try again", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public List<string> Lay_Share_Parameter_Da_Ton_Tai()
        {
            List<string> share_parameter_Da_Ton_Tai = new List<string>();
            try
            {
                my_data_parameter_need = new ObservableCollection<data_parameter>();
                my_data_parameter_current = new ObservableCollection<data_parameter>();

                F_GetShareParameter.get_share_parameter(doc, my_data_parameter_need, my_data_parameter_current);

                thong_tin_parameter_project.ItemsSource = my_data_parameter_current;
                thong_tin_parameter.ItemsSource = my_data_parameter_need;

                CollectionView view_project = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter_project.ItemsSource);
                view_project.SortDescriptions.Add(new SortDescription("ten_parameter", ListSortDirection.Ascending));
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("ten_parameter", ListSortDirection.Ascending));
            }
            catch (Exception)
            {

            }
            return share_parameter_Da_Ton_Tai;
        }

        //----------------------------------------------------------
        public void Lay_Du_Lieu()
        {
            try
            {
                my_group_share_parameter = new ObservableCollection<data_group_share_parameter>();
                my_item_share_parameter = new ObservableCollection<data_item_share_parameter>();

                F_GetGroupShareParameter.get_share_parameter(my_group_share_parameter, my_item_share_parameter, my_data_parameter_current);

                thong_tin_share_parameter.ItemsSource = my_group_share_parameter;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("ten_group_parameter", ListSortDirection.Ascending));

                view.Filter = Filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter(object item)
        {
            if (string.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as data_group_share_parameter).ten_group_parameter.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ((item as data_group_share_parameter).Children.Any(x => x.ten_parameter.Equals(search_material_project.Text, StringComparison.OrdinalIgnoreCase) == true)));
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void search_parameter_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(thong_tin_parameter.SelectedItem != null)
                {
                    data_parameter item = (data_parameter)thong_tin_parameter.SelectedItem;
                    search_material_project.Text = item.ten_parameter;
                    if (!CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource).IsEmpty)
                    {
                        CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource).Cast<data_group_share_parameter>().ToList().ForEach(x => x.ValueExpanded = true);
                        CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource).Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_Item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (data_group_share_parameter group in my_group_share_parameter)
                {
                    List<bool> check = new List<bool>();
                    foreach (data_item_share_parameter parameter in group.Children)
                    {
                        check.Add(parameter.exist_parameter);
                    }
                    if (check.Contains(true) == false)
                    {
                        group.color = Source.color;
                    }
                    else
                    {
                        group.color = Source.color_group;
                    }
                    group.count_check = check.Count(x => x == true).ToString();
                }
                thong_tin_share_parameter.Items.Refresh();
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_Data_Nhu_Ban_Dau(object sender, RoutedEventArgs e)
        {
            try
            {
                search_material_project.Text = "";
                F_GetInformation.get_information(doc, number);
                Lay_Share_Parameter_Da_Ton_Tai();
                Lay_Du_Lieu();
                block.Text = string.IsNullOrEmpty(doc.ProjectInformation.BuildingName) ? "xx" : doc.ProjectInformation.BuildingName;
            }
            catch (Exception)
            {

            }
        }

        private void Them_Hoac_Xoa_Parameter_Trong_Project(object sender, RoutedEventArgs e)
        {
            try
            {
                my_binding_share_parameter.number = number;
                my_binding_share_parameter.block = block;
                my_binding_share_parameter.thong_tin_parameter = thong_tin_parameter;
                my_binding_share_parameter.thong_tin_parameter_project = thong_tin_parameter_project;
                my_binding_share_parameter.thong_tin_share_parameter = thong_tin_share_parameter;
                my_binding_share_parameter.my_group_share_parameter = my_group_share_parameter;
                my_binding_share_parameter.my_data_parameter_need = my_data_parameter_need;
                my_binding_share_parameter.my_data_parameter_current = my_data_parameter_current;
                my_binding_share_parameter.descipline = Class;
                e_binding_share_parameter.Raise();
            }
            catch (Exception)
            {

            }
        }

        private void Sua_Parameter_Bat_Buoc(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Source.path_share_parameter_default);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
