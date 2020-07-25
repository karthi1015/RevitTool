using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tool_ViewInformation
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
        public ObservableCollection<list_mct_descipline> list_mct_descipline_data { get; set; }
        public ObservableCollection<list_descipline> list_descipline_data { get; set; }
        public ObservableCollection<string> list_system_name_data { get; set; }
        public ObservableCollection<string> list_parameter_share_data { get; set; }
    }

    public class list_image
    {
        public string image_name { get; set; }
        public string image_path { get; set; }
    }

    public class list_descipline
    {
        public string descipline_name { get; set; }
        public string descipline_key { get; set; }
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
}
