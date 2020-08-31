using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_TypeSetup.Data.Binding
{
    class data_parameters
    {
        public string ten_parameter { get; set; }

        public string gia_tri_parameter { get; set; }

        public string group_parameter { get; set; }

        public Parameter parameter { get; set; }

        public CompoundStructureLayer layer { get; set; }

        public CompoundStructure compound { get; set; }
    }
}
