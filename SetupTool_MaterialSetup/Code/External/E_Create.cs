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

namespace SetupTool_MaterialSetup.Code.External
{
    class E_Create : IExternalEventHandler
    {
        public ListView thong_tin_vat_lieu_project { get; set; }

        public TextBox ten_vat_lieu { get; set; }

        public TextBox ma_cong_tac { get; set; }

        public ComboBox don_vi { get; set; }

        public CheckBox use_appearence { get; set; }

        public TextBox tranparency_value { get; set; }

        public Button color_shading { get; set; }
        public Button color_surface { get; set; }
        public Button color_cut { get; set; }

        public ComboBox name_surface { get; set; }
        public ComboBox name_cut { get; set; }

        public TextBox ton_value { get; set; }

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
                TransactionGroup transaction = new TransactionGroup(doc);
                transaction.Start("Create Material");

                string result = Tao_Vat_Lieu_Moi_Project(uiapp, doc);

                transaction.Assimilate();
                if (result == "S")
                {
                    F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);
                    thong_tin_vat_lieu_project.Items.Refresh();
                    MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
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
        public string Tao_Vat_Lieu_Moi_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                {
                    if (my_material_project.Any(x => x.ten_vat_lieu_project == ten_vat_lieu.Text) == false)
                    {
                        if (my_material_project.Any(x => x.ma_cong_tac_project == ma_cong_tac.Text) == false || string.IsNullOrEmpty(ma_cong_tac.Text))
                        {
                            Transaction tr1 = new Transaction(doc);
                            tr1.Start("Create Material");
                            ElementId material_id_new = Material.Create(doc, ten_vat_lieu.Text);
                            Material material = doc.GetElement(material_id_new) as Material;

                            System.Windows.Media.Color shading = ((SolidColorBrush)color_shading.Background).Color;
                            System.Windows.Media.Color surface = ((SolidColorBrush)color_surface.Background).Color;
                            System.Windows.Media.Color cut = ((SolidColorBrush)color_cut.Background).Color;

                            string time = DateTime.Now.ToString();
                            string ton = "2.5";
                            if (!string.IsNullOrEmpty(ton_value.Text)) ton = ton_value.Text;

                            material.Name = ten_vat_lieu.Text;
                            F_GetSchema.SetDataStorage(material, Source.MCT[1], Source.MCT[0], ma_cong_tac.Text);
                            F_GetSchema.SetDataStorage(material, Source.DV[1], Source.DV[0], don_vi.Text);
                            F_GetSchema.SetDataStorage(material, Source.TON[1], Source.TON[0], ton);
                            F_GetSchema.SetDataStorage(material, Source.USER[1], Source.USER[0], user);
                            F_GetSchema.SetDataStorage(material, Source.TIME[1], Source.TIME[0], time);
                            material.SurfaceBackgroundPatternId = ((data_fill_pattern)name_surface.SelectedItem).pattern_id;
                            material.SurfaceBackgroundPatternColor = new Color(surface.R, surface.G, surface.B);
                            material.CutBackgroundPatternId = ((data_fill_pattern)name_cut.SelectedItem).pattern_id;
                            material.CutBackgroundPatternColor = new Color(cut.R, cut.G, cut.B);
                            if (use_appearence.IsChecked == false)
                            {
                                material.Color = new Color(shading.R, shading.G, shading.B);
                                material.Transparency = Convert.ToInt32(tranparency_value.Text);
                            }
                            else
                            {
                                material.Color = Source.material_color;
                            }
                            tr1.Commit();

                            Transaction tr2 = new Transaction(doc);
                            tr2.Start("Edit Material");
                            F_SetAppearanceAsset.set_appearance_asset(doc, material);
                            tr2.Commit();

                            my_material_project.Add(new data_material_project()
                            {
                                material_project = material,
                                ma_cong_tac_project = ma_cong_tac.Text,
                                ten_vat_lieu_project = ten_vat_lieu.Text,
                                don_vi_project = don_vi.Text,
                                user = user,
                                time = time,
                                mau_vat_lieu = material.Color,
                                do_trong_suot_vat_lieu = material.Transparency,
                                id_surface = material.SurfaceBackgroundPatternId,
                                mau_surface = material.SurfaceBackgroundPatternColor,
                                id_cut = material.CutBackgroundPatternId,
                                mau_cut = material.CutBackgroundPatternColor,
                                ton = ton,
                                color = Support.Check_Color(ma_cong_tac.Text),
                                color_sort = Support.Check_Color(ma_cong_tac.Text).ToString()
                            });
                            result = "S";
                        }
                        else
                        {
                            MessageBox.Show("The material id you supplied is already in use. Enter a unique id.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("The material name you supplied is already in use. Enter a unique name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The material name not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
