using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
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

namespace Tool_ViewInformation
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void closeWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ObservableCollection<Quantity> my_Quantity { get; set; }

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
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = myFunctionSupport.Get_Data_All(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, select_image, search_image});
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Element(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                Reference reference = selection.PickObject(ObjectType.Element);

                if (reference != null)
                {
                    Show_Khoi_Luong_Len_ListView(doc.GetElement(reference));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_Element(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(search_material_company.Text) && search_material_company.Text != "...")
                { 
                    Element element = null;
                    try
                    {
                        if (unique_id.IsChecked == false)
                        {
                            element = new FilteredElementCollector(doc)
                            .WhereElementIsNotElementType()
                            .Where(x => x.Id.ToString() == search_material_company.Text).First();
                        }
                        else
                        {
                            element = new FilteredElementCollector(doc)
                            .WhereElementIsNotElementType()
                            .Where(y => y.LookupParameter(myAll_Data.list_parameter_share_data[3]) != null && y.LookupParameter(myAll_Data.list_parameter_share_data[3]).AsString() == search_material_company.Text).First();
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }


                    if (element != null)
                    {
                        Show_Khoi_Luong_Len_ListView(element);
                    }
                    else
                    {
                        MessageBox.Show("Cấu kiện không tồn tại hoặc thông tin tìm kiếm không đúng!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Show_Khoi_Luong_Len_ListView(Element model)
        {
            try
            {
                my_Quantity = new ObservableCollection<Quantity>();
                Show_Khoi_Luong(model, my_Quantity);
                if (model is FamilyInstance)
                {
                    FamilyInstance familyInstance = model as FamilyInstance;
                    if (familyInstance.SuperComponent == null)
                    {
                        Support_Foreach(familyInstance, my_Quantity);
                    }
                }

                thong_tin_quantity.ItemsSource = my_Quantity;

                CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity.ItemsSource);
                view_total.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                Selection selection = uidoc.Selection;
                selection.SetElementIds(new List<ElementId>() { model.Id });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Support_Foreach(FamilyInstance model_child, ObservableCollection<Quantity> my_Quantity)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        Show_Khoi_Luong(model_child, my_Quantity);
                        Support_Foreach(model_child, my_Quantity);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Show_Khoi_Luong(Element element, ObservableCollection<Quantity> my_Quantity)
        {
            try
            {
                if (unique_id.IsChecked == true)
                {
                    if (element.LookupParameter(myAll_Data.list_parameter_share_data[3]) != null)
                    {
                        value_id.Text = element.LookupParameter(myAll_Data.list_parameter_share_data[3]).AsString();
                    }
                }
                else value_id.Text = element.Id.ToString();

                List<Level> levels = new List<Level>();
                new FilteredElementCollector(doc).OfClass(typeof(Level)).ToList().ForEach(x => levels.Add(x as Level));

                var list_materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();

                List<Material_Paint_Or_NoPaint> materials = new List<Material_Paint_Or_NoPaint>();

                myFunctionSupport.Get_All_Material_Of_Element_By_Category(doc, element, materials);
                myFunctionSupport.Get_All_Material_Of_Element(doc, element.GetMaterialIds(false), element.GetMaterialIds(true), materials);

                if (materials.Count() > 0)
                {
                    string block = doc.ProjectInformation.BuildingName;

                    string level = "";
                    if (element.LookupParameter(myAll_Data.list_parameter_share_data[4]).AsString() != null) level = element.LookupParameter(myAll_Data.list_parameter_share_data[4]).AsString();

                    string name = "";
                    if (element.LookupParameter(myAll_Data.list_parameter_share_data[2]).AsString() != null) name = element.LookupParameter(myAll_Data.list_parameter_share_data[2]).AsString();

                    string id_cau_kien = "";
                    if (element.LookupParameter(myAll_Data.list_parameter_share_data[3]).AsString() != null) id_cau_kien = element.LookupParameter(myAll_Data.list_parameter_share_data[3]).AsString();

                    foreach (Material_Paint_Or_NoPaint material in materials)
                    {
                        if (material.vat_lieu.Name.Contains("Default") == false && material.vat_lieu.Name.Contains("sub") == false)
                        {
                            string ma_cong_tac = "";
                            if (!string.IsNullOrEmpty(myFunctionSupport.Check_Para_And_Get_Para(material.vat_lieu, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name)))
                                ma_cong_tac = myFunctionSupport.Check_Para_And_Get_Para(material.vat_lieu, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);

                            if ((ma_cong_tac.Split('.').Count() > 0 && ma_cong_tac.Split('.')[0] == myAll_Data.list_mct_descipline_data.First(x => x.descipline == doc.Title.Split('_')[3]).mct) || ma_cong_tac == "")
                            {
                                string don_vi = "";
                                if (!string.IsNullOrEmpty(myFunctionSupport.Check_Para_And_Get_Para(material.vat_lieu, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name)))
                                    don_vi = myFunctionSupport.Check_Para_And_Get_Para(material.vat_lieu, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name);

                                Brush color = myAll_Data.list_color_UI_data[0];
                                if (string.IsNullOrEmpty(ma_cong_tac) || string.IsNullOrEmpty(level) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id_cau_kien)) color = myAll_Data.list_color_UI_data[1];

                                double ton = Convert.ToDouble(myFunctionSupport.Check_Para_And_Get_Para(material.vat_lieu, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name));

                                double quantity = myFunctionSupport.Get_Quantity(doc, don_vi, ton, element, material, myAll_Data, list_materials);

                                double elevation = 10000000000;
                                if (levels.Any(x => x.Name == level) == true) elevation = levels.First(x => x.Name == level).Elevation;

                                my_Quantity.Add(new Quantity()
                                {
                                    block = block,
                                    level = level,
                                    ten_cau_kien = name,
                                    id_cau_kien = id_cau_kien,
                                    ten_vat_lieu = material.vat_lieu.Name,
                                    ma_cong_tac = ma_cong_tac,
                                    quantity = quantity,
                                    don_vi = don_vi,
                                    color = color,
                                    color_sort = color.ToString(),

                                    cau_kien = element,
                                    vat_lieu = material,
                                    elevation = elevation,
                                    link_file_name = doc.Title
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        
    }
}
