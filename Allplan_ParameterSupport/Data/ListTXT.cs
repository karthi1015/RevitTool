using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Allplan_ParameterSupport
{
    public class All_Data
    {
        public ObservableCollection<list_image> list_path_image_data { get; set; }
        public ObservableCollection<Brush> list_color_UI_data { get; set; }
        public ObservableCollection<string> list_parameter_share_data { get; set; }
    }

    public class list_image
    {
        public string image_name { get; set; }
        public string image_path { get; set; }
    }
}
