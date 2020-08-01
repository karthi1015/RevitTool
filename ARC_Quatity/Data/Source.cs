using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ARC_Quatity.Data
{
    class Source
    {
        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";
        public static string type_element = "element_type";
        public static string type_symbol = "symbol_type";
        public static string Duplicate = "Duplicate";
        public static string Update = "Update";
        public static string error = "ERROR";

        public static Brush color_error = Brushes.LightSlateGray;
        public static Brush color = Brushes.White;

        public static bool paint = true;
        public static bool no_paint = false;

        public static string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";

        public static string path_revit = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=RevitDataBase;User Id=RevitUser; Password = revit@connect";
        public static string path_WEB = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=WEBDataBase;User Id=WebUser; Password = webconnect456";

        public static List<data_descipline> desciplines = new List<data_descipline>()
        {
            new data_descipline(){name = "arc", key = "a"},
            new data_descipline(){name = "str", key = "s"},
            new data_descipline(){name = "la", key = "la"},
            new data_descipline(){name = "mep", key = "mep"},
            new data_descipline(){name = "mod", key = "mod"}
        };

        public static string share_para_name = "Name";
        public static string share_para_level = "LevelPart";
        public static string share_para_text1 = "Text1";
        public static string share_para_text2 = "Text2";
        public static string share_para_text3 = "Text3";
        public static string share_para_text4 = "Text4";

        public static List<string> MCT = new List<string>() { "MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E" };
        public static List<string> DV = new List<string>() { "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
        public static List<string> TON = new List<string>() { "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
        public static List<string> USER = new List<string>() { "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
        public static List<string> TIME = new List<string>() { "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };

        public static List<data_unit> units_convert = new List<data_unit>()
        {
            new data_unit(){name = "m3", value = Math.Pow(0.3048, 3)},
            new data_unit(){name = "m2", value = Math.Pow(0.3048, 2)},
            new data_unit(){name = "m", value = 0.3048},
            new data_unit(){name = "md", value = 0.3048},
            new data_unit(){name = "cm", value = 30.48},
            new data_unit(){name = "mm", value = 304.8},
            new data_unit(){name = "deg", value = 57.2957795},
            new data_unit(){name = "tấn", value = 2.5},
            new data_unit(){name = "ton", value = 2.5},
            new data_unit(){name = "kg", value = 2500},
        };

        public static List<data_unit> units = new List<data_unit>()
        {
            new data_unit(){name = "m3", value = Math.Pow(0.3048, 3)},
            new data_unit(){name = "100m3", value = Math.Pow(0.3048, 3) / 100},
            new data_unit(){name = "m2", value = Math.Pow(0.3048, 2)},
            new data_unit(){name = "100m2", value = Math.Pow(0.3048, 2) / 100},
            new data_unit(){name = "m", value = 0.3048},
            new data_unit(){name = "100m", value = 0.3048 / 100},
            new data_unit(){name = "md", value = 0.3048},
            new data_unit(){name = "cái", value = 1},
            new data_unit(){name = "bộ", value = 1}
        };

        public static List<data_unit> units_document = new List<data_unit>()
        {
            new data_unit(){name = "Millimeters", value = 1000},
            new data_unit(){name = "Centimeters", value = 100},
            new data_unit(){name = "Decimeters", value = 10},
            new data_unit(){name = "Meters", value = 1},
        };
    }
}
