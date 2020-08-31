using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_TypeSetup.Code.Function;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace SetupTool_TypeSetup.Code.External
{
    class E_Create : IExternalEventHandler
    {
        public ListView thong_tin_kich_thuoc { get; set; }

        public ListView thong_tin_cong_tac_vat_lieu { get; set; }

        public TextBlock name { get; set; }

        public TreeView thong_tin_family_type { get; set; }

        public string unit_length { get; set; }

        public ObservableCollection<data_family> my_family { get; set; }
        public ObservableCollection<data_parameters> my_parameters { get; set; }
        public ObservableCollection<data_materials> my_materials { get; set; }
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Create Type");

                string result = Tao_Type_Project(uiapp, doc);
                if (result == "S")
                {
                    MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "Create Type";
        }

        //----------------------------------------------------------
        public string Tao_Type_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                data_type type = (data_type)thong_tin_family_type.SelectedItem;
                if (my_family.First(x => x.ten_family_type == type.element_type.FamilyName).Children.Any(x => x.ten_element_type == name.Text) == false)
                {
                    if (type.type_type == Source.type_symbol)
                    {
                        FamilySymbol symbol = type.element_type.Duplicate(name.Text) as FamilySymbol;

                        foreach (data_parameters parameters in my_parameters)
                        {
                            if (symbol.LookupParameter(parameters.parameter.Definition.Name).StorageType == StorageType.Integer)
                                symbol.LookupParameter(parameters.parameter.Definition.Name).Set(parameters.gia_tri_parameter == "False" ? 0 : (parameters.gia_tri_parameter == "True" ? 1 : (Convert.ToInt32(parameters.gia_tri_parameter))));
                            else
                            {
                                if (parameters.parameter.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                                    symbol.LookupParameter(parameters.parameter.Definition.Name).Set(Convert.ToDouble(parameters.gia_tri_parameter) / Source.units_convert.First(x => x.name == "deg").value);
                                else
                                    symbol.LookupParameter(parameters.parameter.Definition.Name).Set(Convert.ToDouble(parameters.gia_tri_parameter) / Source.units_document.First(x => x.name == unit_length).value);
                            }
                        }
                        foreach (data_materials material in my_materials)
                        {
                            if (material.ten_vat_lieu.name != "<By Category>")
                            {
                                symbol.LookupParameter(material.parameter.Definition.Name).Set(material.ten_vat_lieu.vat_lieu.Id);
                            }
                            else
                            {
                                symbol.LookupParameter(material.parameter.Definition.Name).Set(new ElementId(-1));
                            }
                        }
                        data_type element_Type = new data_type()
                        {
                            ten_element_type = name.Text,
                            element_type = symbol,
                            type_type = Source.type_symbol,
                            delete_type = false
                        };

                        my_family.First(x => x.ten_family_type == symbol.FamilyName).Children.Add(element_Type);
                        result = "S";
                    }
                    else
                    {
                        ElementType elementType = type.element_type as ElementType;

                        ElementType elementType_new = F_SetParameterAndMaterialType.set_elementtype(null, my_materials, my_parameters, elementType, Source.Duplicate, name.Text, unit_length);

                        data_type element_Type = new data_type()
                        {
                            ten_element_type = name.Text,
                            element_type = elementType_new,
                            type_type = Source.type_element,
                            delete_type = false,
                            ValueIsSelect = true
                        };

                        my_family.First(x => x.ten_family_type == elementType.FamilyName).Children.Add(element_Type);
                        result = "S";
                    }
                    thong_tin_family_type.Items.Refresh();
                }
                else
                {
                    MessageBox.Show("The type name you supplied is already in use. Enter a unique name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
