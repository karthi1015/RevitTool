using ARC_Quatity.Code;
using ARC_Quatity.Code.Function;
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_element_link> my_element_link { get; set; }
        ObservableCollection<data_quantity> my_quantity_total { get; set; }
        ObservableCollection<data_quantity> my_quatity_item { get; set; }
        ObservableCollection<data_quantity> my_quantity_detail { get; set; }
        ObservableCollection<data_file_link> my_file_link { get; set; }
        ObservableCollection<data_table_note> my_table_note { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_visible_link = new E_VisibleLink();
            e_visible_link = ExternalEvent.Create(my_visible_link);

            Function_Dau_Vao();
            F_QuantityNotes.Get_Data_Notes_Web(project_number, Class, my_table_note, thong_tin_quantity_total_web);
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                List<string> file_name = doc.Title.Split('_').ToList();
                project_number = doc.ProjectInformation.Number;
                block = doc.ProjectInformation.BuildingName;
                Class = doc.ProjectInformation.LookupParameter("Class").AsString();
                id_file = file_name[0] + "_" + file_name[1] + "_" + file_name[2] + "_" + file_name[3] + "_";
                get_unit_length_type();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void get_quantity_all_Click(object sender, RoutedEventArgs e)
        {
            Get_Quantity_All();
        }
        //----------------------------------------------------------
        public void Get_Quantity_All()
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
                    F_Chart.Show_Du_Lieu_Len_Chart(bieu_do_category, my_element_link, DataContext, this);
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
        private void get_unit_length_type()
        {
            try
            {
                Units units = doc.GetUnits();
                List<UnitType> unit_types = UnitUtils.GetValidUnitTypes().ToList();
                FormatOptions format_option = units.GetFormatOptions(unit_types.First(x => x == UnitType.UT_Length));
                unit_length = LabelUtils.GetLabelFor(format_option.DisplayUnits);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        F_GetQuantity.Support_Get_Khoi_Luong(element, levels, my_quatity_item_support, doc, Class, unit_length, block);
                        List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                        foreach (ElementId id in elements_child)
                        {
                            FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                            F_GetQuantity.Support_Get_Khoi_Luong(model_child, levels, my_quatity_item_support, doc, Class, unit_length, block);
                            Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item_support, doc);
                        }
                    }
                    else
                    {
                        F_GetQuantity.Support_Get_Khoi_Luong(element, levels, my_quatity_item_support, doc, Class, unit_length, block);
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
        private void Support_Show_Khoi_Luong_Len_ListView(FamilyInstance model_child, List<Level> levels, ObservableCollection<data_quantity> my_quatity_item, Document doc)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        F_GetQuantity.Support_Get_Khoi_Luong(model_child, levels, my_quatity_item, doc, Class, unit_length, block);
                        Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item, doc);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_total(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
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
        private bool Filter_thong_tin_detail(object item)
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
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_element(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
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
                    if (thong_tin_quantity_total_project.Items.Count > 0)
                    {
                        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                        view.Filter = Filter_thong_tin_total;
                        CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Refresh();
                    }
                }
                if (any.IsChecked == true)
                {
                    if (thong_tin_quantity_project.Items.Count > 0)
                    {
                        CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                        view1.Filter = Filter_thong_tin_total;
                        CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Refresh();
                    }
                }

                if (any.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    if (thong_tin_detail.Items.Count > 0)
                    {
                        item_select = null;
                        CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                        view_detail.Filter = Filter_thong_tin_detail;
                    }
                    bieu_do_download.Value = thong_tin_quantity_project.Items.Count;
                }

                if (total.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    if (thong_tin_detail.Items.Count > 0)
                    {
                        item_select = (data_quantity)thong_tin_quantity_total_project.SelectedItem;
                        CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                        view_detail.Filter = Filter_thong_tin_detail;
                    }
                    bieu_do_download.Value = thong_tin_quantity_total_project.Items.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_By(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Bao_Gom_Ca_Link_File(object sender, RoutedEventArgs e)
        {
            try
            {
                Get_List_Link_File(false);
                Khoi_Luong_Cua_Link_File();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

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
                    F_Chart.Show_Du_Lieu_Len_Chart(bieu_do_category, my_element_link, DataContext, this);
                }
                Search();

                e_visible_link.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Lay_Hoac_Khong_Lay_Khoi_Luong_Link(object sender, RoutedEventArgs e)
        {
            Khoi_Luong_Cua_Link_File();
            if (my_file_link.Any(x => x.chon_file_link != true) == false) check_all.IsChecked = true;
            if (my_file_link.Any(x => x.chon_file_link == true) == false) check_all.IsChecked = false;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Upload_Du_Lieu(object sender, RoutedEventArgs e)
        {
            string result = Upload();
            if (result == "S") MessageBox.Show("Upload Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //----------------------------------------------------------
        public string Upload()
        {
            string result = "F";
            try
            {
                List<string> color_check = my_quantity_total.Select(x => x.color_sort).ToList();
                if (color_check.Distinct().ToList().Count() == 1)
                {
                    string user = uiapp.Application.Username;
                    string hostName = Dns.GetHostName();
                    string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    string path = Source.pathUserPassword + "\\" + myIP;
                    if (File.Exists(path))
                    {
                        var infor = File.ReadAllLines(path).ToList();
                        user = infor[0];
                    }

                    List<List<string>> values_material = new List<List<string>>();
                    List<List<string>> values_element = new List<List<string>>();

                    CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Cast<data_quantity>().ToList().ForEach(x =>
                        values_material.Add(new List<string>()
                        {
                            id_file,
                            x.block,
                            x.level,
                            x.ten_vat_lieu,
                            x.ma_cong_tac,
                            x.quantity.ToString(),
                            x.don_vi,
                            user,
                            DateTime.Now.ToString()
                        }));

                    CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Cast<data_quantity>().ToList().ForEach(x =>
                        values_element.Add(new List<string>()
                        {
                            id_file,
                            x.block,
                            x.level,
                            x.id_cau_kien,
                            x.ten_cau_kien,
                            x.ten_vat_lieu,
                            x.ma_cong_tac,
                            x.quantity.ToString(),
                            x.don_vi,
                            user,
                            DateTime.Now.ToString()
                        }));

                    List<List<List<string>>> values = new List<List<List<string>>>() { values_material, values_element };
                    result = F_Upload.Upload(doc, values, my_quantity_total.Select(x => x.ma_cong_tac).ToList(), id_file);
                }
                else
                {
                    MessageBox.Show("Missing data. Upload fail!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Element_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                my_element_link = new ObservableCollection<data_element_link>();
                my_quantity_total = new ObservableCollection<data_quantity>();
                my_quatity_item = new ObservableCollection<data_quantity>();
                my_quantity_detail = new ObservableCollection<data_quantity>();
                my_file_link = new ObservableCollection<data_file_link>();

                get_unit_length_type();

                F_GetElement.get_element_by_select(uidoc, doc, my_element_link);

                Show_Khoi_Luong_Len_ListView();

                F_Chart.Show_Du_Lieu_Len_Chart(bieu_do_category, my_element_link, DataContext, this);

                Search();

                Get_Source();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Support_Foreach(FamilyInstance model_child)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        my_element_link.Add(new data_element_link() { cau_kien = doc.GetElement(id_for), doc = doc });
                        Support_Foreach(model_child);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        void Get_Source()
        {
            try
            {
                if (thong_tin_quantity_total_project.Items.Count > 0)
                {
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                    view.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Refresh();
                }

                if (thong_tin_quantity_project.Items.Count > 0)
                {
                    CollectionView view1 = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                    view1.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            if (thong_tin_quantity_total_web.SelectedItem != null)
            {
                data_table_note item = (data_table_note)thong_tin_quantity_total_web.SelectedItem;
                Get_Source();
                string result = Upload();
                if (result == "S")
                {
                    MessageBox.Show("Upload Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    List<string> para = new List<string>() { "@DBProjectNumber", "@DBId", "@DBStatus" };
                    var a = JsonConvert.SerializeObject(new data_status() { open = true, edited = true });
                    foreach (string id in item.id.Split('\n').ToList())
                    {
                        List<object> para_value = new List<object>() { project_number, id, a };
                        SQL.SQLWrite(Source.path_WEB, "dbo.spUpdate_OnlyStatusQuantityNotes", Source.type_Procedure, para, para_value);
                    }
                    my_table_note.Remove(item);
                    thong_tin_quantity_total_web.Items.Refresh();
                }
            }
        }
    }
}
