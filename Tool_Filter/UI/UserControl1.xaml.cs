using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Tool_Filter
{
    /// <summary>
    /// Interaction logic for Window.xaml
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
        public ObservableCollection<element_information> my_element_information { get; set; }
        public ObservableCollection<element_information_category> my_element_information_category { get; set; }
        public ObservableCollection<element_information_family> my_element_information_family { get; set; }
        public ObservableCollection<element_information_type> my_element_information_type { get; set; }
        public ObservableCollection<element_information_parameter> my_element_information_parameter { get; set; }
        public ObservableCollection<element_information_parameter_value> my_element_information_parameter_value { get; set; }
        public ObservableCollection<parameter_information> my_parameter_information { get; set; }

        public ObservableCollection<element_information> my_element_information_quickly { get; set; }
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
        List<Element> list_element { get; set; }
        public void Function_Dau_Vao()
        {
            try
            {
                myFunctionSupport.Default_Image(myAll_Data, new List<Image>() { logo_image, show_image, hide_image, isolate_image, select_image, refresh_image, duplicate_image });

                my_element_information = new ObservableCollection<element_information>();
                my_element_information_category = new ObservableCollection<element_information_category>();
                my_element_information_family = new ObservableCollection<element_information_family>();
                my_element_information_type = new ObservableCollection<element_information_type>();
                my_element_information_parameter = new ObservableCollection<element_information_parameter>();
                my_element_information_parameter_value = new ObservableCollection<element_information_parameter_value>();

                list_element = new FilteredElementCollector(doc, doc.ActiveView.Id)
                    .WhereElementIsNotElementType()
                    .ToList();

                Distint_Category(list_element);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Visible_List_View()
        {

            try
            {
                if (thong_tin_family.Items.Count > 0) column_family.Width = GridLength.Auto;
                else column_family.Width = new GridLength(0, GridUnitType.Star);

                if (thong_tin_type.Items.Count > 0) column_type.Width = GridLength.Auto;
                else column_type.Width = new GridLength(0, GridUnitType.Star);

                if (thong_tin_parameter.Items.Count > 0) column_parameter.Width = GridLength.Auto;
                else column_parameter.Width = new GridLength(0, GridUnitType.Star);

                if (gia_tri_parameter.Items.Count > 0) column_gia_tri_parameter.Width = GridLength.Auto;
                else column_gia_tri_parameter.Width = new GridLength(0, GridUnitType.Star);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Distint_Category(List<Element> list_element)
        {
            try
            {
                ObservableCollection<element_information> my_element_information = new ObservableCollection<element_information>();
                foreach (Element element in list_element)
                {
                    try
                    {
                        my_element_information.Add(new element_information()
                        {
                            category_name = element.Category.Name,
                            element = element
                        });
                    }
                    catch (Exception)
                    {

                    }
                    
                }
                my_element_information_category = new ObservableCollection<element_information_category>(my_element_information.GroupBy(x => new
                {
                    x.category_name
                }).Select(y => new element_information_category()
                {
                    category_name = y.Key.category_name,
                    elements = y.Select(z => z.element).ToList(),

                    check = false,
                    count = y.Select(z => z.element).ToList().Count()
                }).OrderBy(z => z.category_name));

                thong_tin_category.ItemsSource = my_element_information_category;
                Visible_List_View();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_By_Category(object sender, RoutedEventArgs e)
        {
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void Distint_Family(List<Element> list_element)
        {
            try
            {
                List<string> category_list = my_element_information_category.Where(x => x.check == true).Select(y => y.category_name).ToList();
                List<string> family_list = my_element_information_family.Where(x => x.check == true).Select(y => y.family_name).ToList();
                my_element_information_family.Clear();

                ObservableCollection<element_information> my_element_information = new ObservableCollection<element_information>();
                foreach (Element element in list_element)
                {
                    try
                    {
                        if (category_list.Contains(element.Category.Name))
                        {
                            my_element_information.Add(new element_information()
                            {
                                family_name = element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString(),
                                element = element
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                new ObservableCollection<element_information_family>(my_element_information
                    .GroupBy(x => new
                    {
                        x.family_name
                    }).Select(y => new element_information_family()
                    {
                        family_name = y.Key.family_name,
                        elements = y.Select(z => z.element).ToList(),

                        check = false,
                        count = y.Select(z => z.element).ToList().Count()
                    }).OrderBy(z => z.family_name)).ToList().ForEach(i => my_element_information_family.Add(i));

                my_element_information_family.Where(x => family_list.Contains(x.family_name)).ToList().ForEach(y => y.check = true);

                thong_tin_family.ItemsSource = my_element_information_family;
                Visible_List_View();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_By_Family(object sender, RoutedEventArgs e)
        {
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void Distint_Type(List<Element> list_element)
        {
            try
            {
                List<string> category_list = my_element_information_category.Where(x => x.check == true).Select(y => y.category_name).ToList();
                List<string> family_list = my_element_information_family.Where(x => x.check == true).Select(y => y.family_name).ToList();
                List<string> type_list = my_element_information_type.Where(x => x.check == true).Select(y => y.type_name).ToList();
                my_element_information_type.Clear();

                ObservableCollection<element_information> my_element_information = new ObservableCollection<element_information>();
                foreach (Element element in list_element)
                {
                    try
                    {
                        if (category_list.Contains(element.Category.Name) && family_list.Contains(element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString()))
                        {
                            my_element_information.Add(new element_information()
                            {
                                type_name = element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString(),
                                element = element
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

                new ObservableCollection<element_information_type>(my_element_information
                    .GroupBy(x => new
                    {
                        x.type_name
                    }).Select(y => new element_information_type()
                    {
                        type_name = y.Key.type_name,
                        elements = y.Select(z => z.element).ToList(),

                        check = false,
                        count = y.Select(z => z.element).ToList().Count()
                    }).OrderBy(z => z.type_name)).ToList().ForEach(i => my_element_information_type.Add(i));

                my_element_information_type.Where(x => type_list.Contains(x.type_name)).ToList().ForEach(y => y.check = true);

                thong_tin_type.ItemsSource = my_element_information_type;
                Visible_List_View();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Type(object sender, RoutedEventArgs e)
        {
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void Distint_Parameter(List<Element> list_element)
        {
            try
            {
                List<string> category_list = my_element_information_category.Where(x => x.check == true).Select(y => y.category_name).ToList();
                List<string> family_list = my_element_information_family.Where(x => x.check == true).Select(y => y.family_name).ToList();
                List<string> type_list = my_element_information_type.Where(x => x.check == true).Select(y => y.type_name).ToList();
                List<string> parameter_list = my_element_information_parameter.Where(x => x.check == true).Select(y => y.parameter_name).ToList();
                my_element_information_parameter.Clear();

                ObservableCollection<element_information> my_element_information = new ObservableCollection<element_information>();
                foreach (Element element in list_element)
                {
                    try
                    {
                        if (category_list.Contains(element.Category.Name) && family_list.Contains(element.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString()) && type_list.Contains(element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString()))
                        {
                            List<parameter_information> parameter_Information = new List<parameter_information>();
                            foreach (Parameter para in element.Parameters)
                            {
                                parameter_Information.Add(new parameter_information()
                                {
                                    parameter = para,

                                    parameter_name = para.Definition.Name,
                                    value = myFunctionSupport.Get_Parameter_Information(para, doc)
                                });
                            }

                            ElementType type = doc.GetElement(element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;
                            List<parameter_information> parameter_Information_type = new List<parameter_information>();
                            foreach (Parameter para in type.Parameters)
                            {
                                if (para.Definition.ParameterGroup != BuiltInParameterGroup.PG_MATERIALS)
                                {
                                    parameter_Information_type.Add(new parameter_information()
                                    {
                                        parameter = para,

                                        parameter_name = para.Definition.Name,
                                        value = myFunctionSupport.Get_Parameter_Information(para, doc)
                                    });
                                }
                            }

                            ObservableCollection<material_data> materials = new ObservableCollection<material_data>();

                            var elements = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                            materials.Add(new material_data()
                            {
                                material_name = "<By Category>",
                                material = null
                            });
                            foreach (Material material in elements)
                            {
                                materials.Add(new material_data()
                                {
                                    material_name = material.Name,
                                    material = material
                                });
                            }

                            var TypeOftype = type.GetType();
                            if (myAll_Data.list_system_name_data.Contains(TypeOftype.Name))
                            {

                                CompoundStructure compound = null;
                                if (TypeOftype.Name == myAll_Data.list_system_name_data[0])
                                {
                                    var wall = type as WallType;
                                    compound = wall.GetCompoundStructure();
                                }
                                if (TypeOftype.Name == myAll_Data.list_system_name_data[1])
                                {
                                    var wall = type as FloorType;
                                    compound = wall.GetCompoundStructure();
                                }
                                if (TypeOftype.Name == myAll_Data.list_system_name_data[2])
                                {
                                    var wall = type as RoofType;
                                    compound = wall.GetCompoundStructure();
                                }
                                if (TypeOftype.Name == myAll_Data.list_system_name_data[3])
                                {
                                    var wall = type as CeilingType;
                                    compound = wall.GetCompoundStructure();
                                }

                                IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                                foreach (var layer in getlayer)
                                {
                                    string ten = "<By Category>";
                                    string ma = "";
                                    if (layer.MaterialId.IntegerValue != -1)
                                    {
                                        ten = doc.GetElement(layer.MaterialId).Name;
                                        ma = myFunctionSupport.Check_Para_And_Get_Para(doc.GetElement(layer.MaterialId) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                                    }
                                    if (parameter_Information_type.Any(x => x.value == ten || x.value == ma) == false)
                                    {
                                        parameter_Information_type.Add(new parameter_information()
                                        {
                                            parameter_name = "Structural Material",
                                            value = ten
                                        });
                                        parameter_Information_type.Add(new parameter_information()
                                        {
                                            parameter_name = "Structural Material",
                                            value = ma
                                        });
                                    }
                                }
                            }
                            else
                            {
                                foreach (Parameter para in type.Parameters)
                                {
                                    if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_MATERIALS)
                                    {
                                        string ten = "<By Category>";
                                        string ma = "";
                                        if (para.AsElementId().IntegerValue != -1)
                                        {
                                            ten = doc.GetElement(para.AsElementId()).Name;
                                            ma = myFunctionSupport.Check_Para_And_Get_Para(doc.GetElement(para.AsElementId()) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                                        }
                                        if (parameter_Information_type.Any(x => x.value == ten || x.value == ma) == false)
                                        {
                                            parameter_Information_type.Add(new parameter_information()
                                            {
                                                parameter_name = "Structural Material",
                                                value = ten
                                            });
                                            parameter_Information_type.Add(new parameter_information()
                                            {
                                                parameter_name = "Structural Material",
                                                value = ma
                                            });
                                        }
                                    }
                                }
                            }

                            my_element_information.Add(new element_information()
                            {
                                parameters_type = parameter_Information_type,
                                parameters = parameter_Information,
                                element = element
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }

                }

                ObservableCollection<parameter_information> support = new ObservableCollection<parameter_information>();
                var data = new List<element_information>(my_element_information.ToList());

                data.ForEach(y => y.parameters.ForEach(yy => support.Add(new parameter_information()
                {
                    parameter_name = yy.parameter_name,
                    value = yy.value,
                    element = y.element
                })));
                data.ForEach(y => y.parameters_type.ForEach(yy => support.Add(new parameter_information()
                {
                    parameter_name = yy.parameter_name,
                    value = yy.value,
                    element = y.element
                })));

                support.GroupBy(x => new
                {
                    x.parameter_name
                }).Select(y => new element_information_parameter()
                {
                    parameter_name = y.Key.parameter_name,
                    elements = y.Select(z => z.element).ToList(),

                    check = false,
                    count = y.Select(z => z.element).ToList().Count()
                }).OrderBy(z => z.parameter_name).ToList().ForEach(i => my_element_information_parameter.Add(i));

                my_element_information_parameter.Where(x => parameter_list.Contains(x.parameter_name)).ToList().ForEach(y => y.check = true);

                thong_tin_parameter.ItemsSource = my_element_information_parameter;
                Visible_List_View();

                my_parameter_information = new ObservableCollection<parameter_information>(support.Select(y => new parameter_information()
                {
                    parameter_name = y.parameter_name,
                    parameter_value = y.value,
                    element = y.element
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Parameter(object sender, RoutedEventArgs e)
        {
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void Distint_Parameter_Value(List<Element> list_element)
        {
            try
            {
                List<string> category_list = my_element_information_category.Where(x => x.check == true).Select(y => y.category_name).ToList();
                List<string> family_list = my_element_information_family.Where(x => x.check == true).Select(y => y.family_name).ToList();
                List<string> type_list = my_element_information_type.Where(x => x.check == true).Select(y => y.type_name).ToList();
                List<string> parameter_list = my_element_information_parameter.Where(x => x.check == true).Select(y => y.parameter_name).ToList();
                List<string> value_list = my_element_information_parameter_value.Where(x => x.check == true).Select(y => y.parameter_value).ToList();

                my_element_information_parameter_value.Clear();

                my_parameter_information.Where(i => parameter_list.Contains(i.parameter_name))
                .GroupBy(x => new
                {
                    x.parameter_value
                }).Select(y => new element_information_parameter_value()
                {
                    parameter_value = y.Key.parameter_value,
                    elements = y.Select(z => z.element).ToList(),

                    check = false,
                    count = y.Select(z => z.element).ToList().Count()
                }).OrderBy(z => z.parameter_value).ToList().ForEach(i => my_element_information_parameter_value.Add(i));

                my_element_information_parameter_value.Where(x => value_list.Contains(x.parameter_value)).ToList().ForEach(y => y.check = true);

                gia_tri_parameter.ItemsSource = my_element_information_parameter_value;
                Visible_List_View();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Custom_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                var element_list = selection.GetElementIds().Select(x => doc.GetElement(x)).ToList();
                my_element_information = new ObservableCollection<element_information>();
                my_element_information_category = new ObservableCollection<element_information_category>();
                my_element_information_family = new ObservableCollection<element_information_family>();
                my_element_information_type = new ObservableCollection<element_information_type>();
                my_element_information_parameter = new ObservableCollection<element_information_parameter>();
                my_element_information_parameter_value = new ObservableCollection<element_information_parameter_value>();
                Distint_Category(element_list);
                Distint_Family(element_list);
                Distint_Type(element_list);
                Distint_Parameter(element_list);
                Distint_Parameter_Value(element_list);
                All_Check();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refesh(object sender, RoutedEventArgs e)
        {

            try
            {
                my_element_information = new ObservableCollection<element_information>();
                my_element_information_category = new ObservableCollection<element_information_category>();
                my_element_information_family = new ObservableCollection<element_information_family>();
                my_element_information_type = new ObservableCollection<element_information_type>();
                my_element_information_parameter = new ObservableCollection<element_information_parameter>();
                my_element_information_parameter_value = new ObservableCollection<element_information_parameter_value>();

                Distint_Category(list_element);
                Distint_Family(list_element);
                Distint_Type(list_element);
                Distint_Parameter(list_element);
                Distint_Parameter_Value(list_element);
                All_Check();

                Data_for_ExtenalEvent();
                myExampleDraw.command = "Reset View";
                Draw.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public List<ElementId> ids { get; set; }
        private void Hight_Lights_Filter(object sender, RoutedEventArgs e)
        {
            try
            {
                ids = new List<ElementId>();
                if (!string.IsNullOrEmpty(parameter_name.Text) && !string.IsNullOrEmpty(parameter_value.Text))
                {
                    Parameter_Quickly_Filter();
                    foreach (element_information information in my_element_information_quickly)
                    {
                        try
                        {
                            information.parameters.Where(x => x.parameter_name == parameter_name.Text && x.value == parameter_value.Text).ToList().ForEach(y => ids.Add(y.element.Id));
                            information.parameters_type.Where(x => x.parameter_name == parameter_name.Text && x.value == parameter_value.Text).ToList().ForEach(y => ids.Add(y.element.Id));
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    if (my_element_information_parameter_value.Where(x => x.check == true).Select(y => y.elements).ToList().Count() > 0)
                    {
                        my_element_information_parameter_value.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                    }
                    else if (my_element_information_parameter.Where(x => x.check == true).Select(y => y.elements).ToList().Count() > 0)
                    {
                        my_element_information_parameter.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                    }
                    else if (my_element_information_type.Where(x => x.check == true).Select(y => y.elements).ToList().Count() > 0)
                    {
                        my_element_information_type.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                    }
                    else if (my_element_information_family.Where(x => x.check == true).Select(y => y.elements).ToList().Count() > 0)
                    {
                        my_element_information_family.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                    }
                    else if (my_element_information_category.Where(x => x.check == true).Select(y => y.elements).ToList().Count() > 0)
                    {
                        my_element_information_category.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                    }
                }

                Data_for_ExtenalEvent();
                myExampleDraw.command = "Hight Lights";
                Draw.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Parameter_Quickly_Filter()
        {
            try
            {
                foreach (Element element in list_element)
                {
                    try
                    {
                        List<parameter_information> parameter_Information = new List<parameter_information>();
                        foreach (Parameter para in element.Parameters)
                        {
                            parameter_Information.Add(new parameter_information()
                            {
                                parameter = para,

                                parameter_name = para.Definition.Name,
                                value = myFunctionSupport.Get_Parameter_Information(para, doc)
                            });
                        }

                        ElementType type = doc.GetElement(element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;
                        List<parameter_information> parameter_Information_type = new List<parameter_information>();
                        foreach (Parameter para in type.Parameters)
                        {
                            if (para.Definition.ParameterGroup != BuiltInParameterGroup.PG_MATERIALS)
                            {
                                parameter_Information_type.Add(new parameter_information()
                                {
                                    parameter = para,

                                    parameter_name = para.Definition.Name,
                                    value = myFunctionSupport.Get_Parameter_Information(para, doc)
                                });
                            }
                        }

                        ObservableCollection<material_data> materials = new ObservableCollection<material_data>();

                        var elements = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                        materials.Add(new material_data()
                        {
                            material_name = "<By Category>",
                            material = null
                        });
                        foreach (Material material in elements)
                        {
                            materials.Add(new material_data()
                            {
                                material_name = material.Name,
                                material = material
                            });
                        }

                        var TypeOftype = type.GetType();
                        if (myAll_Data.list_system_name_data.Contains(TypeOftype.Name))
                        {

                            CompoundStructure compound = null;
                            if (TypeOftype.Name == myAll_Data.list_system_name_data[0])
                            {
                                var wall = type as WallType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == myAll_Data.list_system_name_data[1])
                            {
                                var wall = type as FloorType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == myAll_Data.list_system_name_data[2])
                            {
                                var wall = type as RoofType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == myAll_Data.list_system_name_data[3])
                            {
                                var wall = type as CeilingType;
                                compound = wall.GetCompoundStructure();
                            }

                            IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                            foreach (var layer in getlayer)
                            {
                                string ten = "<By Category>";
                                string ma = "";
                                if (layer.MaterialId.IntegerValue != -1)
                                {
                                    ten = doc.GetElement(layer.MaterialId).Name;
                                    ma = myFunctionSupport.Check_Para_And_Get_Para(doc.GetElement(layer.MaterialId) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                                }
                                if (parameter_Information_type.Any(x => x.value == ten || x.value == ma) == false)
                                {
                                    parameter_Information_type.Add(new parameter_information()
                                    {
                                        parameter_name = "Structural Material",
                                        value = ten
                                    });
                                    parameter_Information_type.Add(new parameter_information()
                                    {
                                        parameter_name = "Structural Material",
                                        value = ma
                                    });
                                }
                            }
                        }
                        else
                        {
                            foreach (Parameter para in type.Parameters)
                            {
                                if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_MATERIALS)
                                {
                                    string ten = "<By Category>";
                                    string ma = "";
                                    if (para.AsElementId().IntegerValue != -1)
                                    {
                                        ten = doc.GetElement(para.AsElementId()).Name;
                                        ma = myFunctionSupport.Check_Para_And_Get_Para(doc.GetElement(para.AsElementId()) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                                    }
                                    if (parameter_Information_type.Any(x => x.value == ten || x.value == ma) == false)
                                    {
                                        parameter_Information_type.Add(new parameter_information()
                                        {
                                            parameter_name = "Structural Material",
                                            value = ten
                                        });
                                        parameter_Information_type.Add(new parameter_information()
                                        {
                                            parameter_name = "Structural Material",
                                            value = ma
                                        });
                                    }
                                }
                            }
                        }

                        my_element_information_quickly.Add(new element_information()
                        {
                            parameters_type = parameter_Information_type,
                            parameters = parameter_Information,
                            element = element
                        });
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void Data_for_ExtenalEvent()
        {
            try
            {
                myExampleDraw.control = control;
                myExampleDraw.ids = ids;
                myExampleDraw.view_name = view_name;
                myExampleDraw.thong_tin_category = thong_tin_category;
                myExampleDraw.thong_tin_family = thong_tin_family;
                myExampleDraw.thong_tin_type = thong_tin_type;
                myExampleDraw.thong_tin_parameter = thong_tin_parameter;
                myExampleDraw.gia_tri_parameter = gia_tri_parameter;

                myExampleDraw.my_element_information = my_element_information;
                myExampleDraw.my_element_information_category = my_element_information_category;
                myExampleDraw.my_element_information_family = my_element_information_family;
                myExampleDraw.my_element_information_type = my_element_information_type;
                myExampleDraw.my_element_information_parameter = my_element_information_parameter;
                myExampleDraw.my_element_information_parameter_value = my_element_information_parameter_value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string control { get; set; }
        private void Hide(object sender, RoutedEventArgs e)
        {
            try
            {
                Data_for_ExtenalEvent();
                myExampleDraw.command = "Hide";
                Draw.Raise();
                control = "Hide";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Hide_Isolate(object sender, RoutedEventArgs e)
        {
            try
            {
                Data_for_ExtenalEvent();
                myExampleDraw.command = "HideIsolate";
                Draw.Raise();
                control = "HideIsolate";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Duplicate_View(object sender, RoutedEventArgs e)
        {
            try
            {
                Data_for_ExtenalEvent();
                myExampleDraw.command = "Duplicate View";
                Draw.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_Value(object sender, RoutedEventArgs e)
        {
            All_Check();
        }

        //----------------------------------------------------------
        public void All_Check()
        {
            try
            {
                if (my_element_information_category.Count() > 0 && my_element_information_category.Any(x => x.check != true) == false) check_all_category.IsChecked = true; else check_all_category.IsChecked = false;
                if (my_element_information_family.Count() > 0 && my_element_information_family.Any(x => x.check != true) == false) check_all_family.IsChecked = true; else check_all_family.IsChecked = false;
                if (my_element_information_type.Count() > 0 && my_element_information_type.Any(x => x.check != true) == false) check_all_type.IsChecked = true; else check_all_type.IsChecked = false;
                if (my_element_information_parameter.Count() > 0 && my_element_information_parameter.Any(x => x.check != true) == false) check_all_parameter.IsChecked = true; else check_all_parameter.IsChecked = false;
                if (my_element_information_parameter_value.Count() > 0 && my_element_information_parameter_value.Any(x => x.check != true) == false) check_all_value.IsChecked = true; else check_all_value.IsChecked = false;

                thong_tin_category.Items.Refresh();
                thong_tin_family.Items.Refresh();
                thong_tin_type.Items.Refresh();
                thong_tin_parameter.Items.Refresh();
                gia_tri_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_Value(object sender, RoutedEventArgs e)
        {
            CheckAll_Not_value();
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void CheckAll_Not_value()
        {
            try
            {
                if (check_all_value.IsChecked == true) my_element_information_parameter_value.ToList().ForEach(y => y.check = true); else my_element_information_parameter_value.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_parameter(object sender, RoutedEventArgs e)
        {
            CheckAll_Not_parameter();
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void CheckAll_Not_parameter()
        {
            try
            {
                if (check_all_parameter.IsChecked == true) my_element_information_parameter.ToList().ForEach(y => y.check = true); else my_element_information_parameter.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_type(object sender, RoutedEventArgs e)
        {
            CheckAll_Not_type();
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void CheckAll_Not_type()
        {
            try
            {
                if (check_all_type.IsChecked == true) my_element_information_type.ToList().ForEach(y => y.check = true); else my_element_information_type.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_family(object sender, RoutedEventArgs e)
        {
            CheckAll_Not_family();
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void CheckAll_Not_family()
        {
            try
            {
                if (check_all_family.IsChecked == true) my_element_information_family.ToList().ForEach(y => y.check = true); else my_element_information_family.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_category(object sender, RoutedEventArgs e)
        {
            CheckAll_Not_category();
            Distint_Family(list_element);
            Distint_Type(list_element);
            Distint_Parameter(list_element);
            Distint_Parameter_Value(list_element);
            All_Check();
        }

        //----------------------------------------------------------
        public void CheckAll_Not_category()
        {
            try
            {
                if (check_all_category.IsChecked == true) my_element_information_category.ToList().ForEach(y => y.check = true); else my_element_information_category.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}


      
  


