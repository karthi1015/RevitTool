#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Interop;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace SetupTool_MaterialSetup
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            //HwndSource hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
            //Window wnd = hwndSource.RootVisual as Window;

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("Create Material");
                //UserControl1 viewWPF = new UserControl1(uiapp);
                //viewWPF.Owner = wnd;
                //viewWPF.Show();
                tr.Commit();
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
