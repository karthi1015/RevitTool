using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_MaterialSetup.Data.Binding
{
    class data_material_factor
    {
        public string ton { get; set; }
        public List<Material> list_vat_lieu { get; set; }
        public int count { get; set; }
        public Material vat_lieu { get; set; }
    }
}
