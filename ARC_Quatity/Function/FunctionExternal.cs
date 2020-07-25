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
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace WEB_SaveAs
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

        public CheckBox bao_gom_link { get; set; }

        public ObservableCollection<Link_File> myLink_File { get; set; }

        ListSource mySource;
        FunctionSupoort myFunctionSupport;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new FunctionSupoort();

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Parameters");

                if (command == "Visible Link File")
                {
                    Visible_Link_File(uiapp, doc);
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
        public string Visible_Link_File(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {


                var UIView = uiapp.ActiveUIDocument.GetOpenUIViews();
                foreach (var view in UIView)
                {
                    if (view.ViewId == doc.ActiveView.Id)
                    {
                        view.ZoomToFit();
                    }
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
        public string Them_Hoac_Xoa_Information_Trong_Project(UIApplication uiapp,Document doc)
        {
            string result = "F";
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Xoa_Type_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

    }
}
