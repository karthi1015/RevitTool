using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Draw_Opening
{
    
    public class level_data
    {
        public Level level { get; set; }
        public string single_value { get; set; }
        public double elevation { get; set; }
    }

    public class family_data
    {
        public string single_value { get; set; }
        public string path_image { get; set; }
        public List<ElementType> types { get; set; }
    }

    public class type_data
    {
        public ElementType type { get; set; }
        public string single_value { get; set; }
    }

    public class parameter_data
    {
        public Parameter parameter { get; set; }
        public string parameter_name { get; set; }
        public string parameter_value { get; set; }
    }

    public class multi_or_cad_data
    {
        public string data { get; set; }
        public List<parameter_data> parameter { get; set; }
    }
}
