using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARC_Quatity.Data.Binding
{
    class data_file_link
    {
        public string ten_file { get; set; }
        public bool chon_file_link { get; set; }
        public double elevation { get; set; }
        public RevitLinkType link_file { get; set; }
    }
}
