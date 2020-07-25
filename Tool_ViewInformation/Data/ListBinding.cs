using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Tool_ViewInformation
{
    public class Quantity
    {
        public string block { get; set; }
        public string level { get; set; }
        public string ten_cau_kien { get; set; }
        public string id_cau_kien { get; set; }
        public string ten_vat_lieu { get; set; }
        public string ma_cong_tac { get; set; }
        public double quantity { get; set; }
        public string don_vi { get; set; }
        public Brush color { get; set; }
        public string color_sort { get; set; }

        public Element cau_kien { get; set; }
        public Material_Paint_Or_NoPaint vat_lieu { get; set; }

        public double elevation { get; set; }
        public string link_file_name { get; set; }
    }

    public class Material_Paint_Or_NoPaint
    {
        public Material vat_lieu { get; set; }
        public bool paint_or_nopaint { get; set; }
    }
}
