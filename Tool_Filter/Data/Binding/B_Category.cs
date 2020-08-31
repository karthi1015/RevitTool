using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Filter.Data.Binding
{
    class data_category
    {
        public string category_name { get; set; }
        public List<Element> elements { get; set; }

        public bool check { get; set; }
        public double count { get; set; }
    }
}
