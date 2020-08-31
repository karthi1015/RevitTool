using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

using System.Diagnostics;
using WEB_SaveAs.Code.Function;
using WEB_SaveAs.Data.Binding;

namespace WEB_SaveAs
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

        E_SaveAs my_save_as;
        ExternalEvent e_save_as;
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_file> my_data_file { get; set; }
        List<Level> level_list { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_save_as = new E_SaveAs();
            e_save_as = ExternalEvent.Create(my_save_as);

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
                        my_data_file = new ObservableCollection<data_file>();

                        folder.Text = @"C:\";

                        F_ElementByLevel.get_element_by_level(doc, level_list, level);
                        F_Folder.get_file_data(doc, my_data_file, folder, thong_tin_file);
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
        private void Select_Folder(object sender, RoutedEventArgs e)
        {
            F_Folder.select_folder(doc, my_data_file, folder, thong_tin_file);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Open_File(object sender, MouseButtonEventArgs e)
        {
            try
            {
                data_file item = (data_file)thong_tin_file.SelectedItem;
                if (item != null)
                {
                    uiapp.OpenAndActivateDocument(item.path);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_File(object sender, RoutedEventArgs e)
        {
            try
            {
                var result_message = MessageBox.Show("Bạn có chắc chắn muốn xóa!", "QUESTION", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result_message == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < thong_tin_file.SelectedItems.Count; i++)
                    {
                        data_file item = (data_file)thong_tin_file.SelectedItems[i];
                        File.Delete(item.path);
                    }
                    F_Folder.get_file_data(doc, my_data_file, folder, thong_tin_file);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh(object sender, RoutedEventArgs e)
        {
            try
            {
                F_ElementByLevel.get_element_by_level(doc, level_list, level);
                F_Folder.get_file_data(doc, my_data_file, folder, thong_tin_file);
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Open_Folder(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(folder.Text);
            }
            catch (Exception)
            {

            }
        }
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Save_As(object sender, RoutedEventArgs e)
        {
            string result = Save();
        }

        //----------------------------------------------------------
        public string Save()
        {
            string result = "F";
            try
            {
                my_save_as.level = level;
                my_save_as.path = folder;
                my_save_as.name = name;
                my_save_as.option_normal = option_normal;
                my_save_as.my_data_file = my_data_file;
                my_save_as.thong_tin_file = thong_tin_file;
                e_save_as.Raise();
                result = "S";
            }
            catch (Exception)
            {

            }
            return result;
        }

        
    }
}
