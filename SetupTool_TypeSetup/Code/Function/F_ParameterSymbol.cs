using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_ParameterSymbol
    {
        //----------------------------------------------------------
        public static void parameter_symbol(Document doc, ElementType type, ListView thong_tin_kich_thuoc, ObservableCollection<data_parameters> my_parameters, string unit_length)
        {
            try
            {
                foreach (Parameter para in type.Parameters)
                {
                    if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY && para.IsReadOnly == false)
                    {
                        my_parameters.Add(new data_parameters()
                        {
                            ten_parameter = para.Definition.Name,
                            gia_tri_parameter = get_value(para, unit_length),
                            group_parameter = "Dimensions",
                            parameter = para
                        });
                    }
                    else if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_DATA && para.IsReadOnly == false)
                    {
                        my_parameters.Add(new data_parameters()
                        {
                            ten_parameter = para.Definition.Name,
                            gia_tri_parameter = get_value(para, unit_length),
                            group_parameter = "Data",
                            parameter = para
                        });
                    }
                    else if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_VISIBILITY && para.IsReadOnly == false)
                    {
                        string value = "0";
                        if (para.StorageType == StorageType.Integer)
                        {
                            if (ParameterType.YesNo == para.Definition.ParameterType)
                            {
                                if (para.AsInteger() == 0)
                                {
                                    value = "False";
                                }
                                else
                                {
                                    value = "True";
                                }
                            }
                            else
                            {
                                value = para.AsInteger().ToString();
                            }
                        }
                        my_parameters.Add(new data_parameters()
                        {
                            ten_parameter = para.Definition.Name,
                            gia_tri_parameter = value,
                            group_parameter = "Visibility (Example: True or False)",
                            parameter = para,
                        });
                    }
                }

                thong_tin_kich_thuoc.ItemsSource = my_parameters;

                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_kich_thuoc.ItemsSource) as ListCollectionView;
                PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("group_parameter");
                view.GroupDescriptions.Add(groupDescription1);
                view.CustomSort = new sort_data_parameters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public static string get_value(Parameter para, string unit_length)
        {
            string value = "0";
            try
            {
                if (para.StorageType == StorageType.Integer)
                {
                    if (ParameterType.YesNo == para.Definition.ParameterType)
                    {
                        if (para.AsInteger() == 0)
                        {
                            value = "False";
                        }
                        else
                        {
                            value = "True";
                        }
                    }
                    else
                    {
                        value = para.AsInteger().ToString();
                    }
                }
                else
                {
                    if (para.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                    {
                        value = Math.Round(para.AsDouble() * Source.units_convert.First(x => x.name == "deg").value, 0).ToString();
                    }
                    else
                    {
                        value = Math.Round(para.AsDouble() * Source.units_document.First(x => x.name == unit_length).value, 0).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + para.Definition.Name);
            }
            return value;
        }
    }
}
