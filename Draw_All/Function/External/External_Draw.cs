#region Namespaces
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using RadioButton = System.Windows.Controls.RadioButton;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;
using System.Globalization;
#endregion

namespace Draw_All
{
    public class External_Draw : IExternalEventHandler
    {
        public string command { get; set; }

        public ListView thong_tin_parameter { get; set; }
        public TextBox b { get; set; }
        public TextBox h { get; set; }
        public ComboBox material { get; set; }
        public ComboBox category { get; set; }
        public TextBox elevation_bottom { get; set; }
        public ComboBox level_bottom { get; set; }
        public TextBox elevation_top { get; set; }
        public ComboBox level_top { get; set; }
        public ComboBox family { get; set; }
        public ComboBox type { get; set; }
        public ComboBox location_line { get; set; }

        public ObservableCollection<type_data> my_type_data { get; set; }
        public ObservableCollection<parameter_data> my_parameter_data { get; set; }

        public ElementType element { get; set; }
        public Element host_of_opening { get; set; }
        public Element element_update { get; set; }
        public geometry_data data_point { get; set; }
        public XYZ origin { get; set; }

        public All_Data myAll_Data { get; set; }

        ListSource mySource;
        Support_All myFunctionSupport;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new Support_All();

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Draw");

                if (command == "Update Data Tag")
                {
                    string result = Update_Data_Tag(uiapp, doc);
                }

                if (command == "Update By Revit")
                {
                    Update_By_Revit(uiapp, doc);
                }

                if (command == "Draw By Revit")
                {
                    if (data_point != null)
                    {
                        string result = Draw_By_Revit(uiapp, doc);
                        if (result == "S")
                        {
                            data_point = null;
                        }
                    }
                }

                if (command == "Draw By CAD")
                {
                    Draw_By_CAD(uiapp, doc);
                }

                if (command == "Duplicate Type")
                {
                    Duplicate_Type(uiapp, doc);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "External Event Example";
        }

        //----------------------------------------------------------
        public string Update_Data_Tag(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                myFunctionSupport.Change_Data_Tag(uiapp, doc, myAll_Data);
                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Duplicate_Type(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                type_data item_type = (type_data)type.SelectedItem;
                material_data item_material = (material_data)material.SelectedItem;

                CultureInfo culture = CultureInfo.CreateSpecificCulture("eu-ES");
                string specifier = "G";

                if (item_type.type.Category.Name == myAll_Data.list_category_draw[3])
                {
                    WallType wall_Type = item_type.type.Duplicate("STB " + b.Text + " " + item_material.single_value) as WallType;
                    CompoundStructure compound_new = wall_Type.GetCompoundStructure();
                    IList<CompoundStructureLayer> cslayers = compound_new.GetLayers();

                    for (int i = 0; i < cslayers.Count; i++)
                    {
                        compound_new.SetLayerWidth(cslayers[i].LayerId, Convert.ToDouble(b.Text) / myAll_Data.list_unit_value_data[2]);
                        compound_new.SetMaterialId(cslayers[i].LayerId, item_material.material.Id);
                    }
                    wall_Type.SetCompoundStructure(compound_new);

                    my_type_data.Add(new type_data()
                    {
                        type = wall_Type,
                        single_value = wall_Type.Name
                    });
                    type.Items.Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value == wall_Type.Name);

                    double width = Math.Round(wall_Type.Width * myAll_Data.list_unit_value_data[6], 3);
                    wall_Type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS).Set("h = " + width.ToString(specifier, culture) + " cm");
                }
                else if(item_type.type.Category.Name == myAll_Data.list_category_draw[4])
                {
                    ElementType new_type = item_type.type.Duplicate(b.Text + "/" + h.Text + " " + item_material.single_value);
                    new_type.LookupParameter(myAll_Data.list_parameter_column[0]).Set(Convert.ToDouble(b.Text) / myAll_Data.list_unit_value_data[2]);
                    new_type.LookupParameter(myAll_Data.list_parameter_column[1]).Set(Convert.ToDouble(h.Text) / myAll_Data.list_unit_value_data[2]);

                    my_type_data.Add(new type_data()
                    {
                        type = new_type,
                        single_value = new_type.Name
                    });
                    type.Items.Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value == new_type.Name);

