using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace Allplan_ParameterSupport
{
    public class FunctionSupoort
    {
        ListSource mySource;
        //----------------------------------------------------------
        public string Get_Data_String(List<string> list, string key)
        {
            string value = "";
            try
            {
                value = list.First(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key);
            }
            catch (Exception)
            {

            }
            return value;
        }

        //----------------------------------------------------------
        public List<string> Get_Data_List(List<string> list, string key)
        {
            List<string> value = new List<string>();
            try
            {
                value = list.Where(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key).ToList();
            }
            catch (Exception)
            {

            }
            return value;
        }
        //----------------------------------------------------------
        public All_Data Get_Data_All(string path)
        {
            All_Data myAll_Data = new All_Data();
            try
            {
                List<string> du_lieu = File.ReadAllLines(path).ToList();

                myAll_Data = new All_Data()
                {
                    list_path_image_data = new ObservableCollection<list_image>(Get_Data_List(du_lieu, "path_image").Select(x => new list_image()
                    {
                        image_name = x.Split('\t')[2],
                        image_path = x.Split('\t')[3]
                    })),

                    list_color_UI_data = new ObservableCollection<Brush>(Get_Data_List(du_lieu, "color_UI").Select(x =>
                    (SolidColorBrush)new BrushConverter().ConvertFromString(x.Split('\t')[2]))),

                    list_parameter_share_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "parameter_share").Select(x => x.Split('\t')[2]))
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myAll_Data;
        }

        //----------------------------------------------------------
        public void Default_Image(All_Data myAll_Data, List<Image> list_control_image)
        {
            try
            {
                for (int i = 0; i < list_control_image.Count; i++)
                {
                    list_control_image[i].Source = new BitmapImage(new Uri(myAll_Data.list_path_image_data.First(x => x.image_name == list_control_image[i].Name).image_path));
                }
            }
            catch (Exception)
            {

            }
        }

        public void Add_Data_Listview(Element model, Element model_add, All_Data myAll_Data, ObservableCollection<Data> list_data)
        {

            try
            {
                if (model.LookupParameter(myAll_Data.list_parameter_share_data[0]) != null && model.LookupParameter(myAll_Data.list_parameter_share_data[1]) != null)
                {
                    string name = "";
                    string level = "";
                    Brush color = myAll_Data.list_color_UI_data[1];
                    if (model.LookupParameter(myAll_Data.list_parameter_share_data[0]).AsString() != null)
                    {
                        name = model.LookupParameter(myAll_Data.list_parameter_share_data[0]).AsString();
                    }
                    if (model.LookupParameter(myAll_Data.list_parameter_share_data[1]).AsString() != null)
                    {
                        level = model.LookupParameter(myAll_Data.list_parameter_share_data[1]).AsString();
                    }
                    if (model.LookupParameter(myAll_Data.list_parameter_share_data[0]).AsString() != null && model.LookupParameter(myAll_Data.list_parameter_share_data[1]).AsString() != null) color = myAll_Data.list_color_UI_data[0];
                    list_data.Add(new Data()
                    {
                        ten_cau_kien = name,
                        level_cau_kien = level,
                        id_cau_kien = model.UniqueId,
                        cau_kien = model_add,
                        color = color,
                        color_sort = color.ToString()
                    });
                }
            }
            catch (Exception ex)
            { 

            }
        }
    }
}
