using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json.Linq;
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
        public ObservableCollection<Group_Share_Parameter> list_group_parameter { get; set; } // Lay_Du_Lieu_Share_Parameter

        public ObservableCollection<Share_Parameter> list_parameter { get; set; } // Lay_Du_Lieu_Share_Parameter

        public ObservableCollection<Data> list_data { get; set; } // Lay_Thong_Tin_ProjectNumber

        public ObservableCollection<Data> list_data_parameter_need { get; set; } // Lay_Du_Lieu_Share_Parameter

        public ObservableCollection<Data> list_data_parameter_current { get; set; } // Lay_Du_Lieu_Share_Parameter

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
        public void Function_Dau_Vao()
        {
            try
            {
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, add_image, modify_image, refresh_image });

                Lay_Du_Lieu();
                Lay_Thong_Tin_ProjectNumber();
                SelectProjectNumber();
                block.Text = doc.ProjectInformation.BuildingName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = myFunctionSupport.Get_Data_All(path);
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Lay_Thong_Tin_ProjectNumber()
        {
            try
            {
                list_data = new ObservableCollection<Data>();
                list_data.Add(new Data()
                {
                    project_name = "",
                    project_address = "",
                    single_value = ""
                });

                var listtotal = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[1], "select BucketKey,BucketData from dbo.ProjectInformation where [ObjectName] is null", mySource.type_Query, new List<string>(), new List<string>());
                int rows = listtotal.Rows.Count;
                for (var i = 0; i < rows; i++)
                {
                    JObject item = JObject.Parse(listtotal.Rows[i]["BucketData"].ToString());
                    list_data.Add(new Data()
                    {
                        single_value = listtotal.Rows[i]["BucketKey"].ToString(),
                        project_address = item.GetValue("bucketLocation").ToString(),
                        project_name = item.GetValue("bucketName").ToString()
                    });
                }
                number.ItemsSource = list_data;
                number.SelectedItem = list_data.First(x => x.single_value == doc.ProjectInformation.Number);
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
                list_data_parameter_need = new ObservableCollection<Data>();
                list_data_parameter_current = new ObservableCollection<Data>();

                BindingMap map = doc.ParameterBindings;
                DefinitionBindingMapIterator it = map.ForwardIterator();
                while (it.MoveNext())
                {
                    Definition def = it.Key;
                    share_parameter_Da_Ton_Tai.Add(def.Name);
                    list_data_parameter_current.Add(new Data()
                    {
                        ten_parameter = def.Name,
                    });
                }
                thong_tin_parameter_project.ItemsSource = list_data_parameter_current;

                CollectionView view_project = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter_project.ItemsSource);
                // sort list view
                view_project.SortDescriptions.Add(new SortDescription("ten_parameter", ListSortDirection.Ascending));

                if (File.Exists(myAll_Data.list_path_foder_data[3]))
                {
                    List<string> du_lieu = File.ReadAllLines(myAll_Data.list_path_foder_data[3]).ToList();
                    foreach(string data in du_lieu)
                    {
                        Brush color = myAll_Data.list_color_UI_data[0];
                        if (list_data_parameter_current.Any(x => x.ten_parameter == data)) color = myAll_Data.list_color_UI_data[2];
                        list_data_parameter_need.Add(new Data()
                        {
                            ten_parameter = data,
                            color = color
                        });
                    }
                }
                thong_tin_parameter.ItemsSource = list_data_parameter_need;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource);
                // sort list view
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
                list_group_parameter = new ObservableCollection<Group_Share_Parameter>();
                List<string> share_parameter_Da_Ton_Tai = Lay_Share_Parameter_Da_Ton_Tai();

                List<string> du_lieu = new List<string>();
                if (File.Exists(myAll_Data.list_path_foder_data[2]))
                {
                    du_lieu = File.ReadAllLines(myAll_Data.list_path_foder_data[2]).ToList();
                }

                foreach (string data in du_lieu)
                {
                    var line = data.Split('\t');
                    if (line[0] == "GROUP")
                    {
                        list_parameter = new ObservableCollection<Share_Parameter>();
                        Brush color = myAll_Data.list_color_UI_data[0];
                        bool expander = false;
                        int count_check = 0;
                        foreach (string data_para in du_lieu)
                        {
                            var line_para = data_para.Split('\t');
                            if (line_para[0] == "PARAM")
                            {
                                if (line_para[5] == line[1])
                                {
                                    bool check = false;
                                    if (share_parameter_Da_Ton_Tai.Contains(line_para[2])) check = true;

                                    list_parameter.Add(new Share_Parameter()
                                    {
                                        type = line_para[0],
                                        guid_parameter = line_para[1],
                                        ten_parameter = line_para[2],
                                        type_parameter = line_para[3],
                                        category_parameter = line_para[4],
                                        group_parameter = line_para[5],
                                        visible_parameter = line_para[6],
                                        description_parameter = line_para[7],
                                        user_modify_parameter = line_para[8],
                                        exist_parameter = check,
                                        ValueIsSelect = false,
                                    });
                                    if (check == true)
                                    {
                                        color = myAll_Data.list_color_UI_data[3];
                                        expander = true;
                                        count_check++;
                                    }
                                }
                            }
                        }
                        var a = list_parameter.OrderBy(x => x.ten_parameter).ToList();
                        list_parameter.Clear();
                        foreach (var b in a)
                        {
                            list_parameter.Add(b);
                        }
                        list_group_parameter.Add(new Group_Share_Parameter()
                        {
                            type = line[0],
                            id_group_parameter = line[1],
                            ten_group_parameter = line[2],
                            ValueExpanded = expander,
                            Children = list_parameter,
                            color = color,
                            count_check = count_check.ToString()
                        });
                    };
                }
                thong_tin_share_parameter.ItemsSource = list_group_parameter;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("ten_group_parameter", ListSortDirection.Ascending));

                // filter list view theo tên vật liệu
                try { view.Filter = Filter_ten_vat_lieu; } catch (Exception) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            if (String.IsNullOrEmpty(search_material_project.Text) || search_material_project.Text == "...")
                return true;
            else
                return ((item as Group_Share_Parameter).ten_group_parameter.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (search_material_project.Text != "...")
                {
                    CollectionViewSource.GetDefaultView(thong_tin_share_parameter.ItemsSource).Refresh();
                }
            }
            catch (Exception)
            {

            }

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Lay_Du_Lieu_Share_Parameter(object sender, EventArgs e)
        {
            SelectProjectNumber();
        }

        //----------------------------------------------------------
        public void SelectProjectNumber()
        {
            try
            {
                Data item = (Data)number.SelectedItem;
                name.Text = item.project_name;
                address.Text = item.project_address;
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_Item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Group_Share_Parameter group in list_group_parameter)
                {
                    List<bool> check = new List<bool>();
                    foreach (Share_Parameter parameter in group.Children)
                    {
                        check.Add(parameter.exist_parameter);
                    }
                    if (check.Contains(true) == false)
                    {
                        group.color = myAll_Data.list_color_UI_data[0];
                    }
                    else
                    {
                        group.color = myAll_Data.list_color_UI_data[3];
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
                Lay_Du_Lieu();
                Lay_Thong_Tin_ProjectNumber();
                SelectProjectNumber();
                block.Text = doc.ProjectInformation.BuildingName;
            }
            catch (Exception)
            {

            }
        }

        private void Them_Hoac_Xoa_Parameter_Trong_Project(object sender, RoutedEventArgs e)
        {
            try
            {
                myExampleDraw.command = "Create";
                Data_for_ExternalEvent();
                Draw.Raise();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Data_for_ExternalEvent()
        {
            myExampleDraw.myAll_Data = myAll_Data;
            myExampleDraw.number = number;
            myExampleDraw.name = name;
            myExampleDraw.address = address;
            myExampleDraw.block = block;
            myExampleDraw.thong_tin_parameter = thong_tin_parameter;
            myExampleDraw.thong_tin_parameter_project = thong_tin_parameter_project;
            myExampleDraw.thong_tin_share_parameter = thong_tin_share_parameter;
            myExampleDraw.list_group_parameter = list_group_parameter;
            myExampleDraw.list_data_parameter_need = list_data_parameter_need;
            myExampleDraw.list_data_parameter_current = list_data_parameter_current;
        }

        private void Sua_Parameter_Bat_Buoc(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(myAll_Data.list_path_foder_data[3]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
