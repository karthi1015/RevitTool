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
using System.Windows;
using ARC_SupportLayout.UI;
using System.Linq;
using ARC_SupportLayout.Data;
using ARC_SupportLayout.Code.Function;
#endregion

namespace ARC_SupportLayout
{
    [Transaction(TransactionMode.Manual)]
    public class ARC_SupportLayout : IExternalCommand
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
                string result = F_SetWidth.set_width(doc);
                if (result == "S")
                {
                    Transaction tr = new Transaction(doc);
                    tr.Start("SupportLayout");
                    UserControl1 viewWPF = new UserControl1(uiapp);
                    viewWPF.Owner = wnd;
                    viewWPF.Show();
                    tr.Commit();
                }
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
