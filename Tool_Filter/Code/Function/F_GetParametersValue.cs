using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tool_Filter.Data.Binding;

namespace Tool_Filter.Code.Function
{
    class F_GetParametersValue
    {
        //----------------------------------------------------------
        public static void distint_parameter_values(Document doc, ObservableCollection<data_parameters> my_parameters, ListView thong_tin_parameter,
            ObservableCollection<data_parameters_value> my_parameters_value, ListView gia_tri_parameter)
        {
            try
            {
                List<string> value_list = new List<string>();
                try
                {
                    value_list = CollectionViewSource.GetDefaultView(gia_tri_parameter.ItemsSource).Cast<data_parameters_value>().Where(x => x.check == true).Select(y => y.parameter_value).ToList();
                }
                catch (Exception)
                {

                }
                

                List<data_element> my_element = new List<data_element>();
                if (my_parameters.Count() > 0)
                {
                    var data = CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Cast<data_parameters>().Where(x => x.check == true).ToList();
                    for(int i = 0; i < data.Count(); i++)
                    {
                        foreach(Element element in data[i].elements)
                        {
                            if(data[i].parameter_compound != null)
                            {
                                get_data_compound(doc, data[i].parameter_compound, my_element, element, data[i].parameter_name);
                            }
                            else
                            {
                                my_element.Add(new data_element()
                                {
                                    parameter_value = data[i].parameter_name.Contains("Type : ") ? 
                                    Support.Get_Parameter_Information(doc.GetElement(element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()).LookupParameter(data[i].para.Definition.Name), doc) : 
                                    Support.Get_Parameter_Information(element.LookupParameter(data[i].para.Definition.Name), doc),
                                    element = element,
                                    parameter_name = data[i].parameter_name
                                });
                            }
                        }
                    }
                }

                my_element.GroupBy(x => new
                {
                    x.parameter_value
                }).Select(y => new data_parameters_value()
                {
                    parameter_value = y.Key.parameter_value,
                    elements = y.Select(z => z.element).ToList(),
                    parameter_name = y.Select(z => z.parameter_name).First(),

                    check = false,
                    count = y.Count()
                }).ToList().ForEach(x => my_parameters_value.Add(x));

                my_parameters_value.Where(x => value_list.Contains(x.parameter_value)).ToList().ForEach(y => y.check = true);

                gia_tri_parameter.ItemsSource = my_parameters_value;
                ListCollectionView view = CollectionViewSource.GetDefaultView(gia_tri_parameter.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_parameters_value();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public static void get_data_compound(Document doc, CompoundStructure compound, List<data_element> my_element, Element element, string parameter_name)
        {
            try
            {
                IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                foreach (var layer in getlayer)
                {
                    if (layer.MaterialId.IntegerValue != -1)
                    {
                        my_element.Add(new data_element()
                        {
                            parameter_value = doc.GetElement(layer.MaterialId).Name,
                            element = element,
                            parameter_name = parameter_name
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
