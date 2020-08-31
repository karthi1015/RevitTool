using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetupTool_TypeSetup.Data.Binding
{
    class data_materials
    {
        public string ten_cong_tac { get; set; }

        public ObservableCollection<data_material> ten_vat_lieu_list { get; set; }

        public data_material ten_vat_lieu { get; set; }

        public Parameter parameter { get; set; }

        public CompoundStructureLayer layer { get; set; }

        public CompoundStructure compound { get; set; }
    }
}
