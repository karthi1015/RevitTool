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
    class F_GetParameters
    {
        //----------------------------------------------------------
        public static void distint_parameters(Document doc, ObservableCollection<data_type> my_type, ListView thong_tin_type,
            ObservableCollection<data_parameters> my_parameters, ListView thong_tin_parameter)
        {
            try
            {
                List<string> parameter_list = new List<string>();
                try
                {
                    parameter_list = CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Cast<data_parameters>().Where(x => x.check == true).Select(y => y.parameter_name).ToList();
                }
                catch (Exception)
                {

                }
                

                List<data_element> my_element = new List<data_element>();
                if (my_type.Count() > 0)
                {
                    foreach (List<Element> list in CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource).Cast<data_type>().Where(x => x.check == true).Select(x => x.elements))
                    {
                        foreach (Parameter para in list[0].Parameters)
                        {
                            my_element.Add(new data_element()
                            {
                                para = para,
                                parameter_name = "Instance : " + para.Definition.Name,
                                type_elements = list,
                                parameter_compound = null
                            });
                        }
                        ElementType type = doc.GetElement(list[0].get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;

                        if (type != null)
                        {
                            var TypeOftype = type.GetType();
                            CompoundStructure compound = null;
                            if (TypeOftype.Name == "WallType")
                            {
                                var wall = type as WallType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == "FloorType")
                            {
                                var wall = type as FloorType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == "RoofType")
                            {
                                var wall = type as RoofType;
                                compound = wall.GetCompoundStructure();
                            }
                            if (TypeOftype.Name == "CeilingType")
                            {
                                var wall = type as CeilingType;
                                compound = wall.GetCompoundStructure();
                            }

                            if (compound != null)
                            {
                                my_element.Add(new data_element()
                                {
                                    parameter_compound = compound,
                                    parameter_name = "1. Material : " + type.Name,
                                    type_elements = list,
                                    para = null
                                });
                            }

                            foreach (Parameter para in type.Parameters)
                            {
                                if (para.Definition.ParameterGroup != BuiltInParameterGroup.PG_MATERIALS)
                                {
                                    my_element.Add(new data_element()
                                    {
                                        para = para,
                                        parameter_name = "Type : " + para.Definition.Name,
                                        type_elements = list,
                                        parameter_compound = null
                                    });
                                }
                                if (compound == null)
                                {
                                    if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_MATERIALS)
                                    {
                                        my_element.Add(new data_element()
                                        {
                                            para = para,
                                            parameter_name = "2. Material Type : " + para.Definition.Name,
                                            type_elements = list,
                                            parameter_compound = null
                                        });
                                    }
                                }
                            } 
                        }
                    }
                }

                my_element.GroupBy(x => new
                {
                    x.parameter_name
                }).Select(y => new data_parameters()
                {
                    parameter_name = y.Key.parameter_name,
                    elements = y.Select(z => z.type_elements).SelectMany(x => x).ToList(),
                    parameter_compound = y.Select(z => z.parameter_compound).First(),
                    para = y.Select(z => z.para).First(),

                    check = false,
                    count = y.Select(z => z.type_elements).SelectMany(x => x).Count()
                }).ToList().ForEach(x => my_parameters.Add(x));

                my_parameters.Where(x => parameter_list.Contains(x.parameter_name)).ToList().ForEach(y => y.check = true);

                thong_tin_parameter.ItemsSource = my_parameters;
                ListCollectionView view = CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource) as ListCollectionView;
                view.CustomSort = new sort_data_parameters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
