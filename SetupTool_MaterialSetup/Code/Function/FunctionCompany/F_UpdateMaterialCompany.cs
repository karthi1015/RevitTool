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
    class F_UpdateMaterialCompany
    {
        //----------------------------------------------------------
        public static string update_material(ListView thong_tin_vat_lieu_company, ObservableCollection<data_material_company> my_material_company,
            TextBox ten_vat_lieu, TextBox ma_cong_tac, ComboBox don_vi, string user)
        {
            string result = "F";
            try
            {
                data_material_company item = (data_material_company)thong_tin_vat_lieu_company.SelectedItem;
                if (my_material_company.Any(x => x.ma_cong_tac_company == ma_cong_tac.Text) == false)
                {
                    MessageBox.Show("Not found material id!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                    {
                        if (!string.IsNullOrEmpty(don_vi.Text))
                        {
                            var result_message = MessageBox.Show("Are you sure! This process cannot be undone", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (result_message == MessageBoxResult.Yes)
                            {
                                List<string> Para1 = new List<string>() { "@WorkId" };
                                List<string> Para1_Values = new List<string>() { ma_cong_tac.Text };
                                SQL.SQLDelete(Source.path_Quantity, "dbo.spDelete_UserMaterial", Source.type_Procedure, Para1, Para1_Values);

                                my_material_company.Remove(item);
                                string time = DateTime.Now.ToString();
                                List<string> Para = new List<string>()
                                        {
                                            "@WorkId",
                                            "@MaterialName",
                                            "@MaterialName_ForSearch",
                                            "@CreatedBy",
                                            "@Time",
                                            "@Unit"
                                        };
                                List<string> Para_Values = new List<string>()
                                        {
                                            ma_cong_tac.Text,
                                            ten_vat_lieu.Text,
                                            Support.RemoveUnicode(ten_vat_lieu.Text),
                                            user,
                                            time,
                                            don_vi.Text
                                        };
                                SQL.SQLWrite(Source.path_Quantity, "dbo.spInsert_UserMaterial", Source.type_Procedure, Para, Para_Values);

                                my_material_company.Add(new data_material_company()
                                {
                                    ma_cong_tac_company = ma_cong_tac.Text,
                                    ten_vat_lieu_company = ten_vat_lieu.Text,
                                    user = user,
                                    time = time,
                                    don_vi_company = don_vi.Text
                                });
                                result = "S";
                            }
                        }
                        else
                        {
                            MessageBox.Show("The unit not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The material name not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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
