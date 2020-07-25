using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tool_CheckUpdateModel.Data;

namespace Tool_CheckUpdateModel.Function
{
    class E_Focus : IExternalEventHandler
    {
        public Document doc_link { get; set; }
        public Element_Change item { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Focus");

                Focus(uiapp, doc);

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
        void Focus(UIApplication uiapp, Document doc)
        {
            BoundingBoxXYZ boundingBoxXYZ = new BoundingBoxXYZ();
            try
            {
                ResetView(uiapp, doc);
                double a = 5;
                if (item.element != null && item.element_link != null)
                {
                    BoundingBoxXYZ boundingbox_1 = item.element.get_BoundingBox(null);
                    BoundingBoxXYZ boundingbox_2 = item.element_link.get_BoundingBox(null);

                    var x_1_min = boundingbox_1.Min.X;
                    var y_1_min = boundingbox_1.Min.Y;
                    var z_1_min = boundingbox_1.Min.Z;

                    var x_2_min = boundingbox_2.Min.X;
                    var y_2_min = boundingbox_2.Min.Y;
                    var z_2_min = boundingbox_2.Min.Z;

                    XYZ min = new XYZ(Math.Max(x_1_min, x_2_min) - a, Math.Max(y_1_min, y_2_min) - a, Math.Max(z_1_min, z_2_min) - a);

                    var x_1_max = boundingbox_1.Max.X;
                    var y_1_max = boundingbox_1.Max.Y;
                    var z_1_max = boundingbox_1.Max.Z;

                    var x_2_max = boundingbox_2.Max.X;
                    var y_2_max = boundingbox_2.Max.Y;
                    var z_2_max = boundingbox_2.Max.Z;

                    XYZ max = new XYZ(Math.Min(x_1_max, x_2_max) + a, Math.Min(y_1_max, y_2_max) + a, Math.Min(z_1_max, z_2_max) + a);

                    boundingBoxXYZ.Min = min;
                    boundingBoxXYZ.Max = max;
                }
                else if (item.element != null && item.element_link == null)
                {
                    BoundingBoxXYZ boundingbox_1 = item.element.get_BoundingBox(null);

                    XYZ min = new XYZ(boundingbox_1.Min.X - a, boundingbox_1.Min.Y - a, boundingbox_1.Min.Z - a);
                    XYZ max = new XYZ(boundingbox_1.Max.X + a, boundingbox_1.Max.Y + a, boundingbox_1.Max.Z + a);

                    boundingBoxXYZ.Min = min;
                    boundingBoxXYZ.Max = max;
                }
                else if (item.element == null && item.element_link != null)
                {
                    BoundingBoxXYZ boundingbox_1 = item.element_link.get_BoundingBox(null);

                    XYZ min = new XYZ(boundingbox_1.Min.X - a, boundingbox_1.Min.Y - a, boundingbox_1.Min.Z - a);
                    XYZ max = new XYZ(boundingbox_1.Max.X + a, boundingbox_1.Max.Y + a, boundingbox_1.Max.Z + a);

                    boundingBoxXYZ.Min = min;
                    boundingBoxXYZ.Max = max;
                }

                View3D view = doc.ActiveView as View3D;
                view.SetSectionBox(boundingBoxXYZ);

                var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                foreach (var view1 in UIView)
                {
                    if (view1.ViewId == view.Id)
                    {
                        view1.ZoomAndCenterRectangle(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
                    }
                }

                Selection select = uiapp.ActiveUIDocument.Selection;
                if (item.element != null) select.SetElementIds(new List<ElementId>() { item.element.Id });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        void HideIsolateAndZoom(List<ElementId> ElementIsolate, UIApplication uiapp, Document doc)
        {
            try
            {
                List<ElementId> elementsId = new List<ElementId>();
                foreach (var id in ElementIsolate)
                {
                    elementsId.Add(id);
                }
                doc.ActiveView.IsolateElementsTemporary(elementsId);
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
                MessageBox.Show("ERROR in Hide Isolate And Zoom" + "\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //----------------------------------------------------------
        void ResetView(UIApplication uiapp, Document doc)
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
                View3D view1 = viewCurrent as View3D;
                view1.IsSectionBoxActive = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in Reset View" + "\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
