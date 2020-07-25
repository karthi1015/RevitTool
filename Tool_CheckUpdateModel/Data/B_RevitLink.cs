using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_CheckUpdateModel.Data
{
    class data_revit_link
    {
        public RevitLinkType type { get; set; }
        public string name { get; set; }
        public Document document { get; set; }
    }
}
