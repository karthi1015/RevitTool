using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SetupTool_MaterialSetup.Code.Function.FunctionCompany
{
    class F_DeleteMaterialCompany
    {
        //----------------------------------------------------------
        public static string delete_material(ListView thong_tin_vat_lieu_company, ObservableCollection<data_material_company> my_material_company)
        {
            string result = "F";
            try
            {
                data_material_company item = (data_material_company)thong_tin_vat_lieu_company.SelectedItem;
                var result_message = MessageBox.Show("Are you sure! This process cannot be undone.", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result_message == MessageBoxResult.Yes)
                {
                    List<string> Para1 = new List<string>() { "@WorkId" };
                    List<string> Para1_Values = new List<string>() { item.ma_cong_tac_company };
                    SQL.SQLDelete(Source.path_Quantity, "dbo.spDelete_UserMaterial", Source.type_Procedure, Para1, Para1_Values);

                    my_material_company.Remove(item);
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = "F";
            }
            return result;
        }
    }
}
