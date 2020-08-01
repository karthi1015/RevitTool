using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Allplan_ParameterSupport
{
    class Source
    {

        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";

        public static string share_para_name = "Name";
        public static string share_para_level = "LevelPart";
        public static string share_para_text1 = "Text1";
        public static string share_para_text2 = "Text2";
        public static string share_para_text3 = "Text3";
        public static string share_para_text4 = "Text4";
        public static string share_para_class = "Class";

        public static Brush color_error = Brushes.LightSlateGray;
        public static Brush color = Brushes.White;

    }
}
