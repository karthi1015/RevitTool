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
#endregion

namespace Allplan_ParameterSupport
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

                if (command == "Create")
                {
                    string result = Them_Hoac_Xoa_Information_Trong_Project(uiapp, doc);
                    string result1 = Them_Hoac_Xoa_Parameter_Trong_Project(uiapp, doc);
                    if (result == "S" && result1 == "S")
                    {
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
        public string Them_Hoac_Xoa_Parameter_Trong_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                
                   
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
