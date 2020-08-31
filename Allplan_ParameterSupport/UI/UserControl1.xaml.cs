using Allplan_ParameterSupport.Code.Function;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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

namespace Allplan_ParameterSupport
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

        E_AllplanData my_allplan_data;
        ExternalEvent e_allplan_data;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<Data> list_data { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_allplan_data = new E_AllplanData();
            e_allplan_data = ExternalEvent.Create(my_allplan_data);

            Function_Dau_Vao();
        }

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
                    List<string> format = new List<string>() ;
                    if (project_number != file_name[0]) format.Add("Project Number");

                    if (block != file_name[1]) format.Add("Block");

                    if (Class != file_name[3]) format.Add("Class");

                    if(format.Count() == 0)
                    {
                        F_GetData.search_material_project = search_material_project;
                        F_GetData.Them_Data_Vao_List_View(list_data, doc, number, thong_tin_parameter);
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
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Refresh();
                number.Content = thong_tin_parameter.Items.Count.ToString() + " items";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Hight_Light_Cau_Kien_Duoc_Chon(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_parameter.SelectedItems != null)
                {
                    Selection selection = uidoc.Selection;
                    List<ElementId> ids = new List<ElementId>();
                    for (int i = 0; i < thong_tin_parameter.SelectedItems.Count; i++)
                    {
                        Data item = (Data)thong_tin_parameter.SelectedItems[i];
                        ids.Add(item.cau_kien.Id);
                    }
                    selection.SetElementIds(ids);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_Data(object sender, RoutedEventArgs e)
        {
            F_GetData.search_material_project = search_material_project;
            F_GetData.Them_Data_Vao_List_View(list_data, doc, number, thong_tin_parameter);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Them_Thong_tin(object sender, RoutedEventArgs e)
        {
            try
            {
                my_allplan_data.thong_tin_parameter = thong_tin_parameter;
                e_allplan_data.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
