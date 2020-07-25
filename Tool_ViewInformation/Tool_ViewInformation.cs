#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace Tool_ViewInformation
{
    [Transaction(TransactionMode.Manual)]
    public class Tool_ViewInformation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            HwndSource hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
            Window wnd = hwndSource.RootVisual as Window;

            try
            {
                UserControl1 user = new UserControl1(uiapp);
                user.Owner = wnd;
                user.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("ERROR", ex.Message);
                return Result.Failed;
            }
        }
    }
}

