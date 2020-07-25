using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Tool_Filter
{
    [Transaction(TransactionMode.Manual)]
    public class Tool_Filter : IExternalCommand
    { 
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {  
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            HwndSource hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
            Window wnd = hwndSource.RootVisual as Window;

            try
            {
                UserControl1 win = new UserControl1(uiapp);
                win.Owner = wnd;
                win.Show();
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
