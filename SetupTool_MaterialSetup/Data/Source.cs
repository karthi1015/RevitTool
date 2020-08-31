using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_MaterialSetup
{
    class Source
    {
        public static Autodesk.Revit.DB.Color material_color = new Autodesk.Revit.DB.Color(128, 128, 128);

        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";

        public static string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";

        public static Brush color_error = Brushes.LightSlateGray;
        public static Brush color = Brushes.White;

        public static string path_revit = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=RevitDataBase;User Id=RevitUser; Password = revit@connect";
        public static string path_WEB = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=WEBDataBase;User Id=WebUser; Password = webconnect456";
        public static string path_Quantity = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=QuatityDataBase;User Id=RevitUser; Password = revit@connect";

        public static List<data_descipline> desciplines = new List<data_descipline>()
        {
            new data_descipline(){name = "arc", key = "a"},
            new data_descipline(){name = "str", key = "s"},
            new data_descipline(){name = "la", key = "la"},
            new data_descipline(){name = "mep", key = "mep"},
            new data_descipline(){name = "mod", key = "mod"}
        };

        public static List<string> MCT = new List<string>() { "MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E" };
        public static List<string> DV = new List<string>() { "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
        public static List<string> TON = new List<string>() { "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
        public static List<string> USER = new List<string>() { "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
        public static List<string> TIME = new List<string>() { "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };
    }

    class MessageString
    {

    }
}
