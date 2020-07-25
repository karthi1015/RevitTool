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
using TreeView = System.Windows.Controls.TreeView;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;
#endregion

namespace Naviswork_ClashComment
{
    public class E_Focus : IExternalEventHandler
    {
        public ListView thong_tin_clash_child { get; set; }

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
        public string Focus(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                child item = (child)thong_tin_clash_child.SelectedItem;
                if (item != null)
                {
                    SectionBox(uiapp, doc, item);
                }
                   
                result = "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        void SectionBox(UIApplication uiapp, Document doc, child item)
        {
            try
            {
                ResetView(uiapp, doc);
                var document1 = item.doc1_child;
                var document2 = item.doc2_child;
                ElementId ele1 = new ElementId(Convert.ToInt32(item.id1_child));
                ElementId ele2 = new ElementId(Convert.ToInt32(item.id2_child));


                BoundingBoxXYZ boundingbox_1 = document1.GetElement(ele1).get_BoundingBox(null);
                BoundingBoxXYZ boundingbox_2 = document2.GetElement(ele2).get_BoundingBox(null);
                double a = 10;
                if (boundingbox_1 != null && boundingbox_2 != null)
                {
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

                    BoundingBoxXYZ boundingBoxXYZ = new BoundingBoxXYZ();
                    boundingBoxXYZ.Min = min;
                    boundingBoxXYZ.Max = max;

                    View3D view = doc.ActiveView as View3D;
                    view.SetSectionBox(boundingBoxXYZ);

                    var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                    foreach (var view1 in UIView)
                    {
                        if (view1.ViewId == view.Id)
                        {
                            view1.ZoomAndCenterRectangle(min, max);
                        }
                    }

                    Selection select = uiapp.ActiveUIDocument.Selection;
                    if (doc.Title == document1.Title) select.SetElementIds(new List<ElementId>() { ele1 });
                    else if (doc.Title == document2.Title) select.SetElementIds(new List<ElementId>() { ele2 });
                }
            }
            catch (Exception)
            {

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

                //var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                //foreach (var view in UIView)
                //{
                //    if (view.ViewId == viewCurrent.Id)
                //    {
                //        view.ZoomToFit();
                //    }
                //}
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
