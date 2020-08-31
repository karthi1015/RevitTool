using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_ViewInformation.Code.Function
{
    class F_GetUnits
    {
        //----------------------------------------------------------
        public static string get_unit_length_type(Document doc)
        {
            string unit_length = "Millimeters";
            try
            {
                Units units = doc.GetUnits();
                List<UnitType> unit_types = UnitUtils.GetValidUnitTypes().ToList();
                FormatOptions format_option = units.GetFormatOptions(unit_types.First(x => x == UnitType.UT_Length));
                unit_length = LabelUtils.GetLabelFor(format_option.DisplayUnits);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return unit_length;
        }
    }
}
