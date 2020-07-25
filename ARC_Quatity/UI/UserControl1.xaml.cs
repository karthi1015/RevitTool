using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
using Path = System.IO.Path;

namespace WEB_SaveAs
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
        public ObservableCollection<Element_Link> myElement_Link { get; set; }
        public ObservableCollection<Quatity> myQuatity_total { get; set; }
        public ObservableCollection<Quatity> myQuatity { get; set; }
        public ObservableCollection<Quatity> myQuatity_detail { get; set; }
        public ObservableCollection<Link_File> myLink_File { get; set; }

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
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, upload_image, excel_image});

                myElement_Link = new ObservableCollection<Element_Link>();
                myQuatity_total = new ObservableCollection<Quatity>();
                myQuatity = new ObservableCollection<Quatity>();
                myQuatity_detail = new ObservableCollection<Quatity>();
                myLink_File = new ObservableCollection<Link_File>();
                Get_ELement_Link_Or_NoLink(doc);

                Show_Khoi_Luong_Len_ListView();
                Show_Du_Lieu_Len_Chart();
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_ELement_Link_Or_NoLink(Document doc)
        {
            try
            {
                List<Element> elements = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(x => x.Parameters.Cast<Parameter>().Any(y => y.Definition.Name == "Volume" || y.Definition.Name == "Area" || y.Definition.Name == "Length") == true)
                    .Where(x => x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    //.Where(x => x.LookupParameter("Volume") != null && x.LookupParameter("Volume").AsDouble() != 0)
                    .ToList();
                foreach (Element element in elements)
                {
                    if (element is FamilyInstance)
                    {
                        FamilyInstance familyInstance = element as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            myElement_Link.Add(new Element_Link() { cau_kien = element, doc = doc });
                        }
                    }
                    else
                    {
                        myElement_Link.Add(new Element_Link() { cau_kien = element, doc = doc });
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        //----------------------------------------------------------
        public void Show_Du_Lieu_Len_Chart()
        {
            try
            {
                bieu_do_category.Series.Clear();

                List<string> category_name_check = new List<string>();
                foreach (Element_Link ele_link in myElement_Link)
                {
                    Element ele = ele_link.cau_kien;
                    if (category_name_check.Contains(ele.Category.Name) == false)
                    {
                        bieu_do_category.Series.Add(new PieSeries
                        {
                            Title = ele.Category.Name,
                            Values = new ChartValues<ObservableValue> { new ObservableValue(myElement_Link.Count(x => x.cau_kien.Category.Name == ele.Category.Name)) },
                            StrokeThickness = 1,
                            DataLabels = true,
                            FontSize = 12
                        });
                        category_name_check.Add(ele.Category.Name);
                    }
                }

                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Show_Khoi_Luong_Len_ListView()
        {
            try
            {
                myQuatity = new ObservableCollection<Quatity>();
                myQuatity_detail = new ObservableCollection<Quatity>();

                
                List<Level> levels = new List<Level>();
                new FilteredElementCollector(doc).OfClass(typeof(Level)).ToList().ForEach(x => levels.Add(x as Level));

                List<string> type_name = new List<string>();

                foreach (Element_Link element_link in myElement_Link)
                {
                    Element element = element_link.cau_kien;
                    Document doc = element_link.doc;
                    List<Material> list_materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().ToList();

                    ObservableCollection<Quatity> myQuatity_support = new ObservableCollection<Quatity>();
                    ObservableCollection<Quatity> myQuatity_detail_support = new ObservableCollection<Quatity>();
                    if (element is FamilyInstance)
                    {
                        FamilyInstance familyInstance = element as FamilyInstance;
                        Support_Get_Khoi_Luong(element, list_materials, levels, myQuatity_support, doc);
                        List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                        foreach (ElementId id in elements_child)
                        {
                            FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                            Support_Get_Khoi_Luong(model_child, list_materials, levels, myQuatity_support, doc);
                            Support_Show_Khoi_Luong_Len_ListView(model_child, list_materials, levels, myQuatity_support, doc);
                        }
                    }
                    else
                    {
                        Support_Get_Khoi_Luong(element, list_materials, levels, myQuatity_support, doc);
                    }

                    //List<Quatity> support = new List<Quatity>(myQuatity_support.GroupBy(x => new
                    //{
                    //    x.block,
                    //    x.level,
                    //    x.ten_vat_lieu,
                    //    x.ma_cong_tac,
                    //    x.don_vi,
                    //    x.color,
                    //    x.elevation,
                    //    x.link_file_name
                    //}).Select(y => new Quatity()
                    //{
                    //    block = y.Key.block,
                    //    level = y.Key.level,
                    //    ten_vat_lieu = y.Key.ten_vat_lieu,
                    //    ma_cong_tac = y.Key.ma_cong_tac,
                    //    quantity = y.Sum(x => x.quantity),
                    //    don_vi = y.Key.don_vi,
                    //    color = y.Key.color,
                    //    color_sort = y.Key.color.ToString(),

                    //    elevation = y.Key.elevation,
                    //    link_file_name = y.Key.link_file_name,
                    //    ten_cau_kien = y.First().ten_cau_kien,
                    //    id_cau_kien = y.First().id_cau_kien,
                    //    cau_kien = y.First().cau_kien
                    //})).ToList();

                    myQuatity_support.ToList().ForEach(x => myQuatity.Add(x));
                    myQuatity_support.ToList().ForEach(x => myQuatity_detail.Add(x));
                }

                myQuatity_total = new ObservableCollection<Quatity>(myQuatity.GroupBy(x => new
                {
                    x.block,
                    x.level,
                    x.ten_vat_lieu,
                    x.ma_cong_tac,
                    x.don_vi,
                    x.color,
                    x.elevation,
                    x.link_file_name
                }).Select(y => new Quatity()
                {
                    block = y.Key.block,
                    level = y.Key.level,
                    ten_vat_lieu = y.Key.ten_vat_lieu,
                    ma_cong_tac = y.Key.ma_cong_tac,
                    quantity = y.Sum(x => x.quantity),
                    don_vi = y.Key.don_vi,
                    color = y.Key.color,
                    color_sort = y.Key.color.ToString(),

                    elevation = y.Key.elevation,
                    link_file_name = y.Key.link_file_name,
                    ten_cau_kien = string.Join("|", y.Select(x => x.ten_cau_kien))
                }));

                thong_tin_quantity_total_project.ItemsSource = myQuatity_total;
                thong_tin_quantity_project.ItemsSource = myQuatity;
                thong_tin_detail.ItemsSource = myQuatity_detail;

                CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                view_total.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view_total.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Descending));
                view_total.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("color_sort", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("ten_cau_kien", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));

                CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                view_detail.SortDescriptions.Add(new SortDescription("ten_cau_kien", ListSortDirection.Ascending));
                view_detail.Filter = Filter_thong_tin_detail;

                bieu_do_download.Value = view_total.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Support_Show_Khoi_Luong_Len_ListView(FamilyInstance model_child, List<Material> list_materials, List<Level> levels, ObservableCollection<Quatity> myQuatity, Document doc)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        Support_Get_Khoi_Luong(model_child, list_materials, levels, myQuatity, doc);
                        Support_Show_Khoi_Luong_Len_ListView(model_child, list_materials, levels, myQuatity, doc);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Support_Get_Khoi_Luong(Element element, List<Material> list_materials, List<Level> levels, ObservableCollection<Quatity> myQuatity, Document doc)
        {
            try
            {
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

                                if (quantity != 0)
                                {
                                    myQuatity.Add(new Quatity()
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
            }
            catch (Exception)
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        Quatity item_select = null;
        private void Chon_Vat_Lieu_total(object sender, MouseButtonEventArgs e)
        {
            try
            {
                item_select = (Quatity)thong_tin_quantity_total_project.SelectedItem;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                view.Filter = Filter_thong_tin_detail;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_thong_tin_total(object item)
        {
            bool value = true;
            try
            {
                if (string.IsNullOrEmpty(search_material_project.Text) || search_material_project.Text == "...")
                {
                    return true;
                }
                else if (search_block.IsChecked == true)
                {
                    return ((item as Quatity).block.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else if (search_level.IsChecked == true)
                {
                    return ((item as Quatity).level.Equals(search_material_project.Text, StringComparison.OrdinalIgnoreCase));
                }
                else if (search_material.IsChecked == true)
                {
                    return ((item as Quatity).ma_cong_tac.IndexOf(search_material_project.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else
                {
                    return ((item as Quatity).ten_cau_kien.Split('|').Any(x => x.Equals(search_material_project.Text, StringComparison.OrdinalIgnoreCase)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_total(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Chon_Cau_Kien_detail(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<ElementId> ids = new List<ElementId>();
                for (int i = 0; i < thong_tin_detail.SelectedItems.Count; i++)
                {
                    Quatity item_select = (Quatity)thong_tin_detail.SelectedItems[i];
                    ids.Add(item_select.cau_kien.Id);
                }
                Selection selection = uidoc.Selection;
                selection.SetElementIds(ids);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_thong_tin_detail(object item)
        {
            bool value = false;
            try
            {
                if (item_select == null)
                    return false;
                else
                    return ((item as Quatity).ten_vat_lieu.Equals(item_select.ten_vat_lieu, StringComparison.OrdinalIgnoreCase) &&
                            (item as Quatity).block.Equals(item_select.block, StringComparison.OrdinalIgnoreCase) &&
                            (item as Quatity).level.Equals(item_select.level, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Chon_Cau_Kien_element(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<ElementId> ids = new List<ElementId>();
                for (int i = 0; i < thong_tin_quantity_project.SelectedItems.Count; i++)
                {
                    Quatity item_select = (Quatity)thong_tin_quantity_project.SelectedItems[i];
                    ids.Add(item_select.cau_kien.Id);
                }
                Selection selection = uidoc.Selection;
                selection.SetElementIds(ids);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Xem_Du_Lieu_Quantity_element(object sender, RoutedEventArgs e)
        {
            Search(); 
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        //----------------------------------------------------------
        public void Search()
        {
            try
            {
                if (total.IsChecked == true)
                {
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource);
                    view.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Refresh();
                }
                if (any.IsChecked == true)
                {
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource);
                    view.Filter = Filter_thong_tin_total;
                    CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Refresh();
                }

                if (any.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    item_select = null;
                    CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                    view_detail.Filter = Filter_thong_tin_detail;

                    bieu_do_download.Value = thong_tin_quantity_project.Items.Count;
                }

                if (total.IsChecked == true)
                {
                    Selection selection = uidoc.Selection;
                    selection.SetElementIds(new List<ElementId>());

                    item_select = (Quatity)thong_tin_quantity_total_project.SelectedItem;
                    CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
                    view_detail.Filter = Filter_thong_tin_detail;

                    bieu_do_download.Value = thong_tin_quantity_total_project.Items.Count;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_By(object sender, RoutedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Bao_Gom_Ca_Link_File(object sender, RoutedEventArgs e)
        {
            try
            {
                Get_List_Link_File(false);
                Khoi_Luong_Cua_Link_File();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //----------------------------------------------------------
        public void Data_For_ExtenalEvent()
        {
            try
            {
                myExampleDraw.bao_gom_link = bao_gom_link;
                myExampleDraw.myLink_File = myLink_File;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_List_Link_File(bool check)
        {
            try
            {
                myLink_File = new ObservableCollection<Link_File>();
                List<Level> levels = new List<Level>();
                new FilteredElementCollector(doc).OfClass(typeof(Level)).ToList().ForEach(x => levels.Add(x as Level));
                var list_link = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList();
                foreach(var link in list_link)
                {
                    double elevetion = 1000000000;
                    if (levels.Any(y => myFunctionSupport.RemoveUnicode(y.Name) == link.Name.Split('.')[0].Split('_')[1]))
                    {
                        elevetion = levels.First(y => myFunctionSupport.RemoveUnicode(y.Name) == link.Name.Split('.')[0].Split('_')[1]).Elevation;
                    }
                    myLink_File.Add(new Link_File()
                     {
                         ten_file = link.Name.Split('.')[0],
                         chon_file_link = check,
                         elevation = elevetion,
                         link_file = link
                    });
                }
                thong_tin_link_file.ItemsSource = myLink_File;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_link_file.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("chon_file_link", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("ten_file", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Khoi_Luong_Cua_Link_File()
        {
            try
            {
                List<Element_Link> list_remove = new List<Element_Link>();
                foreach (Link_File file in myLink_File)
                {
                    Document doc1 = uiapp.Application.Documents.Cast<Document>().First(x => x.Title == file.ten_file);
                    if (file.chon_file_link == true)
                    {
                        if (myElement_Link.Any(x => x.doc.Title == doc1.Title) == false)
                        {
                            myElement_Link.Where(x => x.doc.Title == doc1.Title).ToList().ForEach(y => list_remove.Add(y));

                            Get_ELement_Link_Or_NoLink(doc1);
                        }
                    }
                    else
                    {
                        myElement_Link.Where(x => x.doc.Title == doc1.Title).ToList().ForEach(y => list_remove.Add(y));
                    }
                }
                list_remove.ForEach(x => myElement_Link.Remove(x));

                if(myLink_File.Count() > 0)
                {
                    Show_Khoi_Luong_Len_ListView();
                    Show_Du_Lieu_Len_Chart();
                }
                Search();
                if (bao_gom_link.IsChecked == true) bao_gom_link.Foreground = myAll_Data.list_color_UI_data[2];
                else bao_gom_link.Foreground = myAll_Data.list_color_UI_data[3];

                Data_For_ExtenalEvent();
                myExampleDraw.command = "Visible Link File";
                Draw.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Lay_Hoac_Khong_Lay_Khoi_Luong_Link(object sender, RoutedEventArgs e)
        {
            Khoi_Luong_Cua_Link_File();
            if (myLink_File.Any(x => x.chon_file_link != true) == false) check_all.IsChecked = true;
            if (myLink_File.Any(x => x.chon_file_link == true) == false) check_all.IsChecked = false;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_Tat_Ca_Link_File(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Link_File link in myLink_File)
                {
                    link.chon_file_link = check_all.IsChecked.Value;
                }
                thong_tin_link_file.Items.Refresh();
                Khoi_Luong_Cua_Link_File();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Upload_Du_Lieu(object sender, RoutedEventArgs e)
        {
            string result = Upload();
            if (result == "S") MessageBox.Show("Upload Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //----------------------------------------------------------
        public string Upload()
        {
            string result = "F";
            try
            {               
                List<List<string>> values_material = new List<List<string>>();
                List<List<string>> values_element = new List<List<string>>();

                CollectionViewSource.GetDefaultView(thong_tin_quantity_total_project.ItemsSource).Cast<Quatity>().ToList().ForEach(x =>
                    values_material.Add(new List<string>()
                    {
                        x.block,
                        x.level,
                        x.ten_vat_lieu,
                        x.ma_cong_tac,
                        x.quantity.ToString(),
                        x.don_vi,
                        uiapp.Application.Username,
                        DateTime.Now.ToString()
                    }));

                CollectionViewSource.GetDefaultView(thong_tin_quantity_project.ItemsSource).Cast<Quatity>().ToList().ForEach(x =>
                    values_element.Add(new List<string>()
                    {
                        x.block,
                        x.level,
                        x.id_cau_kien,
                        x.ten_cau_kien,
                        x.ten_vat_lieu,
                        x.ma_cong_tac,
                        x.quantity.ToString(),
                        x.don_vi,
                        uiapp.Application.Username,
                        DateTime.Now.ToString()
                    }));

                List<List<List<string>>> values = new List<List<List<string>>>() { values_material, values_element };
                string path_excel = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(doc.PathName), myAll_Data.list_path_foder_data[4]));
                result = myFunctionSupport.Upload(doc, myAll_Data.list_procedure_data.Select(x => x.procedure_name).ToList(), values, myAll_Data, path_excel);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Element_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, upload_image, excel_image });

                myElement_Link = new ObservableCollection<Element_Link>();
                myQuatity_total = new ObservableCollection<Quatity>();
                myQuatity = new ObservableCollection<Quatity>();
                myQuatity_detail = new ObservableCollection<Quatity>();
                myLink_File = new ObservableCollection<Link_File>();

                Selection selection = uidoc.Selection;
                ObservableCollection<Element_Link> myElement_Link_support = new ObservableCollection<Element_Link>();
                selection.GetElementIds().Select(x => doc.GetElement(x))
                    .Where(x => x.Parameters.Cast<Parameter>().Any(y => y.Definition.Name == "Volume" || y.Definition.Name == "Area" || y.Definition.Name == "Length") == true)
                    .Where(x => x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    //.Where(x => x.LookupParameter("Volume") != null && x.LookupParameter("Volume").AsDouble() != 0)
                    .ToList().ForEach(item => myElement_Link_support.Add(new Element_Link() { cau_kien = item, doc = doc }));
                foreach (Element_Link element in myElement_Link_support)
                {
                    Element model = element.cau_kien;
                    
                    if (model is FamilyInstance)
                    {
                        FamilyInstance familyInstance = model as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            myElement_Link.Add(new Element_Link() { cau_kien = model, doc = doc });
                        }
                    }
                    else
                    {
                        myElement_Link.Add(new Element_Link() { cau_kien = model, doc = doc });
                    }
                }

                Show_Khoi_Luong_Len_ListView();
                Show_Du_Lieu_Len_Chart();
                Search();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Support_Foreach(FamilyInstance model_child)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        myElement_Link.Add(new Element_Link() { cau_kien = doc.GetElement(id_for), doc = doc });
                        Support_Foreach(model_child);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
