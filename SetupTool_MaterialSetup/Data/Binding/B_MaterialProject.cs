using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace SetupTool_MaterialSetup.Data.Binding
{
    class data_material_project
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
}
