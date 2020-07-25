#region Namespaces
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using RadioButton = System.Windows.Controls.RadioButton;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;
using System.Globalization;
#endregion

namespace Draw_All
{
    public class External_Crop : IExternalEventHandler
    {
        public string command { get; set; }

        public ComboBox view_tem { get; set; }
        public ComboBox view_apply { get; set; }

        public All_Data myAll_Data { get; set; }

        ListSource mySource;
        Support_All myFunctionSupport;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new Support_All();

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Crop");

                if (command == "Crop View")
                {
                    Apply_Crop(uiapp, doc);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "External Event Example";
        }

        //----------------------------------------------------------
        public void Apply_Crop(UIApplication uiapp, Document doc)
        {
            try
            {
                view_plan_name_data item_TEM = (view_plan_name_data)view_tem.SelectedItem;
                view_plan_name_data item = (view_plan_name_data)view_apply.SelectedItem;

                CurveLoop cl = Get_CurveLoop(uiapp, doc);
                ViewPlan view_plan = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).Cast<ViewPlan>().First(x => x.Name == item.single_value);
                ViewCropRegionShapeManager vcrShapeMgr = view_plan.GetCropRegionShapeManager();
                if (vcrShapeMgr.CanHaveShape) vcrShapeMgr.SetCropShape(cl);

                view_plan.Scale = item_TEM.view_plan.Scale;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public CurveLoop Get_CurveLoop(UIApplication uiapp, Document doc)
        {
            CurveLoop cl = null;
            try
            {
                view_plan_name_data item = (view_plan_name_data)view_tem.SelectedItem;

                ViewPlan view_plan = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan)).Cast<ViewPlan>().First(x => x.Name == item.single_value);
                ViewCropRegionShapeManager vcrShapeMgr = view_plan.GetCropRegionShapeManager();
                cl = vcrShapeMgr.GetCropShape()[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return cl;
        }
    }
}
