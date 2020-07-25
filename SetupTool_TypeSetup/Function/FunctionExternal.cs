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
using TreeView = System.Windows.Controls.TreeView;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace SetupTool_TypeSetup
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

        public ListView thong_tin_kich_thuoc { get; set; }

        public ListView thong_tin_cong_tac_vat_lieu { get; set; }

        public TextBox name { get; set; }

        public TreeView thong_tin_family_type { get; set; }

        public ObservableCollection<Family_Type> myFamily_Type { get; set; }
        public ObservableCollection<Element_Type> myElement_Type { get; set; }
        public ObservableCollection<Parameters_Family> myParameters_Family { get; set; }
        public ObservableCollection<Material_Family> myMaterial_Family { get; set; }
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
                transaction.Start("Family Type");

                if (command == "Create")
                {
                    string result = Tao_Type_Project(uiapp, doc);
                    if (result == "S")
                    {
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (command == "Modify")
                {
                    string result = Sua_Thong_Tin_Type_Project(uiapp, doc);
                    if (result == "S")
                    {
                        MessageBox.Show("Modify Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (command == "Delete")
                {
                    string result = Xoa_Type_Project(uiapp, doc);
                    if (result == "S")
                    {
                        MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public string Tao_Type_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                Element_Type type = (Element_Type)thong_tin_family_type.SelectedItem;
                if (type.type_type == mySource.type_symbol)
                {
                    FamilySymbol symbol = type.element_type.Duplicate(name.Text) as FamilySymbol;
                    ElementId id = new ElementId(-1);

                    foreach (Parameters_Family parameters in myParameters_Family)
                    {
                        symbol.LookupParameter(parameters.parameter.Definition.Name).Set(Convert.ToDouble(parameters.gia_tri_parameter) / myAll_Data.list_unit_value_data[1]);
                    }
                    foreach (Material_Family material in myMaterial_Family)
                    {
                        if (material.ten_vat_lieu.single_value != "<By Category>")
                        {
                            symbol.LookupParameter(material.parameter.Definition.Name).Set(material.ten_vat_lieu.vat_lieu.Id);
                        }
                        else
                        {
                            symbol.LookupParameter(material.parameter.Definition.Name).Set(id);
                        }
                    }
                    Element_Type element_Type = new Element_Type()
                    {
                        ten_element_type = name.Text,
                        element_type = symbol,
                        type_type = mySource.type_symbol,
                        delete_type = false
                    };

                    myFamily_Type.First(x => x.ten_family_type == symbol.FamilyName).Children.Add(element_Type);
                    result = "S";
                }
                else
                {
                    ElementType elementType = type.element_type as ElementType;

                    ElementType elementType_new = myFunctionSupport.Set_Type(null, myMaterial_Family, myParameters_Family, elementType, mySource.Duplicate, name.Text, myAll_Data);

                    Element_Type element_Type = new Element_Type()
                    {
                        ten_element_type = name.Text,
                        element_type = elementType_new,
                        type_type = mySource.type_element,
                        delete_type = false
                    };

                    myFamily_Type.First(x => x.ten_family_type == elementType.FamilyName).Children.Add(element_Type);
                    result = "S";
                }
                thong_tin_family_type.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Sua_Thong_Tin_Type_Project(UIApplication uiapp,Document doc)
        {
            string result = "F";
            try
            {
                Element_Type type = (Element_Type)thong_tin_family_type.SelectedItem;
                if (type.type_type == mySource.type_symbol)
                {
                    FamilySymbol symbol = type.element_type as FamilySymbol;
                    symbol.Name = name.Text;
                    ElementId id = new ElementId(-1);

                    foreach (Parameters_Family parameters in myParameters_Family)
                    {
                        if (parameters.parameter.Definition.ParameterGroup == myAll_Data.list_builtin_parameter_group_data[3].builtin)
                        {
                            parameters.parameter.Set(Convert.ToInt32(parameters.gia_tri_parameter));
                        }
                        else
                        {
                            parameters.parameter.Set(Convert.ToDouble(parameters.gia_tri_parameter) / myAll_Data.list_unit_value_data[1]);
                        }
                    }
                    foreach (Material_Family material in myMaterial_Family)
                    {
                        if (material.ten_vat_lieu.single_value != "<By Category>")
                        {
                            material.parameter.Set(material.ten_vat_lieu.vat_lieu.Id);
                        }
                        else
                        {
                            material.parameter.Set(id);
                        }
                    }
                    Element_Type element_Type = new Element_Type()
                    {
                        ten_element_type = name.Text,
                        element_type = symbol,
                        type_type = mySource.type_symbol,
                        delete_type = false
                    };

                    type.ten_element_type = name.Text;
                    thong_tin_family_type.Items.Refresh();
                    result = "S";
                }
                else
                {
                    ElementType elementType = type.element_type as ElementType;
                    elementType.Name = name.Text;

                    CompoundStructure compound = myParameters_Family[0].compound;
                    myFunctionSupport.Set_Type(compound, myMaterial_Family, myParameters_Family, elementType, mySource.Update, "", myAll_Data);

                    type.ten_element_type = name.Text;
                    thong_tin_family_type.Items.Refresh();
                    result = "S";
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
                List<ElementId> ids = new List<ElementId>();
                List<Element_Type> myElement_Type_delete = new List<Element_Type>();
                foreach (Family_Type family in myFamily_Type)
                {
                    if (family.delete_family == true)
                    {
                        foreach (Element_Type type in family.Children)
                        {
                            ids.Add(type.element_type.Id);
                            myElement_Type_delete.Add(type);
                        }
                    }
                    else
                    {
                        foreach (Element_Type type in family.Children)
                        {
                            if (type.delete_type == true)
                            {
                                ids.Add(type.element_type.Id);
                                myElement_Type_delete.Add(type);
                            }
                        }
                    }
                }
                if(ids.Count() > 0)
                {
                    var result_message = MessageBox.Show("Type sẽ được xóa trực tiếp trên dự án.\nBạn có chắc muốn xóa không!", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result_message == MessageBoxResult.Yes)
                    {
                        doc.Delete(ids);
                        foreach (Family_Type family in myFamily_Type)
                        {
                            var type_delete = family.Children.Where(x => x.delete_type == true).ToList();
                            foreach (Element_Type type in type_delete)
                            {
                                try { family.Children.Remove(type); } catch (Exception) { }
                            }
                        }
                        var family_delete = myFamily_Type.Where(x => x.delete_family == true).ToList();
                        foreach (Family_Type family in family_delete)
                        {
                            try { myFamily_Type.Remove(family); } catch (Exception) { }
                        }
                        thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<Parameters_Family>();
                        thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<Material_Family>();
                        result = "S";
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn type muốn xóa và thử lại lần nữa!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

    }
}
