using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using SetupTool_MaterialSetup.Code.External;
using SetupTool_MaterialSetup.Code.Function.FunctionCompany;
using SetupTool_MaterialSetup.Code.Function.FunctionExcel;
using SetupTool_MaterialSetup.Code.Function.FunctionProject;
using SetupTool_MaterialSetup.Code.Function.FunctionTemplate;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace SetupTool_MaterialSetup
{
    public partial class UserControl1 : Window
    {
        #region Instance
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;
        int version;
        string user;
        string project_number;
        string block;
        string Class;

        E_Create my_create;
        ExternalEvent e_create;
        E_Update my_update;
        ExternalEvent e_update;
        E_Delete my_delete;
        ExternalEvent e_delete;
        E_CreateByExcel my_create_by_excel;
        ExternalEvent e_create_by_excel;
        E_ChangeFactor my_change_factor;
        ExternalEvent e_change_factor;

        ColorDialog colorDialog;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_material_factor> my_material_factor { get; set; }
        ObservableCollection<data_material_project> my_material_project { get; set; }
        ObservableCollection<data_material_company> my_material_company { get; set; }
        ObservableCollection<data_material_template> my_material_template { get; set; }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            version = Convert.ToInt32(uiapp.Application.VersionNumber);

            user = uiapp.Application.Username;
            string myIP = Dns.GetHostAddresses(Dns.GetHostName()).First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            string path = Source.pathUserPassword + "\\" + myIP;
            if (File.Exists(path))
            {
                data_information information = JsonConvert.DeserializeObject<data_information>(File.ReadAllText(path));
                user = information.user_name;
            }

            Function_Dau_Vao();
        }
        
        //----------------------------------------------------------
        private void Function_Dau_Vao()
        {
            try
            {
                List<string> file_name = doc.Title.Split('_').ToList();
                project_number = doc.ProjectInformation.Number;
                block = doc.ProjectInformation.BuildingName;
                Class = doc.ProjectInformation.LookupParameter("Class") == null ? "" : doc.ProjectInformation.LookupParameter("Class").AsString();
                if (string.IsNullOrEmpty(Class)) MessageBox.Show("Share Parameter Class not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

                if (file_name.Count() > 3)
                {
                    List<string> format = new List<string>();
                    if (project_number != file_name[0]) format.Add("Project Number");

                    if (block != file_name[1]) format.Add("Block");

                    if (Class != file_name[3]) format.Add("Class");

                    if (format.Count() == 0)
                    {
                        my_material_factor = new ObservableCollection<data_material_factor>();
                        my_material_project = new ObservableCollection<data_material_project>();
                        my_material_company = new ObservableCollection<data_material_company>();
                        my_material_template = new ObservableCollection<data_material_template>();

                        register_external();
                        F_GetUnit.get_unit_company(don_vi);
                        F_GetPattern.get_pattern(doc, name_cut, name_surface);
                        Show_Infor_Material();
                        F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);

                        colorDialog = new ColorDialog();
                        colorDialog.FullOpen = true;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        void register_external()
        {
            my_create = new E_Create();
            e_create = ExternalEvent.Create(my_create);
            my_update = new E_Update();
            e_update = ExternalEvent.Create(my_update);
            my_delete = new E_Delete();
            e_delete = ExternalEvent.Create(my_delete);
            my_create_by_excel = new E_CreateByExcel();
            e_create_by_excel = ExternalEvent.Create(my_create_by_excel);
            my_change_factor = new E_ChangeFactor();
            e_change_factor = ExternalEvent.Create(my_change_factor);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Material Project
        //-----------------------------------------------------------
        private void Show_Infor_Material()
        {
            try
            {
                my_material_project = new ObservableCollection<data_material_project>();
                F_GetMaterialProject.get_material(doc, my_material_project);
                thong_tin_vat_lieu_project.ItemsSource = my_material_project;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_project.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_project", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu_project", ListSortDirection.Ascending));

                // filter list view theo tên vật liệu
                view.Filter = Filter_Project;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private bool Filter_Project(object item)
        {
            if (String.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as data_material_project).ma_cong_tac_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_project).ten_vat_lieu_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Support.RemoveUnicode((item as data_material_project).ten_vat_lieu_project).IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_project).don_vi_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_project).user.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_project).time.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //-----------------------------------------------------------
        private void Choose_Color_Shading(object sender, RoutedEventArgs e)
        {
            color_shading.Background = F_ChooseColor.choose_color(color_shading, color_surface, color_cut, colorDialog);
        }

        //-----------------------------------------------------------
        private void Choose_Color_Surface(object sender, RoutedEventArgs e)
        {
            color_surface.Background = F_ChooseColor.choose_color(color_shading, color_surface, color_cut, colorDialog);
        }

        //-----------------------------------------------------------
        private void Choose_Color_Cut(object sender, RoutedEventArgs e)
        {
            color_cut.Background = F_ChooseColor.choose_color(color_shading, color_surface, color_cut, colorDialog);
        }

        //-----------------------------------------------------------
        private void Choose_Surface(object sender, EventArgs e)
        {
            if (name_surface.SelectedItem != null)
            {
                data_fill_pattern surface = (data_fill_pattern)name_surface.SelectedItem;

            }
        }

        private void Choose_Cut(object sender, EventArgs e)
        {
            if (name_cut.SelectedItem != null)
            {
                data_fill_pattern cut = (data_fill_pattern)name_cut.SelectedItem;

            }
        }

        //-----------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu(object sender, MouseButtonEventArgs e)
        {
            F_SelectMaterialProject.select_material(doc, thong_tin_vat_lieu_project, ten_vat_lieu, ma_cong_tac, don_vi, color_shading, color_surface, color_cut, tranparency_bar, ton_value, name_surface, name_cut);
        }

        //----------------------------------------------------------
        private void Xoa_Vat_Lieu(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_vat_lieu_project.SelectedItems.Count > 0)
                {
                    my_delete.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
                    my_delete.my_material_project = my_material_project;
                    my_delete.my_material_factor = my_material_factor;
                    my_delete.thong_tin_he_so_vat_lieu_project = thong_tin_he_so_vat_lieu_project;
                    e_delete.Raise();
                }
                else
                {
                    MessageBox.Show("Please choose material and delete again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private void Change_Ton_Kg_Factor(object sender, RoutedEventArgs e)
        {
            try
            {
                my_change_factor.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
                my_change_factor.user = user;
                my_change_factor.my_material_project = my_material_project;
                my_change_factor.my_material_factor = my_material_factor;
                my_change_factor.thong_tin_he_so_vat_lieu_project = thong_tin_he_so_vat_lieu_project;
                e_change_factor.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private void Cancel_Change_Factor_Ton_Kg(object sender, RoutedEventArgs e)
        {
            thong_tin_he_so_vat_lieu_project.Items.Refresh();
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Material Company
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Chon_Vat_Lieu_Tu_Company_Database(object sender, RoutedEventArgs e)
        {
            try
            {
                if (company.IsChecked == true) project.IsChecked = true;
                else company.IsChecked = true;

                if (my_material_company.Count() == 0)
                {
                    get_material_company();
                }
                search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void get_material_company()
        {
            try
            {
                my_material_company = new ObservableCollection<data_material_company>();
                F_GetMaterialCompany.get_material(my_material_company);
                thong_tin_vat_lieu_company.ItemsSource = my_material_company;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_company.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_company", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu_company", ListSortDirection.Ascending));

                view.Filter = Filter_Company;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private bool Filter_Company(object item)
        {
            if (String.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as data_material_company).ma_cong_tac_company.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_company).ten_vat_lieu_company.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Support.RemoveUnicode((item as data_material_company).ten_vat_lieu_company).IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu_Company(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_vat_lieu_company.SelectedItem != null)
                {
                    data_material_company item = (data_material_company)thong_tin_vat_lieu_company.SelectedItem;
                    ten_vat_lieu.Text = item.ten_vat_lieu_company;
                    ma_cong_tac.Text = item.ma_cong_tac_company;
                    don_vi.Text = item.don_vi_company;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Xoa_Vat_Lieu_Company(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_vat_lieu_company.SelectedItems.Count > 0)
                {
                    string result = F_DeleteMaterialCompany.delete_material(thong_tin_vat_lieu_company, my_material_company);
                    if (result == "S")
                    {
                        thong_tin_vat_lieu_company.Items.Refresh();
                        MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please choose material and delete again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Material Template
        //----------------------------------------------------------
        private void Chon_Vat_Lieu_Tu_Template_Database(object sender, RoutedEventArgs e)
        {
            try
            {
                if (template.IsChecked == true) project.IsChecked = true;
                else template.IsChecked = true;

                if (my_material_template.Count() == 0)
                {
                    get_material_template();
                }
                search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------------------------
        private void get_material_template()
        {
            try
            {
                my_material_template = new ObservableCollection<data_material_template>();
                F_GetMaterialTemplate.get_material(my_material_template);
                thong_tin_vat_lieu_template.ItemsSource = my_material_template;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_template.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_template_MCT", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu_template_MCT", ListSortDirection.Ascending));
                view.Filter = Filter_Tempalte;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private bool Filter_Tempalte(object item)
        {
            if (String.IsNullOrEmpty(search_material_project.Text))
                return true;
            else
                return ((item as data_material_template).ma_cong_tac_template_MCT.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as data_material_template).ten_vat_lieu_template_MCT.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Support.RemoveUnicode((item as data_material_template).ten_vat_lieu_template_MCT).IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu_Template(object sender, MouseButtonEventArgs e)
        {
            Xem_Thong_Tin_Vat_Lieu();
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin_Vat_Lieu()
        {
            try
            {
                if (thong_tin_vat_lieu_template.SelectedItem != null)
                {
                    data_material_template item = (data_material_template)thong_tin_vat_lieu_template.SelectedItem;
                    List<Material_Sup_Template> myMaterial_Sub_Template = new List<Material_Sup_Template>();
                    List<Material_DM_Template> myMaterial_DM_Template = new List<Material_DM_Template>();

                    var list_DM = SQL.SQLRead(Source.path_Quantity, "dbo.spRead_DM", Source.type_Procedure, new List<string>() { "@DBSubId" }, new List<string>() { item.ma_cong_tac_template_MCT_DM });
                    int rows_DM = list_DM.Rows.Count;
                    for (var i = 0; i < rows_DM; i++)
                    {
                        myMaterial_DM_Template.Add(new Material_DM_Template()
                        {
                            so_thu_tu_DM = Convert.ToInt32(list_DM.Rows[i]["STT"].ToString()),
                            ma_cong_tac_template_MCT_DM = list_DM.Rows[i]["DM_SubId"].ToString(),
                            ma_cong_tac_template_DM_VL = list_DM.Rows[i]["MaterialCode_DM"].ToString(),
                            so_luong_template_DM = list_DM.Rows[i]["Amount"].ToString()
                        });
                    }
                    foreach (Material_DM_Template DM in myMaterial_DM_Template)
                    {
                        var list_VL = SQL.SQLRead(Source.path_Quantity, "dbo.spRead_VL", Source.type_Procedure, new List<string>() { "@DBMaterialCode_DM" }, new List<string>() { DM.ma_cong_tac_template_DM_VL });
                        int rows_VL = list_VL.Rows.Count;
                        if (rows_VL > 0)
                        {
                            myMaterial_Sub_Template.Add(new Material_Sup_Template()
                            {
                                no_sup_template = DM.so_thu_tu_DM,
                                ten_vat_lieu_sup_template = list_VL.Rows[0]["MaterialName"].ToString(),
                                so_luong_sup_template = DM.so_luong_template_DM,
                                don_vi_sup_template = list_VL.Rows[0]["Unit"].ToString()
                            });
                        }
                    }
                    thong_tin_vat_lieu_sup_template.ItemsSource = myMaterial_Sub_Template;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_sup_template.ItemsSource);
                    // sort list view
                    view.SortDescriptions.Add(new SortDescription("no_sub_template", ListSortDirection.Ascending));

                    ten_vat_lieu.Text = item.ten_vat_lieu_template_MCT;
                    ma_cong_tac.Text = item.ma_cong_tac_template_MCT;
                    don_vi.Text = item.don_vi_template_MCT;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Material Excel
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Tao_Vat_Lieu_Tu_Excel_Database(object sender, RoutedEventArgs e)
        {
            Tao_Vat_Lieu_Tu_Excel();
        }

        //----------------------------------------------------------
        private void Tao_Vat_Lieu_Tu_Excel()
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = file.FileName;
                    List<data_material_excel> data_create = new List<data_material_excel>();
                    F_GetMaterialExcel.get_material(data_create, path, name_surface);

                    my_create_by_excel.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
                    my_create_by_excel.my_material_project = my_material_project;
                    my_create_by_excel.my_material_factor = my_material_factor;
                    my_create_by_excel.thong_tin_he_so_vat_lieu_project = thong_tin_he_so_vat_lieu_project;
                    my_create_by_excel.data_create = data_create;
                    my_create_by_excel.user = user;
                    e_create_by_excel.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-----------------------------------------------------------
        private void export_excel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if(save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = save.FileName;
                    string result = F_ExportExcel.Export_Excel(my_material_project, path, "Material", name_surface);
                    if(result == "S")
                    {
                        MessageBox.Show("Export Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Material All
        //-----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            search();
        }

        void search()
        {
            if (project.IsChecked == true && my_material_project.Count() > 0) CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_project.ItemsSource).Refresh();
            if (company.IsChecked == true && my_material_company.Count() > 0) CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_company.ItemsSource).Refresh();
            if (template.IsChecked == true && my_material_template.Count() > 0) CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_template.ItemsSource).Refresh();
        }

        //----------------------------------------------------------
        private void Tao_Vat_Lieu_Moi(object sender, RoutedEventArgs e)
        {
            try
            {
                if (project.IsChecked == true)
                {
                    my_create.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
                    my_create.ten_vat_lieu = ten_vat_lieu;
                    my_create.ma_cong_tac = ma_cong_tac;
                    my_create.don_vi = don_vi;
                    my_create.use_appearence = use_appearence;
                    my_create.tranparency_value = tranparency_value;
                    my_create.color_shading = color_shading;
                    my_create.color_surface = color_surface;
                    my_create.color_cut = color_cut;
                    my_create.name_surface = name_surface;
                    my_create.name_cut = name_cut;
                    my_create.ton_value = ton_value;
                    my_create.user = user;
                    my_create.my_material_project = my_material_project;
                    my_create.my_material_factor = my_material_factor;
                    my_create.thong_tin_he_so_vat_lieu_project = thong_tin_he_so_vat_lieu_project;
                    e_create.Raise();
                }
                if(company.IsChecked == true)
                {
                    string result = F_AddMaterialCompany.add_material(thong_tin_vat_lieu_company, my_material_company, ten_vat_lieu, ma_cong_tac, don_vi, user);
                    if (result == "S")
                    {
                        thong_tin_vat_lieu_company.Items.Refresh();
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        private void Sua_Thong_Tin_Vat_Lieu(object sender, RoutedEventArgs e)
        {
            try
            {
                if (project.IsChecked == true)
                {
                    if (thong_tin_vat_lieu_project.SelectedItem != null)
                    {
                        my_update.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
                        my_update.ten_vat_lieu = ten_vat_lieu;
                        my_update.ma_cong_tac = ma_cong_tac;
                        my_update.don_vi = don_vi;
                        my_update.use_appearence = use_appearence;
                        my_update.tranparency_value = tranparency_value;
                        my_update.color_shading = color_shading;
                        my_update.color_surface = color_surface;
                        my_update.color_cut = color_cut;
                        my_update.name_surface = name_surface;
                        my_update.name_cut = name_cut;
                        my_update.ton_value = ton_value;
                        my_update.user = user;
                        my_update.my_material_project = my_material_project;
                        my_update.my_material_factor = my_material_factor;
                        my_update.thong_tin_he_so_vat_lieu_project = thong_tin_he_so_vat_lieu_project;
                        e_update.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Please choose material and update again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if (company.IsChecked == true)
                {
                    if (thong_tin_vat_lieu_company.SelectedItem != null)
                    {
                        string result = F_UpdateMaterialCompany.update_material(thong_tin_vat_lieu_company, my_material_company, ten_vat_lieu, ma_cong_tac, don_vi, user);
                        if (result == "S")
                        {
                            thong_tin_vat_lieu_company.Items.Refresh();
                            MessageBox.Show("Update Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please choose material and update again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        private void Refresh_All_Du_Lieu(object sender, RoutedEventArgs e)
        {
            F_GetPattern.get_pattern(doc, name_cut, name_surface);
            Show_Infor_Material();
            F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);
        }

        #endregion
    }
}
