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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SetupTool_MaterialSetup
{
    /// <summary>
    /// Interaction logic for UserSourceSQL.xaml
    /// </summary>
    public partial class UserSourceSQL : Window
    {
        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

        UIApplication uiapp;

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
        public UserSourceSQL(UIApplication _uiapp, All_Data _myAll_Data)
        {
            InitializeComponent();
            myAll_Data = _myAll_Data;

            mySQL = new FunctionSQL();
            myFunctionSupport = new FunctionSupoort();
            mySource = new ListSource();

            uiapp = _uiapp;

            myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, cloud_image, add_image, modify_image, select_image });

            GetData_Source_Company();
        }

        //----------------------------------------------------------
        public ObservableCollection<Material_Company> myMaterial_Company { get; set; }
        public void GetData_Source_Company()
        {
            try
            {
                myMaterial_Company = new ObservableCollection<Material_Company>();
                var listtotal = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[3], "dbo.spVW_WorkIdDesCription_UserMaterial", mySource.type_Procedure, new List<string>(), new List<string>());
                int rows = listtotal.Rows.Count;
                for (var i = 0; i < rows; i++)
                {
                    myMaterial_Company.Add(new Material_Company()
                    {
                        ma_cong_tac_company = listtotal.Rows[i][0].ToString(),
                        ten_vat_lieu_company = listtotal.Rows[i][1].ToString(),
                        ten_vat_lieu_company_for_search = listtotal.Rows[i][2].ToString(),
                        user = listtotal.Rows[i][3].ToString(),
                        time = listtotal.Rows[i][4].ToString(),
                        don_vi_company = listtotal.Rows[i][5].ToString()
                    });
                }
                thong_tin_vat_lieu_company.ItemsSource = myMaterial_Company;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_company.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac_company", ListSortDirection.Ascending));

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
            if (String.IsNullOrEmpty(search_material_company.Text) || search_material_company.Text == "...")
                return true;
            else
                return ((item as Material_Company).ma_cong_tac_company.IndexOf(search_material_company.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as Material_Company).ten_vat_lieu_company.IndexOf(search_material_company.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            if (search_material_company.Text != "...")
            {
                CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_company.ItemsSource).Refresh();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Share_Data_From_Project(object sender, RoutedEventArgs e)
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
        private void Sua_Thong_Tin_Vat_Lieu_Company(object sender, RoutedEventArgs e)
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
                Material_Company item = (Material_Company)thong_tin_vat_lieu_company.SelectedItem;
                if (thong_tin_vat_lieu_company.SelectedItem != null)
                {
                    if (myMaterial_Company.Any(x => x.ten_vat_lieu_company == ten_vat_lieu.Text) == false)
                    {
                        MessageBox.Show("Không tìm thấy vật liệu để chỉnh sửa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        if (myMaterial_Company.Any(x => x.ma_cong_tac_company == ma_cong_tac.Text) == false)
                        {
                            MessageBox.Show("Không tìm thấy vật liệu để chỉnh sửa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(don_vi.Text))
                            {
                                var result_message = MessageBox.Show("Dữ liệu được chỉnh sửa sẽ không thể phục hồi.\nBạn có chắc muốn chỉnh sửa!", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                if (result_message == MessageBoxResult.Yes)
                                {
                                    List<string> Para1 = new List<string>() { "@WorkId" };
                                    List<string> Para1_Values = new List<string>() { ma_cong_tac.Text };
                                    mySQL.SQLDelete(myAll_Data.list_path_connect_SQL_data[3], "dbo.spDelete_UserMaterial", mySource.type_Procedure, Para1, Para1_Values);

                                    myMaterial_Company.Remove(item);

                                    List<string> Para = new List<string>()
                                        {
                                            "@WorkId",
                                            "@MaterialName",
                                            "@MaterialName_ForSearch",
                                            "@CreatedBy",
                                            "@Time",
                                            "@Unit"
                                        };
                                    List<string> Para_Values = new List<string>()
                                        {
                                            ma_cong_tac.Text,
                                            ten_vat_lieu.Text,
                                            myFunctionSupport.RemoveUnicode(ten_vat_lieu.Text),
                                            uiapp.Application.Username,
                                            DateTime.Now.ToString(),
                                            don_vi.Text
                                        };
                                    mySQL.SQLWrite(myAll_Data.list_path_connect_SQL_data[3], "dbo.spInsert_UserMaterial", mySource.type_Procedure, Para, Para_Values);

                                    List<Material_Company> data_material_modify = new List<Material_Company>() { };
                                    data_material_modify.Add(new Material_Company()
                                    {
                                        ma_cong_tac_company = ma_cong_tac.Text,
                                        ten_vat_lieu_company = ten_vat_lieu.Text,
                                        ten_vat_lieu_company_for_search = myFunctionSupport.RemoveUnicode(ten_vat_lieu.Text),
                                        user = uiapp.Application.Username,
                                        time = DateTime.Now.ToString(),
                                        don_vi_company = don_vi.Text
                                    });
                                    myMaterial_Company.Add(data_material_modify[0]);

                                    result = "S";
                                }
                            }
                            else
                            {
                                MessageBox.Show("\"Đơn Vị\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn chỉnh sửa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Them_Vat_Lieu_Company(object sender, RoutedEventArgs e)
        {
            string result = Them_Vat_Lieu();
            if (result == "S")
            {
                MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //----------------------------------------------------------
        public string Them_Vat_Lieu()
        {
            string result = "F";
            try
            {
                if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                {
                    if (myMaterial_Company.Any(x => x.ten_vat_lieu_company == ten_vat_lieu.Text) == false)
                    {
                        if (!string.IsNullOrEmpty(ma_cong_tac.Text))
                        {
                            if (myMaterial_Company.Any(x => x.ma_cong_tac_company == ma_cong_tac.Text) == false)
                            {
                                if (!string.IsNullOrEmpty(don_vi.Text))
                                {
                                    List<string> Para = new List<string>()
                                        {
                                            "@WorkId",
                                            "@MaterialName",
                                            "@MaterialName_ForSearch",
                                            "@CreatedBy",
                                            "@Time",
                                            "@Unit"
                                        };
                                    List<string> Para_Values = new List<string>()
                                        {
                                            ma_cong_tac.Text,
                                            ten_vat_lieu.Text,
                                            myFunctionSupport.RemoveUnicode(ten_vat_lieu.Text),
                                            uiapp.Application.Username,
                                            DateTime.Now.ToString(),
                                            don_vi.Text
                                        };
                                    mySQL.SQLWrite(myAll_Data.list_path_connect_SQL_data[3], "dbo.spInsert_UserMaterial", mySource.type_Procedure, Para, Para_Values);

                                    List<Material_Company> data_material_modify = new List<Material_Company>() { };
                                    data_material_modify.Add(new Material_Company()
                                        {
                                            ma_cong_tac_company = ma_cong_tac.Text,
                                            ten_vat_lieu_company = ten_vat_lieu.Text,
                                            ten_vat_lieu_company_for_search = myFunctionSupport.RemoveUnicode(ten_vat_lieu.Text),
                                            user = uiapp.Application.Username,
                                            time = DateTime.Now.ToString(),
                                            don_vi_company = don_vi.Text
                                        });
                                    myMaterial_Company.Add(data_material_modify[0]);
                                    result = "S";
                                }
                                else
                                {
                                    MessageBox.Show("\"Đơn Vị\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("\"Mã Công Tác\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("\"Mã Công Tác\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
            catch (Exception)
            {

            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Vat_Lieu_Company(object sender, MouseButtonEventArgs e)
        {
            Xem_Thong_Tin_Vat_Lieu();
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin_Vat_Lieu()
        {
            try
            {
                Material_Company item = (Material_Company)thong_tin_vat_lieu_company.SelectedItem;
                if (thong_tin_vat_lieu_company.SelectedItem != null)
                {
                    ten_vat_lieu.Text = item.ten_vat_lieu_company;
                    ma_cong_tac.Text = item.ma_cong_tac_company;
                    don_vi.Text = item.don_vi_company;
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_Vat_Lieu_Company(object sender, RoutedEventArgs e)
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
                Material_Company item = (Material_Company)thong_tin_vat_lieu_company.SelectedItem;
                if (thong_tin_vat_lieu_company.SelectedItem != null)
                {
                    var result_message = MessageBox.Show("Dữ liệu được xóa sẽ không thể phục hồi.\nBạn có chắc muốn xóa!", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result_message == MessageBoxResult.Yes)
                    {
                        List<string> Para1 = new List<string>() { "@WorkId" };
                        List<string> Para1_Values = new List<string>() { item.ma_cong_tac_company };
                        mySQL.SQLDelete(myAll_Data.list_path_connect_SQL_data[3], "dbo.spDelete_UserMaterial", mySource.type_Procedure, Para1, Para1_Values);

                        myMaterial_Company.Remove(item);
                        result = "S";
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn xóa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {

            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Chon_Vat_Lieu_Tu_Template_Database(object sender, RoutedEventArgs e)
        {
            Chon_Vat_Lieu();
        }

        //----------------------------------------------------------
        public void Chon_Vat_Lieu()
        {
            try
            {
                UserSourceTemplate userSourceTemplate = new UserSourceTemplate(myAll_Data);
                userSourceTemplate.Owner = this;
                userSourceTemplate.ShowDialog();
                ten_vat_lieu.Text = userSourceTemplate.ten_vat_lieu_send;
                ma_cong_tac.Text = userSourceTemplate.ma_cong_tac_send;
                don_vi.Text = userSourceTemplate.don_vi_send;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
