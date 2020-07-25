using Autodesk.Revit.DB;
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
using Color = Autodesk.Revit.DB.Color;
using MessageBox = System.Windows.MessageBox;

namespace SetupTool_MaterialSetup
{
    /// <summary>
    /// Interaction logic for UserSourceExcel.xaml
    /// </summary>
    public partial class UserSourceExcel : Window
    {

        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        All_Data myAll_Data;

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
        List<Data> myData_fillPattern = new List<Data>();
        public UserSourceExcel(UIApplication _uiapp , All_Data _myAll_Data)
        {
            InitializeComponent();
            myAll_Data = _myAll_Data;

            mySQL = new FunctionSQL();
            myFunctionSupport = new FunctionSupoort();
            mySource = new ListSource();

            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, add_image, modify_image, export_image, select_image });

            Vi_Tri_Ban_Dau();
            myFunctionSupport.Get_SurfacePatternName_And_CutPatternName(doc, myData_fillPattern, name_cut, name_surface);
        }

        //----------------------------------------------------------
        public ObservableCollection<list_ten_du_lieu_excel> myPosition_Excel { get; set; }
        public void Vi_Tri_Ban_Dau()
        {
            try
            {
                myPosition_Excel = new ObservableCollection<list_ten_du_lieu_excel>();
                foreach(list_ten_du_lieu_excel position in myAll_Data.list_ten_du_lieu_excel_data)
                {
                    myPosition_Excel.Add(position);
                }
                vi_tri_bat_dau.ItemsSource = myPosition_Excel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Export_Du_Lieu_To_Excel(object sender, RoutedEventArgs e)
        {
            string result = Export_Du_Lieu();
            if (result == "S")
            {
                MessageBox.Show("Export Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //----------------------------------------------------------
        public string Export_Du_Lieu()
        {
            string result = "F";
            try
            {
                string path = "";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Export material information";
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<Material_Project> myMaterial_Project = new List<Material_Project>();
                    List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                    foreach (Material material in list_material)
                    {
                        myMaterial_Project.Add(new Material_Project()
                        {
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
                        });
                    }

                    myFunctionSupport.Export_Excel(myPosition_Excel, myMaterial_Project, saveFileDialog.FileName, "Material in Project", doc);
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Du_Lieu_To_Excel(object sender, RoutedEventArgs e)
        {
            Select_Du_Lieu();
        }

        //----------------------------------------------------------
        public ObservableCollection<Material_Excel> myMaterial_Project { get; set; }
        public string Select_Du_Lieu()
        {
            string result = "F";
            try
            {
                myMaterial_Project = new ObservableCollection<Material_Excel>();
                string path = null;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = openFileDialog.FileName;
                }
                if (!string.IsNullOrEmpty(path))
                {
                    foreach(Material_Excel excel in myFunctionSupport.Get_Excel(myPosition_Excel, path))
                    {
                        myMaterial_Project.Add(excel);
                    }
                    thong_tin_vat_lieu_excel.ItemsSource = myMaterial_Project;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_excel.ItemsSource);
                    // sort list view
                    view.SortDescriptions.Add(new SortDescription("ma_cong_tac_excel", ListSortDirection.Ascending));

                    // filter list view theo tên vật liệu
                    view.Filter = Filter_ten_vat_lieu;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            if (String.IsNullOrEmpty(search_material_project.Text) || search_material_project.Text == "...")
                return true;
            else
                return ((item as Material_Excel).ma_cong_tac_excel.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            if (search_material_project.Text != "...")
            {
                CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_excel.ItemsSource).Refresh();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<Material_Excel> data_send_create = new ObservableCollection<Material_Excel>();
        private void Tao_Vat_Lieu_Moi_Excel(object sender, RoutedEventArgs e)
        {
            data_send_create = myMaterial_Project;
            Close();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Sua_Thong_Tin_Vat_Lieu_Excel(object sender, RoutedEventArgs e)
        {
            string result = Sua_Thong_Tin_Vat_Lieu();
            if (result == "S")
            {
                MessageBox.Show("Modify Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //----------------------------------------------------------
        public string Sua_Thong_Tin_Vat_Lieu()
        {
            string result = "F";
            try
            {
                Material_Excel item = (Material_Excel)thong_tin_vat_lieu_excel.SelectedItem;
                if (thong_tin_vat_lieu_excel.SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                    {
                        if (myMaterial_Project.Any(x => x.ten_vat_lieu_excel == ten_vat_lieu.Text) == false || item.ten_vat_lieu_excel == ten_vat_lieu.Text)
                        {
                            if (myMaterial_Project.Any(x => x.ma_cong_tac_excel == ma_cong_tac.Text) == false || item.ma_cong_tac_excel == ma_cong_tac.Text)
                            {
                                Color color_Shading_list = item.mau_vat_lieu_excel;
                                int tranparency = item.do_trong_suot_vat_lieu_excel;
                                Color color_Surface_list = item.mau_surface_excel;
                                Color color_Cut_list = item.mau_cut_excel;
                                string fillPattern_Surface_list = item.name_surface_excel;
                                string fillPattern_Cut_list = item.name_cut_excel;

                                if (color_Shading != null)
                                {
                                    color_Shading_list = color_Shading;
                                }
                                tranparency = Convert.ToInt32(tranparency_value.Text);

                                if (color_Surface != null) color_Surface_list = color_Surface;
                                if (fillPattern_Surface != null)
                                {
                                    FillPatternElement fillPattern_Surface_element = doc.GetElement(fillPattern_Surface.Id) as FillPatternElement;
                                    fillPattern_Surface_list = fillPattern_Surface_element.GetFillPattern().Name;
                                }

                                if (color_Cut != null) color_Cut_list = color_Cut;
                                if (fillPattern_Cut != null)
                                {
                                    FillPatternElement fillPattern_Cut_element = doc.GetElement(fillPattern_Cut.Id) as FillPatternElement;
                                    fillPattern_Cut_list = fillPattern_Cut_element.GetFillPattern().Name;
                                }

                                List<Material_Excel> data_material_modify = new List<Material_Excel>();
                                data_material_modify.Add(new Material_Excel()
                                {
                                    ma_cong_tac_excel = ma_cong_tac.Text,
                                    ten_vat_lieu_excel = ten_vat_lieu.Text,
                                    don_vi_excel = don_vi.Text,
                                    mau_vat_lieu_excel = color_Shading_list,
                                    do_trong_suot_vat_lieu_excel = tranparency,
                                    name_surface_excel = fillPattern_Surface_list,
                                    mau_surface_excel = color_Surface_list,
                                    name_cut_excel = fillPattern_Cut_list,
                                    mau_cut_excel = color_Cut_list,
                                });

                                myMaterial_Project.Remove(item);
                                myMaterial_Project.Add(data_material_modify[0]);
                                result = "S";
                            }
                            else
                            {
                                MessageBox.Show("\"Mã Công Tác\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("\"Tên Vật Liệu\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("\"Tên Vật Liệu\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn chỉnh sửa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu_Excel(object sender, MouseButtonEventArgs e)
        {
            Xem_Thong_Tin_Vat_Lieu_Project();
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin_Vat_Lieu_Project()
        {
            try
            {
                Material_Excel item = (Material_Excel)thong_tin_vat_lieu_excel.SelectedItem;
                if (thong_tin_vat_lieu_excel.SelectedItem == null)
                {
                    thong_tin_vat_lieu_excel.Items.Refresh();
                }
                else
                {
                    string fillPattern_Surface_name = "None";
                    if (item.name_surface_excel != "None" && myData_fillPattern.Any(x => x.value_image == item.name_surface_excel))
                    {
                        fillPattern_Surface_name = item.name_surface_excel;
                    }

                    string fillPattern_Cut_name = "None";
                    if (item.name_cut_excel != "None" && myData_fillPattern.Any(x => x.value_image == item.name_cut_excel))
                    {
                        fillPattern_Cut_name = item.name_cut_excel;
                    }

                    ten_vat_lieu.Text = item.ten_vat_lieu_excel;
                    ma_cong_tac.Text = item.ma_cong_tac_excel;
                    don_vi.Text = item.don_vi_excel;
                    color_shading.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(item.mau_vat_lieu_excel.Red, item.mau_vat_lieu_excel.Green, item.mau_vat_lieu_excel.Blue));
                    tranparency_bar.Value = item.do_trong_suot_vat_lieu_excel;
                    color_surface.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(item.mau_surface_excel.Red, item.mau_surface_excel.Green, item.mau_surface_excel.Blue));
                    name_surface.SelectedItem = myData_fillPattern.Find(x => x.value_image == fillPattern_Surface_name);
                    color_cut.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_cut_excel.Red, item.mau_cut_excel.Green, item.mau_cut_excel.Blue));
                    name_cut.SelectedItem = myData_fillPattern.Find(x => x.value_image == fillPattern_Cut_name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_Vat_Lieu_Excel(object sender, RoutedEventArgs e)
        {
            string result = Xoa_Vat_Lieu();
            if (result == "S")
            {
                MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //----------------------------------------------------------
        public string Xoa_Vat_Lieu()
        {
            string result = "F";
            try
            {
                if (thong_tin_vat_lieu_excel.SelectedItems.Count > 0)
                {
                    List<Material_Excel> list_delete = new List<Material_Excel>();
                    for(int i = 0; i < thong_tin_vat_lieu_excel.SelectedItems.Count; i++)
                    {
                        Material_Excel item = (Material_Excel)thong_tin_vat_lieu_excel.SelectedItems[i];
                        list_delete.Add(item);
                    }
                    if (list_delete.Count > 0)
                    {
                        foreach (Material_Excel excel in list_delete)
                        {
                            myMaterial_Project.Remove(excel);
                        }
                        result = "S";
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn xóa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Change_Value_Hang(object sender, TextChangedEventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(value_hang.Text))
                {
                    value_hang.Text = "2";
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(value_hang.Text, "[^0-9]"))
                { 
                    value_hang.Text = "2";
                }
                foreach(list_ten_du_lieu_excel position in myPosition_Excel)
                {
                    position.excel_ngang = value_hang.Text;
                }
                vi_tri_bat_dau.Items.Refresh();
            }
            catch (Exception)
            {

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

    }
}
