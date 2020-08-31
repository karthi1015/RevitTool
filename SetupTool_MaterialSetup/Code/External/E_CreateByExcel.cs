using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SetupTool_MaterialSetup.Code.Function;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SetupTool_MaterialSetup.Data.Binding;
using SetupTool_MaterialSetup.Code.Function.FunctionProject;
using System.Windows.Data;

namespace SetupTool_MaterialSetup.Code.External
{
    class E_CreateByExcel : IExternalEventHandler
    {
        public ListView thong_tin_vat_lieu_project { get; set; }
        public ObservableCollection<data_material_project> my_material_project { get; set; }

        public ObservableCollection<data_material_factor> my_material_factor { get; set; }
        public ListView thong_tin_he_so_vat_lieu_project { get; set; }

        public List<data_material_excel> data_create { get; set; }
        public string user { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Create Material Excel");

                string result = Tao_Vat_Lieu_Moi_Excel(uiapp, doc);

                transaction.Commit();
                if (result == "S")
                {
                    CollectionViewSource.GetDefaultView(thong_tin_vat_lieu_project.ItemsSource).Refresh();
                    MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    F_GetFactor.get_material_factor(doc, my_material_factor, thong_tin_he_so_vat_lieu_project);
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
            return "Create Material Excel";
        }

        //----------------------------------------------------------
        public string Tao_Vat_Lieu_Moi_Excel(UIApplication uiapp, Document doc)
        {
            string result = "F";
            string name = "";
            try
            {
                foreach (data_material_excel data in data_create)
                {
                    if (my_material_project.Any(x => x.ten_vat_lieu_project == data.ten_vat_lieu_excel) == false)
                    {
                        if (my_material_project.Any(x => x.ma_cong_tac_project == data.ma_cong_tac_excel) == false || string.IsNullOrEmpty(data.ma_cong_tac_excel))
                        {
                            try
                            {
                                ElementId material_id_new = Material.Create(doc, data.ten_vat_lieu_excel);
                                Material material = doc.GetElement(material_id_new) as Material;

                                string time = DateTime.Now.ToString();

                                material.Name = data.ten_vat_lieu_excel;
                                F_GetSchema.SetDataStorage(material, Source.MCT[1], Source.MCT[0], data.ma_cong_tac_excel);
                                F_GetSchema.SetDataStorage(material, Source.DV[1], Source.DV[0], data.don_vi_excel);
                                F_GetSchema.SetDataStorage(material, Source.TON[1], Source.TON[0], data.ton);
                                F_GetSchema.SetDataStorage(material, Source.USER[1], Source.USER[0], user);
                                F_GetSchema.SetDataStorage(material, Source.TIME[1], Source.TIME[0], time);
                                material.SurfaceBackgroundPatternId = data.id_surface_excel;
                                material.SurfaceBackgroundPatternColor = data.mau_surface_excel;
                                material.CutBackgroundPatternId = data.id_cut_excel;
                                material.CutBackgroundPatternColor = data.mau_cut_excel;
                                material.Color = data.mau_vat_lieu_excel;
                                material.Transparency = data.do_trong_suot_vat_lieu_excel;

                                F_SetAppearanceAsset.set_appearance_asset(doc, material);

                                my_material_project.Add(new data_material_project()
                                {
                                    material_project = material,
                                    ma_cong_tac_project = data.ma_cong_tac_excel,
                                    ten_vat_lieu_project = data.ten_vat_lieu_excel,
                                    don_vi_project = data.don_vi_excel,
                                    user = user,
                                    time = time,
                                    mau_vat_lieu = material.Color,
                                    do_trong_suot_vat_lieu = material.Transparency,
                                    id_surface = material.SurfaceBackgroundPatternId,
                                    mau_surface = material.SurfaceBackgroundPatternColor,
                                    id_cut = material.CutBackgroundPatternId,
                                    mau_cut = material.CutBackgroundPatternColor,
                                    ton = data.ton,
                                    color = Support.Check_Color(data.ma_cong_tac_excel),
                                    color_sort = Support.Check_Color(data.ma_cong_tac_excel).ToString()
                                });
                                result = "S";
                            }
                            catch (Exception)
                            {
                                name += data.ten_vat_lieu_excel + Environment.NewLine;
                            }
                        }
                        else
                        {
                            //MessageBox.Show(data.ma_cong_tac_excel + Environment.NewLine + "The material id you supplied is already in use. Enter a unique id.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            name += data.ten_vat_lieu_excel + Environment.NewLine;
                        }
                    }
                    else
                    {
                        //MessageBox.Show(data.ten_vat_lieu_excel + Environment.NewLine + "The material name you supplied is already in use. Enter a unique name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        name += data.ten_vat_lieu_excel + Environment.NewLine;
                    }
                }
                if(!string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("The material name error :" + Environment.NewLine + name, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
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
