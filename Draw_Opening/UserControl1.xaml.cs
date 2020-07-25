using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Draw_Opening
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
        public ObservableCollection<level_data> my_level_data { get; set; }
        public ObservableCollection<level_data> my_level_data_support { get; set; }
        public ObservableCollection<family_data> my_family_data { get; set; }
        public ObservableCollection<type_data> my_type_data { get; set; }
        public ObservableCollection<parameter_data> my_parameter_data { get; set; }

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
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, select_image, modify_image, duplicate_image, revit_image, cad_image});

                my_level_data = new ObservableCollection<level_data>();
                my_level_data_support = new ObservableCollection<level_data>();
                my_family_data = new ObservableCollection<family_data>();
                my_type_data = new ObservableCollection<type_data>();
                my_parameter_data = new ObservableCollection<parameter_data>();
                data_point = new List<List<XYZ>>();

                element = null;

                Get_Level();
                Get_Family();
                Get_Type();

                //Get_Element();
                Get_Parameter(element);
                Update_UK_MA();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public FamilyInstance element { get; set; }
        public void Get_Element()
        {
            try
            {
                family_data item_family = (family_data)family.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;
                ElementMulticategoryFilter filter_category = new ElementMulticategoryFilter(mySource.my_BuiltInCategory);
                element = new FilteredElementCollector(doc)
                    .WherePasses(filter_category)
                    .WhereElementIsNotElementType()
                    .First(x => x.Name == item_type.single_value && x.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString() == item_family.single_value) as FamilyInstance;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Level()
        {
            try
            {
                my_level_data = new ObservableCollection<level_data>(new FilteredElementCollector(doc)
                    .OfClass(typeof(Level)).Cast<Level>()
                    .OrderBy(x => x.Elevation)
                    .ToList().Select(x => new level_data()
                {
                    level = x,
                    single_value = x.Name,
                    elevation = x.Elevation
                }));

                level.ItemsSource = my_level_data;
                level.SelectedIndex = 0;
                level_data item = (level_data)level.SelectedItem;
                elevation.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Family()
        {
            try
            {
                string category_name = "";
                if (door.IsChecked == true) category_name = door.Content.ToString();
                else if (window.IsChecked == true) category_name = window.Content.ToString();
                else category_name = generic_model.Content.ToString();

                ElementMulticategoryFilter filter_category = new ElementMulticategoryFilter(mySource.my_BuiltInCategory.Where(x => Category.GetCategory(doc, x).Name == category_name).ToList());
                my_family_data = new ObservableCollection<family_data>(new FilteredElementCollector(doc)
                    .WherePasses(filter_category)
                    .WhereElementIsElementType()
                    .Cast<ElementType>()
                    .GroupBy(x => new
                    {
                        x.FamilyName,
                    })
                    .ToList().Select(x => new family_data()
                    {
                        single_value = x.Key.FamilyName,
                        path_image = myFunctionSupport.CreatePreview(x.First(), myAll_Data),
                        types = x.Select(y => y).ToList()
                    }).OrderBy(x => x.single_value));

                if(my_family_data.Count() == 0)
                {
                    my_family_data = new ObservableCollection<family_data>()
                    {
                        new family_data()
                        {
                            single_value = "",
                            path_image = "",
                            types = new List<ElementType>()
                        }
                    };
                }

                family.ItemsSource = my_family_data;
                family.SelectedIndex = 0;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(family.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Type()
        {
            try
            {
                family_data item = (family_data)family.SelectedItem;
                if (item != null && item.single_value != "")
                {
                    my_type_data = new ObservableCollection<type_data>(my_family_data
                    .Where(x => x.single_value == item.single_value)
                    .First().types.Select(xx => new type_data()
                    {
                        type = xx,
                        single_value = xx.Name
                    }).OrderBy(x => x.single_value));
                }
                else
                {
                    my_type_data = new ObservableCollection<type_data>()
                    {
                        new type_data()
                        {
                            type = null,
                            single_value = ""
                        }
                    };
                }

                type.ItemsSource = my_type_data;
                type.SelectedIndex = 0;

                if (search.Text != "search" && door.IsChecked == true)
                {
                    CollectionViewSource.GetDefaultView(type.ItemsSource).Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value.Contains(search.Text));
                }

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(type.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
                view.Filter = Filter_ten_vat_lieu;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Parameter(FamilyInstance element)
        {
            try
            {
                family_data item = (family_data)family.SelectedItem;
                if (item != null && item.single_value != "")
                {
                    if (element != null)
                    {
                        my_parameter_data = new ObservableCollection<parameter_data>(myFunctionSupport.View_Dimensions_FamilySymbol(doc, element, myAll_Data));
                    }
                    else
                    {
                        Family fa = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.Name == item.single_value).First();
                        ObservableCollection<parameter_data> support = new ObservableCollection<parameter_data>();
                        if (fa.IsEditable)
                        {
                            Document familyDoc = doc.EditFamily(fa);
                            FamilyManager manager = familyDoc.FamilyManager;

                            foreach (FamilyParameter para in manager.Parameters)
                            {
                                try
                                {
                                    if (para.IsInstance && para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY && mySource.para_name_not_use.Contains(para.Definition.Name) == false && para.IsReadOnly == false)
                                    {
                                        support.Add(new parameter_data()
                                        {
                                            parameter_name = para.Definition.Name,
                                            parameter_value = myFunctionSupport.Get_Parameter_Information_Family(para, familyDoc, manager, myAll_Data),
                                            parameter = null
                                        });
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                            familyDoc.Close(false);
                        }

                        my_parameter_data = support;
                    }

                    thong_tin_parameter.ItemsSource = my_parameter_data;

                    CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource);
                    view_total.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));
                }
                else
                {
                    thong_tin_parameter.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public Element host_of_opening { get; set; }
        private void Select_Host(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                SelectElementsFilter conduitFilter = new SelectElementsFilter(mySource.my_select_host_filter);
                var element = doc.GetElement(selection.PickObject(ObjectType.Element, conduitFilter, "Please select floor or wall element"));
                if (element != null)
                {
                    host.Content = element.Name;
                    host.ToolTip = element.UniqueId;

                    host_of_opening = element;
                    level.SelectedItem = my_level_data.First(x => x.single_value == doc.GetElement(element.LevelId).Name);
                    level.IsEnabled = true;

                    level_data item = (level_data)level.SelectedItem;
                    elevation.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();

                    Update_UK_MA();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Level(object sender, EventArgs e)
        {
            try
            {
                level_data item = (level_data)level.SelectedItem;
                elevation.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();

                Update_UK_MA();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_By_Category(object sender, RoutedEventArgs e)
        {
            Get_Family();
            Get_Type();

            //Get_Element();
            Get_Parameter(element);
            Update_UK_MA();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Type_By_Family(object sender, EventArgs e)
        {
            Get_Type();

            //Get_Element();
            Get_Parameter(element);
            Update_UK_MA();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Parameter_By_Type(object sender, EventArgs e)
        {
            //Get_Element();
            Get_Parameter(element);
            Update_UK_MA();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Select_Element_Update(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                SelectElementsFilter conduitFilter = new SelectElementsFilter(mySource.my_select_filter);
                element = doc.GetElement(selection.PickObject(ObjectType.Element, conduitFilter, "Please select opening element")) as FamilyInstance;
                if (element != null)
                {
                    host.Content = element.Host.Name;
                    host.ToolTip = element.Host.UniqueId;

                    host_of_opening = element.Host;
                    if(new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == host_of_opening.Name).ToList().Count() > 0)
                    {
                        level.SelectedItem = my_level_data.First(x => x.single_value == element.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString());
                        if (element.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).IsReadOnly == true) level.IsEnabled = false; else level.IsEnabled = true;
                    }
                    else
                    {
                        level.SelectedItem = my_level_data.First(x => x.single_value == doc.GetElement(element.LevelId).Name);
                        if (element.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).IsReadOnly == true) level.IsEnabled = false; else level.IsEnabled = true;
                    }

                    level_data item = (level_data)level.SelectedItem;
                    if(element.Category.Name == "Doors")
                    {
                        elevation.Text = Math.Round((item.elevation + element.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).AsDouble()) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                        door.IsChecked = true;
                    }
                    else
                    {
                        elevation.Text = Math.Round((item.elevation + element.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble()) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                        generic_model.IsChecked = true;
                    }

                    Get_Family();
                    family.SelectedItem = my_family_data.First(x => x.single_value == element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString());

                    Get_Type();
                    type.SelectedItem = my_type_data.First(x => x.single_value == element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString());

                    Get_Parameter(element);

                    Update_UK_MA();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Draw_Opening(object sender, RoutedEventArgs e)
        {
            if(by_revit.IsChecked == true)
            {
                Data_For_ExtenalEvent();
                myExampleDraw.command = "Draw By Revit";
                Draw.Raise();
            }
            else
            {
                Data_For_ExtenalEvent();
                myExampleDraw.command = "Draw By CAD";
                Draw.Raise();
            }
            data_point.Clear();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Update_Opening(object sender, RoutedEventArgs e)
        {
            Data_For_ExtenalEvent();
            myExampleDraw.command = "Update By Revit";
            Draw.Raise();
            data_point.Clear();
        }

        //----------------------------------------------------------
        public void Data_For_ExtenalEvent()
        {
            try
            {
                myExampleDraw.thong_tin_parameter = thong_tin_parameter;
                myExampleDraw.b = b;
                myExampleDraw.h = h;
                myExampleDraw.door = door;
                myExampleDraw.elevation = elevation;
                myExampleDraw.level = level;
                myExampleDraw.family = family;
                myExampleDraw.type = type;

                myExampleDraw.my_type_data = my_type_data;
                myExampleDraw.my_parameter_data = my_parameter_data;

                myExampleDraw.element = element;
                myExampleDraw.host_of_opening = host_of_opening;
                myExampleDraw.data_point = data_point;

                myExampleDraw.myAll_Data = myAll_Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<List<XYZ>> data_point { get; set; }
        private void Get_Data_For_Revit(object sender, RoutedEventArgs e)
        {
            try
            {
                data_point = new List<List<XYZ>>();
                Selection selection = uidoc.Selection;
                if (point_2.IsChecked == true)
                {
                    List<XYZ> support = new List<XYZ>();
                    support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints, "Select start point"));
                    support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints, "Select end point"));
                    data_point.Add(support);
                }
                else
                {
                    List<XYZ> support = new List<XYZ>();
                    support.Add(selection.PickPoint(ObjectSnapTypes.Midpoints, "Select center point"));
                    data_point.Add(support);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Get_Data_For_CAD(object sender, RoutedEventArgs e)
        {

            //try
            //{
            //    UIView uiview = uidoc.GetOpenUIViews().ToList().First(x => x.ViewId == doc.ActiveView.Id);

            //    Rectangle rect = uiview.GetWindowRectangle();

            //    System.Drawing.Point p = System.Windows.Forms.Cursor.Position;

            //    double dx = (double)(p.X - rect.Left)
            //      / (rect.Right - rect.Left);

            //    double dy = (double)(p.Y - rect.Bottom)
            //      / (rect.Top - rect.Bottom);

            //    IList<XYZ> corners = uiview.GetZoomCorners();
            //    XYZ a = corners[0];
            //    XYZ b = corners[1];
            //    XYZ v = b - a;

            //    XYZ q = a
            //      + dx * v.X * XYZ.BasisX
            //      + dy * v.Y * XYZ.BasisY;

            //    MessageBox.Show(q.ToString());
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Update_UK_MA(object sender, TextChangedEventArgs e)
        {
            Update_UK_MA();
        }

        //----------------------------------------------------------
        public void Update_UK_MA()
        {
            try
            {
                level_data item_level = (level_data)level.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;
                if (door.IsChecked == true)
                {
                    my_parameter_data.Where(x => x.parameter_name == mySource.para_name[2] || x.parameter_name == mySource.para_name[4])
                    .ToList().ForEach(x => x.parameter_value = (Convert.ToDouble(elevation.Text) + (item_type.type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).AsDouble()) * myAll_Data.list_unit_value_data[2]).ToString());
                }
                else
                {
                    my_parameter_data.Where(x => x.parameter_name == mySource.para_name[2] || x.parameter_name == mySource.para_name[3])
                    .ToList().ForEach(x => x.parameter_value = elevation.Text);
                }

                thong_tin_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        //----------------------------------------------------------
        private void Enter_UK_MA(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    elevation.Text = new System.Data.DataTable().Compute(elevation.Text.Trim(), null).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Duplicate_Type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (double.TryParse(b.Text, out double x) && double.TryParse(h.Text, out double y) && door.IsChecked == true)
                {
                    Data_For_ExtenalEvent();
                    myExampleDraw.command = "Duplicate Type";
                    Draw.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Search_Type(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (search.Text != "search" && door.IsChecked == true)
                {
                    CollectionViewSource.GetDefaultView(type.ItemsSource).Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value.Contains(search.Text));
                    //Get_Element();
                    Get_Parameter(element);
                    Update_UK_MA();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            if (string.IsNullOrEmpty(search.Text) || search.Text == "search" || door.IsChecked == false)
                return true;
            else
                return ((item as type_data).single_value.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
