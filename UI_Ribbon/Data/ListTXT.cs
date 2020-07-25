using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UI_Ribbon
{
    public class All_Data
    {
        public ObservableCollection<list_image> list_path_image_data { get; set; }
        public ObservableCollection<Brush> list_color_UI_data { get; set; }
        public ObservableCollection<string> list_path_foder_data { get; set; }
        public ObservableCollection<string> list_path_connect_SQL_data { get; set; }
    }

    public class list_image
    {
        public string image_name { get; set; }
        public string image_path { get; set; }
    }
}
