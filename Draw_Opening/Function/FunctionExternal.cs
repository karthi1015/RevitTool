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
#endregion

namespace Draw_Opening
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

        public TextBox elevation { get; set; }
        public TextBox b { get; set; }
        public TextBox h { get; set; }

        public ComboBox level { get; set; }
        public ComboBox family { get; set; }
        public ComboBox type { get; set; }

        public RadioButton door { get; set; }

        public ListView thong_tin_parameter { get; set; }

        public ObservableCollection<type_data> my_type_data { get; set; }
        public ObservableCollection<parameter_data> my_parameter_data { get; set; }

        public FamilyInstance element { get; set; }

        public Element host_of_opening { get; set; }

        public List<List<XYZ>> data_point { get; set; }

        public All_Data myAll_Data { get; set; }

        ListSource mySource;
        FunctionSupoort myFunctionSupport;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new FunctionSupoort();

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Opening");

                if (command == "Update By Revit")
                {
                    Update_By_Revit(uiapp, doc);
                }

                if (command == "Draw By Revit")
                {
                    Draw_By_Revit(uiapp, doc);
                }

                if (command == "Draw By CAD")
                {
                    Draw_By_CAD(uiapp, doc);
                }

                if(command == "Duplicate Type")
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
        public string Duplicate_Type(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                type_data item_type = (type_data)type.SelectedItem;

                ElementType new_type = item_type.type.Duplicate(b.Text + "x" + h.Text);
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
        public string Update_By_Revit(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                level_data item_level = (level_data)level.SelectedItem;
                family_data item_family = (family_data)family.SelectedItem;
                type_data item_type = (type_data)type.SelectedItem;
                FamilySymbol familySymbol = item_type.type as FamilySymbol;
                if (familySymbol.IsActive == false) familySymbol.Activate();

                if (element.Symbol != familySymbol) element.Symbol = familySymbol;

                double elevation_value = Convert.ToDouble(elevation.Text) / myAll_Data.list_unit_value_data[2] - item_level.elevation;
                if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == host_of_opening.Name).ToList().Count() == 0 && host_of_opening.Category.Name != "Walls")
                {
                    
                }
                else
                {
                    if (element.Category.Name == "Doors")
                    {
                        element.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(elevation_value);
                        if (element.LookupParameter(mySource.para_name[0]) != null) element.LookupParameter(mySource.para_name[0]).Set(Convert.ToDouble(elevation.Text) / myAll_Data.list_unit_value_data[2]);
                        if (element.LookupParameter(mySource.para_name[1]) != null) element.LookupParameter(mySource.para_name[1]).Set(0);
                    }
                    else
                    {
                        if (host_of_opening.Category.Name != "Walls")
                        {
                            element.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(item_level.level.Id);
                        }
                        element.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_value);
                    }
                }

                foreach(parameter_data data in my_parameter_data)
                {
                    if (data.parameter_value == "True")
                    {
                        element.LookupParameter(data.parameter_name).Set(1);
                    }
                    else if (data.parameter_value == "False")
                    {
                        element.LookupParameter(data.parameter_name).Set(0);
                    }
                    else
                    {
                        element.LookupParameter(data.parameter_name).Set(Convert.ToDouble(data.parameter_value) / myAll_Data.list_unit_value_data[2]);
                    }
                }

                if (data_point.Count() > 0)
                {
                    XYZ center = new XYZ();
                    if (data_point[0].Count() == 2)
                    {
                        center = (data_point[0][0] + data_point[0][1]) / 2;
                    }
                    else
                    {
                        center = data_point[0][0];
                    }

                    LocationPoint location = element.Location as LocationPoint;
                    XYZ location_point = location.Point;

                    XYZ move_point = new XYZ(center.X - location_point.X, center.Y - location_point.Y, 0);

                    ElementTransformUtils.MoveElement(doc, element.Id, move_point);
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
                if (data_point.Count() > 0)
                {
                    level_data item_level = (level_data)level.SelectedItem;
                    family_data item_family = (family_data)family.SelectedItem;
                    type_data item_type = (type_data)type.SelectedItem;

                    XYZ center = new XYZ();
                    if (data_point[0].Count() == 2)
                    {
                        center = (data_point[0][0] + data_point[0][1]) / 2;
                    }
                    else
                    {
                        center = data_point[0][0];
                    }
                    FamilySymbol familySymbol = item_type.type as FamilySymbol;
                    Level level_opening = item_level.level;
                    if (familySymbol.IsActive == false) familySymbol.Activate();

                    var instance = doc.Create.NewFamilyInstance(center, familySymbol, host_of_opening, level_opening, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                    double elevation_value = Convert.ToDouble(elevation.Text) / myAll_Data.list_unit_value_data[2] - item_level.elevation;
                    if (new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Where(x => x.Name == host_of_opening.Name).ToList().Count() == 0 && host_of_opening.Category.Name != "Walls")
                    {

                    }
                    else
                    {
                        if (door.IsChecked == true)
                        {
                            instance.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM).Set(elevation_value);
                            if (instance.LookupParameter(mySource.para_name[0]) != null) instance.LookupParameter(mySource.para_name[0]).Set(Convert.ToDouble(elevation.Text) / myAll_Data.list_unit_value_data[2]);
                            if (instance.LookupParameter(mySource.para_name[1]) != null) instance.LookupParameter(mySource.para_name[1]).Set(0);
                        }
                        else
                        {
                            if (host_of_opening.Category.Name != "Walls")
                            {
                                instance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).Set(item_level.level.Id);
                            }
                            instance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).Set(elevation_value);
                        }
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
                

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
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
