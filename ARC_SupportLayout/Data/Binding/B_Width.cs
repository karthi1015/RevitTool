using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARC_SupportLayout.Data.Binding
{
    class data_width
    {
        public ElementType type { get; set; }

        public List<ElementType> types { get; set; }

        public string width { get; set; }

        public int count { get; set; }
    }
}
