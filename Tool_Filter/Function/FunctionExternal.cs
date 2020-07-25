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
using View = Autodesk.Revit.DB.View;
using System.IO;
using Autodesk.Revit.UI.Selection;
#endregion

namespace Tool_Filter
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

        public TextBox view_name { get; set; }

        public TextBox search_material_company { get; set; }

        public ListView thong_tin_category { get; set; }

        public ListView thong_tin_family { get; set; }

        public ListView thong_tin_type { get; set; }

        public ListView thong_tin_parameter { get; set; }

        public ListView gia_tri_parameter { get; set; }

        public ObservableCollection<element_information> my_element_information { get; set; }

        public ObservableCollection<element_information_parameter_value> my_element_information_parameter_value { get; set; }

        public ObservableCollection<element_information_parameter> my_element_information_parameter { get; set; }

        public ObservableCollection<element_information_type> my_element_information_type { get; set; }

        public ObservableCollection<element_information_family> my_element_information_family { get; set; }

        public ObservableCollection<element_information_category> my_element_information_category { get; set; }

        public List<ElementId> ids { get; set; }

        public string control { get; set; }

        ListSource mySource;
        FunctionSupoort myFunctionSupport;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new FunctionSupoort();

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("Filter");

                if (command == "Hight Lights")
                {
                    Hight_Lights_Filter(uiapp, doc);
                }

                if (command == "Hide")
                {
                    Hide(uiapp, doc);
                }

                if (command == "HideIsolate")
                {
                    Hide_Isolate(uiapp, doc);
                }

                if (command == "Reset View")
                {
                    Reset_View(uiapp, doc);
                }

                tr.Commit();

                if (command == "Duplicate View")
                {
                    Duplicate_View(uiapp, doc);
                }
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

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Hight_Lights_Filter(UIApplication uiapp, Document doc)
        {
            try
            {
                Selection selection = uiapp.ActiveUIDocument.Selection;
                if (ids.Count() > 0)
                {
                    selection.SetElementIds(ids);
                }
                else
                {
                    selection.SetElementIds(new List<ElementId>());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Hide(UIApplication uiapp, Document doc)
        {
            try
            {
                doc.ActiveView.HideElementsTemporary(ids);
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
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Hide_Isolate(UIApplication uiapp, Document doc)
        {
            try
            {
                doc.ActiveView.IsolateElementsTemporary(ids);
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
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Reset_View(UIApplication uiapp, Document doc)
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
                Selection selection = uiapp.ActiveUIDocument.Selection;
                selection.SetElementIds(new List<ElementId>());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Duplicate_View(UIApplication uiapp, Document doc)
        {
            try
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
                List<string> elementsIsolateIdString = new List<string>();
                foreach (var ele in ids)
                {
                    elementsIsolateIdString.Add(ele.ToString());
                }
                List<ElementId> elementsId = new List<ElementId>();
                bool check = false;
                if (control == "Hide") check = true;
                foreach (var ele in elements)
                {
                    if (ele.CanBeHidden(viewDup) && elementsIsolateIdString.Contains(ele.Id.ToString()) == check)
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
