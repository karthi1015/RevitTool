using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace UI_Ribbon
{
    public partial class UserControl1 : Window
    {
        string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";

        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

        string path = "";

        public UserControl1()
        {
            InitializeComponent();
            mySQL = new FunctionSQL();
            myFunctionSupport = new FunctionSupoort();
            mySource = new ListSource();

            var listtotal = mySQL.SQLRead(@"Server=18.141.116.111,1433\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789", "Select * from dbo.PathSource", "Query", new List<string>(), new List<string>());
            path_data = listtotal.Rows[0][1].ToString();
            Function_TXT();

            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            path = pathUserPassword + "\\" + myIP;

            if (Directory.Exists(pathUserPassword) == false) Directory.CreateDirectory(pathUserPassword);

            if (File.Exists(path))
            {
                var infor = File.ReadAllLines(path).ToList();
                File.Delete(path);
                List<string> listUser = new List<string>() { infor[0], infor[1] };
                File.WriteAllLines(path, listUser);

                user.Text = infor[0];
            }
        }

        //----------------------------------------------------------
        public string path_data = "";
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = myFunctionSupport.Get_Data_All(path_data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string Login()
        {
            string result = "F";
            try
            {
                if (user.Text != "" && password.Password != "")
                {
                    List<string> UserId = new List<string>();
                    List<string> UserName = new List<string>();
                    List<string> Password = new List<string>();
                    List<string> SystemRole = new List<string>();

                    var listDataUser = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[1], "dbo.spRead_User", mySource.type_Query, new List<string>(), new List<string>());
                    for (var i = 0; i < listDataUser.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(listDataUser.Rows[i]["SystemRole"].ToString()))
                        {
                            UserId.Add(listDataUser.Rows[i]["UserId"].ToString());
                            UserName.Add(listDataUser.Rows[i]["UserName"].ToString());
                            Password.Add(listDataUser.Rows[i]["UserPassWord"].ToString());
                            SystemRole.Add(listDataUser.Rows[i]["SystemRole"].ToString());
                        }
                    }

                    for (var i = 0; i < UserName.Count; i++)
                    {
                        if (UserName[i] == user.Text && BCrypt.Net.BCrypt.Verify(password.Password, Password[i]) == true)
                        {

                            List<string> listUser = new List<string>() { UserName[i], UserId[i], Password[i] };
                            File.WriteAllLines(path, listUser);

                            result = "S";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string result = Login();
                if (result == "S")
                {
                    MessageBox.Show("Login success!!!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User or password not exist!!!");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Enter_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string result = Login();
                if (result == "S")
                {
                    MessageBox.Show("Login success!!!");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("User or password not exist!!!");
                }
                e.Handled = true;
            }
            pass.Text = password.Password;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ViewPassword_Click(object sender, MouseButtonEventArgs e)
        {
            password.Visibility = Visibility.Hidden;
            pass.Visibility = Visibility.Visible;
        }

        private void HiddenPassword_Click(object sender, MouseButtonEventArgs e)
        {
            password.Visibility = Visibility.Visible;
            pass.Visibility = Visibility.Hidden;
        }
    }
}
