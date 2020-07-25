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

namespace Tool_ViewInformation
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }

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

                tr.Commit();
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
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        
    }
}