                    double width = Math.Round(new_type.LookupParameter(myAll_Data.list_parameter_column[0]).AsDouble() * myAll_Data.list_unit_value_data[6], 3);
                    double height = Math.Round(new_type.LookupParameter(myAll_Data.list_parameter_column[1]).AsDouble() * myAll_Data.list_unit_value_data[6], 3);
                    new_type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS).Set("b/h = " + width.ToString(specifier, culture) + "/" + height.ToString(specifier, culture) + " cm");
                }
                else if(item_type.type.Category.Name == myAll_Data.list_category_draw[5])
                {
                    ElementType new_type = item_type.type.Duplicate(b.Text + "/" + h.Text + " " + item_material.single_value);
                    new_type.LookupParameter(myAll_Data.list_parameter_framing[0]).Set(Convert.ToDouble(b.Text) / myAll_Data.list_unit_value_data[2]);
                    new_type.LookupParameter(myAll_Data.list_parameter_framing[1]).Set(Convert.ToDouble(h.Text) / myAll_Data.list_unit_value_data[2]);

                    my_type_data.Add(new type_data()
                    {
                        type = new_type,
                        single_value = new_type.Name
                    });
                    type.Items.Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value == new_type.Name);

                    double width = Math.Round(new_type.LookupParameter(myAll_Data.list_parameter_framing[0]).AsDouble() * myAll_Data.list_unit_value_data[6], 3);
                    double height = Math.Round(new_type.LookupParameter(myAll_Data.list_parameter_framing[1]).AsDouble() * myAll_Data.list_unit_value_data[6], 3);
                    new_type.get_Parameter(BuiltInParameter.ALL_MODEL_TYPE_COMMENTS).Set("b/h = " + width.ToString(specifier, culture) + "/" + height.ToString(specifier, culture) + " cm");
                }
                else if (item_type.type.Category.Name == myAll_Data.list_category_draw[1] || item_type.type.Category.Name == myAll_Data.list_category_draw[2])
                {
                    ElementType new_type = item_type.type.Duplicate(b.Text + "/" + h.Text);
                    new_type.get_Parameter(BuiltInParameter.FAMILY_WIDTH_PARAM).Set(Convert.ToDouble(b.Text) / myAll_Data.list_unit_value_data[2]);
                    new_type.get_Parameter(BuiltInParameter.FAMILY_HEIGHT_PARAM).Set(Convert.ToDouble(h.Text) / myAll_Data.list_unit_value_data[2]);

                    my_type_data.Add(new type_data()
                    {
                        type = new_type,
                        single_value = new_type.Name
                    });
                    type.Items.Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value == new_type.Name);

                    Update_UK_MA();
                }
                else
                {
                    ElementType new_type = item_type.type.Duplicate(item_type.type.Name + " " + b.Text);

                    my_type_data.Add(new type_data()
                    {
                        type = new_type,
                        single_value = new_type.Name
                    });
                    type.Items.Refresh();
                    type.SelectedItem = my_type_data.First(x => x.single_value == new_type.Name);

                    Update_UK_MA();
                }

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

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

                my_parameter_data.Where(x => myAll_Data.list_parameter_tag.Contains(x.parameter_name))
                .ToList().ForEach(x => x.parameter_value = (Convert.ToDouble(elevation_bottom.Text) + offset).ToString());

                thong_tin_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {

            }
        }

        //----------------------------------------------------------
        public string Update_By_Revit(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                level_data item_level_bottom = (level_data)level_bottom.SelectedItem;
                level_data item_level_top = (level_data)level_top.SelectedItem;
                family_data item_family = (family_data)family.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;

                //Draw_Data(item_type, item_family, doc);

                Level level_element_bottom = item_level_bottom.level;
                Level level_element_top = item_level_top.level;

                double elevation_bottom_value = Convert.ToDouble(elevation_bottom.Text) / myAll_Data.list_unit_value_data[2] - item_level_bottom.elevation;
                double elevation_top_value = Convert.ToDouble(elevation_top.Text) / myAll_Data.list_unit_value_data[2] - item_level_top.elevation;

                if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == item_type.type.Name).ToList().Count() > 0)
                {
                    FamilyInstance instance = element_update as FamilyInstance;
                    FamilySymbol familySymbol = item_type.type as FamilySymbol;
                    if (familySymbol.IsActive == false) familySymbol.Activate();

                    if (instance.Symbol != familySymbol) instance.Symbol = familySymbol;

                    if (item_type.type.Category.Name == myAll_Data.list_category_draw[4])
                    {
                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).Set(level_element_bottom.Id);
                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(elevation_bottom_value);
                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(level_element_top.Id);
                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(elevation_top_value);
                    }
                    else if (item_type.type.Category.Name == myAll_Data.list_category_draw[5])
                    {
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.1);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.1);

                        instance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).Set(level_element_top.Id);
                        instance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(elevation_top_value);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0);

                        StructuralFramingUtils.DisallowJoinAtEnd(instance, 0);
                        StructuralFramingUtils.DisallowJoinAtEnd(instance, 1);
                    }
                    else
                    {
                        if (host_of_opening != null)
                        {
                            if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[0])
                            {
                                instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_top.Id);
                            }
                            else if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[1])
                            {
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                if (instance.LookupParameter(myAll_Data.list_parameter_tag[4]) != null) instance.LookupParameter(myAll_Data.list_parameter_tag[4]).Set(Convert.ToDouble(elevation_bottom.Text) / myAll_Data.list_unit_value_data[2]);
                                if (instance.LookupParameter(myAll_Data.list_parameter_tag[0]) != null) instance.LookupParameter(myAll_Data.list_parameter_tag[0]).Set(0);
                            }
                            else
                            {
                                if (instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).IsReadOnly == false)
                                {
                                    instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                }
                                else if (instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                    && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                    && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).IsReadOnly == false)
                                {
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                }
                            }
                        }
                        else
                        {
                            if (instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).IsReadOnly == false)
                            {
                                instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                            }
                            else if (instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).IsReadOnly == false)
                            {
                                instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                            }
                        }
                    }

                    if (my_draw_data != null && my_draw_data.point1 != null)
                    {
                        Move_Element(my_draw_data.point1, my_draw_data.point2, doc, instance, item_type, item_family);
                    }

                    foreach (parameter_data data in my_parameter_data)
                    {
                        if (instance.LookupParameter(data.parameter_name).IsReadOnly == false)
                        {
                            if (data.parameter_value == "True")
                            {
                                instance.LookupParameter(data.parameter_name).Set(1);
                            }
                            else if (data.parameter_value == "False")
                            {
                                instance.LookupParameter(data.parameter_name).Set(0);
                            }
                            else
                            {
                                instance.LookupParameter(data.parameter_name).Set(Convert.ToDouble(data.parameter_value) / myAll_Data.list_unit_value_data[2]);
                            }
                        }
                    }
                }
                else
                {
                    Wall instance = element_update as Wall;
                    WallType familySymbol = item_type.type as WallType;

                    if (instance.WallType != familySymbol) instance.WallType = familySymbol;

                    instance.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(level_element_bottom.Id);
                    instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(elevation_bottom_value);
                    instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level_element_top.Id);
                    instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(elevation_top_value);

                    WallUtils.DisallowWallJoinAtEnd(instance, 0);
                    WallUtils.DisallowWallJoinAtEnd(instance, 1);
                    instance.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(0);

                    if (my_draw_data != null && my_draw_data.point1 != null)
                    {
                        Move_Element(my_draw_data.point1, my_draw_data.point2, doc, instance, item_type, item_family);
                    }
                }

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Draw_By_Revit(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {

                level_data item_level_bottom = (level_data)level_bottom.SelectedItem;
                level_data item_level_top = (level_data)level_top.SelectedItem;
                family_data item_family = (family_data)family.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;

                Draw_Data(item_type, item_family, doc);

                Level level_element_bottom = item_level_bottom.level;
                Level level_element_top = item_level_top.level;
                   
                double elevation_bottom_value = Convert.ToDouble(elevation_bottom.Text) / myAll_Data.list_unit_value_data[2] - item_level_bottom.elevation;
                double elevation_top_value = Convert.ToDouble(elevation_top.Text) / myAll_Data.list_unit_value_data[2] - item_level_top.elevation;

                if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == item_type.type.Name).ToList().Count() > 0)
                {
                    FamilyInstance instance = null;
                    FamilySymbol familySymbol = item_type.type as FamilySymbol;
                    if (familySymbol.IsActive == false) familySymbol.Activate();

                    if (item_type.type.Category.Name == myAll_Data.list_category_draw[4])
                    {
                        instance = doc.Create.NewFamilyInstance(my_draw_data.center, familySymbol, level_element_bottom, StructuralType.Column) as FamilyInstance;

                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM).Set(level_element_bottom.Id);
                        instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM).Set(elevation_bottom_value);
                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM).Set(level_element_top.Id);
                        instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM).Set(elevation_top_value);
                    }
                    else if (item_type.type.Category.Name == myAll_Data.list_category_draw[5])
                    {
                        instance = doc.Create.NewFamilyInstance(my_draw_data.curve, familySymbol, level_element_top, StructuralType.Beam) as FamilyInstance;

                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0.1);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0.1);
                        
                        instance.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).Set(level_element_top.Id);
                        instance.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE).Set(elevation_top_value);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION).Set(0);
                        instance.get_Parameter(BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION).Set(0);

                        StructuralFramingUtils.DisallowJoinAtEnd(instance, 0);
                        StructuralFramingUtils.DisallowJoinAtEnd(instance, 1);
                    }
                    else
                    {
                        instance = doc.Create.NewFamilyInstance(my_draw_data.center, familySymbol, host_of_opening, level_element_bottom, StructuralType.NonStructural);
                        if (instance.Host != null)
                        {
                            if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[0])
                            {
                                instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_top.Id);
                            }
                            else if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[1])
                            {
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                if (instance.LookupParameter(myAll_Data.list_parameter_tag[4]) != null) instance.LookupParameter(myAll_Data.list_parameter_tag[4]).Set(Convert.ToDouble(elevation_bottom.Text) / myAll_Data.list_unit_value_data[2]);
                                if (instance.LookupParameter(myAll_Data.list_parameter_tag[0]) != null) instance.LookupParameter(myAll_Data.list_parameter_tag[0]).Set(0);
                            }
                            else
                            {
                                if (instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).IsReadOnly == false)
                                {
                                    instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                }
                                else if (instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                    && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                    && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).IsReadOnly == false)
                                {
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                    instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                                }
                            }
                        }
                        else
                        {
                            if (instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).IsReadOnly == false)
                            {
                                instance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                            }
                            else if (instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1
                                && instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).IsReadOnly == false)
                            {
                                instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(level_element_bottom.Id);
                                instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_bottom_value);
                            }
                        }
                    }

                    if (my_draw_data.point1 != null)
                    {
                        Move_Element(my_draw_data.point1, my_draw_data.point2, doc, instance, item_type, item_family);
                    }

                    foreach (parameter_data data in my_parameter_data)
                    {
                        if (instance.LookupParameter(data.parameter_name).IsReadOnly == false)
                        {
                            if (data.parameter_value == "True")
                            {
                                instance.LookupParameter(data.parameter_name).Set(1);
                            }
                            else if (data.parameter_value == "False")
                            {
                                instance.LookupParameter(data.parameter_name).Set(0);
                            }
                            else
                            {
                                instance.LookupParameter(data.parameter_name).Set(Convert.ToDouble(data.parameter_value) / myAll_Data.list_unit_value_data[2]);
                            }
                        }
                    }

                    if (host_of_opening == null)
                    {
                        Join_Element(doc, instance);
                    }
                }
                else
                {
                    WallType wallType = item_type.type as WallType;
                    Wall instance = Wall.Create(doc, my_draw_data.curve, wallType.Id, level_element_bottom.Id, 10, 0, true, true);

                    instance.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).Set(level_element_bottom.Id);
                    instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).Set(elevation_bottom_value);
                    instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level_element_top.Id);
                    instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET).Set(elevation_top_value);

                    WallUtils.DisallowWallJoinAtEnd(instance, 0);
                    WallUtils.DisallowWallJoinAtEnd(instance, 1);
                    instance.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM).Set(0);

                    if (my_draw_data.point1 != null)
                    {
                        Move_Element(my_draw_data.point1, my_draw_data.point2, doc, instance, item_type, item_family);
                    }
                    Join_Element(doc, instance);
                }

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public void Join_Element(Document doc, Element element)
        {
            try
            {
                BoundingBoxXYZ bb = element.get_BoundingBox(doc.ActiveView);
                Outline outline = new Outline(bb.Min, bb.Max);
                BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector collectors = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<ElementId> idsExclude = new List<ElementId>();
                idsExclude.Add(element.Id);
                var collector = collectors.Excluding(idsExclude).WherePasses(bbfilter).ToElements();
                foreach (Element ele in collector)
                {
                    if (JoinGeometryUtils.AreElementsJoined(doc, element, ele) == false && ele.Category.AllowsBoundParameters == true && ele.Category.CategoryType.ToString() == "Model")
                    {
                        JoinGeometryUtils.JoinGeometry(doc, element, ele);
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        draw_data my_draw_data = new draw_data();
        public void Draw_Data(type_data item_type, family_data item_family, Document doc)
        {
            try
            {
                if(data_point.name == mySource.list_option_selection_name[0])
                {
                    my_draw_data = new draw_data()
                    {
                        center = data_point.list_point[0]
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[1])
                {
                    my_draw_data = new draw_data()
                    {
                        center = (data_point.list_point[0] + data_point.list_point[1]) / 2,
                        point1 = data_point.list_point[0],
                        point2 = data_point.list_point[1],
                        curve = Line.CreateBound(data_point.list_point[0], data_point.list_point[1])
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[2])
                {
                    XYZ center = data_point.list_point[0];
                    XYZ start_point = data_point.list_point[1];
                    XYZ end_point = data_point.list_point[2];

                    double start_angle = XYZ.BasisX.AngleOnPlaneTo((start_point - center), XYZ.BasisZ);
                    double end_angle = XYZ.BasisX.AngleOnPlaneTo((end_point - center), XYZ.BasisZ);

                    if (start_angle > end_angle) start_angle = start_angle - Math.PI * 2;
                    else if (start_angle == end_angle) end_angle = end_angle + Math.PI;

                    my_draw_data = new draw_data()
                    {
                        center = center,
                        point1 = data_point.list_point[0],
                        point2 = data_point.list_point[1],
                        curve = Arc.Create(center, Line.CreateBound(center, start_point).Length, start_angle, end_angle, XYZ.BasisX, XYZ.BasisY)
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[3])
                {
                    XYZ start_point_1 = data_point.list_point[0];
                    XYZ end_point_1 = data_point.list_point[1];
                    XYZ start_point_2 = data_point.list_point[2];
                    XYZ end_point_2 = data_point.list_point[3];

                    my_draw_data = new draw_data()
                    {
                        center = (start_point_1 + end_point_1) / 2,
                        point1 = start_point_1,
                        point2 = end_point_1,
                        curve = Ellipse.CreateCurve((start_point_1 + end_point_1) / 2, Line.CreateBound(start_point_1, end_point_1).Length / 2, Line.CreateBound(start_point_2, end_point_2).Length / 2, XYZ.BasisX, XYZ.BasisY, 0, Math.PI)
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[4])
                {
                    var new_line = data_point.line.CreateTransformed(Transform.CreateTranslation(origin));
                    my_draw_data = new draw_data()
                    {
                        center = (new_line.GetEndPoint(0) + new_line.GetEndPoint(1)) / 2,
                        point1 = new_line.GetEndPoint(0),
                        point2 = new_line.GetEndPoint(1),
                        curve = new_line
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[5])
                {
                    double b = 0;
                    if (item_type.type.Category.Name == myAll_Data.list_category_draw[3])
                    {
                        WallType wallType = item_type.type as WallType;
                        b = Math.Round(wallType.Width * myAll_Data.list_unit_value_data[2], mySource.lam_tron);
                    }
                    else if (item_type.type.Category.Name == myAll_Data.list_category_draw[4])
                    {
                        b = Math.Round(item_type.type.LookupParameter(myAll_Data.list_parameter_column[0]).AsDouble() * myAll_Data.list_unit_value_data[2], mySource.lam_tron);
                    }
                    else if (item_type.type.Category.Name == myAll_Data.list_category_draw[5])
                    {
                        b = Math.Round(item_type.type.LookupParameter(myAll_Data.list_parameter_framing[0]).AsDouble() * myAll_Data.list_unit_value_data[2], mySource.lam_tron);
                    }
                    else if (item_type.type.Category.Name == myAll_Data.list_category_draw[0])
                    {
                        Family fa = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.Name == item_family.single_value).First();
                        if(fa.LookupParameter("Host") != null && fa.LookupParameter("Host").AsString() == "Floor" && my_parameter_data.First(x => x.parameter_name.Contains(myAll_Data.list_parameter_opening[0])) != null)
                        {
                            b = Convert.ToDouble(my_parameter_data.First(x => x.parameter_name.Contains(myAll_Data.list_parameter_opening[0])).parameter_value);
                        }
                    }

                    PolyLine polyLine = data_point.polyLine;
                    Line line_1 = Line.CreateBound(polyLine.GetCoordinate(0), polyLine.GetCoordinate(1));
                    Line line_2 = Line.CreateBound(polyLine.GetCoordinate(1), polyLine.GetCoordinate(2));
                    Line line_3 = Line.CreateBound(polyLine.GetCoordinate(2), polyLine.GetCoordinate(3));
                    Line line_4 = Line.CreateBound(polyLine.GetCoordinate(3), polyLine.GetCoordinate(0));
                    XYZ center = (polyLine.GetCoordinate(0) + polyLine.GetCoordinate(2)) / 2;

                    XYZ center_1 = new XYZ();
                    XYZ center_2 = new XYZ();
                    if (Math.Round(line_1.Length * myAll_Data.list_unit_value_data[2], mySource.lam_tron) == b)
                    {
                        center_1 = (line_1.GetEndPoint(0) + line_1.GetEndPoint(1)) / 2;
                        center_2 = (line_3.GetEndPoint(0) + line_3.GetEndPoint(1)) / 2;
                    }
                    else
                    {
                        center_1 = (line_2.GetEndPoint(0) + line_2.GetEndPoint(1)) / 2;
                        center_2 = (line_4.GetEndPoint(0) + line_4.GetEndPoint(1)) / 2;
                    }

                    my_draw_data = new draw_data()
                    {
                        center = center + origin,
                        point1 = center_1 + origin,
                        point2 = center_2 + origin,
                        curve = Line.CreateBound(center_1 + origin, center_2 + origin)
                    };
                }
                else if (data_point.name == mySource.list_option_selection_name[6])
                {
                    if (data_point.arc != null)
                    {
                        Arc polyLine = data_point.arc;
                        if (polyLine.IsBound == false) polyLine.MakeBound(0, Math.PI);
                        Arc new_line = polyLine.CreateTransformed(Transform.CreateTranslation(origin)) as Arc;
                        my_draw_data = new draw_data()
                        {
                            center = new_line.Center,
                            point1 = new_line.GetEndPoint(0),
                            point2 = new_line.GetEndPoint(1),
                            curve = new_line
                        };
                    }
                    if (data_point.ellipse != null)
                    {
                        Ellipse polyLine = data_point.ellipse;
                        if (polyLine.IsBound == false) polyLine.MakeBound(0, Math.PI);
                        Ellipse new_line = polyLine.CreateTransformed(Transform.CreateTranslation(origin)) as Ellipse;
                        my_draw_data = new draw_data()
                        {
                            center = new_line.Center,
                            point1 = new_line.GetEndPoint(0),
                            point2 = new_line.GetEndPoint(1),
                            curve = new_line
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Move_Element(XYZ point1, XYZ point2, Document doc, Element element, type_data item_type, family_data item_family)
        {
            try
            {
                Line curve = Line.CreateBound(point1, point2);
                Transform rot_90 = Transform.CreateRotation(XYZ.BasisZ, Math.PI / 2);
                Line cRot = curve.CreateTransformed(rot_90) as Line;

                XYZ center = (point1 + point2) / 2;
                Line axis = Line.CreateBound(center, new XYZ(center.X, center.Y, center.Z + 1000));
                double xAngle = XYZ.BasisX.AngleOnPlaneTo(curve.Direction, XYZ.BasisZ);

                double factor_move = 0;
                location_line_data item = (location_line_data)location_line.SelectedItem;
                if (item.single_value == mySource.location_line_data[0].single_value) factor_move = mySource.location_line_data[0].value;
                else
                {
                    factor_move = mySource.location_line_data[1].value;
                }

                if (element.Category.Name == myAll_Data.list_category_draw[3])
                {
                    Wall wall = element as Wall;
                    wall.Location.Move(cRot.Direction * wall.Width / 2 * factor_move);
                }
                else if (element.Category.Name == myAll_Data.list_category_draw[5])
                {
                    FamilyInstance framing = element as FamilyInstance;
                    framing.Location.Move(cRot.Direction * item_type.type.LookupParameter(myAll_Data.list_parameter_framing[0]).AsDouble() / 2 * factor_move);
                }
                else if (element.Category.Name == myAll_Data.list_category_draw[4])
                {
                    FamilyInstance column = element as FamilyInstance;
                    double b = item_type.type.LookupParameter(myAll_Data.list_parameter_column[0]).AsDouble();
                    double distance_move = 0;
                    if (Math.Round(b * myAll_Data.list_unit_value_data[2],mySource.lam_tron) == Math.Round(curve.Length * myAll_Data.list_unit_value_data[2], mySource.lam_tron))
                    {
                        column.Location.Rotate(axis, xAngle - Math.PI / 2);
                        distance_move = item_type.type.LookupParameter(myAll_Data.list_parameter_column[1]).AsDouble();
                    }
                    else
                    {
                        column.Location.Rotate(axis,xAngle);
                        distance_move = item_type.type.LookupParameter(myAll_Data.list_parameter_column[0]).AsDouble();
                    }

                    column.Location.Move(cRot.Direction * distance_move / 2 * factor_move);
                }
                else if (item_type.type.Category.Name == myAll_Data.list_category_draw[0])
                {
                    Family fa = new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(x => x.Name == item_family.single_value).First();
                    if (fa.LookupParameter("Host") != null && fa.LookupParameter("Host").AsValueString() == "Floor")
                    {
                        FamilyInstance column = element as FamilyInstance;
                        double b = Convert.ToDouble(my_parameter_data.First(x => x.parameter_name.Contains(myAll_Data.list_parameter_opening[0])).parameter_value);
                        double distance_move = 0;
                        if (Convert.ToDouble(my_parameter_data.First(x => x.parameter_name.Contains(myAll_Data.list_parameter_opening[0])).parameter_value) == Math.Round(curve.Length * myAll_Data.list_unit_value_data[2], mySource.lam_tron))
                        {
                            column.Location.Rotate(axis, xAngle - Math.PI / 2);
                            distance_move = Convert.ToDouble(my_parameter_data.First(x => x.parameter_name.Contains(myAll_Data.list_parameter_opening[1])).parameter_value);
                        }
                        else
                        {
                            column.Location.Rotate(axis, xAngle);
                            distance_move = b;
                        }
                        column.Location.Move(cRot.Direction * distance_move / myAll_Data.list_unit_value_data[2] / 2 * factor_move);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public string Draw_By_CAD(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

    }
}
