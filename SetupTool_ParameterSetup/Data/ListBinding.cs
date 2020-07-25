using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace SetupTool_ParameterSetup
{
    public class Data
    {
        public string single_value { get; set; }

        public string project_name { get; set; }

        public string project_address { get; set; }

        public string object_name { get; set; }

        public string ten_parameter { get; set; }

        public string group_parameter { get; set; }

        public Brush color { get; set; }
    }

    public class Category_Group
    {
        public string ten_category { get; set; }

        public BuiltInCategory category { get; set; }
    }

    public class Group_Share_Parameter
    {
        public string type { get; set; }

        public string ten_group_parameter { get; set; }

        public string id_group_parameter { get; set; }

        public bool ValueExpanded { get; set; }

        public ObservableCollection<Share_Parameter> Children { get; set; }

        public Brush color { get; set; }

        public string count_check { get; set; }
    }

    public class Share_Parameter
    {
        public string type { get; set; }

        public string ten_parameter { get; set; }

        public string guid_parameter { get; set; }

        public string type_parameter { get; set; }

        public string category_parameter { get; set; }

        public string group_parameter { get; set; }

        public string visible_parameter { get; set; }

        public string description_parameter { get; set; }

        public string user_modify_parameter { get; set; }

        public bool exist_parameter { get; set; }

        public bool ValueIsSelect { get; set; }

        public Brush color { get; set; }
    }

}
