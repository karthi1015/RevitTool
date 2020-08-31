using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_Filter.Code.External
{
    class E_Refresh : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("Refresh");
                Reset_View(uiapp, doc);
                tr.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "Refresh";
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset_View(UIApplication uiapp, Document doc)
        {
            try
            {
                View viewCurrent = doc.ActiveView;
                if (viewCurrent.IsTemporaryHideIsolateActive())
                {
                    TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;
                    viewCurrent.DisableTemporaryViewMode(tempView);
                }
                var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                foreach (var view in UIView)
                {
                    if (view.ViewId == viewCurrent.Id)
                    {
                        view.ZoomToFit();
                    }
                }
                Selection selection = uiapp.ActiveUIDocument.Selection;
                selection.SetElementIds(new List<ElementId>());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
