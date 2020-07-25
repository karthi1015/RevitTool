using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace SetupTool_TypeSetup
{
    public class Data
    {
        public string single_value { get; set; }

        public string ten_type { get; set; }

        public Material vat_lieu { get; set; }
    }

    public class Family_Type
    {
        public string ten_family_type { get; set; }

        public string path { get; set; }

        public bool ValueExpanded { get; set; }

        public ObservableCollection<Element_Type> Children { get; set; }

        public bool delete_family { get; set; }
    }

    public class Element_Type
    {
        public string ten_element_type { get; set; }

        public ElementType element_type { get; set; }

        public string type_type { get; set; }

        public bool delete_type { get; set; }

        public bool ValueIsSelect { get; set; }
    }

    public class Parameters_Family
    {
        public string ten_parameter { get; set; }

        public string gia_tri_parameter { get; set; }

        public string group_parameter { get; set; }

        public Parameter parameter { get; set; }

        public CompoundStructureLayer layer { get; set; }

        public CompoundStructure compound { get; set; }
    }

    public class Material_Family
    {
        public string ten_cong_tac { get; set; }

        public ObservableCollection<Data> ten_vat_lieu_list { get; set; }

        public Data ten_vat_lieu { get; set; }

        public Parameter parameter { get; set; }

        public CompoundStructureLayer layer { get; set; }

        public CompoundStructure compound { get; set; }
    }

}
