using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using Ellipse = Autodesk.Revit.DB.Ellipse;
using Line = Autodesk.Revit.DB.Line;
using TextElement = Autodesk.Revit.DB.TextElement;

namespace Draw_All
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        #region Main  ************************************************************************************************************************************************************************************
        FunctionSQL mySQL;
        ListSource mySource;

        Support_All mySupport_All;
        Support_Crop mySupport_Crop;
        Support_Draw mySupport_Draw;

        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        External_Draw myExternal_Draw;
        ExternalEvent Draw;
        External_Crop myExternal_Crop;
        ExternalEvent Crop;
        External_Sort_Section myExternal_Sort_Section;
        ExternalEvent Sort;

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
        public ObservableCollection<level_data> my_level_data_bottom { get; set; }
        public ObservableCollection<level_data> my_level_data_top { get; set; }
        public ObservableCollection<category_data> my_category_data { get; set; }
        public ObservableCollection<family_data> my_family_data { get; set; }
        public ObservableCollection<type_data> my_type_data { get; set; }
        public ObservableCollection<parameter_data> my_parameter_data { get; set; }
        public ObservableCollection<material_data> my_material_data { get; set; }
        public ObservableCollection<location_line_data> location_line_data { get; set; }
        public geometry_data data_point { get; set; }
        public XYZ origin = new XYZ(0, 0, 0);
        public Element host_of_opening { get; set; }
        public Element element_update { get; set; }

        public ObservableCollection<view_plan_name_data> my_view_plan_name_data_tem { get; set; }
        public ObservableCollection<view_plan_name_data> my_view_plan_name_data_apply { get; set; }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            myExternal_Draw = new External_Draw();
            Draw = ExternalEvent.Create(myExternal_Draw);
            myExternal_Crop = new External_Crop();
            Crop = ExternalEvent.Create(myExternal_Crop);
            myExternal_Sort_Section = new External_Sort_Section();
            Sort = ExternalEvent.Create(myExternal_Sort_Section);

            mySupport_All = new Support_All();
            mySupport_Draw = new Support_Draw();
            Data_for_Support_Draw();
            mySupport_Crop = new Support_Crop();
            Data_for_Support_Crop();

            mySQL = new FunctionSQL();
            mySource = new ListSource();

            var listtotal = mySQL.SQLRead(@"Server=18.141.116.111,1433\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789", "Select * from dbo.PathSource", "Query", new List<string>(), new List<string>());
            path = listtotal.Rows[0][1].ToString();
            Function_TXT();
            Function_Dau_Vao();
        }

        //----------------------------------------------------------
        public void Data_for_Support_Draw()
        {
            mySupport_Draw.my_level_data_bottom = my_level_data_bottom;
            mySupport_Draw.my_level_data_top = my_level_data_top;
            mySupport_Draw.my_category_data = my_category_data;
            mySupport_Draw.my_family_data = my_family_data;
            mySupport_Draw.my_type_data = my_type_data;
            mySupport_Draw.my_parameter_data = my_parameter_data;
            mySupport_Draw.my_material_data = my_material_data;
            mySupport_Draw.location_line_data = location_line_data;
            mySupport_Draw.data_point = data_point;
            mySupport_Draw.origin = origin;
            mySupport_Draw.host_of_opening = host_of_opening;
            mySupport_Draw.element_update = element_update;

            mySupport_Draw.search = search;
        }
        //----------------------------------------------------------
        public void Data_for_Support_Crop()
        {
            mySupport_Crop.my_view_plan_name_data_tem = my_view_plan_name_data_tem;
            mySupport_Crop.my_view_plan_name_data_apply = my_view_plan_name_data_apply;

            mySupport_Crop.search_view_tem = search_view_tem;
            mySupport_Crop.search_view_apply = search_view_apply;
        }

        //----------------------------------------------------------
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = mySupport_All.Get_Data_All(path);
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
                mySupport_All.Default_Image(myAll_Data, new List<Image>() { logo_image, select_image, modify_image, duplicate_image, revit_image, select_host_image });

                my_material_data = new ObservableCollection<material_data>();
                my_level_data_bottom = new ObservableCollection<level_data>();
                my_level_data_top = new ObservableCollection<level_data>();
                my_category_data = new ObservableCollection<category_data>();
                my_family_data = new ObservableCollection<family_data>();
                my_type_data = new ObservableCollection<type_data>();
                my_parameter_data = new ObservableCollection<parameter_data>();
                data_point = new geometry_data();

                my_view_plan_name_data_apply = new ObservableCollection<view_plan_name_data>();
                my_view_plan_name_data_tem = new ObservableCollection<view_plan_name_data>();

                mySupport_Draw.Add_Category(doc, category, myAll_Data);
                mySupport_Draw.Get_Location_Line(location_line);
                mySupport_Draw.Get_Material(doc, material);

                mySupport_Draw.Get_Level_Bottom(doc, level_bottom, elevation_bottom, myAll_Data);
                mySupport_Draw.Get_Level_Top(doc, level_top, elevation_top, myAll_Data);
                mySupport_Draw.Get_Family(doc, category, family, myAll_Data);
                mySupport_Draw.Get_Type(doc, family, type, material, search, myAll_Data);

                mySupport_Draw.Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
                mySupport_Draw.Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);

                mySupport_Crop.Get_View_Data(view_tem, view_apply, doc);

                Function_Dau_Vao_Section_Sort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Draw *************************************************************************************************************************************************************************************
        //----------------------------------------------------------
        public void Update_UK_MA()
        {
            try
            {
                type_data item_type = (type_data)type.SelectedItem;

                double offset = 0;
                if (item_type.type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM) != null)
                {
                    offset = item_type.type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).AsDouble() * myAll_Data.list_unit_value_data[2];
                }

                mySupport_Draw.my_parameter_data.Where(x => myAll_Data.list_parameter_tag.Contains(x.parameter_name))
                .ToList().ForEach(x => x.parameter_value = (Convert.ToDouble(elevation_bottom.Text) + offset).ToString());

                thong_tin_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        //----------------------------------------------------------
        public void Search()
        {
            try
            {
                if (search.Text != "search")
                {
                    CollectionViewSource.GetDefaultView(type.ItemsSource).Refresh();
                    type.SelectedItem = mySupport_Draw.my_type_data.First(x => x.single_value.Contains(search.Text));
                    //Get_Element();
                    mySupport_Draw.Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
                    Update_UK_MA();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Host(object sender, RoutedEventArgs e)
        {
            mySupport_Draw.Select_Host(uidoc, doc, host, level_bottom, level_top, elevation_bottom, elevation_top, myAll_Data,
                type, thong_tin_parameter);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Level_Bottom(object sender, EventArgs e)
        {
            mySupport_Draw.Select_Level_Bottom(level_bottom, elevation_bottom, myAll_Data, type, thong_tin_parameter);
        }

        private void Update_UK_MA_Bottom(object sender, TextChangedEventArgs e)
        {
            Update_UK_MA();
        }

        private void Enter_UK_MA_Bottom(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    elevation_bottom.Text = new System.Data.DataTable().Compute(elevation_bottom.Text.Trim(), null).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Level_Top(object sender, EventArgs e)
        {
            try
            {
                level_data item = (level_data)level_top.SelectedItem;
                elevation_top.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void Enter_UK_MA_Top(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    elevation_top.Text = new System.Data.DataTable().Compute(elevation_top.Text.Trim(), null).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Update_UK_MA_Top(object sender, TextChangedEventArgs e)
        {
            
        }
        
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_type_by_Category(object sender, EventArgs e)
        {
            mySupport_Draw.Get_Family(doc, category, family, myAll_Data);
            mySupport_Draw.Get_Type(doc, family, type, material, search, myAll_Data);
            mySupport_Draw.Search(doc, search, elevation_bottom, category, family, type, thong_tin_parameter, myAll_Data);

            mySupport_Draw.Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
            mySupport_Draw.Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Type_By_Family(object sender, EventArgs e)
        {
            mySupport_Draw.Get_Type(doc, family, type, material, search, myAll_Data);
            mySupport_Draw.Search(doc, search, elevation_bottom, category, family, type, thong_tin_parameter, myAll_Data);

            mySupport_Draw.Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
            mySupport_Draw.Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_Type(object sender, TextChangedEventArgs e)
        {
            Search();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Parameter_By_Type(object sender, EventArgs e)
        {
            mySupport_Draw.Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
            mySupport_Draw.Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
        }

        //----------------------------------------------------------
        public void Data_For_Extenal_Draw()
        {
            try
            {
                myExternal_Draw.origin = mySupport_Draw.origin;
                myExternal_Draw.location_line = location_line;
                myExternal_Draw.thong_tin_parameter = thong_tin_parameter;
                myExternal_Draw.b = b;
                myExternal_Draw.h = h;
                myExternal_Draw.material = material;
                myExternal_Draw.category = category;
                myExternal_Draw.elevation_bottom = elevation_bottom;
                myExternal_Draw.level_bottom = level_bottom;
                myExternal_Draw.elevation_top = elevation_top;
                myExternal_Draw.level_top = level_top;
                myExternal_Draw.family = family;
                myExternal_Draw.type = type;

                myExternal_Draw.my_type_data = mySupport_Draw.my_type_data;
                myExternal_Draw.my_parameter_data = mySupport_Draw.my_parameter_data;

                myExternal_Draw.data_point = mySupport_Draw.data_point;
                myExternal_Draw.host_of_opening = mySupport_Draw.host_of_opening;
                myExternal_Draw.element_update = mySupport_Draw.element_update;

                myExternal_Draw.myAll_Data = myAll_Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool multi = true;
        private void Get_Data_For_Revit(object sender, RoutedEventArgs e)
        {
            try
            {
                multi = false;
                Thread t = new Thread(Pick_Element);
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Pick_Element()
        {
            try
            {
                while (!multi)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        mySupport_Draw.Data_Pick_Element(uidoc, doc, center_mid_point, line, arc, ellipse, pick);
                        Function_Draw();
                    }));
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Function_Draw()
        {
            try
            {
                Data_For_Extenal_Draw();
                myExternal_Draw.command = "Draw By Revit";
                Draw.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Element_Update(object sender, RoutedEventArgs e)
        {
            mySupport_Draw.Select_Update(uidoc, doc,
                host, level_bottom, level_top, elevation_bottom, elevation_top, material, search, thong_tin_parameter, category, family, type, myAll_Data);
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Update_Element(object sender, RoutedEventArgs e)
        {
            Data_For_Extenal_Draw();
            myExternal_Draw.command = "Update By Revit";
            Draw.Raise();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Duplicate_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                category_data item = (category_data)category.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;
                if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == item_type.type.Name).ToList().Count() == 0)
                {
                    if (double.TryParse(b.Text, out double x) && item.single_value != myAll_Data.list_category_draw[0])
                    {
                        Data_For_Extenal_Draw();
                        myExternal_Draw.command = "Duplicate Type";
                        Draw.Raise();
                    }
                }
                else
                {
                    if (double.TryParse(b.Text, out double x) && double.TryParse(h.Text, out double y))
                    {
                        Data_For_Extenal_Draw();
                        myExternal_Draw.command = "Duplicate Type";
                        Draw.Raise();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Material(object sender, EventArgs e)
        {
            try
            {
                material_data item = (material_data)material.SelectedItem;
                material.ToolTip = item.single_value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Enter_b(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    b.Text = new System.Data.DataTable().Compute(b.Text.Trim(), null).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Enter_h(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    h.Text = new System.Data.DataTable().Compute(h.Text.Trim(), null).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Update_Data_Tag(object sender, RoutedEventArgs e)
        {
            Data_For_Extenal_Draw();
            myExternal_Draw.command = "Update Data Tag";
            Draw.Raise();
        }
        #endregion

        #region Crop  ************************************************************************************************************************************************************************************
        //----------------------------------------------------------
        public void Search_view_tem()
        {
            try
            {
                if (search_view_tem.Text != "search")
                {
                    CollectionViewSource.GetDefaultView(view_tem.ItemsSource).Refresh();
                    view_tem.SelectedItem = mySupport_Crop.my_view_plan_name_data_tem.First(x => x.single_value.Contains(search_view_tem.Text));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Search_view_apply()
        {
            try
            {
                if (search_view_apply.Text != "search")
                {
                    CollectionViewSource.GetDefaultView(view_apply.ItemsSource).Refresh();
                    view_apply.SelectedItem = mySupport_Crop.my_view_plan_name_data_apply.First(x => x.single_value.Contains(search_view_apply.Text));
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Apply_Crop(object sender, RoutedEventArgs e)
        {
            try
            {
                Data_for_External_Crop();
                myExternal_Crop.command = "Crop View";
                Crop.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Data_for_External_Crop()
        {
            try
            {
                myExternal_Crop.view_apply = view_apply;
                myExternal_Crop.view_tem = view_tem;

                myExternal_Crop.myAll_Data = myAll_Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_View_Tem(object sender, TextChangedEventArgs e)
        {
            Search_view_tem();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_View_Apply(object sender, TextChangedEventArgs e)
        {
            Search_view_apply();
        }

        #endregion

        #region Section Sort  ************************************************************************************************************************************************************************************
        public ObservableCollection<parameter_data> my_parameter_list_data { get; set; }
        //----------------------------------------------------------
        public void Function_Dau_Vao_Section_Sort()
        {
            try
            {
                my_parameter_list_data = new ObservableCollection<parameter_data>();

                Get_Data_Item();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Get_Data_Item()
        {
            try
            {
                var list_viewsection = new FilteredElementCollector(doc).OfClass(typeof(ViewSection)).Cast<ViewSection>().ToList();

                foreach(Parameter para in list_viewsection[0].Parameters)
                {
                    my_parameter_list_data.Add(new parameter_data()
                    {
                        parameter = para,
                        parameter_name = para.Definition.Name,
                        parameter_value = mySupport_All.Get_Parameter_Information(para, doc, myAll_Data)
                    });
                }

                paramater_1.ItemsSource = my_parameter_list_data;
                paramater_1.SelectedItem = my_parameter_list_data.First(X => X.parameter_name == "Ansichtstyp");
                CollectionView view_paramater_1 = (CollectionView)CollectionViewSource.GetDefaultView(paramater_1.ItemsSource);
                view_paramater_1.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));

                paramater_2.ItemsSource = my_parameter_list_data;
                paramater_2.SelectedItem = my_parameter_list_data.First(X => X.parameter_name == "Planart");
                CollectionView view_paramater_2 = (CollectionView)CollectionViewSource.GetDefaultView(paramater_2.ItemsSource);
                view_paramater_2.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));

                paramater_3.ItemsSource = my_parameter_list_data;
                paramater_3.SelectedItem = my_parameter_list_data.First(X => X.parameter_name == "Plantype");
                CollectionView view_paramater_3 = (CollectionView)CollectionViewSource.GetDefaultView(paramater_3.ItemsSource);
                view_paramater_3.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Data_For_Extennal_Sort()
        {
            myExternal_Sort_Section.myAll_Data = myAll_Data;

            myExternal_Sort_Section.parameter_1 = paramater_1;
            myExternal_Sort_Section.parameter_2 = paramater_2;
            myExternal_Sort_Section.parameter_3 = paramater_3;

            myExternal_Sort_Section.value_1 = value_1;
            myExternal_Sort_Section.value_2 = value_2;
            myExternal_Sort_Section.value_3 = value_3;

            myExternal_Sort_Section.sheet_start = sheet_start;
            myExternal_Sort_Section.sheet_end = sheet_end;
        }
        private void Section_Sort(object sender, RoutedEventArgs e)
        {
            try
            {
                Data_For_Extennal_Sort();
                myExternal_Sort_Section.command = "Sort Section";
                Sort.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion


    }
}
