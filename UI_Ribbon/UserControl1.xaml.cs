using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using UI_Ribbon.Data;
using UI_Ribbon.Data.Binding;

namespace UI_Ribbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        public UserControl1()
        {
            InitializeComponent();

            Logout();
        }

        //----------------------------------------------------------
        string path = "";
        public string Logout()
        {
            string result = "F";
            try
            {
                string myIP = Dns.GetHostAddresses(Dns.GetHostName()).First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                path = Source.pathUserPassword + "\\" + myIP;

                if (Directory.Exists(Source.pathUserPassword) == false) Directory.CreateDirectory(Source.pathUserPassword);

                if (File.Exists(path))
                {
                    data_information data = JsonConvert.DeserializeObject<data_information>(File.ReadAllText(path));
                    File.Delete(path);
                    data_information listUser = new data_information()
                    {
                        user_name = data.user_name,
                        user_id = data.user_id
                    };
                    File.WriteAllText(path, JsonConvert.SerializeObject(listUser));

                    user.Text = data.user_name;
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                File.Delete(path);
            }
            return result;
        }

        string result = "F";
        public void Login()
        {
            try
            {
                if (user.Text != "" && password.Password != "")
                {
                    List<data_information> data = new List<data_information>();
                    var listDataUser = SQL.SQLRead(Source.path_WEB, "dbo.spRead_User", Source.type_Query, new List<string>(), new List<string>());
                    for (var i = 0; i < listDataUser.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(listDataUser.Rows[i]["SystemRole"].ToString()))
                        {
                            data.Add(new data_information()
                            {
                                user_name = listDataUser.Rows[i]["UserName"].ToString(),
                                user_id = listDataUser.Rows[i]["UserId"].ToString(),
                                user_password = listDataUser.Rows[i]["UserPassWord"].ToString(),
                                role = listDataUser.Rows[i]["SystemRole"].ToString()
                            });
                        }
                    }

                    List<data_role> role = new List<data_role>();
                    var listDataRole = SQL.SQLRead(Source.path_Manage, "dbo.sp_ReadData_RoleManage", Source.type_Query, new List<string>(), new List<string>());
                    for (var i = 0; i < listDataRole.Rows.Count; i++)
                    {
                        role.Add(new data_role()
                        {
                            role = listDataRole.Rows[i]["Role"].ToString(),
                            addin = listDataRole.Rows[i]["Addin"].ToString()
                        });
                    }

                    //var xx = data.Select(x => x.role).Intersect(role.Select(x => x.role));

                    var infor = data.Where(x => x.user_name == user.Text && BCrypt.Net.BCrypt.Verify(password.Password, x.user_password) == true).ToList();
                    if (infor.Count() == 1)
                    {
                        File.WriteAllText(path, JsonConvert.SerializeObject(infor.First()));
                        result = "S";
                    }
                    else if (infor.Count() == 0)
                    {
                        message.Text = Source.fail_incorrect;
                    }
                }
                else
                {
                    message.Text = Source.fail_empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = "F";
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login();
                visible_message.Visibility = Visibility.Visible;
                if (result == "S")
                {
                    message.Text = Source.success;
                }
            }
            catch (Exception)
            {

            }
        }

        private void Enter_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
                visible_message.Visibility = Visibility.Visible;
                if (result == "S")
                {
                    message.Text = Source.success;
                }
                e.Handled = true;
            }
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_TrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            this.Show();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            string result = Logout();
            if (result == "S")
            {
                MessageBox.Show("You have been successfully logged out!!!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.None);
            }
        }

        private void show_password(object sender, RoutedEventArgs e)
        {
            pass_support.Content = password.Password;
        }

        private void visible_message_Click(object sender, RoutedEventArgs e)
        {
            visible_message.Visibility = Visibility.Collapsed;
            if (result == "S")
            {
                this.Hide();
                password.Password = "";
            }
        }

        private void open_web_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://18.141.116.111/home/login");
        }
    }
}
