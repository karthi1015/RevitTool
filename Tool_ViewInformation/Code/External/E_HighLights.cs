using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_ViewInformation.Code.External
{
    class E_HighLights : IExternalEventHandler
    {
        public List<ElementId> ids { get; set; }
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("HighLights");
                Hight_Lights_Filter(uiapp, doc);
                tr.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "HighLights";
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Hight_Lights_Filter(UIApplication uiapp, Document doc)
        {
            try
            {
                Selection selection = uiapp.ActiveUIDocument.Selection;
                if (ids.Count() > 0)
                {
                    selection.SetElementIds(ids);
                }
                else
                {
                    selection.SetElementIds(new List<ElementId>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
