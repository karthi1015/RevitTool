using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
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
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Color = Autodesk.Revit.DB.Color;
using Color1 = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;

namespace SetupTool_MaterialSetup
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

        List<Data> myData_fillPattern = new List<Data>();

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
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, cloud_image, excel_image, add_image, modify_image, refresh_image });

                GetData_Source_Material();
                myFunctionSupport.Get_SurfacePatternName_And_CutPatternName(doc, myData_fillPattern, name_cut, name_surface);
                Show_Infor_Material();
                Get_Data_Factor();
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
        List<Data> myData_donvi = new List<Data>();
        public void GetData_Source_Material()
        {
            try
            {
                List<string> combobox_don_vi = new List<string>();
                combobox_don_vi.Add("");

                var listtotal = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[3], "dbo.spVW_WorkIdDesCription_UserMaterial", mySource.type_Procedure, new List<string>(), new List<string>());
                int rows = listtotal.Rows.Count;
                for (var i = 0; i < rows; i ++)
                {
                    combobox_don_vi.Add(listtotal.Rows[i][5].ToString());
                }
                List<string> UniqueData = combobox_don_vi.Distinct().ToList();
                foreach (string don_vi in UniqueData)
                {
                    myData_donvi.Add(new Data()
                    {
                        single_value = don_vi
                    });
                }
                don_vi.ItemsSource = myData_donvi;
                don_vi.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public ObservableCollection<Material_Factor> myMaterial_Project_Factor { get; set; }
        public void Get_Data_Factor()
        {
            try
            {
                myMaterial_Project_Factor = new ObservableCollection<Material_Factor>();
                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                ObservableCollection<Material_Project> support = new ObservableCollection<Material_Project>();

                foreach (Material material in list_material)
                {
                    support.Add(new Material_Project()
                    {
                        material_project = material,
                        ton = Convert.ToDouble(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name)).ToString(),
                    });
                }

                new ObservableCollection<Material_Factor>(support
                    .GroupBy(x => new
                    {
                        x.ton
                    }).Select(y => new Material_Factor()
                    {
                        ton_value = y.Key.ton,
                        list_vat_lieu = y.Select(z => z.material_project).ToList(),
                        count = y.Select(z => z.material_project).ToList().Count()
                    }).OrderBy(z => z.ton_value)).ToList().ForEach(i => myMaterial_Project_Factor.Add(i));

                thong_tin_he_so_vat_lieu_project.ItemsSource = myMaterial_Project_Factor;
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public ObservableCollection<Material_Project> myMaterial_Project { get; set; }
        public void Show_Infor_Material()
        {
            try
            {
                myMaterial_Project = new ObservableCollection<Material_Project>();
                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                
                foreach (Material material in list_material)
                {
                    if(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name) == null)
                    {
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name, "");
                    }
                    if (myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name) == null)
                    {
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name, "");
                    }
                    if (myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name) == null ||
                        myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name) == "")
                    {
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, myAll_Data.list_unit_value_data[0].ToString());
                    }
                    if (myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name) == null)
                    {
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name, "");
                    }
                    if (myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name) == null)
                    {
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name, "");
                    }
                    myMaterial_Project.Add(new Material_Project()
                    {
                        material_project = material,
                        ma_cong_tac_project = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name),
                        ten_vat_lieu_project = material.Name,
                        don_vi_project = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name),
                        user = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name),
                        time = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name),
                        mau_vat_lieu = material.Color,
                        do_trong_suot_vat_lieu = material.Transparency,
                        id_surface = material.SurfacePatternId,
                        mau_surface = material.SurfacePatternColor,
                        id_cut = material.CutPatternId,
                        mau_cut = material.CutPatternColor,
                        ton = Convert.ToDouble(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name)).ToString(),
                        color = myFunctionSupport.Check_Color(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name), myAll_Data),
                        color_sort = myFunctionSupport.Check_Color(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name), myAll_Data).ToString()
                    });
                }
                thong_tin_vat_lieu_project.ItemsSource = myMaterial_Project;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_project.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_project", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu_project", ListSortDirection.Ascending));

                // filter list view theo tên vật liệu
                view.Filter = Filter_ten_vat_lieu;
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
                return ((item as Material_Project).ma_cong_tac_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Material_Project).ten_vat_lieu_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Material_Project).don_vi_project.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            if (search_material_project.Text != "...")
            {
                CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_project.ItemsSource).Refresh();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Color color_Shading = null;
        private void Choose_Color_Shading(object sender, RoutedEventArgs e)
        {
            color_Shading = myFunctionSupport.Choose_Color(color_shading);
        }
        Color color_Surface = null;
        private void Choose_Color_Surface(object sender, RoutedEventArgs e)
        {
            color_Surface = myFunctionSupport.Choose_Color(color_surface);
        }
        Color color_Cut = null;
        private void Choose_Color_Cut(object sender, RoutedEventArgs e)
        {
            color_Cut = myFunctionSupport.Choose_Color(color_cut);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        FillPatternElement fillPattern_Surface = null;
        private void Choose_Surface(object sender, EventArgs e)
        {
            if (name_surface.SelectedItem != null)
            {
                Data surface = (Data)name_surface.SelectedItem;
                fillPattern_Surface = myFunctionSupport.Choose_Fill_Pattern(doc, surface.single_value);
            }
        }
        FillPatternElement fillPattern_Cut = null;
        private void Choose_Cut(object sender, EventArgs e)
        {
            if (name_cut.SelectedItem != null)
            {
                Data cut = (Data)name_cut.SelectedItem;
                fillPattern_Cut = myFunctionSupport.Choose_Fill_Pattern(doc, cut.single_value);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu(object sender, MouseButtonEventArgs e)
        {
            Xem_Thong_Tin_Vat_Lieu_Project();
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin_Vat_Lieu_Project()
        {
            try
            {
                Material_Project item = (Material_Project)thong_tin_vat_lieu_project.SelectedItem;
                if (thong_tin_vat_lieu_project.SelectedItem == null)
                {
                    thong_tin_vat_lieu_project.Items.Refresh();
                    Default();
                }
                else
                {
                    string fillPattern_Surface_name = "None";
                    FillPatternElement fillPattern_Surface_Check = null;
                    if (item.id_surface.IntegerValue != -1)
                    {
                        fillPattern_Surface_Check = doc.GetElement(item.id_surface) as FillPatternElement;
                        fillPattern_Surface_name = fillPattern_Surface_Check.GetFillPattern().Name;
                    }

                    string fillPattern_Cut_name = "None";
                    FillPatternElement fillPattern_Cut_Check = null;
                    if (item.id_cut.IntegerValue != -1)
                    {
                        fillPattern_Cut_Check = doc.GetElement(item.id_cut) as FillPatternElement;
                        fillPattern_Cut_name = fillPattern_Cut_Check.GetFillPattern().Name;
                    }

                    Material material = item.material_project;

                    ten_vat_lieu.Text = item.ten_vat_lieu_project;
                    ma_cong_tac.Text = item.ma_cong_tac_project;
                    don_vi.SelectedItem = myData_donvi.Find(x => x.single_value == item.don_vi_project);
                    color_shading.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_vat_lieu.Red, item.mau_vat_lieu.Green, item.mau_vat_lieu.Blue));
                    color_Shading = material.Color;
                    tranparency_bar.Value = item.do_trong_suot_vat_lieu;
                    color_surface.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_surface.Red, item.mau_surface.Green, item.mau_surface.Blue));
                    color_Surface = material.SurfacePatternColor;
                    name_surface.SelectedItem = myData_fillPattern.Find(x => x.value_image == fillPattern_Surface_name);
                    fillPattern_Surface = fillPattern_Surface_Check;
                    color_cut.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_cut.Red, item.mau_cut.Green, item.mau_cut.Blue));
                    color_Cut = material.CutPatternColor;
                    name_cut.SelectedItem = myData_fillPattern.Find(x => x.value_image == fillPattern_Cut_name);
                    fillPattern_Cut = fillPattern_Cut_Check;

                    ton_value.Text = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Default()
        {
            try
            {
                ten_vat_lieu.Text = "";
                ma_cong_tac.Text = "";
                don_vi.SelectedIndex = 0;
                color_shading.Background = Brushes.Transparent;
                color_surface.Background = Brushes.Transparent;
                color_cut.Background = Brushes.Transparent;
                tranparency_bar.Value = 0;
                name_surface.SelectedIndex = 0;
                name_cut.SelectedIndex = 0;
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Sua_Thong_Tin_Vat_Lieu(object sender, RoutedEventArgs e)
        {
            try
            {
                myExampleDraw.command = "Modify";
                Data_for_ExternalEvent();
                Draw.Raise();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        private void Change_Ton_Kg_Factor(object sender, RoutedEventArgs e)
        {
            myExampleDraw.command = "Modify_Ton";
            Data_for_ExternalEvent();
            Draw.Raise();
        }
        
        //----------------------------------------------------------
        private void Cancel_Change_Factor_Ton_Kg(object sender, RoutedEventArgs e)
        {
            thong_tin_he_so_vat_lieu_project.Items.Refresh();
        }

        //----------------------------------------------------------
        public void Data_for_ExternalEvent()
        {
            myExampleDraw.myAll_Data = myAll_Data;

            myExampleDraw.thong_tin_vat_lieu_project = thong_tin_vat_lieu_project;
            myExampleDraw.ten_vat_lieu = ten_vat_lieu;
            myExampleDraw.ma_cong_tac = ma_cong_tac;
            myExampleDraw.don_vi = don_vi;
            myExampleDraw.use_appearence = use_appearence;
            myExampleDraw.tranparency_value = tranparency_value;
            myExampleDraw.color_Shading = color_Shading;
            myExampleDraw.color_Surface = color_Surface;
            myExampleDraw.color_Cut = color_Cut;
            myExampleDraw.fillPattern_Surface = fillPattern_Surface;
            myExampleDraw.fillPattern_Cut = fillPattern_Cut;
            myExampleDraw.myMaterial_Project = myMaterial_Project;
            myExampleDraw.ton_value = ton_value;
            myExampleDraw.myMaterial_Project_Factor = myMaterial_Project_Factor;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Tao_Vat_Lieu_Moi(object sender, RoutedEventArgs e)
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_Vat_Lieu(object sender, RoutedEventArgs e)
        {
            try
            {
                myExampleDraw.command = "Delete";
                Data_for_ExternalEvent();
                Draw.Raise();
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_All_Du_Lieu(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        //----------------------------------------------------------
        public void Refresh()
        {
            try
            {
                myFunctionSupport.Get_SurfacePatternName_And_CutPatternName(doc, myData_fillPattern, name_cut, name_surface);
                Show_Infor_Material();
                Default();
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Chon_Vat_Lieu_Tu_Company_Database(object sender, RoutedEventArgs e)
        {
            Chon_Vat_Lieu();
        }

        //----------------------------------------------------------
        public void Chon_Vat_Lieu()
        {
            try
            {
                UserSourceSQL userSourceSQL = new UserSourceSQL(uiapp, myAll_Data);
                userSourceSQL.Owner = this;
                userSourceSQL.ShowDialog();
                ten_vat_lieu.Text = userSourceSQL.ten_vat_lieu_send;
                ma_cong_tac.Text = userSourceSQL.ma_cong_tac_send;
                don_vi.SelectedItem = myData_donvi.Find(x => x.single_value == userSourceSQL.don_vi_send);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Tao_Vat_Lieu_Tu_Excel_Database(object sender, RoutedEventArgs e)
        {
            Tao_Vat_Lieu_Tu_Excel();
        }

        //----------------------------------------------------------
        ObservableCollection<Material_Excel> data_receive_create = new ObservableCollection<Material_Excel>();
        public string  Tao_Vat_Lieu_Tu_Excel()
        {
            string result = "F";
            try
            {
                ObservableCollection<Material_Project> data_create = new ObservableCollection<Material_Project>();
                UserSourceExcel userSourceExcel = new UserSourceExcel(uiapp, myAll_Data);
                userSourceExcel.Owner = this;
                userSourceExcel.ShowDialog();
                data_receive_create = userSourceExcel.data_send_create;

                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                foreach (Material_Excel excel in data_receive_create)
                {
                    ElementId fillPattern_Surface_id = new ElementId(-1);
                    if (myFunctionSupport.Choose_Fill_Pattern(doc, excel.name_surface_excel) != null) fillPattern_Surface_id = myFunctionSupport.Choose_Fill_Pattern(doc, excel.name_surface_excel).Id;

                    ElementId fillPattern_Cut_id = new ElementId(-1);
                    if (myFunctionSupport.Choose_Fill_Pattern(doc, excel.name_cut_excel) != null) fillPattern_Cut_id = myFunctionSupport.Choose_Fill_Pattern(doc, excel.name_cut_excel).Id;

                    Material material = null;
                    string ton_check = myAll_Data.list_unit_value_data[0].ToString();
                    try
                    {
                        material = list_material[0] as Material;
                        ton_check = myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name);
                    }
                    catch { }
                    

                    data_create.Add(new Material_Project()
                    {
                        material_project = material,
                        ma_cong_tac_project = excel.ma_cong_tac_excel,
                        ten_vat_lieu_project = excel.ten_vat_lieu_excel,
                        don_vi_project = excel.don_vi_excel,
                        user = uiapp.Application.Username,
                        time = DateTime.Now.ToString(),
                        mau_vat_lieu = new Color(excel.mau_vat_lieu_excel.Red, excel.mau_vat_lieu_excel.Green, excel.mau_vat_lieu_excel.Blue),
                        do_trong_suot_vat_lieu = excel.do_trong_suot_vat_lieu_excel,
                        id_surface = fillPattern_Surface_id,
                        mau_surface = new Color(excel.mau_surface_excel.Red, excel.mau_surface_excel.Green, excel.mau_surface_excel.Blue),
                        id_cut = fillPattern_Cut_id,
                        mau_cut = new Color(excel.mau_cut_excel.Red, excel.mau_cut_excel.Green, excel.mau_cut_excel.Blue),
                        ton = ton_check
                    });
                }
                myExampleDraw.command = "Excel";
                myExampleDraw.myMaterial_Project = myMaterial_Project;
                myExampleDraw.data_create = data_create;
                Draw.Raise();
                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
