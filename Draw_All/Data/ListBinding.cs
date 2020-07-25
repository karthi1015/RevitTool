using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Draw_All
{
    
    public class level_data
    {
        public Level level { get; set; }
        public string single_value { get; set; }
        public double elevation { get; set; }
    }

    public class category_data
    {
        public string single_value { get; set; }
        public BuiltInCategory builtin { get; set; }
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

    public class view_section_list_data
    {
        public ViewSection view_section { get; set; }
        public string sheet_number { get; set; }
    }

    public class multi_or_cad_data
    {
        public string data { get; set; }
        public List<parameter_data> parameter { get; set; }
    }

    public class location_line_data
    {
        public string single_value { get; set; }
        public int value { get; set; }
    }

    public class geometry_data
    {
        public string name { get; set; }
        public List<XYZ> list_point { get; set; }

        public PolyLine polyLine { get; set; }

        public Line line { get; set; }

        public Arc arc { get; set; }

        public Ellipse ellipse { get; set; }
    }

    public class draw_data
    {
        public XYZ center { get; set; }
        public XYZ point1 { get; set; }
        public XYZ point2 { get; set; }
        public Curve curve { get; set; }
    }

    public class material_data
    {
        public string single_value { get; set; }
        public Material material { get; set; }
    }

    public class view_plan_name_data
    {
        public string single_value { get; set; }
        public ViewPlan view_plan { get; set; }
    }
}
