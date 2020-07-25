using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Tool_Filter
{
    public class element_information
    {
        public Element element { get; set; }
        public string category_name { get; set; }
        public string family_name { get; set; }
        public string type_name { get; set; }

        public List<parameter_information> parameters_type { get; set; }

        public List<parameter_information> parameters { get; set; }
    }

    public class element_information_category
    {
        public string category_name { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }

    public class element_information_family
    {
        public string family_name { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }

    public class element_information_type
    {
        public string type_name { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }

    public class element_information_parameter
    {
        public string parameter_name { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }

    public class element_information_parameter_value
    {
        public string parameter_value { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }

    public class parameter_information
    {
        public Parameter parameter { get; set; }
        public string parameter_name { get; set; }
        public string parameter_value { get; set; }
        public string value { get; set; }

        public Element element { get; set; }
    }

    public class material_data
    {
        public Material material { get; set; }
        public string material_name { get; set; }
        public string material_key { get; set; }
    }
}
