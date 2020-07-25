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
    public class External_Sort_Section : IExternalEventHandler
    {
        public string command { get; set; }

        public ComboBox parameter_1 { get; set; }
        public ComboBox parameter_2 { get; set; }
        public ComboBox parameter_3 { get; set; }
        public TextBox value_1 { get; set; }
        public TextBox value_2 { get; set; }
        public TextBox value_3 { get; set; }
        public TextBox sheet_start { get; set; }
        public TextBox sheet_end { get; set; }


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
                transaction.Start("Sort");

                if (command == "Sort Section")
                {
                    Apply_Sort(uiapp, doc);
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
        public void Apply_Sort(UIApplication uiapp, Document doc)
        {
            try
            {
                myFunctionSupport = new Support_All();
                parameter_data item1 = (parameter_data)parameter_1.SelectedItem;
                parameter_data item2 = (parameter_data)parameter_2.SelectedItem;
                parameter_data item3 = (parameter_data)parameter_3.SelectedItem;
                List<ViewSection> list_view_section = new List<ViewSection>();

                var list_viewsection = new FilteredElementCollector(doc).OfClass(typeof(ViewSection)).Cast<ViewSection>().ToList();
                foreach (var view in list_viewsection)
                {
                    if (myFunctionSupport.Get_Parameter_Information(view.LookupParameter(item1.parameter_name), doc, myAll_Data) == value_1.Text &&
                        myFunctionSupport.Get_Parameter_Information(view.LookupParameter(item2.parameter_name), doc, myAll_Data) == value_2.Text &&
                        myFunctionSupport.Get_Parameter_Information(view.LookupParameter(item3.parameter_name), doc, myAll_Data) == value_3.Text)
                    {
                        list_view_section.Add(view);
                    }
                }

                List<view_section_list_data> list_view_section_data = new List<view_section_list_data>();
                for (int i = Convert.ToInt32(sheet_start.Text); i < Convert.ToInt32(sheet_end.Text) + 1; i++)
                {
                    foreach (ViewSection view in list_view_section)
                    {
                        if (myFunctionSupport.Get_Parameter_Information(view.LookupParameter("Sheet Number"), doc, myAll_Data) == i.ToString())
                        {
                            list_view_section_data.Add(new view_section_list_data()
                            {
                                view_section = view,
                                sheet_number = i.ToString()
                            });
                        }
                    }
                }

                Change_Number(uiapp, doc, list_view_section_data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Change_Number(UIApplication uiapp, Document doc, List<view_section_list_data> list_view_section_data)
        {
            try
            {
                int A = 1;
                foreach(view_section_list_data view in list_view_section_data)
                {
                    view.view_section.LookupParameter("View Name").Set(A.ToString());
                    A++;
                }
                int b = 1;
                for (int i = Convert.ToInt32(sheet_start.Text); i < Convert.ToInt32(sheet_end.Text) + 1; i++)
                {
                    foreach (view_section_list_data view in list_view_section_data)
                    {
                        if (view.sheet_number == i.ToString())
                        {
                            view.view_section.LookupParameter("View Name").Set(value_2.Text + " " + value_3.Text + " " + b.ToString("00"));
                            b++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
