using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SetupTool_MaterialSetup.Data.Binding
{
    class data_fill_pattern
    {
        public string name { get; set; }
        public BitmapImage image { get; set; }
        public ElementId pattern_id { get; set; }
    }
}
