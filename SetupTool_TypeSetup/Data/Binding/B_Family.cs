using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SetupTool_TypeSetup.Data.Binding
{
    class data_family
    {
        public string ten_family_type { get; set; }

        public BitmapImage image { get; set; }

        public bool ValueExpanded { get; set; }

        public ObservableCollection<data_type> Children { get; set; }

        public bool delete_family { get; set; }
    }
}
