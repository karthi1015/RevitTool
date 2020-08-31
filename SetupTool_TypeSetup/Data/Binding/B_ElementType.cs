using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_TypeSetup.Data.Binding
{
    class data_type
    {
        public string ten_element_type { get; set; }

        public ElementType element_type { get; set; }

        public string type_type { get; set; }

        public bool delete_type { get; set; }

        public bool ValueIsSelect { get; set; }
    }
}
