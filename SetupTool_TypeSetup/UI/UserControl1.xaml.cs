using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
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
using ComboBox = System.Windows.Controls.ComboBox;

namespace SetupTool_TypeSetup
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
        public ObservableCollection<Data> list_descipline_name { get; set; }
        public ObservableCollection<Data> list_category_name { get; set; }                                              // Lay_Du_Lieu_Category
        public ObservableCollection<Family_Type> myFamily_Type { get; set; }                                            // Show_Tat_Ca_Family_Type
        public ObservableCollection<Parameters_Family> myParameters_Family { get; set; }                                // Xem_Thong_Tin
        public ObservableCollection<Material_Family> myMaterial_Family { get; set; }                                    // Xem_Thong_Tin
        public ObservableCollection<Data> materials { get; set; }                                                       // Xem_Thong_Tin

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
            myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, duplicate_image, modify_image, refresh_image });

            Lay_Du_Lieu_Category();
            myParameters_Family = new ObservableCollection<Parameters_Family>();
            Lay_Type_Duoc_Chon();
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
        public void Lay_Type_Duoc_Chon()
        {
            try
            {
                Selection selection = uidoc.Selection;
                var element_selects = selection.GetElementIds().ToList();
                if (element_selects.Count() == 1)
                {
                    Element element_select = doc.GetElement(element_selects[0]);
                    ElementType type = doc.GetElement(element_select.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;

                    string descipline_name = type.Name.Split('_')[0];

                    string position = type.Name.Split('_')[1];

                    string category_name = type.Name.Split('_')[2];

                    descipline.SelectedItem = list_descipline_name.First(x => x.ten_type == descipline_name);

                    if (position == myAll_Data.list_position_data[0].position_key) outhome.IsChecked = true;
                    else if (position == myAll_Data.list_position_data[1].position_key) inhome.IsChecked = true;
                    else kc.IsChecked = true;

                    category.SelectedItem = list_category_name.First(x => x.ten_type == category_name);

                    Show_Tat_Ca_Family_Type();
                    foreach(Family_Type family in myFamily_Type)
                    {
                        if (family.ten_family_type == type.FamilyName)
                        {
                            family.ValueExpanded = true;
                            foreach (Element_Type element_Type in family.Children)
                            {
                                if (element_Type.ten_element_type == type.Name)
                                {
                                    element_Type.ValueIsSelect = true;
                                    Xem_Thong_Tin(element_Type);
                                }
                            }
                        }
                    }
                    thong_tin_family_type.Items.Refresh();
                }
                //else
                //{
                //    category.SelectedItem = list_category_name.First(x => x.single_value == myAll_Data.list_category_data[0].single_value);
                //    Show_Tat_Ca_Family_Type();
                //}
                Change();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Lay_Du_Lieu_Category()
        {
            list_descipline_name = myAll_Data.list_descipline_data;
            descipline.ItemsSource = list_descipline_name;
            descipline.SelectedIndex = 0;

            list_category_name = myAll_Data.list_category_data;
            category.ItemsSource = list_category_name;
            category.SelectedIndex = 0;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Element_Type_By_Category(object sender, EventArgs e)
        {
            Show_Tat_Ca_Family_Type();
            Change();
        }

        //-----------------------------------------------------------
        public void Show_Tat_Ca_Family_Type()
        {
            try
            {
                myFamily_Type = new ObservableCollection<Family_Type>();
                myParameters_Family = new ObservableCollection<Parameters_Family>();
                myMaterial_Family = new ObservableCollection<Material_Family>();
                thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<Parameters_Family>();
                thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<Material_Family>();

                Data descipline_data = (Data)descipline.SelectedItem;
                string position = "";
                if (outhome.IsChecked == true) position = myAll_Data.list_position_data[0].position_key;
                else if (inhome.IsChecked == true) position = myAll_Data.list_position_data[1].position_key;
                else position = myAll_Data.list_position_data[2].position_key;
                Data category_data = (Data)category.SelectedItem;
                
                var elements = new FilteredElementCollector(uidoc.Document).OfClass(typeof(ElementType)).ToElements();
                var familys = new FilteredElementCollector(uidoc.Document).OfClass(typeof(FamilySymbol)).ToElements();
                List<string> family_check = new List<string>();
                foreach (ElementType ele_family in elements)
                {
                    if (ele_family.Name.Split('_')[0] == descipline_data.ten_type && ele_family.Name.Split('_')[1] == position && ele_family.Name.Split('_')[2] == category_data.ten_type)
                    {
                        if (family_check.Contains(ele_family.FamilyName) == false)
                        {
                            ObservableCollection<Element_Type> myElement_Type = new ObservableCollection<Element_Type>();
                            foreach (ElementType ele_type in elements)
                            {
                                if (ele_family.FamilyName == ele_type.FamilyName && ele_type.Name.Split('_')[0] == descipline_data.ten_type && ele_type.Name.Split('_')[1] == position && ele_type.Name.Split('_')[2] == category_data.ten_type)
                                {
                                    string type_type = mySource.type_element;
                                    if (familys.Any(x => x.Id.IntegerValue == ele_type.Id.IntegerValue))
                                    {
                                        type_type = mySource.type_symbol;
                                    }
                                    myElement_Type.Add(new Element_Type()
                                    {
                                        ten_element_type = ele_type.Name,
                                        element_type = ele_type,
                                        type_type = type_type,
                                        delete_type = false,
                                        ValueIsSelect = false,
                                    });
                                }
                            }
                            ObservableCollection<Element_Type> myElement_Type_sort = new ObservableCollection<Element_Type>(myElement_Type.OrderBy(x => x.ten_element_type).ToList());

                            myFamily_Type.Add(new Family_Type()
                            {
                                ten_family_type = ele_family.FamilyName,
                                path = myFunctionSupport.Create_Preview(ele_family, myAll_Data),
                                ValueExpanded = false,
                                Children = myElement_Type_sort
                            });
                        }
                        family_check.Add(ele_family.FamilyName);
                    }
                }

                thong_tin_family_type.ItemsSource = myFamily_Type;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_family_type.ItemsSource);
                // sort list view
                view.SortDescriptions.Add(new SortDescription("ten_family_type", ListSortDirection.Ascending));

                // filter list view theo tên vật liệu
                try { view.Filter = Filter_ten_vat_lieu; } catch (Exception){}
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
                return ((item as Family_Type).ten_family_type.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (search_material_project.Text != "...")
                {
                    CollectionViewSource.GetDefaultView(thong_tin_family_type.ItemsSource).Refresh();
                }
            }
            catch (Exception)
            {

            }

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Thong_Tin_Element_Type(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem is Element_Type)
                {
                    Element_Type type = (Element_Type)thong_tin_family_type.SelectedItem;
                    Xem_Thong_Tin(type);
                }
                Change();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Xem_Thong_Tin(Element_Type type)
        {
            try
            {
                myParameters_Family = new ObservableCollection<Parameters_Family>();
                myMaterial_Family = new ObservableCollection<Material_Family>();
                materials = new ObservableCollection<Data>();

                var elements = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                materials.Add(new Data()
                {
                    single_value = "<By Category>",
                    vat_lieu = null
                });
                foreach (Material material in elements)
                {
                    materials.Add(new Data()
                    {
                        single_value = material.Name,
                        vat_lieu = material
                    });
                }
                var a = materials.OrderBy(x => x.single_value).ToList();
                materials.Clear();
                foreach (var b in a)
                {
                    materials.Add(b);
                }

                if (type.type_type == mySource.type_symbol)
                {
                    myFunctionSupport.View_Dimensions_FamilySymbol(doc, type.element_type, thong_tin_kich_thuoc, myParameters_Family, myAll_Data);
                    myFunctionSupport.View_Material_FamilySymbol(doc, type.element_type, thong_tin_cong_tac_vat_lieu, myMaterial_Family, materials, myAll_Data);
                }
                else
                {
                    myFunctionSupport.View_Dimensions_And_Material_System(doc, type.element_type, thong_tin_kich_thuoc, myParameters_Family,
                                                                                                 thong_tin_cong_tac_vat_lieu, myMaterial_Family, materials, myAll_Data);
                }
                try
                {
                    var list_name = type.element_type.Name.Split('_').ToList();
                    list_name.RemoveRange(0, 3);
                    custom.Text = string.Join("_", list_name);
                    Change();
                }
                catch (Exception)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Change_Name_Descipline(object sender, EventArgs e)
        {
            Show_Tat_Ca_Family_Type();
            Change();
        }

        private void Change_Name_Position(object sender, RoutedEventArgs e)
        {
            Show_Tat_Ca_Family_Type();
            Change();
        }

        private void Change_Name_Custom(object sender, TextChangedEventArgs e)
        {
            Change();
        }

        //----------------------------------------------------------
        public void Change()
        {
            try
            {
                var ptc = "";
                Data item = (Data)descipline.SelectedItem;

                string position = "";
                if (outhome.IsChecked == true) position = myAll_Data.list_position_data[0].position_key;
                else if (inhome.IsChecked == true) position = myAll_Data.list_position_data[1].position_key;
                else position = myAll_Data.list_position_data[2].position_key;

                Data item1 = (Data)category.SelectedItem;

                //List<string> text2 = new List<string>();
                //foreach (Parameters_Family para in myParameters_Family)
                //{
                //    if (para.group_parameter == "Dimensions" && Convert.ToDouble(para.gia_tri_parameter) > 0)
                //    {
                //        text2.Add(para.gia_tri_parameter.ToString());
                //    }
                //}
                name.Text = ptc + item.ten_type + "_" + position + "_" + item1.ten_type + "_" + custom.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_All_Du_Lieu(object sender, RoutedEventArgs e)
        {
            Lay_Type_Duoc_Chon();
        }

        //----------------------------------------------------------
        public void Data_for_ExternalEvent()
        {
            myExampleDraw.myAll_Data = myAll_Data;
            myExampleDraw.thong_tin_kich_thuoc = thong_tin_kich_thuoc;
            myExampleDraw.thong_tin_cong_tac_vat_lieu = thong_tin_cong_tac_vat_lieu;
            myExampleDraw.name = name;
            myExampleDraw.thong_tin_family_type = thong_tin_family_type;
            myExampleDraw.myFamily_Type = myFamily_Type;
            myExampleDraw.myParameters_Family = myParameters_Family;
            myExampleDraw.myMaterial_Family = myMaterial_Family;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Sua_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem is Element_Type)
                {
                    Element_Type type = (Element_Type)thong_tin_family_type.SelectedItem;
                    if (myFamily_Type.First(x => x.ten_family_type == type.element_type.FamilyName).Children.Any(x => x.ten_element_type == name.Text) == false || type.ten_element_type == name.Text)
                    {
                        myExampleDraw.command = "Modify";
                        Data_for_ExternalEvent();
                        Draw.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Tên Type không được trùng nhau", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn type muốn sửa và thử lại lần nữa!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Tao_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (thong_tin_family_type.SelectedItem is Element_Type)
                {
                    Element_Type type = (Element_Type)thong_tin_family_type.SelectedItem;
                    if (myFamily_Type.First(x => x.ten_family_type == type.element_type.FamilyName).Children.Any(x => x.ten_element_type == name.Text) == false)
                    {
                        myExampleDraw.command = "Create";
                        Data_for_ExternalEvent();
                        Draw.Raise();
                    }
                    else
                    {
                        MessageBox.Show("Tên Type không được trùng nhau", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn type muốn duplicate và thử lại lần nữa!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xoa_Type(object sender, RoutedEventArgs e)
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

    }
}
