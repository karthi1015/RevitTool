using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Allplan_ParameterSupport
{
    public class Data
    {
        public string single_value { get; set; }

        public string level_cau_kien { get; set; }

        public string ten_cau_kien { get; set; }

        public string id_cau_kien { get; set; }

        public Element cau_kien { get; set; }

        public Brush color { get; set; }

        public string color_sort { get; set; }
    }


}
