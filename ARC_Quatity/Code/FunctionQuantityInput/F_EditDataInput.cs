using ARC_Quatity.Data.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ARC_Quatity.Code.FunctionQuantityInput
{
    class F_EditDataInput
    {
        public static void edit_data_input(string id_file, ListView thong_tin_quantity_input)
        {
            try
            {
                data_quantity item = (data_quantity)thong_tin_quantity_input.SelectedItem;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
