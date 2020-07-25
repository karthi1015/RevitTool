using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace CompanyRibbon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string pathUserPassword = @"\\192.168.1.250\data\DataBases\09 DataAddIn";

        string pathconnectSQLFileStream = @"Server=18.141.116.111,1433\SQLEXPRESS;Database=FileStreamDataBase;User Id=FilestreamUser; Password = fileconnect@123";
        string pathconnectSQLWEB = @"Server=18.141.116.111,1433\SQLEXPRESS;Database=WEBDataBase;User Id=WebUser; Password = webconnect456";
        string pathconnectSQLManage = @"Server=18.141.116.111,1433\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789";
        string connetionStringRevit = @"Server=18.141.116.111,1433\SQLEXPRESS;Database=RevitDataBase;User Id=RevitUser; Password = revit@connect";
        string connetionStringAllplan = @"Server=18.141.116.111,1433\SQLEXPRESS;Database=AllplanDataBase;User Id=AllplanUser; Password = allplan@connect";

        FunctionSQL mySQL;

        string type_Procedure = "StoredProcedure";
        string type_Query = "Query";

        public MainWindow()
        {
            InitializeComponent();
            mySQL = new FunctionSQL();
            //RegisterInStartup(false);
            var path = Environment.GetEnvironmentVariable("PathUser", EnvironmentVariableTarget.User);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            Environment.SetEnvironmentVariable("PathUser", "", EnvironmentVariableTarget.User);
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
                    var listDataUser = mySQL.SQLRead(pathconnectSQLWEB, "dbo.spRead_User", type_Query, new List<string>(), new List<string>());
                    for (var i = 0; i < listDataUser.Count(); i = i + 6)
                    {
                        UserId.Add(listDataUser[i]);
                        UserName.Add(listDataUser[i + 1]);
                        Password.Add(listDataUser[i + 2]);
                        SystemRole.Add(listDataUser[i + 5]);
                    }

                    for(var i = 0; i < UserName.Count; i++)
                    {
                        if (UserName[i] == user.Text && Password[i] == password.Password)
                        {

                            List<string> lines = new List<string>();
                            lines.Add(UserId[i]);
                            lines.Add(Password[i]);
                            string pathFile = pathUserPassword + "\\" + UserId[i] + ".txt";
                            File.WriteAllLines(pathFile, lines);
                            File.SetAttributes(pathFile, FileAttributes.Encrypted);
                            File.SetAttributes(pathFile, FileAttributes.Hidden);


                            Environment.SetEnvironmentVariable("PathUser", pathFile, EnvironmentVariableTarget.User);

                            result =  "S";
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

        private void RegisterInStartup(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue("Login", Application.Current);
            }
            else
            {
                registryKey.DeleteValue("Login");
            }
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
        }
    }
}
