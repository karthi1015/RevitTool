using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Draw_Opening
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

        public string user = @"C:\Users\" + Environment.UserName;

        public int lam_tron = 6;

        public List<BuiltInCategory> my_BuiltInCategory = new List<BuiltInCategory>()
        {
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Windows
        };

        public List<string> my_select_filter = new List<string>()
        {
            "Generic Models",
            "Doors",
            "Windows"
        };

        public List<string> my_select_host_filter = new List<string>()
        {
            "Floors",
            "Walls"
        };

        public List<string> para_name = new List<string>()
        {
            "BRH",
            "Fußbodenaufbau",
            "UK",
            "MA",
            "UKSturz"
        };

        public List<string> para_name_not_use = new List<string>()
        {
            "Volume",
            "Area",
            "Rough Height",
            "Length"
        };
    }
}
