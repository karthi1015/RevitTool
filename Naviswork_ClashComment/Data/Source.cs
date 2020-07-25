using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Naviswork_ClashComment
{
    class Source
    {
        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";
        public static string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";

        public static List<status_data> list_status_child = new List<status_data>() { 
            new status_data() { name = "Comments", color = Brushes.Red, isEnable = true},
            new status_data() { name = "Resolved", color = Brushes.Aqua, isEnable = true},
            new status_data() { name = "Approved", color = Brushes.Chartreuse, isEnable = false},
            new status_data() { name = "Ignore", color = Brushes.LightSlateGray, isEnable = false},
            new status_data() { name = "New", color = Brushes.White, isEnable = true}
        };
        public static List<status_data> list_status_parent = new List<status_data>() {
            new status_data() { name = "Not Finish", color = Brushes.White},
            new status_data() { name = "Finish", color = Brushes.Chartreuse}
        };
    }
}
