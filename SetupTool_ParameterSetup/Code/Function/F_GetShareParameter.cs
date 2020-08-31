using Autodesk.Revit.DB;
using SetupTool_ParameterSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupTool_ParameterSetup.Code.Function
{
    class F_GetShareParameter
    {//----------------------------------------------------------
        public static void get_share_parameter(Document doc, ObservableCollection<data_parameter> my_data_parameter_need, ObservableCollection<data_parameter> my_data_parameter_current)
        {
            try
            {
                if (File.Exists(Source.path_share_parameter_default))
                {
                    List<string> du_lieu = File.ReadAllLines(Source.path_share_parameter_default).ToList();
                    BindingMap map = doc.ParameterBindings;
                    DefinitionBindingMapIterator it = map.ForwardIterator();
                    while (it.MoveNext())
                    {
                        Definition def = it.Key;
                        my_data_parameter_current.Add(new data_parameter()
                        {
                            ten_parameter = def.Name,
                            color = du_lieu.Any(x => x == def.Name) == false ? Source.color_error : Source.color
                        });
                    }



                    foreach (string data in du_lieu)
                    {
                        my_data_parameter_need.Add(new data_parameter()
                        {
                            ten_parameter = data,
                            color = my_data_parameter_current.Any(x => x.ten_parameter == data) ? Source.color : Source.color_error
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
