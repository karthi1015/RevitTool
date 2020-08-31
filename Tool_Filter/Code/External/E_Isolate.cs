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
    class E_Isolate : IExternalEventHandler
    {
        public List<ElementId> ids { get; set; }
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("Isolate");
                Hide_Isolate(uiapp, doc);
                tr.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "Isolate";
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Hide_Isolate(UIApplication uiapp, Document doc)
        {
            try
            {
                if(ids.Count() > 0) doc.ActiveView.IsolateElementsTemporary(ids);
                var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                foreach (var view in UIView)
                {
                    if (view.ViewId == doc.ActiveView.Id)
                    {
                        view.ZoomToFit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
