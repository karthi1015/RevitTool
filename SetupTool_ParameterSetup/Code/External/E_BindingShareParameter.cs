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
using SetupTool_ParameterSetup.Data.Binding;
using System.Security.Cryptography;
#endregion

namespace SetupTool_ParameterSetup
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    class E_BindingShareParameter : IExternalEventHandler
    {
        public ComboBox number { get; set; }
        public TextBox block { get; set; }
        public ListView thong_tin_parameter { get; set; }
        public ListView thong_tin_parameter_project { get; set; }
        public TreeView thong_tin_share_parameter { get; set; }
        public ObservableCollection<data_group_share_parameter> my_group_share_parameter { get; set; }
        public ObservableCollection<data_parameter> my_data_parameter_need { get; set; }
        public ObservableCollection<data_parameter> my_data_parameter_current { get; set; }

        public string descipline { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                TransactionGroup transaction = new TransactionGroup(doc);
                transaction.Start("Parameters");

                string result = Them_Hoac_Xoa_Information_Trong_Project(uiapp, doc);
                string result1 = Them_Hoac_Xoa_Parameter_Trong_Project(uiapp, doc);
                if (result == "S" && result1 == "S")
                {
                    MessageBox.Show("Process Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                transaction.Assimilate();
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
                Transaction transaction = new Transaction(doc);
                transaction.Start("Parameters");
                var enums = Enum.GetValues(typeof(BuiltInCategory));
                var enums1 = Enum.GetValues(typeof(BuiltInParameterGroup));
                var categories = doc.Settings.Categories;
                foreach (data_group_share_parameter item in my_group_share_parameter)
                {
                    DefinitionFile parafile = uiapp.Application.OpenSharedParameterFile();
                    if (parafile != null)
                    {
                        DefinitionGroup myGroup = parafile.Groups.get_Item(item.ten_group_parameter);

                        foreach (data_item_share_parameter subitem in item.Children)
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
                                    BindingMap bindingMap = doc.ParameterBindings;
                                    if (subitem.ten_parameter != "Chiều dày hoàn thiện")
                                    {
                                        InstanceBinding instanceBinding = uiapp.Application.Create.NewInstanceBinding(myCategories);
                                        bindingMap.Insert(myDefinition_ProductDate, instanceBinding);
                                    }
                                    else
                                    {
                                        TypeBinding instanceBinding = uiapp.Application.Create.NewTypeBinding(myCategories);
                                        bindingMap.Insert(myDefinition_ProductDate, instanceBinding);
                                    }
                                    if (my_data_parameter_current.Any(x => x.ten_parameter == subitem.ten_parameter) == false) my_data_parameter_current.Add(new data_parameter() { ten_parameter = subitem.ten_parameter });
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
                                    data_parameter item_remove = my_data_parameter_current.First(x => x.ten_parameter == subitem.ten_parameter);
                                    if (item_remove != null) my_data_parameter_current.Remove(item_remove);
                                }
                                catch (Exception)
                                {

                                }
                            }
                            try
                            {
                                if (my_data_parameter_current.Any(x => x.ten_parameter == subitem.ten_parameter) == false) my_data_parameter_need.First(x => x.ten_parameter == subitem.ten_parameter).color = Source.color_error;
                                else my_data_parameter_need.First(x => x.ten_parameter == subitem.ten_parameter).color = Source.color;
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
                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Them_Hoac_Xoa_Information_Trong_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Parameters");
                if (number.SelectedItem != null && block.Text != "")
                {
                    var projectInfor = doc.ProjectInformation;
                    data_information item = (data_information)number.SelectedItem;
                    projectInfor.Number = item.project_number;
                    projectInfor.Name = item.project_name;
                    projectInfor.Address = item.project_address;
                    projectInfor.BuildingName = block.Text;
                    if(projectInfor.LookupParameter("Class") != null) projectInfor.LookupParameter("Class").Set(descipline);
                    result = "S";
                }
                else
                {
                    MessageBox.Show("Missing data!!!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
