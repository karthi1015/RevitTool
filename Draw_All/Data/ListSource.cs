using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Draw_All
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

        public List<string> para_name_not_use = new List<string>()
        {
            "Volume",
            "Area",
            "Rough Height"
        };

        public ObservableCollection<location_line_data> location_line_data = new ObservableCollection<location_line_data>()
        {
            new location_line_data(){ single_value = "Center", value = 0},
            new location_line_data(){ single_value = "Not Center", value = 1},
        };

        public List<string> list_option_selection_name = new List<string>()
        {
            "Center",
            "Line",
            "Arc",
            "Ellipse",
            "Pick - Line",
            "Pick - Polyline",
            "Pick - Curve",
        };
    }
}
