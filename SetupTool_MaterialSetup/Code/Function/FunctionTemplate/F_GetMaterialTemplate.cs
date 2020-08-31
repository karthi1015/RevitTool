using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupTool_MaterialSetup.Code.Function.FunctionTemplate
{
    class F_GetMaterialTemplate
    {
        public static void get_material(ObservableCollection<data_material_template> my_material_template)
        {
            try
            {
                var listtotal = SQL.SQLRead(Source.path_Quantity, "dbo.spRead_MCT", Source.type_Procedure, new List<string>(), new List<string>());
                for (var i = 0; i < listtotal.Rows.Count; i++)
                {
                    my_material_template.Add(new data_material_template()
                    {
                        ma_cong_tac_template_MCT = listtotal.Rows[i]["WorkId"].ToString(),
                        ten_vat_lieu_template_MCT = listtotal.Rows[i]["Description"].ToString(),
                        don_vi_template_MCT = listtotal.Rows[i]["Unit"].ToString(),
                        ma_cong_tac_template_MCT_DM = listtotal.Rows[i]["SubId"].ToString()
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
