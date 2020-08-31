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
    class F_GetMaterialCompany
    {
        public static void get_material(ObservableCollection<data_material_company> my_material_company)
        {
            try
            {
                var listtotal = SQL.SQLRead(Source.path_Quantity, "dbo.spVW_WorkIdDesCription_UserMaterial", Source.type_Procedure, new List<string>(), new List<string>());
                for (var i = 0; i < listtotal.Rows.Count; i++)
                {
                    my_material_company.Add(new data_material_company()
                    {
                        ma_cong_tac_company = listtotal.Rows[i]["WorkId"].ToString(),
                        ten_vat_lieu_company = listtotal.Rows[i]["MaterialName"].ToString(),
                        user = listtotal.Rows[i]["CreatedBy"].ToString(),
                        time = listtotal.Rows[i]["Time"].ToString(),
                        don_vi_company = listtotal.Rows[i]["Unit"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
