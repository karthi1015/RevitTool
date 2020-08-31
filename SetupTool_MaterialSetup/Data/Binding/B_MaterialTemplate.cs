using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_MaterialSetup.Data.Binding
{
    class B_MaterialTemplate
    {
    }
    public class data_material_template // macongtac_tenvatlieu_tenvatlieukhongdau_donvi_macongtacDM
    {
        public string ma_cong_tac_template_MCT { get; set; }

        public string ten_vat_lieu_template_MCT { get; set; }

        public string don_vi_template_MCT { get; set; }

        public string ma_cong_tac_template_MCT_DM { get; set; }
    }

    public class Material_DM_Template // madotim_stt_macongtacMCT_mavatlieu_soluong
    {
        public string ma_cong_tac_template_MCT_DM { get; set; }

        public int so_thu_tu_DM { get; set; }

        public string ma_cong_tac_template_DM_VL { get; set; }

        public string so_luong_template_DM { get; set; }
    }

    public class Material_VL_Template // mavatlieuVL_tenvatlieu_donvi
    {
        public string ma_cong_tac_template_DM_VL { get; set; }

        public string ten_vat_lieu_template_VL { get; set; }

        public string don_vi_template_VL { get; set; }
    }

    public class Material_Sup_Template
    {
        public int no_sup_template { get; set; }

        public string ten_vat_lieu_sup_template { get; set; }

        public string so_luong_sup_template { get; set; }

        public string don_vi_sup_template { get; set; }
    }
}
