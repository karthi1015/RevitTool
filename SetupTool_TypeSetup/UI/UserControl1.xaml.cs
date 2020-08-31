using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_TypeSetup.Code.External;
using SetupTool_TypeSetup.Code.Function;
using SetupTool_TypeSetup.Data.Binding;
using SetupTool_TypeSetup.Data.BindingCompany;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SetupTool_TypeSetup
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;
        string project_number;
        string block;
        string Class;
        string unit_length;

        E_Create my_create;
        ExternalEvent e_create;
        E_Delete my_delete;
        ExternalEvent e_delete;
        E_Update my_update;
        ExternalEvent e_update;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_family> my_family { get; set; }
        ObservableCollection<data_parameters> my_parameters { get; set; }
        ObservableCollection<data_materials> my_materials { get; set; }
        ObservableCollection<data_material> my_material { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            register_external();

            Function_Dau_Vao();
        }

        //----------------------------------------------------------
        void register_external()
        {
            my_create = new E_Create();
            e_create = ExternalEvent.Create(my_create);

            my_delete = new E_Delete();
            e_delete = ExternalEvent.Create(my_delete);

            my_update = new E_Update();
            e_update = ExternalEvent.Create(my_update);
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            List<string> file_name = doc.Title.Split('_').ToList();
            project_number = doc.ProjectInformation.Number;
            block = doc.ProjectInformation.BuildingName;
            Class = doc.ProjectInformation.LookupParameter("Class") == null ? "" : doc.ProjectInformation.LookupParameter("Class").AsString();
            if (string.IsNullOrEmpty(Class)) MessageBox.Show("Share Parameter Class not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

            if (file_name.Count() > 3)
            {
                unit_length = F_GetUnits.get_unit_length_type(doc);
                List<string> format = new List<string>();
                if (project_number != file_name[0]) format.Add("Project Number");

                if (block != file_name[1]) format.Add("Block");

                if (Class != file_name[3]) format.Add("Class");

                if (format.Count() == 0)
                {
                    F_GetDesciplineAndCategory.get_descipline_and_category(descipline, category);
                    CollectionView view_category = (CollectionView)CollectionViewSource.GetDefaultView(category.ItemsSource);
                    view_category.Filter = Filter_category;

                    my_family = new ObservableCollection<data_family>();
                    my_parameters = new ObservableCollection<data_parameters>();
                    my_materials = new ObservableCollection<data_materials>();
                    my_material = new ObservableCollection<data_material>();
                    ElementType type = F_TypeDefault.get_type_select(uidoc, doc, descipline, category, outhome, inhome, kc);
                    Show_Tat_Ca_Family_Type();
                    if (type != null)
                    {
                        foreach (data_family family in my_family)
                        {
                            if (family.ten_family_type == type.FamilyName)
                            {
                                family.ValueExpanded = true;
                                foreach (data_type element_Type in family.Children)
                                {
                                    if (element_Type.ten_element_type == type.Name)
                                    {
                                        element_Type.ValueIsSelect = true;
                                        F_ViewDetail.view_detail(doc, element_Type, my_parameters, my_materials, my_material, thong_tin_kich_thuoc, thong_tin_cong_tac_vat_lieu, unit_length);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Data is incorrect.\nPlease check {0} and try again!", string.Join(",", format)), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("File name is incorrect. Please check and try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        //----------------------------------------------------------
        private bool Filter_category(object item)
        {
            if (descipline.SelectedItem == null)
                return true;
            else
            {
                data_name_key data = (data_name_key)descipline.SelectedItem;
                return (item as data_name_key).descipline_key == data.key;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Element_Type_By_Category(object sender, EventArgs e)
        {
            Show_Tat_Ca_Family_Type();
        }

        //-----------------------------------------------------------
        public void Show_Tat_Ca_Family_Type()
        {
            try
            {
                my_family = new ObservableCollection<data_family>();
                my_parameters = new ObservableCollection<data_parameters>();
                my_materials = new ObservableCollection<data_materials>();
                thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<data_parameters>();
                thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<data_materials>();

                F_ShowTreeView.show_data(doc, descipline, category, outhome, inhome, kc, my_family);
                thong_tin_family_type.ItemsSource = my_family;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_family_type.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_family();
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
                return ((item as data_family).ten_family_type.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ((item as data_family).Children.Any(x => x.ten_element_type.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0)));
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(thong_tin_family_type.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Element_Type(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem != null)
                {
                    my_parameters = new ObservableCollection<data_parameters>();
                    my_materials = new ObservableCollection<data_materials>();
                    my_material = new ObservableCollection<data_material>();
                    thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<data_parameters>();
                    thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<data_materials>();
                    if (thong_tin_family_type.SelectedItem is data_type)
                    {
                        data_type type = (data_type)thong_tin_family_type.SelectedItem;
                        F_ViewDetail.view_detail(doc, type, my_parameters, my_materials, my_material, thong_tin_kich_thuoc, thong_tin_cong_tac_vat_lieu, unit_length);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Change_Name_Descipline(object sender, EventArgs e)
        {
            CollectionViewSource.GetDefaultView(category.ItemsSource).Refresh();
            category.SelectedIndex = 0;
            Show_Tat_Ca_Family_Type();
        }

        private void Change_Name_Position(object sender, RoutedEventArgs e)
        {
            Show_Tat_Ca_Family_Type();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_All_Du_Lieu(object sender, RoutedEventArgs e)
        {
            my_family = new ObservableCollection<data_family>();
            my_parameters = new ObservableCollection<data_parameters>();
            my_materials = new ObservableCollection<data_materials>();
            my_material = new ObservableCollection<data_material>();
            thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<data_parameters>();
            thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<data_materials>();
            thong_tin_family_type.ItemsSource = new ObservableCollection<data_materials>();
            ElementType type = F_TypeDefault.get_type_select(uidoc, doc, descipline, category, outhome, inhome, kc);
            Show_Tat_Ca_Family_Type();
            if (type != null)
            {
                foreach (data_family family in my_family)
                {
                    if (family.ten_family_type == type.FamilyName)
                    {
                        family.ValueExpanded = true;
                        foreach (data_type element_Type in family.Children)
                        {
                            if (element_Type.ten_element_type == type.Name)
                            {
                                element_Type.ValueIsSelect = true;
                                F_ViewDetail.view_detail(doc, element_Type, my_parameters, my_materials, my_material, thong_tin_kich_thuoc, thong_tin_cong_tac_vat_lieu, unit_length);
                            }
                        }
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Sua_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem != null)
                {
                    if (thong_tin_family_type.SelectedItem is data_type)
                    {
                        my_update.unit_length = unit_length;
                        my_update.name = name;
                        my_update.thong_tin_cong_tac_vat_lieu = thong_tin_cong_tac_vat_lieu;
                        my_update.thong_tin_family_type = thong_tin_family_type;
                        my_update.thong_tin_kich_thuoc = thong_tin_kich_thuoc;
                        my_update.my_family = my_family;
                        my_update.my_materials = my_materials;
                        my_update.my_parameters = my_parameters;
                        e_update.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Please choose type and update again!!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Tao_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem != null)
                {
                    if (thong_tin_family_type.SelectedItem is data_type)
                    {
                        my_create.unit_length = unit_length;
                        my_create.name = name;
                        my_create.thong_tin_cong_tac_vat_lieu = thong_tin_cong_tac_vat_lieu;
                        my_create.thong_tin_family_type = thong_tin_family_type;
                        my_create.thong_tin_kich_thuoc = thong_tin_kich_thuoc;
                        my_create.my_family = my_family;
                        my_create.my_materials = my_materials;
                        my_create.my_parameters = my_parameters;
                        e_create.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Please choose type and duplicate again!!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                my_delete.thong_tin_cong_tac_vat_lieu = thong_tin_cong_tac_vat_lieu;
                my_delete.thong_tin_family_type = thong_tin_family_type;
                my_delete.thong_tin_kich_thuoc = thong_tin_kich_thuoc;
                my_delete.my_family = my_family;
                e_delete.Raise();
            }
            catch (Exception)
            {

            }
        }
    }
}
