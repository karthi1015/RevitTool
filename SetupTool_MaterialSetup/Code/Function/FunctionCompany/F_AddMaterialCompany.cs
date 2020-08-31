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
    class F_AddMaterialCompany
    {//----------------------------------------------------------
        public static string add_material(ListView thong_tin_vat_lieu_company, ObservableCollection<data_material_company> my_material_company,
            TextBox ten_vat_lieu, TextBox ma_cong_tac, ComboBox don_vi, string user)
        {
            string result = "F";
            try
            {
                if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                {
                    if (my_material_company.Any(x => x.ten_vat_lieu_company == ten_vat_lieu.Text) == false)
                    {
                        if (!string.IsNullOrEmpty(ma_cong_tac.Text))
                        {
                            if (my_material_company.Any(x => x.ma_cong_tac_company == ma_cong_tac.Text) == false)
                            {
                                string time = DateTime.Now.ToString();
                                if (!string.IsNullOrEmpty(don_vi.Text))
                                {
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
                                else
                                {
                                    MessageBox.Show("The unit not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("The material id you supplied is already in use. Enter a unique id.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The material id not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The material name you supplied is already in use. Enter a unique name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The material name not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
