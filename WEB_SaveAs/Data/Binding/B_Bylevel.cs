using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace WEB_SaveAs.Binding
{
    class data_by_level
    {
        public string number { get; set; }

        public string block { get; set; }

        public string level { get; set; }

        public string level_file { get; set; }

        public string descipline { get; set; }

        public double elevation { get; set; }

        public List<Element> elements { get; set; }
    }
}
