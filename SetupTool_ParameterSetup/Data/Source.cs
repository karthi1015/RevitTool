using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_ParameterSetup
{
    class Source
    {

        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";
        //public string type_element = "element_type";
        //public string type_symbol = "symbol_type";
        //public string Duplicate = "Duplicate";
        //public string Update = "Update";

        //public string error = "ERROR";

        public static Brush color_error = Brushes.LightSlateGray;
        public static Brush color = Brushes.White;
        public static Brush color_group = new SolidColorBrush(Color.FromRgb(0, 255, 220));

        public static string path_WEB = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=WEBDataBase;User Id=WebUser; Password = webconnect456";

        public static string path_share_parameter_default = @"\\192.168.1.250\data\DataBases\01 RevitDataBases\06 ShareParameter\List Default\ShareParameterDefault.txt";
        public static string path_share_parameter = @"\\192.168.1.250\data\DataBases\01 RevitDataBases\06 ShareParameter\PTC_ShareParameter.txt";
    }
}
