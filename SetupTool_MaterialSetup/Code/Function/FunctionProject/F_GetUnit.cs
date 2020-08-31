using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_GetUnit
    {
        public static void get_unit_company(ComboBox don_vi)
        {
            try
            {
                List<string> combobox_don_vi = new List<string>();
                combobox_don_vi.Add("kg");

                var listtotal = SQL.SQLRead(Source.path_Quantity, "dbo.sp_read_unit_from_UserMaterial", Source.type_Procedure, new List<string>(), new List<string>());
                int rows = listtotal.Rows.Count;
                for (var i = 0; i < rows; i++)
                {
                    combobox_don_vi.Add(listtotal.Rows[i]["Unit"].ToString());
                }
                combobox_don_vi.Sort();
                combobox_don_vi.Insert(0, "Default");
                don_vi.ItemsSource = combobox_don_vi;
                don_vi.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
