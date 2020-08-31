using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_ParameterSetup.Data.Binding
{
    class data_group_share_parameter
    {
        public string type { get; set; }

        public string ten_group_parameter { get; set; }

        public string id_group_parameter { get; set; }

        public bool ValueExpanded { get; set; }

        public ObservableCollection<data_item_share_parameter> Children { get; set; }

        public Brush color { get; set; }

        public string count_check { get; set; }
    }
}
