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
    public class E_AllplanData : IExternalEventHandler
    {
        public ListView thong_tin_parameter { get; set; }

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
            return "Parameters";
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
                    item.cau_kien.LookupParameter(Source.share_para_text1).Set(item.ten_cau_kien);
                    item.cau_kien.LookupParameter(Source.share_para_text2).Set(item.id_cau_kien);
                    item.cau_kien.LookupParameter(Source.share_para_text3).Set(item.level_cau_kien);
                    item.cau_kien.LookupParameter(Source.share_para_class).Set(item.descipline);
                }
                result = "S";
            }
            catch (Exception)
            {

            }
            return result;
        }
    }
}
