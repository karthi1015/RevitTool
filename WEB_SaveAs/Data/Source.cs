using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WEB_SaveAs
{
    class Source
    {
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

        public static List<string> visible_false = new List<string>() { "P", "GW", "DW", "BF", "F" };
    }
}
