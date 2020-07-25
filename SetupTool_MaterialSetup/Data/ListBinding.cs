using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace SetupTool_MaterialSetup
{
    public class Data
    {
        public string single_value { get; set; }
        public string value_image { get; set; }
        public string image { get; set; }
        public FillPatternElement fillPatternElement { get; set; }
    }

    public class Material_Project
    {
        public Material material_project { get; set; }

        public string ma_cong_tac_project { get; set; }

        public string ten_vat_lieu_project { get; set; }

        public string don_vi_project { get; set; }

        public string user { get; set; }

        public string time { get; set; }

        public Color mau_vat_lieu { get; set; }

        public int do_trong_suot_vat_lieu { get; set; }

        public ElementId id_surface { get; set; }

        public Color mau_surface { get; set; }

        public ElementId id_cut { get; set; }

        public Color mau_cut { get; set; }

        public string ton { get; set; }

        public Brush color { get; set; }

        public string color_sort { get; set; }
    }

    public class Material_Company
    {
        public string ma_cong_tac_company { get; set; }

        public string ten_vat_lieu_company { get; set; }

        public string don_vi_company { get; set; }

        public string user { get; set; }

        public string time { get; set; }

        public string ten_vat_lieu_company_for_search { get; set; }
    }

    public class Material_MCT_Template // macongtac_tenvatlieu_tenvatlieukhongdau_donvi_macongtacDM
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

    public class Material_Sub_Template
    {
        public int no_sub_template { get; set; }

        public string ten_vat_lieu_sub_template { get; set; }

        public string so_luong_sub_template { get; set; }

        public string don_vi_sub_template { get; set; }
    }

    public class Material_Excel
    {
        public string ma_cong_tac_excel { get; set; }

        public string ten_vat_lieu_excel { get; set; }

        public string don_vi_excel { get; set; }

        public Color mau_vat_lieu_excel { get; set; }

        public int do_trong_suot_vat_lieu_excel { get; set; }

        public string name_surface_excel { get; set; }

        public Color mau_surface_excel { get; set; }

        public string name_cut_excel { get; set; }

        public Color mau_cut_excel { get; set; }
    }

    public class Material_Factor
    {
        public string ton_value { get; set; }
        public List<Material> list_vat_lieu { get; set; }
        public int count { get; set; }
    }

}
