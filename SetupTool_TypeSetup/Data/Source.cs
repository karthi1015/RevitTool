using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using SetupTool_TypeSetup.Data.BindingCompany;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_TypeSetup
{
    class Source
    {
        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";
        public static string type_element = "element_type";
        public static string type_symbol = "symbol_type";
        public static string Duplicate = "Duplicate";
        public static string Update = "Update";

        public string error = "ERROR";

        public static string user = @"C:\Users\" + Environment.UserName;

        public static List<data_name_key> list_discipline = new List<data_name_key>()
        {
            new data_name_key(){    name = "Architectural", key = "A"},
            new data_name_key(){    name = "Structural",    key = "S"},
            new data_name_key() {   name = "Mechanical",    key = "M"},
            new data_name_key() {   name = "Electricity",   key = "E"},
            new data_name_key() {   name = "Plumbing",      key = "P"},
            new data_name_key(){    name = "Fire fighting", key = "FF"}
        };

        public static List<data_name_key> list_position = new List<data_name_key>()
        {
            new data_name_key(){    name = "Ngoài Nhà", key = "O"},
            new data_name_key(){    name = "Trong Nhà", key = "I"},
            new data_name_key(){    name = "None",      key = "xx"}
        };

        public static List<data_name_key> list_category = new List<data_name_key>()
        {
            new data_name_key(){    name = "Column",        key = "C",      builtIn_category = BuiltInCategory.OST_StructuralColumns,       descipline_key = "S"},
            new data_name_key(){    name = "Beam",          key = "B",      builtIn_category = BuiltInCategory.OST_StructuralFraming,       descipline_key = "S"},
            new data_name_key(){    name = "Foundation",    key = "F",      builtIn_category = BuiltInCategory.OST_StructuralFoundation,    descipline_key = "S"},
            new data_name_key(){    name = "Foundation Beam",    key = "BF",     builtIn_category = BuiltInCategory.OST_StructuralFraming,            descipline_key = "S"},
            new data_name_key(){    name = "Slab",          key = "SL",     builtIn_category = BuiltInCategory.OST_Floors,                  descipline_key = "S"},
            new data_name_key(){    name = "RC Wall",       key = "W",      builtIn_category = BuiltInCategory.OST_Walls,                   descipline_key = "S"},
            new data_name_key(){    name = "Stairs",        key = "ST",    builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},
            new data_name_key(){    name = "Pile",          key = "P",      builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},
            new data_name_key(){    name = "Ramp",          key = "RAM",    builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},
            new data_name_key(){    name = "Manhole",       key = "MH",     builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},
            new data_name_key(){    name = "Guide Wall",    key = "GW",     builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},
            new data_name_key(){    name = "Diaphragm Wall",    key = "DW",     builtIn_category = BuiltInCategory.OST_GenericModel,            descipline_key = "S"},


            new data_name_key(){    name = "Wall",          key = "W",      builtIn_category = BuiltInCategory.OST_Walls,           descipline_key = "A"},
            new data_name_key(){    name = "Floor",         key = "F",      builtIn_category = BuiltInCategory.OST_Floors,          descipline_key = "A"},
            new data_name_key(){    name = "Ceiling",       key = "Cei",    builtIn_category = BuiltInCategory.OST_Ceilings,        descipline_key = "A"},
            new data_name_key(){    name = "Door",          key = "D",      builtIn_category = BuiltInCategory.OST_Doors,           descipline_key = "A"},
            new data_name_key(){    name = "Window",        key = "WD",     builtIn_category = BuiltInCategory.OST_Windows,         descipline_key = "A"},
            new data_name_key(){    name = "Curtain Wall",  key = "CTW",    builtIn_category = BuiltInCategory.OST_Walls,           descipline_key = "A"},
            new data_name_key(){    name = "Component",     key = "CM",     builtIn_category = BuiltInCategory.OST_GenericModel,    descipline_key = "A"},
            new data_name_key(){    name = "Railing",       key = "RL",     builtIn_category = BuiltInCategory.OST_Railings,        descipline_key = "A"}
        };

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

        public static List<data_unit> units_document = new List<data_unit>()
        {
            new data_unit(){name = "Millimeters", value = 304.8},
            new data_unit(){name = "Centimeters", value = 30.48},
            new data_unit(){name = "Decimeters", value = 3.048},
            new data_unit(){name = "Meters", value = 0.3048},
        };

        public static List<string> MCT = new List<string>() { "MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E" };
        public static List<string> DV = new List<string>() { "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
        public static List<string> TON = new List<string>() { "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
        public static List<string> USER = new List<string>() { "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
        public static List<string> TIME = new List<string>() { "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };
    }
}
