using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_TypeSetup
{
    public class All_Data
    {
        public ObservableCollection<list_image> list_path_image_data { get; set; }
        public ObservableCollection<double> list_unit_value_data { get; set; }
        public ObservableCollection<Brush> list_color_UI_data { get; set; }
        public ObservableCollection<list_procedure> list_procedure_data { get; set; }
        public ObservableCollection<string> list_path_foder_data { get; set; }
        public ObservableCollection<string> list_path_connect_SQL_data { get; set; }
        public ObservableCollection<list_material_para> list_material_para_data { get; set; }
        public ObservableCollection<Data> list_descipline_data { get; set; }
        public ObservableCollection<list_position> list_position_data { get; set; }
        public ObservableCollection<Data> list_category_data { get; set; }
        public ObservableCollection<BuiltIn_Parameter_Group> list_builtin_parameter_group_data { get; set; }
        public ObservableCollection<Type_Group> list_system_name_data { get; set; }
        public ObservableCollection<string> list_parameter_share_data { get; set; }
        
    }

    public class list_image
    {
        public string image_name { get; set; }
        public string image_path { get; set; }
    }

    public class list_position
    {
        public string position_name { get; set; }
        public string position_key { get; set; }
    }

    public class list_procedure
    {
        public string procedure_name { get; set; }
        public string procedure_para { get; set; }
    }

    public class list_mct_descipline
    {
        public string mct { get; set; }
        public string descipline { get; set; }
    }

    public class list_material_para
    {
        public string material_para_guid { get; set; }
        public string material_para_name { get; set; }
    }

    public class list_ten_du_lieu_excel
    {
        public string excel_name { get; set; }
        public string excel_ngang { get; set; }
        public string excel_dung { get; set; }
    }

    public class BuiltIn_Parameter_Group
    {
        public string ten_builtin { get; set; }

        public BuiltInParameterGroup builtin { get; set; }
    }

    public class Type_Group
    {
        public string ten_type { get; set; }

        public ElementType type { get; set; }
    }
}
