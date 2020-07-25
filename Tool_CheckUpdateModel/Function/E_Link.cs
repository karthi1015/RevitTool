using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tool_CheckUpdateModel.Function
{
    class E_Link : IExternalEventHandler
    {
        public string path { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc);
            transaction.Start("Link");

            Link_File(doc);

            transaction.Commit();
        }

        public string GetName()
        {
            return "Link";
        }

        public void Link_File(Document doc)
        {

            try
            {
                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(path);
                RevitLinkOptions rlo = new RevitLinkOptions(false);
                var linkType = RevitLinkType.Create(doc, mp, rlo);
                var instance = RevitLinkInstance.Create(doc, linkType.ElementId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
