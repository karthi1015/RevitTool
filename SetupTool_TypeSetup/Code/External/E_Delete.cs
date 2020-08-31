using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_TypeSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace SetupTool_TypeSetup.Code.External
{
    class E_Delete : IExternalEventHandler
    {
        public ListView thong_tin_kich_thuoc { get; set; }

        public ListView thong_tin_cong_tac_vat_lieu { get; set; }

        public TreeView thong_tin_family_type { get; set; }

        public ObservableCollection<data_family> my_family { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Delete Type");

                string result = Xoa_Type_Project(uiapp, doc);
                if (result == "S")
                {
                    MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string GetName()
        {
            return "Delete Type";
        }

        //----------------------------------------------------------
        public string Xoa_Type_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                List<ElementId> ids = new List<ElementId>();
                List<data_type> myElement_Type_delete = new List<data_type>();
                foreach (data_family family in my_family)
                {
                    if (family.delete_family == true)
                    {
                        foreach (data_type type in family.Children)
                        {
                            ids.Add(type.element_type.Id);
                            myElement_Type_delete.Add(type);
                        }
                    }
                    else
                    {
                        foreach (data_type type in family.Children)
                        {
                            if (type.delete_type == true)
                            {
                                ids.Add(type.element_type.Id);
                                myElement_Type_delete.Add(type);
                            }
                        }
                    }
                }
                if (ids.Count() > 0)
                {
                    var result_message = MessageBox.Show("Are you sure! This process cannot be undone.", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result_message == MessageBoxResult.Yes)
                    {
                        doc.Delete(ids);
                        foreach (data_family family in my_family)
                        {
                            var type_delete = family.Children.Where(x => x.delete_type == true).ToList();
                            foreach (data_type type in type_delete)
                            {
                                try { family.Children.Remove(type); } catch (Exception) { }
                            }
                        }
                        var family_delete = my_family.Where(x => x.delete_family == true).ToList();
                        foreach (data_family family in family_delete)
                        {
                            try { my_family.Remove(family); } catch (Exception) { }
                        }
                        thong_tin_kich_thuoc.ItemsSource = new ObservableCollection<data_parameters>();
                        thong_tin_cong_tac_vat_lieu.ItemsSource = new ObservableCollection<data_materials>();
                        result = "S";
                    }
                }
                else
                {
                    MessageBox.Show("Please check type and delete again!!", "WARNING", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
