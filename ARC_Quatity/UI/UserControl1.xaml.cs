using ARC_Quatity.Code;
using ARC_Quatity.Code.Function;
using ARC_Quatity.Code.FunctionQuantityInput;
using ARC_Quatity.Code.FunctionWEB;
using ARC_Quatity.Data;
using ARC_Quatity.Data.Binding;
using ARC_Quatity.Data.BindingWEB;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Path = System.IO.Path;

namespace ARC_Quatity
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        E_VisibleLink my_visible_link;
        ExternalEvent e_visible_link;

        string project_number;
        string block;
        string Class;
        string unit_length;
        string id_file;
        string user;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_element_link> my_element_link { get; set; }
        ObservableCollection<data_quantity> my_quantity_total { get; set; }
        ObservableCollection<data_quantity> my_quatity_item { get; set; }
        ObservableCollection<data_quantity> my_quantity_detail { get; set; }
        ObservableCollection<data_file_link> my_file_link { get; set; }
        ObservableCollection<data_table_note> my_table_note { get; set; }
        List<string> material_of_element_in_project { get; set; }
        List<data_quantity> my_quantity_input { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            user = uiapp.Application.Username;
            string myIP = Dns.GetHostAddresses(Dns.GetHostName()).First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            string path = Source.pathUserPassword + "\\" + myIP;
            if (File.Exists(path))
            {
                data_information information = JsonConvert.DeserializeObject<data_information>(File.ReadAllText(path));
                user = information.user_name;
            }

            my_visible_link = new E_VisibleLink();
            e_visible_link = ExternalEvent.Create(my_visible_link);

            Function_Dau_Vao();
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
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
                        id_file = file_name[0] + "_" + file_name[1] + "_" + file_name[2] + "_" + file_name[3] + "_";
                        unit_length = F_GetUnits.get_unit_length_type(doc);

                        input_block.Text = block;
                        F_Material.get_material_combobox(doc, input_material);

                        show_data_input(id_file, thong_tin_quantity_input);
                        material_of_element_in_project = new List<string>();

                        F_QuantityNotes.Get_Data_Notes_Web(project_number, Class, my_table_note, thong_tin_quantity_total_web);
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Show_Khoi_Luong_Len_ListView
        //----------------------------------------------------------
        private void get_quantity_all_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> file_name = doc.Title.Split('_').ToList();
                if (file_name.Count() > 3 && file_name[0] == project_number && file_name[1] == block && file_name[3] == Class)
                {
                    my_element_link = new ObservableCollection<data_element_link>();
                    my_quantity_total = new ObservableCollection<data_quantity>();
                    my_quatity_item = new ObservableCollection<data_quantity>();
                    my_quantity_detail = new ObservableCollection<data_quantity>();
                    my_file_link = new ObservableCollection<data_file_link>();

                    F_GetElement.Get_ELement_Link_Or_NoLink(doc, my_element_link);

                    Show_Khoi_Luong_Len_ListView();
                    material_of_element_in_project = my_quantity_total.Select(x => x.ma_cong_tac).ToList();

                    F_Chart.Show_Du_Lieu_Len_Chart(bieu_do_category, DataContext, this, my_element_link);
                    Search();
                }
                else
                {
                    MessageBox.Show("Thông tin dự án không đồng nhất. Vui lòng kiểm tra lại!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        //----------------------------------------------------------
        public void Show_Khoi_Luong_Len_ListView()
        {
            try
            {
                my_quatity_item = new ObservableCollection<data_quantity>();
                my_quantity_detail = new ObservableCollection<data_quantity>();

                List<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();

                foreach (data_element_link element_link in my_element_link)
                {
                    Element element = element_link.cau_kien;
                    Document doc = element_link.doc;

                    ObservableCollection<data_quantity> my_quatity_item_support = new ObservableCollection<data_quantity>();
                    ObservableCollection<data_quantity> my_quantity_detail_support = new ObservableCollection<data_quantity>();
                    if (element is FamilyInstance)
                    {
                        FamilyInstance familyInstance = element as FamilyInstance;
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, element, levels, Class, unit_length, block, my_quatity_item_support);
                        List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                        foreach (ElementId id in elements_child)
                        {
                            FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                            F_GetQuantity.Support_Get_Khoi_Luong(doc, model_child, levels, Class, unit_length, block, my_quatity_item_support);
                            Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item_support, doc, Class, unit_length, block);
                        }
                    }
                    else
                    {
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, element, levels, Class, unit_length, block, my_quatity_item_support);
                    }

                    my_quatity_item_support.ToList().ForEach(x => my_quatity_item.Add(x));
                    my_quatity_item_support.ToList().ForEach(x => my_quantity_detail.Add(x));
                }

                my_quantity_total = new ObservableCollection<data_quantity>(my_quatity_item.GroupBy(x => new
                {
                    x.block,
                    x.level,
                    x.ten_vat_lieu,
                    x.ma_cong_tac,
                    x.don_vi,
                    x.color,
                    x.elevation,
                    x.link_file_name
                }).Select(y => new data_quantity()
                {
                    block = y.Key.block,
                    level = y.Key.level,
                    ten_vat_lieu = y.Key.ten_vat_lieu,
                    ma_cong_tac = y.Key.ma_cong_tac,
                    quantity = y.Sum(x => x.quantity),
                    don_vi = y.Key.don_vi,
                    color = y.Key.color,
                    color_sort = y.Key.color.ToString(),

                    elevation = y.Key.elevation,
                    link_file_name = y.Key.link_file_name,
                    ten_cau_kien = string.Join("|", y.Select(x => x.ten_cau_kien))
                }));

                thong_tin_quantity_total_project.ItemsSource = my_quantity_total;
                thong_tin_quantity_project.ItemsSource = my_quatity_item;
                thong_tin_detail.ItemsSource = my_quantity_detail;

                CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                view_total.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view_total.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Descending));
                view_total.SortDescriptions.Add(new SortDescription("ma_cong_tac", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("ten_cau_kien", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                view_detail.SortDescriptions.Add(new SortDescription("ten_cau_kien", ListSortDirection.Ascending));
                view_detail.Filter = Filter_thong_tin_detail;

                bieu_do_download.Value = view_total.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        void Support_Show_Khoi_Luong_Len_ListView(FamilyInstance model_child, List<Level> levels, ObservableCollection<data_quantity> my_quatity_item_support, Document doc, string Class, string unit_length, string block)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, model_child, levels, Class, unit_length, block, my_quatity_item_support);
                        Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item_support, doc, Class, unit_length, block);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public bool Filter_thong_tin_detail(object item)
        {
            bool value = false;
            try
            {
                if (item_select == null)
                    return false;
                else
                    return ((item as data_quantity).ten_vat_lieu.Equals(item_select.ten_vat_lieu, StringComparison.OrdinalIgnoreCase) &&
                            (item as data_quantity).block.Equals(item_select.block, StringComparison.OrdinalIgnoreCase) &&
                            (item as data_quantity).level.Equals(item_select.level, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return value;
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Thao tác trên UI
        //----------------------------------------------------------
        data_quantity item_select = null;
        private void Chon_Vat_Lieu_total(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_total_project.SelectedItem != null)
                {
                    item_select = (data_quantity)thong_tin_quantity_total_project.SelectedItem;
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                    view.Filter = Filter_thong_tin_detail;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_thong_tin_total(object item)
        {
            string search = "";
            try
            {
                if (string.IsNullOrEmpty(search_material_project.Text))
                {
                    return true;
                }
                if (search_block.IsChecked == true)
                {
                    search = (item as data_quantity).block;
                }
                if (search_level.IsChecked == true)
                {
                    search += (item as data_quantity).level;
                }
                if (search_material.IsChecked == true)
                {
                    search += (item as data_quantity).ma_cong_tac;
                }
                if (search_element.IsChecked == true)
                {
                    search += string.Join("", (item as data_quantity).ten_cau_kien.Split('|'));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return search.IndexOf(search_material_project.Text.Replace("_", ""), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        //----------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_total(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //----------------------------------------------------------
        private void Chon_Cau_Kien_detail(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<ElementId> ids = new List<ElementId>();
                for (int i = 0; i < thong_tin_detail.SelectedItems.Count; i++)
                {
                    data_quantity item_select = (data_quantity)thong_tin_detail.SelectedItems[i];
                    ids.Add(item_select.cau_kien.Id);
                }
                Selection selection = uidoc.Selection;
                selection.SetElementIds(ids);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Chon_Cau_Kien_element(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<ElementId> ids = new List<ElementId>();
                for (int i = 0; i < thong_tin_quantity_project.SelectedItems.Count; i++)
                {
                    data_quantity item_select = (data_quantity)thong_tin_quantity_project.SelectedItems[i];
                    ids.Add(item_select.cau_kien.Id);
                }
                Selection selection = uidoc.Selection;
                selection.SetElementIds(ids);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_element(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        //----------------------------------------------------------
        public void Search()
        {
            try
            {
                if (total.IsChecked == true)
                {
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                    view.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Refresh();
                }
                if (any.IsChecked == true)
                {
                    CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                    view1.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Refresh();
                }
                if (new_quantity.IsChecked == true)
                {
                    CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_input.ItemsSource);
                    view1.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_input.ItemsSource).Refresh();
                }

                if (any.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    item_select = null;
                    CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                    view_detail.Filter = Filter_thong_tin_detail;
                    bieu_do_download.Value = thong_tin_quantity_project.Items.Count;
                }

                if (total.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    item_select = (data_quantity)thong_tin_quantity_total_project.SelectedItem;
                    CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                    view_detail.Filter = Filter_thong_tin_detail;
                    bieu_do_download.Value = thong_tin_quantity_total_project.Items.Count;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Search_By(object sender, RoutedEventArgs e)
        {
            Search();
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Link File
        //----------------------------------------------------------
        public void Get_List_Link_File(bool check)
        {
            try
            {
                my_file_link = new ObservableCollection<data_file_link>();
                List<Level> levels = new List<Level>();
                new FilteredElementCollector(doc).OfClass(typeof(Level)).ToList().ForEach(x => levels.Add(x as Level));
                var list_link = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList();
                foreach (var link in list_link)
                {
                    List<string> file_name = link.Name.Split('_').ToList();
                    if (file_name.Count() > 3 && file_name[0] == project_number && file_name[1] == block && file_name[3] == Class)
                    {
                        double elevetion = 1000000000;
                        if (levels.Any(y => Support.RemoveUnicode(y.Name) == file_name[2]))
                        {
                            elevetion = levels.First(y => Support.RemoveUnicode(y.Name) == file_name[2]).Elevation;
                        }
                        my_file_link.Add(new data_file_link()
                        {
                            ten_file = link.Name.Split('.')[0],
                            chon_file_link = check,
                            elevation = elevetion,
                            link_file = link
                        });
                    }
                }
                thong_tin_link_file.ItemsSource = my_file_link;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_link_file.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("chon_file_link", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("ten_file", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Khoi_Luong_Cua_Link_File()
        {
            try
            {
                List<data_element_link> list_remove = new List<data_element_link>();
                foreach (data_file_link file in my_file_link)
                {
                    Document doc1 = uiapp.Application.Documents.Cast<Document>().First(x => x.Title == file.ten_file);
                    if (file.chon_file_link == true)
                    {
                        if (my_element_link.Any(x => x.doc.Title == doc1.Title) == false)
                        {
                            my_element_link.Where(x => x.doc.Title == doc1.Title).ToList().ForEach(y => list_remove.Add(y));

                            F_GetElement.Get_ELement_Link_Or_NoLink(doc1, my_element_link);
                        }
                    }
                    else
                    {
                        my_element_link.Where(x => x.doc.Title == doc1.Title).ToList().ForEach(y => list_remove.Add(y));
                    }
                }
                list_remove.ForEach(x => my_element_link.Remove(x));

                if (my_file_link.Count() > 0)
                {
                    Show_Khoi_Luong_Len_ListView();

                    F_Chart.Show_Du_Lieu_Len_Chart(bieu_do_category, DataContext, this, my_element_link);
                }

                e_visible_link.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Bao_Gom_Ca_Link_File(object sender, RoutedEventArgs e)
        {
            try
            {
                Get_List_Link_File(false);
                Khoi_Luong_Cua_Link_File();
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Lay_Hoac_Khong_Lay_Khoi_Luong_Link(object sender, RoutedEventArgs e)
        {
            try
            {
                Khoi_Luong_Cua_Link_File();
                Search();
                if (my_file_link.Any(x => x.chon_file_link != true) == false) check_all.IsChecked = true;
                if (my_file_link.Any(x => x.chon_file_link == true) == false) check_all.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_Tat_Ca_Link_File(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (data_file_link link in my_file_link)
                {
                    link.chon_file_link = check_all.IsChecked.Value;
                }
                thong_tin_link_file.Items.Refresh();

                Khoi_Luong_Cua_Link_File();
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Upload
        private void Upload_Du_Lieu(object sender, RoutedEventArgs e)
        {
            string result = F_UploadData.Upload(uiapp, doc, thong_tin_quantity_total_project, thong_tin_quantity_project, material_of_element_in_project, id_file, user);
            if (result == "S") MessageBox.Show("Upload Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region WEB
        //----------------------------------------------------------
        void Get_Source()
        {
            try
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                view.Filter = Filter_thong_tin_total;
                CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Refresh();

                CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                view1.Filter = Filter_thong_tin_total;
                CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Refresh();

                CollectionView view2 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_input.ItemsSource);
                view2.Filter = Filter_thong_tin_total;
                CollectionViewSource.GetDefaultView(thong_tin_quantity_input.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Chon_Notes_WEB(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_total_web.SelectedItem != null)
                {
                    data_table_note item = (data_table_note)thong_tin_quantity_total_web.SelectedItem;
                    search_material_project.Text = item.block + "_" + item.level + "_" + item.ma_cong_tac;

                    search_block.IsChecked = true;
                    search_level.IsChecked = true;
                    search_material.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Upload_Du_Lieu_WEB(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_total_web.SelectedItem != null)
                {
                    data_table_note item = (data_table_note)thong_tin_quantity_total_web.SelectedItem;
                    Get_Source();
                    string result = "F";
                    if (thong_tin_quantity_total_project.Items.Count > 0)
                    {
                        result = F_UploadData.Upload(uiapp, doc, thong_tin_quantity_total_project, thong_tin_quantity_project, material_of_element_in_project, id_file, user);
                    }
                    if (thong_tin_quantity_input.Items.Count > 0)
                    {
                        F_SaveData.save_data_input(uiapp, doc, input_block, input_level, input_material, input_id, input_quantity, input_unit, id_file, my_quantity_input, false, user);
                        show_data_input(id_file, thong_tin_quantity_input);
                        result = "S";
                    }
                    if (result == "S")
                    {
                        MessageBox.Show("Upload Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                        List<string> para = new List<string>() { "@DBProjectNumber", "@DBId", "@DBStatus" };
                        for (int i = 0; i < item.ids.Count(); i++)
                        {
                            var a = JsonConvert.SerializeObject(new data_status() { open = false, edited = item.editeds[i] });
                            List<object> para_value = new List<object>() { project_number, item.ids[i], a };
                            SQL.SQLWrite(Source.path_WEB, "dbo.spUpdate_OnlyStatusQuantityNotes", Source.type_Procedure, para, para_value);
                        }
                        F_QuantityNotes.Get_Data_Notes_Web(project_number, Class, my_table_note, thong_tin_quantity_total_web);
                    }
                    else
                    {
                        MessageBox.Show("Missing data. Upload fail!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
        #region thêm khối lượng tính tay
        //----------------------------------------------------------
        private void show_data_input(string id_file, ListView thong_tin_quantity_input)
        {
            try
            {
                my_quantity_input = new List<data_quantity>();
                var para = new List<string>() { "@DBProjectNumber", "@DBIdFile", "@DBInput" };
                var para_value = new List<string>() { project_number, id_file, "Input" };
                var data = SQL.SQLRead(Source.path_revit, "dbo.sp_read_quantity_by_idfile", Source.type_Procedure, para, para_value);
                if (data.Rows.Count > 0)
                {
                    List<data_quantity> data_input = new List<data_quantity>();
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        data_input.Add(new data_quantity()
                        {
                            block = data.Rows[i]["BuildingName"].ToString(),
                            level = data.Rows[i]["Level"].ToString(),
                            ten_vat_lieu = data.Rows[i]["MaterialName"].ToString(),
                            ma_cong_tac = data.Rows[i]["ID"].ToString(),
                            quantity = Convert.ToDouble(data.Rows[i]["Quantity"]),
                            don_vi = data.Rows[i]["Unit"].ToString(),
                        });
                    }
                    thong_tin_quantity_input.ItemsSource = data_input;
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_input.ItemsSource);
                    view.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Descending));
                    view.SortDescriptions.Add(new SortDescription("ma_cong_tac", ListSortDirection.Ascending));
                    view.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                    view.Filter = Filter_thong_tin_total;

                    my_quantity_input = data_input;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void select_material_quantity_Click(object sender, EventArgs e)
        {
            try
            {
                if (input_material.SelectedItem != null)
                {
                    data_material item = (data_material)input_material.SelectedItem;
                    input_id.Text = item.id;
                    input_unit.Text = item.unit;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void input_quantity_data_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(input_block.Text) && !string.IsNullOrEmpty(input_level.Text) && input_material.SelectedItem != null &&
                    !string.IsNullOrEmpty(input_id.Text) && !string.IsNullOrEmpty(input_quantity.Text) && !string.IsNullOrEmpty(input_unit.Text))
                {
                    bool update = F_SaveData.save_data_input(uiapp, doc, input_block, input_level, input_material, input_id, input_quantity, input_unit, id_file, my_quantity_input, true, user);
                    if (update) show_data_input(id_file, thong_tin_quantity_input);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Chon_Vat_Lieu_input(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_input.SelectedItem != null)
                {
                    data_quantity item = (data_quantity)thong_tin_quantity_input.SelectedItem;
                    input_block.Text = item.block;
                    input_level.Text = item.level;
                    input_material.SelectedItem = CollectionViewSource.GetDefaultView(input_material.ItemsSource).Cast<data_material>().First(x => x.name == item.ten_vat_lieu);
                    input_id.Text = item.ma_cong_tac;
                    input_quantity.Text = item.quantity.ToString();
                    input_unit.Text = item.don_vi;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void update_data_input_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_input.SelectedItem != null)
                {
                    data_quantity item = (data_quantity)thong_tin_quantity_input.SelectedItem;

                    if (!string.IsNullOrEmpty(input_block.Text) && !string.IsNullOrEmpty(input_level.Text) && input_material.SelectedItem != null &&
                    !string.IsNullOrEmpty(input_id.Text) && !string.IsNullOrEmpty(input_quantity.Text) && !string.IsNullOrEmpty(input_unit.Text))
                    {
                        my_quantity_input.Remove(item);
                        bool update = F_SaveData.save_data_input(uiapp, doc, input_block, input_level, input_material, input_id, input_quantity, input_unit, id_file, my_quantity_input, true, user);
                        if (update) show_data_input(id_file, thong_tin_quantity_input);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void delete_data_input_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_quantity_input.SelectedItem != null)
                {
                    data_quantity item = (data_quantity)thong_tin_quantity_input.SelectedItem;

                    my_quantity_input.Remove(item);
                    bool update = F_SaveData.save_data_input(uiapp, doc, input_block, input_level, input_material, input_id, input_quantity, input_unit, id_file, my_quantity_input, false, user);
                    if (update) show_data_input(id_file, thong_tin_quantity_input);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_input(object sender, RoutedEventArgs e)
        {
            Search();
        }
        #endregion
    }
}
