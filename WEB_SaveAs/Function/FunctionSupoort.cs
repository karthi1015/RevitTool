using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace WEB_SaveAs
{
    public class FunctionSupoort
    {
        ListSource mySource;
        FunctionSQL mySQL;
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

                    list_unit_value_data = new ObservableCollection<double>(Get_Data_List(du_lieu, "unit_value").Select(x => Convert.ToDouble(x.Split('\t')[2]))),

                    list_color_UI_data = new ObservableCollection<Brush>(Get_Data_List(du_lieu, "color_UI").Select(x =>
                    (SolidColorBrush)new BrushConverter().ConvertFromString(x.Split('\t')[2]))),

                    list_procedure_data = new ObservableCollection<list_procedure>(Get_Data_List(du_lieu, "sp_quantity").Select(x => new list_procedure()
                    {
                        procedure_name = x.Split('\t')[2],
                        procedure_para = x.Split('\t')[3]
                    })),

                    list_path_foder_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_foder").Select(x => x.Split('\t')[2])),

                    list_path_connect_SQL_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_connect_SQL").Select(x => x.Split('\t')[2])),

                    list_material_para_data = new ObservableCollection<list_material_para>(Get_Data_List(du_lieu, "material_parameter").Select(x => new list_material_para()
                    {
                        material_para_guid = x.Split('\t')[2],
                        material_para_name = x.Split('\t')[3]
                    })),

                    list_mct_descipline_data = new ObservableCollection<list_mct_descipline>(Get_Data_List(du_lieu, "mct_descipline").Select(x => new list_mct_descipline()
                    {
                        mct = x.Split('\t')[2],
                        descipline = x.Split('\t')[3]
                    })),

                    list_descipline_data = new ObservableCollection<list_descipline>(Get_Data_List(du_lieu, "list_descipline").Select(x => new list_descipline()
                    {
                        descipline_name = x.Split('\t')[2],
                        descipline_key = x.Split('\t')[3]
                    })),

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

        //----------------------------------------------------------
        public string RemoveUnicode(string text)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = text.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(text[index]);
                text = text.Replace(text[index], ReplText[index2]);
            }
            return text;
        }

        
    }
}
