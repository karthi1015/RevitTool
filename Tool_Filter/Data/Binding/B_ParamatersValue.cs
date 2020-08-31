using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Filter.Data.Binding
{
    class data_parameters_value
    {
        public string parameter_value { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }

        public string parameter_name { get; set; }
    }
}
