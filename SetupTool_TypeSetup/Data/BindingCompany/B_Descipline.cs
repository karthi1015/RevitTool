using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_TypeSetup.Data.BindingCompany
{
    class data_name_key
    {
        public string name { get; set; }

        public string key { get; set; }

        public BuiltInCategory builtIn_category { get; set; }

        public string descipline_key { get; set; }
    }
}
