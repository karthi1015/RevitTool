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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SetupTool_MaterialSetup
{
    /// <summary>
    /// Interaction logic for UserSourceTemplate.xaml
    /// </summary>
    public partial class UserSourceTemplate : Window
    {
        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

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
        public UserSourceTemplate(All_Data _myAll_Data)
        {
            InitializeComponent();
            myAll_Data = _myAll_Data;

            mySQL = new FunctionSQL();
            myFunctionSupport = new FunctionSupoort();
            mySource = new ListSource();

            myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, select_image });

            GetData_Source_Company();
        }

        //----------------------------------------------------------
        public ObservableCollection<Material_MCT_Template> myMaterial_Template { get; set; }
        public void GetData_Source_Company()
        {
            try
            {
                myMaterial_Template = new ObservableCollection<Material_MCT_Template>();
                var list_MCT = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[3], "dbo.spRead_MCT", mySource.type_Procedure, new List<string>(), new List<string>());
                int rows_MCT = list_MCT.Rows.Count;
                for (var i = 0; i < rows_MCT; i++)
                {
                    myMaterial_Template.Add(new Material_MCT_Template()
                    {
                        ma_cong_tac_template_MCT = list_MCT.Rows[i][0].ToString(),
                        ten_vat_lieu_template_MCT = list_MCT.Rows[i][1].ToString(),
                        don_vi_template_MCT = list_MCT.Rows[i][2].ToString(),
                        ma_cong_tac_template_MCT_DM = list_MCT.Rows[i][3].ToString()
                    });
                }
                thong_tin_vat_lieu_template.ItemsSource = myMaterial_Template;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_template.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_template_MCT", ListSortDirection.Ascending));

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
            if (String.IsNullOrEmpty(search_material_template.Text) || search_material_template.Text == "...")
                return true;
            else
                return ((item as Material_MCT_Template).ma_cong_tac_template_MCT.IndexOf(search_material_template.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Material_MCT_Template).ten_vat_lieu_template_MCT.IndexOf(search_material_template.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            if (search_material_template.Text != "...")
            {
                CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_template.ItemsSource).Refresh();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Share_Data_From_Company(object sender, RoutedEventArgs e)
        {
            SendOrNotSend();
            Close();
        }

        //----------------------------------------------------------
        public string ten_vat_lieu_send = "";
        public string ma_cong_tac_send = "";
        public string don_vi_send = "";
        public void SendOrNotSend()
        {
            try
            {
                ten_vat_lieu_send = ten_vat_lieu.Text;
                ma_cong_tac_send = ma_cong_tac.Text;
                don_vi_send = don_vi.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu_Template(object sender, MouseButtonEventArgs e)
        {
            Xem_Thong_Tin_Vat_Lieu();
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin_Vat_Lieu()
        {
            try
            {
                Material_MCT_Template item = (Material_MCT_Template)thong_tin_vat_lieu_template.SelectedItem;
                if (thong_tin_vat_lieu_template.SelectedItem != null)
                {
                    List<Material_Sub_Template> myMaterial_Sub_Template = new List<Material_Sub_Template>();
                    List<Material_DM_Template> myMaterial_DM_Template = new List<Material_DM_Template>();

                    var list_DM = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[3], "dbo.spRead_DM", mySource.type_Procedure, new List<string>() { "@DBSubId" }, new List<string>() { item.ma_cong_tac_template_MCT_DM });
                    int rows_DM = list_DM.Rows.Count;
                    for (var i = 0; i < rows_DM; i++)
                    {
                        myMaterial_DM_Template.Add(new Material_DM_Template()
                        {
                            so_thu_tu_DM = Convert.ToInt32(list_DM.Rows[i][0].ToString()),
                            ma_cong_tac_template_MCT_DM = list_DM.Rows[i][1].ToString(),
                            ma_cong_tac_template_DM_VL = list_DM.Rows[i][2].ToString(),
                            so_luong_template_DM = list_DM.Rows[i][3].ToString()
                        });
                    }
                    foreach (Material_DM_Template DM in myMaterial_DM_Template)
                    {
                        var list_VL = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[3], "dbo.spRead_VL", mySource.type_Procedure, new List<string>() { "@DBMaterialCode_DM" }, new List<string>() {DM.ma_cong_tac_template_DM_VL });
                        int rows_VL = list_VL.Rows.Count;
                        if (rows_VL > 0)
                        {
                            myMaterial_Sub_Template.Add(new Material_Sub_Template()
                            {
                                no_sub_template = DM.so_thu_tu_DM,
                                ten_vat_lieu_sub_template = list_VL.Rows[0][1].ToString(),
                                so_luong_sub_template = DM.so_luong_template_DM,
                                don_vi_sub_template = list_VL.Rows[0][2].ToString()
                            });
                        }
                    }
                    thong_tin_vat_lieu_sub_template.ItemsSource = myMaterial_Sub_Template;

                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_sub_template.ItemsSource);
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
    }
}
