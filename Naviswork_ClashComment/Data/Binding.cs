using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = Autodesk.Revit.DB.Color;

namespace Naviswork_ClashComment
{
    class parent
    {
        public string clash_file_name { get; set; }
        public string clash_parent { get; set; }
        public string status_parent { get; set; }
        public Brush color { get; set; }
    }

    class child
    {
        public string id { get; set; }
        public string clash_child { get; set; }
        public int clash_child_sort { get; set; }
        public List<status_data> status_child_list { get; set; }
        public status_data status_child { get; set; }
        public string location_child { get; set; }
        public string approved_by_child { get; set; }
        public string solution_child { get; set; }

        public string id1_child { get; set; }
        public Document doc1_child { get; set; }
        public string id2_child { get; set; }
        public Document doc2_child { get; set; }

        public List<ImageSource> bitmap { get; set; }

        public Brush color { get; set; }
        public bool isEnable { get; set; }
    }

    class Image_Solution
    {
        public ImageSource bitmap { get; set; }
    }

    class status_data
    {
        public string name { get; set; }
        public Brush color { get; set; }
        public bool isEnable { get; set; }
    }

    class count_status
    {
        public string status { get; set; }
        public int count { get; set; }
    }
}
