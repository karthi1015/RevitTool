using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_MaterialSetup.Code.Function.FunctionProject;
using SetupTool_MaterialSetup.Data.Binding;

namespace SetupTool_MaterialSetup.Code.External
{
    class E_Delete : IExternalEventHandler
    {
        public ListView thong_tin_vat_lieu_project { get; set; }

        public ObservableCollection<data_material_project> my_material_project { get; set; }

        public ObservableCollection<data_material_factor> my_material_factor { get; set; }
        public ListView thong_tin_he_so_vat_lieu_project { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Delete Material");

                string result = Xoa_Vat_Lieu_Project(uiapp, doc);
                
                transaction.Commit();
                if (result == "S")
                {
                    F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);
                    thong_tin_vat_lieu_project.Items.Refresh();
                    MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
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
            return "Delete Material";
        }

        //----------------------------------------------------------
        public string Xoa_Vat_Lieu_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                List<ElementId> id_delete = new List<ElementId>();
                List<data_material_project> item_delete = new List<data_material_project>();
                
                for (var i = 0; i < thong_tin_vat_lieu_project.SelectedItems.Count; i++)
                {
                    data_material_project item = (data_material_project)thong_tin_vat_lieu_project.SelectedItems[i];
                    id_delete.Add(item.material_project.Id);
                    item_delete.Add(item);
                }
                
                if (id_delete.Count > 0)
                {
                    doc.Delete(id_delete);
                    foreach (data_material_project material_Project in item_delete)
                    {
                        my_material_project.Remove(material_Project);
                    }
                    result = "S";
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
