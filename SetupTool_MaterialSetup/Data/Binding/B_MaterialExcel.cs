using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace SetupTool_MaterialSetup.Data.Binding
{
    class data_material_excel
    {
        public string ma_cong_tac_excel { get; set; }

        public string ten_vat_lieu_excel { get; set; }

        public string don_vi_excel { get; set; }

        public Color mau_vat_lieu_excel { get; set; }

        public int do_trong_suot_vat_lieu_excel { get; set; }

        public ElementId id_surface_excel { get; set; }

        public Color mau_surface_excel { get; set; }

        public ElementId id_cut_excel { get; set; }

        public Color mau_cut_excel { get; set; }

        public string ton { get; set; }
    }
}
