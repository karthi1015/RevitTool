using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;

namespace Tool_CheckUpdateModel.Data
{
    public class Parameter_Settings
    {
        public bool isCheck { get; set; }
        public string parameter_name { get; set; }
        public Parameter parameter { get; set; }
        public string parameter_group { get; set; }
        public BuiltInCategory parameter_category { get; set; }
        public string parameter_category_name { get; set; }
    }

    public class Element_Group
    {
        public Element element { get; set; }
        public string element_text2 { get; set; }
        public bool link { get; set; }
        public string parameter_category_name { get; set; }
    }

    public class Parameter_Change
    {
        public Parameter parameter { get; set; }
        public Parameter parameter_link { get; set; }
        public string parameter_category_name { get; set; }
    }

    public class Point_Change
    {
        public XYZ point { get; set; }
        public XYZ point_link { get; set; }
    }

    public class Element_Change
    {
        public Element element { get; set; }
        public string element_name { get; set; }
        public string element_id { get; set; }
        public Element element_link { get; set; }
        public string parameter_category_name { get; set; }
        public ObservableCollection<Parameter_Change> parameter_change { get; set; }
        public string type_change { get; set; }
        public string preview { get; set; }
        public bool changeORignore { get; set; }
        public Brush color { get; set; }
    }
}
