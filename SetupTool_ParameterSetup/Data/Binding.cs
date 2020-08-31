using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace SetupTool_ParameterSetup
{
    public class Category_Group
    {
        public string ten_category { get; set; }

        public BuiltInCategory category { get; set; }
    }
}
