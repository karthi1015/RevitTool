using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextBox = System.Windows.Controls.TextBox;

namespace Tool_Filter.Code.External
{
    class E_DuplicateView : IExternalEventHandler
    {
        public List<ElementId> ids { get; set; }
        public TextBox view_name { get; set; }
        public bool state { get; set; }
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Duplicate_View(uiapp, doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "DuplicateView";
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Duplicate_View(UIApplication uiapp, Document doc)
        {
            try
            {
                if (ids.Count() > 0)
                {
                    Transaction transaction = new Transaction(doc);
                    transaction.Start("DuplacateView");
                    View viewCurrent = doc.ActiveView;
                    ElementId viewDupId = viewCurrent.Duplicate(ViewDuplicateOption.Duplicate);
                    View viewDup = doc.GetElement(viewDupId) as View;
                    viewDup.Name = view_name.Text;

                    if (viewDup.IsTemporaryHideIsolateActive())
                    {
                        TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;
                        viewDup.DisableTemporaryViewMode(tempView);
                    }
                    var elements = new FilteredElementCollector(doc, viewDup.Id).WhereElementIsNotElementType().ToElements();
                    List<ElementId> elementsId = new List<ElementId>();
                    foreach (var ele in elements)
                    {
                        if (ele.CanBeHidden(viewDup) && ids.Where(x => x.IntegerValue > 0).Any(x => x.IntegerValue.ToString() == ele.Id.ToString()) == state)
                        {
                            elementsId.Add(ele.Id);
                        }
                    }
                    if (elementsId.Count > 0)
                    {
                        viewDup.HideElements(elementsId);
                    }
                    transaction.Commit();
                    uiapp.ActiveUIDocument.ActiveView = viewDup;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
