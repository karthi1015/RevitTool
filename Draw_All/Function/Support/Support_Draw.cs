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
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;

namespace Draw_All
{
    class Support_Draw
    {
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

        public TextBox search { get; set; }

        Support_All mySupport_All;
        ListSource mySource;
        FunctionSQL mySQL;
        //----------------------------------------------------------
        public void Get_Location_Line(ComboBox location_line)
        {
            mySource = new ListSource();
            try
            {
                location_line_data = mySource.location_line_data;

                location_line.ItemsSource = location_line_data;
                location_line.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Level_Bottom(Document doc, ComboBox level_bottom, TextBox elevation_bottom, All_Data myAll_Data)
        {
            try
            {
                my_level_data_bottom = new ObservableCollection<level_data>(new FilteredElementCollector(doc)
                    .OfClass(typeof(Level)).Cast<Level>()
                    .OrderBy(x => x.Elevation)
                    .ToList().Select(x => new level_data()
                    {
                        level = x,
                        single_value = x.Name,
                        elevation = x.Elevation
                    }));

                level_bottom.ItemsSource = my_level_data_bottom;
                if (new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().Where(x => x.Name == doc.ActiveView.Name).ToList().Count() == 0)
                {
                    level_bottom.SelectedIndex = my_level_data_bottom.IndexOf(my_level_data_bottom.First(x => x.single_value == doc.ActiveView.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL).AsString()));
                }
                else
                {
                    level_bottom.SelectedIndex = 0;
                }
                level_data item = (level_data)level_bottom.SelectedItem;
                elevation_bottom.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Level_Top(Document doc, ComboBox level_top, TextBox elevation_top, All_Data myAll_Data)
        {
            try
            {
                my_level_data_top = new ObservableCollection<level_data>(new FilteredElementCollector(doc)
                    .OfClass(typeof(Level)).Cast<Level>()
                    .OrderBy(x => x.Elevation)
                    .ToList().Select(x => new level_data()
                    {
                        level = x,
                        single_value = x.Name,
                        elevation = x.Elevation
                    }));

                level_top.ItemsSource = my_level_data_top;
                if (new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>().Where(x => x.Name == doc.ActiveView.Name).ToList().Count() == 0)
                {
                    level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == doc.ActiveView.get_Parameter(BuiltInParameter.PLAN_VIEW_LEVEL).AsString())) + 1;
                }
                else
                {
                    level_top.SelectedIndex = 0;
                }
                level_data item = (level_data)level_top.SelectedItem;
                elevation_top.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Add_Category(Document doc,ComboBox category, All_Data myAll_Data)
        {
            try
            {
                my_category_data = new ObservableCollection<category_data>();

                var builtin_category = Enum.GetValues(typeof(BuiltInCategory)).Cast<BuiltInCategory>().ToList();
                var categories = doc.Settings.Categories;
                foreach (string name in myAll_Data.list_category_draw)
                {
                    BuiltInCategory builtInCategory = BuiltInCategory.INVALID;
                    foreach (BuiltInCategory buildCategory in builtin_category)
                    {
                        try
                        {
                            if (categories.get_Item(buildCategory) != null && categories.get_Item(buildCategory).Name == name)
                            {
                                builtInCategory = buildCategory;
                                break;
                            }
                        }
                        catch (Exception) { }
                    }
                    my_category_data.Add(new category_data()
                    {
                        single_value = name,
                        builtin = builtInCategory
                    });
                }

                category.ItemsSource = my_category_data;
                category.SelectedIndex = 0;

                CollectionView view_category = (CollectionView)CollectionViewSource.GetDefaultView(category.ItemsSource);
                view_category.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Family(Document doc, ComboBox category, ComboBox family, All_Data myAll_Data)
        {
            mySupport_All = new Support_All();
            try
            {
                category_data item = (category_data)category.SelectedItem;

                ElementMulticategoryFilter filter_category = new ElementMulticategoryFilter(new List<BuiltInCategory>() { item.builtin });
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
                        path_image = mySupport_All.CreatePreview(x.First(), myAll_Data),
                        types = x.Select(y => y).ToList()
                    }).OrderBy(x => x.single_value));

                family.ItemsSource = my_family_data;
                family.SelectedIndex = 0;

                CollectionView view_family = (CollectionView)CollectionViewSource.GetDefaultView(family.ItemsSource);
                view_family.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Get_Type(Document doc, ComboBox family, ComboBox type, ComboBox material, TextBox search, All_Data myAll_Data)
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
                CollectionView view_type = (CollectionView)CollectionViewSource.GetDefaultView(type.ItemsSource);
                view_type.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
                view_type.Filter = Filter_ten_vat_lieu;

                if (search.Text != "search")
                {
                    CollectionViewSource.GetDefaultView(type.ItemsSource).Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value.Contains(search.Text));
                }

                type_data item_type = (type_data)type.SelectedItem;
                if (item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM) != null)
                {
                    material.SelectedItem = my_material_data.First(x => x.single_value == doc.GetElement(item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsElementId()).Name);
                    material.ToolTip = doc.GetElement(item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsElementId()).Name;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            if (string.IsNullOrEmpty(search.Text) || search.Text == "search")
                return true;
            else
                return ((item as type_data).single_value.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        public void Get_Parameter(Element element_update, Document doc,
            ComboBox category, ComboBox family, ComboBox type,ListView thong_tin_parameter,
            All_Data myAll_Data)
        {
            mySupport_All = new Support_All();
            try
            {
                category_data item_cate = (category_data)category.SelectedItem;
                family_data item = (family_data)family.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;

                ElementMulticategoryFilter filter_category = new ElementMulticategoryFilter(new List<BuiltInCategory>() { item_cate.builtin });
                if (item != null && item.single_value != "" && new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == item_type.type.Name).ToList().Count() > 0)
                {
                    var list_element = new FilteredElementCollector(doc)
                        .WherePasses(filter_category)
                        .WhereElementIsNotElementType()
                        .Where(x => (x as FamilyInstance).Symbol.FamilyName == item.single_value)
                        .Where(x => x.Name == item_type.single_value)
                        .ToList();

                    if (element_update != null)
                    {
                        my_parameter_data = new ObservableCollection<parameter_data>(mySupport_All.View_Dimensions_FamilySymbol(doc, element_update, myAll_Data));
                    }
                    else if (list_element.Count() > 0)
                    {
                        my_parameter_data = new ObservableCollection<parameter_data>(mySupport_All.View_Dimensions_FamilySymbol(doc, list_element[0], myAll_Data));
                    }
                    else
                    {
                        Family fa = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.Name == item.single_value).First();
                        my_parameter_data = new ObservableCollection<parameter_data>();
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
                                        my_parameter_data.Add(new parameter_data()
                                        {
                                            parameter_name = para.Definition.Name,
                                            parameter_value = mySupport_All.Get_Parameter_Information_Family(para, familyDoc, manager, myAll_Data),
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
                    }

                    thong_tin_parameter.ItemsSource = my_parameter_data;
                }
                else
                {
                    thong_tin_parameter.ItemsSource = new ObservableCollection<parameter_data>();
                }

                CollectionView view_para = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource);
                view_para.SortDescriptions.Add(new SortDescription("parameter_name", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                ////MessageBox.Show(ex.Message);
            }
        }

        public void Select_Host(UIDocument uidoc, Document doc,
            Button host, ComboBox level_bottom, ComboBox level_top, TextBox elevation_bottom, TextBox elevation_top,
            All_Data myAll_Data,
            ComboBox type, ListView thong_tin_parameter)
        {
            try
            {
                Selection selection = uidoc.Selection;
                SelectElementsFilter conduitFilter = new SelectElementsFilter(myAll_Data.list_category_draw_host.ToList());
                var element = doc.GetElement(selection.PickObject(ObjectType.Element, conduitFilter, "Please select floor or wall element"));
                if (element != null)
                {
                    host.ToolTip = element.Name;
                    host_of_opening = element;

                    double offset_bottom = 0;
                    double offset_top = 0;
                    if (element.Category.Name == myAll_Data.list_category_draw[3])
                    {
                        offset_bottom = element.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsValueString());

                        offset_top = element.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == (doc.GetElement(element.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId()).Name));
                    }
                    else if (element.Category.Name == myAll_Data.list_category_draw[4])
                    {
                        offset_bottom = element.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsValueString());

                        offset_top = element.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsValueString());
                    }
                    else if (element.Category.Name == myAll_Data.list_category_draw[5])
                    {
                        offset_bottom = element.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble() - (doc.GetElement(element.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsValueString());

                        offset_top = element.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble() - (doc.GetElement(element.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsValueString());
                    }
                    else if (element.Category.Name == myAll_Data.list_category_draw_host[0])
                    {
                        offset_bottom = element.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble() - (doc.GetElement(element.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsValueString());

                        offset_top = element.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsValueString());
                    }

                    level_data item_bottom = (level_data)level_bottom.SelectedItem;
                    level_data item_top = (level_data)level_top.SelectedItem;
                    elevation_bottom.Text = Math.Round((item_bottom.elevation + offset_bottom) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                    elevation_top.Text = Math.Round((item_top.elevation + offset_top) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();

                    Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Select_Level_Bottom(ComboBox level_bottom, TextBox elevation_bottom, All_Data myAll_Data,
            ComboBox type, ListView thong_tin_parameter)
        {
            try
            {
                level_data item = (level_data)level_bottom.SelectedItem;
                elevation_bottom.Text = Math.Round(item.elevation * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();

                Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Update_UK_MA(ComboBox type, All_Data myAll_Data, TextBox elevation_bottom, ListView thong_tin_parameter)
        {
            try
            {
                type_data item_type = (type_data)type.SelectedItem;

                double offset = 0;
                if (item_type.type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM) != null)
                {
                    offset = item_type.type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).AsDouble() * myAll_Data.list_unit_value_data[2];
                }

                my_parameter_data.Where(x => myAll_Data.list_parameter_tag.Contains(x.parameter_name))
                .ToList().ForEach(x => x.parameter_value = (Convert.ToDouble(elevation_bottom.Text) + offset).ToString());

                thong_tin_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        //----------------------------------------------------------
        public void Search(Document doc,
            TextBox search, TextBox elevation_bottom, ComboBox category, ComboBox family, ComboBox type, ListView thong_tin_parameter,
            All_Data myAll_Data)
        {
            try
            {
                if (search.Text != "search")
                {
                    CollectionViewSource.GetDefaultView(type.ItemsSource).Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value.Contains(search.Text));
                    //Get_Element();
                    Get_Parameter(null, doc, category, family, type, thong_tin_parameter, myAll_Data);
                    Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Select_Update(UIDocument uidoc, Document doc,
            Button host, ComboBox level_bottom, ComboBox level_top, TextBox elevation_bottom, TextBox elevation_top, ComboBox material, TextBox search, ListView thong_tin_parameter,
            ComboBox category, ComboBox family, ComboBox type, 
            All_Data myAll_Data)
        {
            try
            {
                Selection selection = uidoc.Selection;
                SelectElementsFilter conduitFilter = new SelectElementsFilter(myAll_Data.list_category_draw.ToList());
                element_update = doc.GetElement(selection.PickObject(ObjectType.Element, conduitFilter, "Please select element update"));
                if (element_update != null)
                {
                    selection.SetElementIds(new List<ElementId>() { element_update.Id });
                    host_of_opening = null;
                    if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == element_update.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString()).ToList().Count() > 0)
                    {
                        FamilyInstance familyInstance = element_update as FamilyInstance;
                        if (familyInstance.Host != null)
                        {
                            host.ToolTip = familyInstance.Host.Name;
                            host_of_opening = familyInstance.Host;
                        }
                    }

                    double offset_bottom = 0;
                    double offset_top = 0;
                    if (element_update.Category.Name == myAll_Data.list_category_draw[3])
                    {
                        offset_bottom = element_update.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsValueString());

                        offset_top = element_update.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == (doc.GetElement(element_update.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).AsElementId()).Name));
                    }
                    else if (element_update.Category.Name == myAll_Data.list_category_draw[4])
                    {
                        offset_bottom = element_update.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).AsDouble();
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).AsValueString());

                        offset_top = element_update.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).AsValueString());
                    }
                    else if (element_update.Category.Name == myAll_Data.list_category_draw[5])
                    {
                        offset_bottom = element_update.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble() - (doc.GetElement(element_update.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsValueString());

                        offset_top = element_update.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble() - (doc.GetElement(element_update.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsValueString());
                    }
                    else if (element_update.Category.Name == myAll_Data.list_category_draw_host[0])
                    {
                        offset_bottom = element_update.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble() - (doc.GetElement(element_update.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId()) as Level).Elevation;
                        level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsValueString());

                        offset_top = element_update.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).AsDouble();
                        level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsValueString());
                    }
                    else
                    {
                        if (host_of_opening != null)
                        {
                            if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[0])
                            {
                                offset_bottom = 0;
                                level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString());

                                offset_top = 0;
                                level_top.SelectedItem = my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString());
                            }
                            else if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[1])
                            {
                                offset_bottom = element_update.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                                level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString()); // read-only

                                offset_top = 0;
                                level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString())) + 1;
                            }
                            else
                            {
                                if (element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                                {
                                    offset_bottom = element_update.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                                    level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString()); // read-only

                                    offset_top = 0;
                                    level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString())) + 1;
                                }
                                else if (element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                    && element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                                {
                                    offset_bottom = element_update.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                                    level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString()); // read-only

                                    offset_top = 0;
                                    level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString())) + 1;
                                }
                            }
                        }
                        else
                        {
                            if (element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                            {
                                offset_bottom = element_update.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                                level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString()); // read-only

                                offset_top = 0;
                                level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsValueString())) + 1;
                            }
                            else if (element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                && element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                            {
                                offset_bottom = element_update.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                                level_bottom.SelectedItem = my_level_data_bottom.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString()); // read-only

                                offset_top = 0;
                                level_top.SelectedIndex = my_level_data_top.IndexOf(my_level_data_top.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsValueString())) + 1;
                            }
                        }
                    }

                    level_data item_bottom = (level_data)level_bottom.SelectedItem;
                    level_data item_top = (level_data)level_top.SelectedItem;
                    elevation_bottom.Text = Math.Round((item_bottom.elevation + offset_bottom) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                    elevation_top.Text = Math.Round((item_top.elevation + offset_top) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();

                    //------------------------------------------------------------------------------------------------------------------------------------------------
                    category.SelectedItem = my_category_data.First(x => x.single_value == element_update.Category.Name);

                    Get_Family(doc, category, family, myAll_Data);
                    family.SelectedItem = my_family_data.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString());

                    Get_Type(doc, family, type, material, search, myAll_Data);
                    type.SelectedItem = my_type_data.First(x => x.single_value == element_update.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString());
                    type_data item_type = (type_data)type.SelectedItem;
                    if (item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM) != null)
                    {
                        material.SelectedItem = my_material_data.First(x => x.single_value == doc.GetElement(item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsElementId()).Name);
                        material.ToolTip = doc.GetElement(item_type.type.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM).AsElementId()).Name;
                    }

                    Get_Parameter(element_update, doc, category, family, type, thong_tin_parameter, myAll_Data);

                    Update_UK_MA(type, myAll_Data, elevation_bottom, thong_tin_parameter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Data_Pick_Element(UIDocument uidoc, Document doc,
            RadioButton center_mid_point, RadioButton line, RadioButton arc, RadioButton ellipse, RadioButton pick)
        {
            mySource = new ListSource();

            Selection selection = uidoc.Selection;
            if (center_mid_point.IsChecked == true && center_mid_point.IsEnabled == true)
            {
                List<XYZ> support = new List<XYZ>();
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select center point"));
                data_point = new geometry_data()
                {
                    name = mySource.list_option_selection_name[0],
                    list_point = support
                };
            }
            if (line.IsChecked == true)
            {
                List<XYZ> support = new List<XYZ>();
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select start point"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select end point"));
                data_point = new geometry_data()
                {
                    name = mySource.list_option_selection_name[1],
                    list_point = support
                };
            }
            if (arc.IsChecked == true && arc.IsEnabled == true)
            {
                List<XYZ> support = new List<XYZ>();
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select center point"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select start point"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select end point, not same start point"));
                data_point = new geometry_data()
                {
                    name = mySource.list_option_selection_name[2],
                    list_point = support
                };
            }
            if (ellipse.IsChecked == true && ellipse.IsEnabled == true)
            {
                List<XYZ> support = new List<XYZ>();
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select start point define radius x"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select end point define radius x"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select start point define radius y"));
                support.Add(selection.PickPoint(ObjectSnapTypes.Endpoints | ObjectSnapTypes.Midpoints | ObjectSnapTypes.Centers | ObjectSnapTypes.Intersections | ObjectSnapTypes.Perpendicular | ObjectSnapTypes.Nearest | ObjectSnapTypes.Quadrants, "Select end point define radius y"));
                data_point = new geometry_data()
                {
                    name = mySource.list_option_selection_name[3],
                    list_point = support
                };
            }
            if (pick.IsChecked == true)
            {
                var refe = selection.PickObject(ObjectType.PointOnElement, "Select reference of element");
                Element element = doc.GetElement(refe);
                GeometryObject geometry = element.GetGeometryObjectFromReference(refe);

                List<string> my_cad_link = new FilteredElementCollector(doc).OfClass(typeof(CADLinkType)).Cast<CADLinkType>().Select(x => x.Name).ToList();
                if (my_cad_link.Count() > 0 && my_cad_link.Contains(doc.GetElement(element.UniqueId).Category.Name))
                {
                    GeometryElement elem = element.get_Geometry(new Options());
                    if (elem != null)
                    {
                        foreach (GeometryObject geometryObject in elem)
                        {
                            GeometryInstance gInstance = geometryObject as GeometryInstance;
                            if (null != gInstance)
                            {
                                origin = gInstance.Transform.Origin;
                            }
                        }
                    }
                }

                if (geometry is Line)
                {
                    Line polyLine = geometry as Line;

                    data_point = new geometry_data()
                    {
                        name = mySource.list_option_selection_name[4],
                        line = polyLine
                    };
                }
                else if (geometry is PolyLine)
                {
                    PolyLine polyLine = geometry as PolyLine;
                    data_point = new geometry_data()
                    {
                        name = mySource.list_option_selection_name[5],
                        polyLine = polyLine
                    };
                }
                else if (geometry is Arc)
                {
                    Arc polyLine = geometry as Arc;

                    data_point = new geometry_data()
                    {
                        name = mySource.list_option_selection_name[6],
                        arc = polyLine
                    };

                }
                else if (geometry is Ellipse)
                {
                    Ellipse polyLine = geometry as Ellipse;

                    data_point = new geometry_data()
                    {
                        name = mySource.list_option_selection_name[6],
                        ellipse = polyLine
                    };
                }
            }
        }

        //----------------------------------------------------------
        public void Get_Material(Document doc, ComboBox material)
        {
            try
            {
                my_material_data = new ObservableCollection<material_data>();
                new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().ToList().ForEach(x => my_material_data.Add(new material_data()
                {
                    single_value = x.Name,
                    material = x
                }));

                material.ItemsSource = my_material_data;
                material.SelectedIndex = 0;
                material.ToolTip = my_material_data[0].single_value;

                CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(material.ItemsSource);
                view_total.SortDescriptions.Add(new SortDescription("single_value", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
