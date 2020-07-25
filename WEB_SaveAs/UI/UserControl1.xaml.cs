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

namespace WEB_SaveAs
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        ExternalEventClass myExampleDraw;
        ExternalEvent Draw;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<data_file> mydata_file { get; set; }
        public ObservableCollection<element_list> myelement_list { get; set; }
        public ObservableCollection<level_block> mylevel_block { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            myExampleDraw = new ExternalEventClass();
            Draw = ExternalEvent.Create(myExampleDraw);

            mySQL = new FunctionSQL();
            myFunctionSupport = new FunctionSupoort();
            mySource = new ListSource();

            var listtotal = mySQL.SQLRead(@"Server=18.141.116.111,1433\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789", "Select * from dbo.PathSource", "Query", new List<string>(), new List<string>());
            path = listtotal.Rows[0][1].ToString();
            Function_TXT();

            Function_Dau_Vao();
        }

        //----------------------------------------------------------
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = myFunctionSupport.Get_Data_All(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, select_image, folder_image, refresh_image, saveas_image });

                mydata_file = new ObservableCollection<data_file>();
                myelement_list = new ObservableCollection<element_list>();
                mylevel_block = new ObservableCollection<level_block>();

                folder.Text = @"C:\";

                Get_ELement_Link_Or_NoLink(doc);
                Data_File();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public List<Level> level_list { get; set; }
        public void Get_ELement_Link_Or_NoLink(Document doc)
        {
            try
            {
                level_list = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
                var list_element = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(x => x.Parameters.Cast<Parameter>().Any(y => y.Definition.Name == "Volume" || y.Definition.Name == "Area" || y.Definition.Name == "Length") == true)
                    .Where(x => x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    .ToList();

                foreach(Element element in list_element)
                {
                    string block = doc.ProjectInformation.BuildingName;

                    string level = "";
                    if (element.LookupParameter(myAll_Data.list_parameter_share_data[4]).AsString() != null) level = element.LookupParameter(myAll_Data.list_parameter_share_data[4]).AsString();

                    double elevation = 100000000;
                    try
                    {
                        elevation = level_list.First(x => level == x.Name).Elevation;
                    }
                    catch (Exception)
                    {

                    }
                    myelement_list.Add(new element_list
                    {
                        cau_kien = element,
                        level = myFunctionSupport.RemoveUnicode(level),
                        elevation = elevation
                    });
                }

                
                mylevel_block = new ObservableCollection<level_block>(myelement_list.GroupBy(x => new
                {
                    x.level,
                    x.elevation
                }).Where(x => !string.IsNullOrEmpty(x.Key.level)).Select(y => new level_block()
                {
                    number = doc.ProjectInformation.Number,
                    block = doc.ProjectInformation.BuildingName,
                    level = y.Key.level,
                    descipline = doc.Title.Split('_')[3],
                    elevation = y.Key.elevation
                }).OrderByDescending(z => z.elevation));

                level.ItemsSource = mylevel_block;
                level.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Folder(object sender, RoutedEventArgs e)
        {
            Select();
        }

        //----------------------------------------------------------
        public void Select()
        {
            try
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    folder.Text  = folderBrowser.SelectedPath;
                }
                Data_File();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Data_File()
        {
            try
            {
                mydata_file = new ObservableCollection<data_file>();
                List<Level> list_level = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
                foreach (string file in Directory.GetFiles(folder.Text, "*.rvt*").ToList())
                {
                    FileInfo infor = new FileInfo(file);
                    double elevation = 1000000000;
                    try
                    {
                        elevation = list_level.Where(x => myFunctionSupport.RemoveUnicode(x.Name) == infor.Name.Split('_')[1]).First().Elevation;
                    }
                    catch (Exception)
                    {

                    }
                    mydata_file.Add(new data_file()
                    {
                        path = file,
                        name = infor.Name,
                        size = Math.Round(Convert.ToDouble(infor.Length / (1024)), 2).ToString() + " Kb",
                        elevation = elevation
                    });
                }
                thong_tin_file.ItemsSource = mydata_file;

                thong_tin_file.Items.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                thong_tin_file.Items.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                    Data_File();
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
                Get_ELement_Link_Or_NoLink(doc);
                Data_File();
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
        public void Data_for_ExternalEvent()
        {
            myExampleDraw.myAll_Data = myAll_Data;
            myExampleDraw.level = level;
            myExampleDraw.path = folder;
            myExampleDraw.name = name;
            myExampleDraw.option_normal = option_normal;
            myExampleDraw.myelement_list = myelement_list;
            myExampleDraw.mydata_file = mydata_file;
            myExampleDraw.thong_tin_file = thong_tin_file;
        }

        //----------------------------------------------------------
        public string Save()
        {
            string result = "F";
            try
            {
                myExampleDraw.command = "Save As";
                Data_for_ExternalEvent();
                Draw.Raise();
                result = "S";
            }
            catch (Exception)
            {

            }
            return result;
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

        
    }
}
