using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UI_Ribbon
{
    class ListSource
    {

        public string type_Procedure = "StoredProcedure";
        public string type_Query = "Query";
        public string type_element = "element_type";
        public string type_symbol = "symbol_type";
        public string Duplicate = "Duplicate";
        public string Update = "Update";
        public string error = "ERROR";

        public bool paint = true;
        public bool no_paint = false;
    }
}
