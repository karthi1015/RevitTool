using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_CheckUpdateModel.Data.Binding
{
    class data_category
    {
        public string name { get; set; }
        public BuiltInCategory code { get; set; }
        public string type { get; set; }
    }
}
