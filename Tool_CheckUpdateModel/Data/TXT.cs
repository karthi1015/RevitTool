using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_CheckUpdateModel.Data
{
    public class TXT
    {
        public ObservableCollection<double> list_unit_value_data { get; set; }
        public ObservableCollection<list_procedure> list_procedure_data { get; set; }
        public ObservableCollection<string> list_path_connect_SQL_data { get; set; }
        public ObservableCollection<list_mct_descipline> list_mct_descipline_data { get; set; }
        public ObservableCollection<string> list_parameter_share_data { get; set; }
    }

    public class list_procedure
    {
        public string procedure_name { get; set; }
        public string procedure_parameter { get; set; }
    }

    public class list_mct_descipline
    {
        public string mct { get; set; }
        public string descipline { get; set; }
    }
}
