using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tool_CheckUpdateModel.Data
{
    class Source
    {
        public static List<string> MCT = new List<string>(){"MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E"};
        public static List<string> DV = new List<string>() { "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
        public static List<string> TON = new List<string>() { "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
        public static List<string> USER = new List<string>() { "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
        public static List<string> TIME = new List<string>() { "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };

        public static List<string> state_list = new List<string>() { "Change", "Ignore" };

        public static List<BuiltInCategory> Category_Check = new List<BuiltInCategory>() 
        { 
            BuiltInCategory.OST_StructuralColumns,
            BuiltInCategory.OST_StructuralFraming,
            BuiltInCategory.OST_Floors,
            BuiltInCategory.OST_Walls
            
        };

        public static string Path = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate";
        public static string Path_Options = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate\\Option";
        public static string Path_Excel = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate\\Source_Excel_Default";

        public static Brush color_used_change = Brushes.Aqua;
        public static Brush color_not_change = Brushes.White;
    }
}
