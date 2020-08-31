using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Filter.Data.Binding
{
    class data_parameters
    {
        public string parameter_name { get; set; }
        public List<Element> elements { get; set; }

        public Parameter para { get; set; }
        public CompoundStructure parameter_compound { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }
}
