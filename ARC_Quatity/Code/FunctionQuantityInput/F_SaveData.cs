using ARC_Quatity.Data;
using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;

namespace ARC_Quatity.Code.FunctionQuantityInput
{
    class F_SaveData
    {
        public static bool save_data_input(UIApplication uiapp, Document doc, TextBox input_block, TextBox input_level, ComboBox input_material, TextBox input_id, TextBox input_quantity, 
            TextBox input_unit, string id_file,List<data_quantity> my_quantity_input, bool add, string user)
        {
            bool update = true;
            try
            {
                List<List<string>> data = new List<List<string>>();
                if (my_quantity_input.Count() > 0)
                {
                    my_quantity_input.ForEach(x => data.Add(new List<string>()
                    {
                        id_file,
                        x.block,
                        x.level,
                        x.ten_vat_lieu,
                        x.ma_cong_tac,
                        x.quantity.ToString(),
                        x.don_vi,
                        user,
                        DateTime.Now.ToString(),
                        "Input"
                    }));
                }
                if (add)
                {
                    if (my_quantity_input.Any(x => x.block == input_block.Text && x.level == input_level.Text && x.ten_vat_lieu == ((data_material)input_material.SelectedItem).name && x.ma_cong_tac == input_id.Text
                     && x.don_vi == input_unit.Text) == false)
                    {
                        data.Add(new List<string>()
                        {
                            id_file,
                            input_block.Text,
                            input_level.Text,
                            ((data_material)input_material.SelectedItem).name,
                            input_id.Text,
                            input_quantity.Text,
                            input_unit.Text,
                            user,
                            DateTime.Now.ToString(),
                            "Input"
                        });
                    }
                    else
                    {
                        MessageBox.Show("Data is exist!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        update = false;
                    }
                }

                if(update)
                {
                    int column_material = 10;
                    DataTable dt1 = new DataTable();
                    for (int i = 0; i < column_material; i++)
                    {
                        dt1.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                    }
                    foreach (List<string> value in data)
                    {
                        dt1.Rows.Add(dt1.NewRow().ItemArray = value.ToArray());
                    }
                    List<string> Para2 = new List<string>() { "@DBProjectNumber", "@DBIdFile", "@DBInput", "@DBTable_Upload" };
                    List<object> Para2_Values = new List<object>() { doc.ProjectInformation.Number, id_file, "Input", dt1 };
                    SQL.SQLWrite(Source.path_revit, "dbo.sp_add_update_delete_quantity_by_idfile", Source.type_Procedure, Para2, Para2_Values);
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return update;
        }
    }
}
