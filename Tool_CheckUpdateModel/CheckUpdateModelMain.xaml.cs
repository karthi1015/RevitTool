using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Tool_CheckUpdateModel.Code.Function.Type;
using Tool_CheckUpdateModel.Data;
using Tool_CheckUpdateModel.Data.Binding;
using Tool_CheckUpdateModel.Function;
using Tool_CheckUpdateModel.Function.Controls;
using ListViewItem = System.Windows.Controls.ListViewItem;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Tool_CheckUpdateModel
{
    public partial class CheckUpdateModelMain : Window
    {
        public CheckUpdateModelMain(UIApplication _uiapp)
        {
            InitializeComponent();

            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            Add_ExternalEvent();

            my_parameter_settings = new ObservableCollection<Parameter_Settings>();
            my_element_change = new ObservableCollection<Element_Change>();
            my_data_revit_link = new ObservableCollection<data_revit_link>();

            category_combobox.ItemsSource = Source.Category_Check;
            category_combobox.SelectedIndex = 0;
            Get_Parameter();
            get_path_excel_default();
        }

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Khai báo biến ban đầu
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        E_All my_change;
        ExternalEvent e_change;

        E_Link my_link;
        ExternalEvent e_link;

        E_LoadForm my_load_form;
        ExternalEvent e_load_form;

        E_ExportImages my_export_images;
        ExternalEvent e_export_images;

        E_Focus my_forcus;
        ExternalEvent e_forcus;
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Add ExternalEvent
        public void Add_ExternalEvent()
        {
            my_change = new E_All();
            e_change = ExternalEvent.Create(my_change);

            my_link = new E_Link();
            e_link = ExternalEvent.Create(my_link);

            my_load_form = new E_LoadForm();
            e_load_form = ExternalEvent.Create(my_load_form);

            my_export_images = new E_ExportImages();
            e_export_images = ExternalEvent.Create(my_export_images);

            my_forcus = new E_Focus();
            e_forcus = ExternalEvent.Create(my_forcus);
        }

        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Khai báo ObservableCollection
        ObservableCollection<Element_Change> my_element_change;
        ObservableCollection<Parameter_Settings> my_parameter_settings;
        ObservableCollection<data_revit_link> my_data_revit_link;
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Select RevitLinkType Combobox
        private void Get_Link_File(object sender, EventArgs e)
        {
            try
            {
                my_element_change = new ObservableCollection<Element_Change>();
                data_update.ItemsSource = my_element_change;

                List<Document> docs = new List<Document>();
                foreach (Document doc in uiapp.Application.Documents)
                {
                    docs.Add(doc);
                }
                var a = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList();
                my_data_revit_link = new ObservableCollection<data_revit_link>(new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList()
                    .Select(x => new data_revit_link()
                    {
                        type = x,
                        name = x.Name,
                        document = docs.FirstOrDefault(y => y.Title + ".rvt" == x.Name)
                    }));

                link_file.ItemsSource = my_data_revit_link;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(link_file.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private void Select_Link_File(object sender, EventArgs e)
        {
            try
            {
                if (link_file.SelectedItem != null)
                {
                    data_revit_link item = (data_revit_link)link_file.SelectedItem;
                    if (item.document == null)
                    {
                        System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
                        file.FileName = MessageBox_Data.discription_loadform;
                        file.Filter = MessageBox_Data.filter_revit;
                        if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            my_load_form.path = file.FileName;
                            my_load_form.link_file = link_file;
                            e_load_form.Raise();
                        }
                        else
                        {
                            link_file.SelectedItem = null;
                        }
                    }
                    else
                    {
                        string path_file = item.document.PathName;
                        string path_direction = Path.GetDirectoryName(path_file);
                        string name_direction = new DirectoryInfo(path_direction).Name;
                        if (item.name != name_direction + ".rvt")
                        {
                            MessageBox.Show(MessageBox_Data.file_not_check, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            link_file.SelectedItem = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private void Select_Path_Check_File_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Document> docs = new List<Document>();
                foreach (Document doc in uiapp.Application.Documents)
                {
                    docs.Add(doc);
                }
                System.Windows.Forms.OpenFileDialog file = new System.Windows.Forms.OpenFileDialog();
                file.Filter = MessageBox_Data.filter_revit;
                if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (docs.Any(x => x.PathName == file.FileName) == false)
                    {
                        FolderBrowserDialog folder = new FolderBrowserDialog();
                        folder.SelectedPath = Path.GetDirectoryName(file.FileName);
                        folder.Description = MessageBox_Data.folder_save_check;
                        if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            string name = Path.GetFileName(file.FileName).Split('.')[0] + "_" + DateTime.Now.ToString("ddMMyyHHmmss");

                            string path_directory = folder.SelectedPath + "\\" + name;

                            if (!Directory.Exists(path_directory))
                                Directory.CreateDirectory(path_directory);

                            if (!File.Exists(path_directory + "\\" + name + ".rvt"))
                                File.Copy(file.FileName, path_directory + "\\" + name + ".rvt");

                            my_link.path = path_directory + "\\" + name + ".rvt";
                            e_link.Raise();
                            link_file.SelectedItem = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageBox_Data.file_is_exist, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Button Check
        Document doc_link = null;
        private void Check_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (link_file.SelectedItem != null)
                {
                    data_revit_link item = (data_revit_link)link_file.SelectedItem;
                    doc_link = item.document;
                    my_element_change = new ObservableCollection<Element_Change>();
                    F_All.Check_Element(doc, doc_link, my_parameter_settings, my_element_change);

                    Add_Element_Change();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Parameter Check Listbox
        private void Get_Parameter()
        {

            try
            {
                F_ListBox.Get_Parameter(my_parameter_settings, doc, parameter_current);

                parameter_current.Items.SortDescriptions.Add(new SortDescription("parameter_group", ListSortDirection.Ascending));
                parameter_current.Items.SortDescriptions.Add(new SortDescription("isCheck", ListSortDirection.Descending));
                parameter_current.Items.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(parameter_current.ItemsSource);
                view.Filter = Filter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private bool Filter(object item)
        {
            data_category select = (data_category)category_combobox.SelectedItem;
            if (select.name == Source.Category_Check[0].name) return true;
            else return ((item as Parameter_Settings).parameter_category_name.IndexOf(select.name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //-------------------------------------------------------
        private void save_setting_Click(object sender, RoutedEventArgs e)
        {
            F_ListBox.save_settings(my_parameter_settings);
        }

        //-------------------------------------------------------
        private void check_all_parameter_current_view_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                for (int i = 0; i < parameter_current.Items.Count; i++)
                {
                    Parameter_Settings item = (Parameter_Settings)parameter_current.Items[i];
                    item.isCheck = true;
                }
                parameter_current.Items.Refresh();
                F_ListBox.save_settings(my_parameter_settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //-------------------------------------------------------
        private void visible_setting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (setting.ToolTip.ToString() == "visible") setting.ToolTip = "not visible";
                else setting.ToolTip = "visible";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Combobox Filter
        private void filter_category_Click(object sender, EventArgs e)
        {
            try
            {
                data_category item = (data_category)category_combobox.SelectedItem;

                if (my_parameter_settings.Count() > 0) CollectionViewSource.GetDefaultView(parameter_current.ItemsSource).Refresh();

                if (my_element_change.Count() > 0) CollectionViewSource.GetDefaultView(data_update.ItemsSource).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Listview
        private void Add_Element_Change()
        {
            try
            {
                data_update.ItemsSource = my_element_change;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(data_update.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("type_change", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("element_name", ListSortDirection.Ascending));
                view.Filter = Filter_Element_Change;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private bool Filter_Element_Change(object item)
        {
            data_category select = (data_category)category_combobox.SelectedItem;
            if (select.name == Source.Category_Check[0].name) return true;
            else return ((item as Element_Change).parameter_category_name.IndexOf(select.name, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //-------------------------------------------------------
        private void High_Light_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (data_update.SelectedItem != null)
                {
                    Element_Change item = (Element_Change)data_update.SelectedItem;
                    my_forcus.item = item;
                    my_change.doc_link = doc_link;
                    e_forcus.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = 0;
                index = data_update.SelectedIndex;

                Element_Change item = (Element_Change)data_update.Items[index];
                if (item.changeORignore && item.color == Source.color_not_change)
                {
                    ObservableCollection<Element_Change> element_Changes = new ObservableCollection<Element_Change>();
                    element_Changes.Add(item);

                    my_change.element_Changes = element_Changes;
                    my_change.my_element_change = my_element_change;
                    my_change.doc_link = doc_link;
                    my_change.data_update = data_update;
                    my_change.link_file = link_file;
                    e_change.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private void Save_Data(object sender, RoutedEventArgs e)
        {
            F_ListBox.Save_Data_Check(my_element_change, link_file, uiapp, doc_link);
        }

        //-------------------------------------------------------
        private void roll_back_Click(object sender, RoutedEventArgs e)
        {
            F_ListBox.Roll_Back_Save_Data_Check(my_element_change, link_file, uiapp, doc_link, doc, my_parameter_settings);

            if (link_file.SelectedItem != null)
            {
                data_revit_link item = (data_revit_link)link_file.SelectedItem;
                doc_link = item.document;
                my_element_change = new ObservableCollection<Element_Change>();
                F_All.Check_Element(doc, doc_link, my_parameter_settings, my_element_change);

                Add_Element_Change();
            }
        }

        //-------------------------------------------------------
        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }

        //-------------------------------------------------------
        private void Update_All_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ObservableCollection<Element_Change> element_Changes = new ObservableCollection<Element_Change>();
                for (int i = 0; i < data_update.Items.Count; i++)
                {
                    Element_Change item = (Element_Change)data_update.Items[i];
                    if (item.changeORignore && item.color == Source.color_not_change)
                    {
                        element_Changes.Add(item);
                        item.color = Source.color_used_change;
                    }
                }
                my_change.element_Changes = element_Changes;
                my_change.my_element_change = my_element_change;
                my_change.doc_link = doc_link;
                my_change.data_update = data_update;
                e_change.Raise();

                data_update.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Function Excel Button
        private void Export_Images(object sender, RoutedEventArgs e)
        {
            if (my_element_change.Count() > 0)
            {
                my_export_images.my_element_change = my_element_change;
                my_export_images.doc_link = doc_link;
                e_export_images.Raise();
            }
        }

        //-------------------------------------------------------
        private void Export_Excel(object sender, RoutedEventArgs e)
        {
            if (my_element_change.Count() > 0)
            {
                if (!string.IsNullOrEmpty(path_excel.ToolTip.ToString())) F_Excel.Export_Excel(doc, doc_link, path_excel.ToolTip.ToString(), my_element_change);
                else MessageBox.Show("Vui lòng chọn excel template và thử lại lần nữa");
            }
        }

        //-------------------------------------------------------
        private void path_excel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path_excel.ToolTip = file.FileName;
                    File.WriteAllText(Source.Path_Excel, path_excel.ToolTip.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        private void get_path_excel_default()
        {
            try
            {
                if (File.Exists(Source.Path_Excel))
                {
                    path_excel.ToolTip = File.ReadAllText(Source.Path_Excel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion
    }
}
