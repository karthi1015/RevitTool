using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Tool_CheckUpdateModel.Code.Function.Type;
using Tool_CheckUpdateModel.Data;
using Tool_CheckUpdateModel.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;
using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;

namespace Tool_CheckUpdateModel.Function.Controls
{
    class F_ListBox
    {
        public static void Get_Parameter(ObservableCollection<Parameter_Settings> my_parameter_settings, Document doc, ListBox parameter_current)
        {

            try
            {
                List<string> paras = new List<string>();
                if (File.Exists(Source.Path_Options))
                {
                    paras = File.ReadAllLines(Source.Path_Options).ToList();
                }
                foreach (data_category data in Source.Category_Check)
                {
                    if (data.name != Source.Category_Check[0].name)
                    {
                        List<Element> list = new FilteredElementCollector(doc).OfCategory(data.code).WhereElementIsNotElementType().ToList();
                        if (list.Count() > 0)
                        {
                            Element ele = new FilteredElementCollector(doc).OfCategory(data.code).WhereElementIsNotElementType().First();
                            foreach (Parameter parameter in ele.Parameters)
                            {
                                if (parameter.Definition.Name == "Volume" ||
                                    parameter.Definition.Name == "Type" ||
                                    (my_parameter_settings.Any(x => x.parameter_name == parameter.Definition.Name && x.parameter_category == data.code) == false &&
                                    (parameter.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY || parameter.Definition.ParameterGroup == BuiltInParameterGroup.PG_CONSTRAINTS || parameter.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY_POSITIONING)))
                                {
                                    bool ischeck = true;
                                    if (paras.Count() > 0)
                                    {
                                        if (paras.Any(x => x.Split('\t')[0] == data.name && x.Split('\t')[1] == parameter.Definition.Name) == true) ischeck = true;
                                        else ischeck = false;
                                    }
                                    my_parameter_settings.Add(new Parameter_Settings()
                                    {
                                        isCheck = ischeck,
                                        parameter = parameter,
                                        parameter_name = parameter.Definition.Name,
                                        parameter_group = parameter.Definition.ParameterGroup.ToString(),
                                        parameter_category = data.code,
                                        parameter_category_name = data.name
                                    });
                                }
                            }
                        }
                    }
                }
                parameter_current.ItemsSource = my_parameter_settings;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        public static void save_settings(ObservableCollection<Parameter_Settings> my_Parameter_settings)
        {
            try
            {
                List<string> Parameters = new List<string>();
                foreach (Parameter_Settings Parameter in my_Parameter_settings)
                {
                    if (Parameter.isCheck)
                    {
                        Parameters.Add(Parameter.parameter_category_name + "\t" + Parameter.parameter_name);
                    }
                }
                if (!Directory.Exists(Source.Path)) Directory.CreateDirectory(Source.Path);
                if (!File.Exists(Source.Path_Options))
                {
                    File.WriteAllLines(Source.Path_Options, Parameters);
                }
                else
                {
                    var data_old = File.ReadAllLines(Source.Path_Options).ToList();
                    foreach (string old in data_old)
                    {
                        if (Parameters.Any(x => x == old) == false)
                        {
                            Parameters.Add(old);
                        }
                    }
                    File.WriteAllLines(Source.Path_Options, Parameters);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        public static void all_parameter(ToggleButton all_Parameter, ObservableCollection<Parameter_Settings> my_Parameter_settings)
        {
            try
            {
                if (all_Parameter.IsChecked == true)
                {
                    foreach (Parameter_Settings Parameter in my_Parameter_settings)
                    {
                        Parameter.isCheck = true;
                    }
                }
                else
                {
                    List<string> Parameters = new List<string>();
                    if (File.Exists(Source.Path_Options))
                    {
                        Parameters = File.ReadAllLines(Source.Path_Options).ToList();
                    }
                    if (Parameters.Count() > 0)
                    {
                        foreach (Parameter_Settings Parameter in my_Parameter_settings)
                        {
                            if (Parameters.Any(x => x.Split('\t')[0] == Parameter.parameter_category_name && x.Split('\t')[1] == Parameter.parameter_name) == true) Parameter.isCheck = true;
                            else Parameter.isCheck = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        public static void Save_Data_Check(ObservableCollection<Element_Change> element_Changes, ComboBox link_file, UIApplication uiapp, Document doc_link)
        {
            try
            {
                List<string> list_data = new List<string>();
                foreach (Element_Change element in element_Changes)
                {
                    list_data.Add(element.element_id + "\t" + element.changeORignore + "\t" + element.color);
                }
                if (File.Exists(doc_link.PathName.Split('.')[0]))
                {
                    string direction = Path.GetDirectoryName(doc_link.PathName) + "\\Backup";
                    if (!Directory.Exists(direction)) Directory.CreateDirectory(direction);

                    File.Move(doc_link.PathName.Split('.')[0], direction + "\\" + Path.GetFileName(doc_link.PathName.Split('.')[0]) + "_" + DateTime.Now.ToString("ddMMyyHHmmss"));
                }

                File.WriteAllLines(doc_link.PathName.Split('.')[0], list_data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //-------------------------------------------------------
        public static void Roll_Back_Save_Data_Check(ObservableCollection<Element_Change> element_Changes, ComboBox link_file, UIApplication uiapp
            , Document doc_link, Document doc, ObservableCollection<Parameter_Settings> my_parameter_settings)
        {
            try
            {
                string direction = Path.GetDirectoryName(doc_link.PathName) + "\\Backup";
                if (Directory.Exists(direction))
                {
                    File.Delete(doc_link.PathName.Split('.')[0]);
                    if (new DirectoryInfo(direction).GetFiles().Length == 0)
                    {
                        Directory.Delete(direction);
                    }
                    else
                    {
                        File.Move(new DirectoryInfo(direction).GetFiles().OrderByDescending(o => o.LastWriteTime).FirstOrDefault().FullName, doc_link.PathName.Split('.')[0]);
                    }
                }
                else
                {
                    File.Delete(doc_link.PathName.Split('.')[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
