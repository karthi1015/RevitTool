using SetupTool_MaterialSetup.Code.Function;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_MaterialSetup.Code.Function.FunctionProject;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = Autodesk.Revit.DB.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using System.Windows.Data;

namespace SetupTool_MaterialSetup.Code.External
{
    class E_ChangeFactor : IExternalEventHandler
    {
        public ListView thong_tin_vat_lieu_project { get; set; }

        public string user { get; set; }

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
                transaction.Start("Create Material");

                string result = Thay_doi_he_so(uiapp, doc);

                transaction.Commit();
                if (result == "S")
                {
                    F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);
                    thong_tin_vat_lieu_project.Items.Refresh();
                    MessageBox.Show("Change Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
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
            return "Create Material";
        }

        //----------------------------------------------------------
        public string Thay_doi_he_so(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                foreach (data_material_factor data in CollectionViewSource.GetDefaultView(thong_tin_he_so_vat_lieu_project.ItemsSource).Cast<data_material_factor>().ToList())
                {
                    foreach (Material material in data.list_vat_lieu)
                    {
                        F_GetSchema.SetDataStorage(material, Source.TON[1], Source.TON[0], data.ton);
                        data_material_project material_project = my_material_project.First(x => x.ten_vat_lieu_project == material.Name);
                        material_project.ton = data.ton;
                        material_project.user = user;
                        material_project.time = DateTime.Now.ToString();
                        result = "S";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = "F";
            }
            return result;
        }
    }
}
