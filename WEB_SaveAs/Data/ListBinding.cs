using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace WEB_SaveAs
{
    public class data_file
    {
        public string path { get; set; }

        public string name { get; set; }

        public string size { get; set; }

        public double elevation { get; set; }
    }

    public class element_list
    {
        public Element cau_kien { get; set; }

        public string level { get; set; }

        public double elevation { get; set; }
    }

    public class level_block
    {
        public string number { get; set; }

        public string block { get; set; }

        public string level { get; set; }

        public string descipline { get; set; }

        public double elevation { get; set; }
    }
}
