using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARC_Quatity.Data.Binding
{
    class data_material
    {
        public Material material { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public string unit { get; set; }
    }
}
