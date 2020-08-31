using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SetupTool_ParameterSetup.Data.Binding
{
    class data_item_share_parameter
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
