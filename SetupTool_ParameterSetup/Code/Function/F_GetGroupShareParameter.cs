using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SetupTool_ParameterSetup.Data.Binding;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace SetupTool_ParameterSetup.Code.Function
{
    class F_GetGroupShareParameter
    {//----------------------------------------------------------
        public static void get_share_parameter(ObservableCollection<data_group_share_parameter> my_group_share_parameter, ObservableCollection<data_item_share_parameter> my_item_share_parameter,
            ObservableCollection<data_parameter> my_data_parameter_current)
        {
            try
            {
                List<string> share_parameter_Da_Ton_Tai = my_data_parameter_current.Select(x => x.ten_parameter).ToList();

                List<string> du_lieu = new List<string>();
                if (File.Exists(Source.path_share_parameter))
                {
                    du_lieu = File.ReadAllLines(Source.path_share_parameter).ToList();
                }

                foreach (string data in du_lieu)
                {
                    var line = data.Split('\t');
                    if (line[0] == "GROUP")
                    {
                        my_item_share_parameter = new ObservableCollection<data_item_share_parameter>();
                        bool expander = false;
                        int count_check = 0;
                        Brush color = Source.color;
                        foreach (string data_para in du_lieu)
                        {
                            var line_para = data_para.Split('\t');
                            if (line_para[0] == "PARAM")
                            {
                                if (line_para[5] == line[1])
                                {
                                    bool check = false;
                                    if (share_parameter_Da_Ton_Tai.Contains(line_para[2])) check = true;

                                    my_item_share_parameter.Add(new data_item_share_parameter()
                                    {
                                        type = line_para[0],
                                        guid_parameter = line_para[1],
                                        ten_parameter = line_para[2],
                                        type_parameter = line_para[3],
                                        category_parameter = line_para[4],
                                        group_parameter = line_para[5],
                                        visible_parameter = line_para[6],
                                        description_parameter = line_para[7],
                                        user_modify_parameter = line_para[8],
                                        exist_parameter = check,
                                        ValueIsSelect = false,
                                    });
                                    if (check == true)
                                    {
                                        color = Source.color_group;
                                        expander = true;
                                        count_check++;
                                    }
                                }
                            }
                        }
                        var a = my_item_share_parameter.OrderBy(x => x.ten_parameter).ToList();
                        my_item_share_parameter.Clear();
                        foreach (var b in a)
                        {
                            my_item_share_parameter.Add(b);
                        }
                        my_group_share_parameter.Add(new data_group_share_parameter()
                        {
                            type = line[0],
                            id_group_parameter = line[1],
                            ten_group_parameter = line[2],
                            ValueExpanded = expander,
                            Children = my_item_share_parameter,
                            color = color,
                            count_check = count_check.ToString()
                        });
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
