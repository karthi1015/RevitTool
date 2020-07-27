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
        public ListView thong_tin_parameter { get; set; }
        public All_Data myAll_Data { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Parameters");

                string result = Them();

                if (result == "S")
                {
                    MessageBox.Show("Cập nhật giá trị parameter cho allplan thành công!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public string Them()
        {
            string result = "F";
            try
            {
                for (int i = 0; i < thong_tin_parameter.Items.Count; i++)
                {
                    Data item = (Data)thong_tin_parameter.Items[i];
                    item.cau_kien.LookupParameter(myAll_Data.list_parameter_share_data[2]).Set(item.ten_cau_kien);
                    item.cau_kien.LookupParameter(myAll_Data.list_parameter_share_data[3]).Set(item.id_cau_kien);
                    item.cau_kien.LookupParameter(myAll_Data.list_parameter_share_data[4]).Set(item.level_cau_kien);
                }
                //add_number_element(uidoc, doc);
                result = "S";
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}
