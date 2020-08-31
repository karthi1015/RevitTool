using ARC_SupportLayout.Data;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ARC_SupportLayout.Code.Function
{
    class F_SetWidth
    {
        //----------------------------------------------------------
        public static string set_width(Document doc)
        {
            Transaction tr = new Transaction(doc);
            tr.Start("SupportLayout");
            string result = "F";
            try
            {

                foreach (ElementType type in new FilteredElementCollector(doc).WhereElementIsElementType().ToList())
                {
                    //if (type.Name.Split('_').Count() > 0 && type.Name.Split('_')[0] == "A")
                    //{
                    if (type.Category != null && type.Category.AllowsBoundParameters == true && type.Category.CategoryType.ToString() == "Model")
                    {
                        if (type.LookupParameter(Source.share_para_hoan_thien) != null)
                        {
                            if (type.LookupParameter(Source.share_para_hoan_thien).AsValueString() == "0" && type.LookupParameter(Source.share_para_hoan_thien).IsReadOnly == false)
                            {
                                type.LookupParameter(Source.share_para_hoan_thien).SetValueString("50");
                            }
                        }
                    }
                    //}
                }

                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            tr.Commit();
            return result;
        }
    }
}
