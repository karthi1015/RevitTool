#region Namespaces
using System;
using System.Linq;
using ARC_SupportLayout.Code.Function;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MessageBox = System.Windows.MessageBox;
#endregion

namespace ARC_SupportLayout
{
    class E_Width : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                F_SetWidth.set_width(doc);

                //Transaction transaction = new Transaction(doc);
                //transaction.Start("Width");

                //set_width(uiapp, doc);

                //transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "Width";
        }
    }
}
