using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Filter.Data.Binding
{
    class data_element
    {
        public Element element { get; set; }

        public string category_name { get; set; }

        public string family_name { get; set; }

        public string type_name { get; set; }

        public string parameter_name { get; set; }

        public string parameter_value { get; set; }

        public Parameter para { get; set; }

        public CompoundStructure parameter_compound { get; set; }

        public List<Element> type_elements { get; set; }
        public List<ElementId> ids { get; set; }
    }
}
