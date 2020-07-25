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
using TreeView = System.Windows.Controls.TreeView;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace SetupTool_ParameterSetup
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

        public ComboBox number { get; set; }
        public TextBox name { get; set; }
        public TextBox address { get; set; }
        public TextBox block { get; set; }
        public ListView thong_tin_parameter { get; set; }
        public ListView thong_tin_parameter_project { get; set; }
        public TreeView thong_tin_share_parameter { get; set; }
        public ObservableCollection<Group_Share_Parameter> list_group_parameter { get; set; }
        public ObservableCollection<Data> list_data_parameter_need { get; set; }
        public ObservableCollection<Data> list_data_parameter_current { get; set; }
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
                transaction.Start("Parameters");

                if (command == "Create")
                {
                    string result = Them_Hoac_Xoa_Information_Trong_Project(uiapp, doc);
                    string result1 = Them_Hoac_Xoa_Parameter_Trong_Project(uiapp, doc);
                    if (result == "S" && result1 == "S")
                    {
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
        public string Them_Hoac_Xoa_Parameter_Trong_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                var enums = Enum.GetValues(typeof(BuiltInCategory));
                var enums1 = Enum.GetValues(typeof(BuiltInParameterGroup));
                var categories = doc.Settings.Categories;
                foreach (Group_Share_Parameter item in list_group_parameter)
                {
                    DefinitionFile parafile = uiapp.Application.OpenSharedParameterFile();
                    if (parafile != null)
                    {
                        DefinitionGroup myGroup = parafile.Groups.get_Item(item.ten_group_parameter);

                        foreach (Share_Parameter subitem in item.Children)
                        {
                            if (subitem.exist_parameter == true)
                            {
                                //BuiltInParameterGroup builtInParameterGroup = BuiltInParameterGroup.PG_TEXT;
                                //foreach (BuiltInParameterGroup builtIn in enums1)
                                //{
                                //    if (builtIn.ToString().Split('_')[0] == subitem.ten_parameter && builtIn != BuiltInParameterGroup.INVALID) builtInParameterGroup = builtIn;
                                //}

                                Definition myDefinition_ProductDate = myGroup.Definitions.get_Item(subitem.ten_parameter);
                                CategorySet myCategories = uiapp.Application.Create.NewCategorySet();
                                foreach (BuiltInCategory buildCategory in enums)
                                {
                                    try
                                    {
                                        Category cate = categories.get_Item(buildCategory);
                                        if (cate.AllowsBoundParameters == true && cate.CategoryType.ToString() == "Model")
                                        {
                                            myCategories.Insert(cate);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                
                                try
                                {
                                    InstanceBinding instanceBinding = uiapp.Application.Create.NewInstanceBinding(myCategories);
                                    BindingMap bindingMap = doc.ParameterBindings;
                                    bindingMap.Insert(myDefinition_ProductDate, instanceBinding);
                                    if (list_data_parameter_current.Any(x => x.ten_parameter == subitem.ten_parameter) == false) list_data_parameter_current.Add(new Data() { ten_parameter = subitem.ten_parameter });
                                    
                                }
                                catch (Exception)
                                {

                                }
                            }
                            else
                            {
                                try
                                {
                                    Definition myDefinition_ProductDate = myGroup.Definitions.get_Item(subitem.ten_parameter);
                                    BindingMap bindingMap = doc.ParameterBindings;
                                    bindingMap.Remove(myDefinition_ProductDate);
                                    Data item_remove = list_data_parameter_current.First(x => x.ten_parameter == subitem.ten_parameter);
                                    if (item_remove != null) list_data_parameter_current.Remove(item_remove);
                                }
                                catch (Exception)
                                {

                                }
                            }
                            try
                            {
                                if (list_data_parameter_current.Any(x => x.ten_parameter == subitem.ten_parameter) == false) list_data_parameter_need.First(x => x.ten_parameter == subitem.ten_parameter).color = myAll_Data.list_color_UI_data[0];
                                else list_data_parameter_need.First(x => x.ten_parameter == subitem.ten_parameter).color = myAll_Data.list_color_UI_data[2];
                                thong_tin_parameter.Items.Refresh();
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Not found share parameter file!!!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
        public string Them_Hoac_Xoa_Information_Trong_Project(UIApplication uiapp,Document doc)
        {
            string result = "F";
            try
            {
                if (number.SelectedItem != null && name.Text != "" && address.Text != "" && block.Text != "")
                {
                    var projectInfor = doc.ProjectInformation;
                    Data item = (Data)number.SelectedItem;
                    projectInfor.Number = item.single_value;
                    projectInfor.Name = name.Text;
                    projectInfor.Address = address.Text;
                    projectInfor.BuildingName = block.Text;
                    result = "S";
                }
                else
                {
                    MessageBox.Show("Missing data!!!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Xoa_Type_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

    }
}
