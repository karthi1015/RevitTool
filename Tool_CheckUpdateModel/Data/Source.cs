using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Tool_CheckUpdateModel.Data.Binding;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Tool_CheckUpdateModel.Data
{
    class Source
    {
        public static List<string> MCT = new List<string>() {   "MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E" };
        public static List<string> DV = new List<string>() {    "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
        public static List<string> TON = new List<string>() {   "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
        public static List<string> USER = new List<string>() {  "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
        public static List<string> TIME = new List<string>() {  "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };

        public static List<string> state_list = new List<string>() { "Change", "Ignore" };

        public static List<data_category> Category_Check = new List<data_category>()
        {
            new data_category(){ name = "All Category"},
            new data_category(){ name = "Structural Columns"   , code = BuiltInCategory.OST_StructuralColumns  , type = "symbol"},
            new data_category(){ name = "Structural Framing"   , code = BuiltInCategory.OST_StructuralFraming  , type = "symbol"},
            new data_category(){ name = "Floors"               , code = BuiltInCategory.OST_Floors             , type = "type"},
            new data_category(){ name = "Walls"                , code = BuiltInCategory.OST_Walls              , type = "type"},
            new data_category(){ name = "Windows"              , code = BuiltInCategory.OST_Windows            , type = "host" },
            new data_category(){ name = "Doors"                , code = BuiltInCategory.OST_Doors              , type = "host" },
            new data_category(){ name = "Generic Models"        , code = BuiltInCategory.OST_GenericModel       , type = "host" },
        };

        public static string Path = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate";
        public static string Path_Options = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate\\Option";
        public static string Path_Excel = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\CheckUpdate\\Source_Excel_Default";

        public static Brush color_used_change = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00FFDC"));
        public static Brush color_not_change = Brushes.White;
    }
}
