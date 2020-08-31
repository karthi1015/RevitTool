using SetupTool_MaterialSetup.Code.Function;
using Autodesk.Revit.DB;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_GetMaterialProject
    {
        public static void get_material(Document doc, ObservableCollection<data_material_project> my_material_project)
        {
            string name = "";
            try
            {
                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();

                foreach (Material material in list_material)
                {
                    name = material.Name;
                    string ma_cong_tac_project = "";
                    if (F_GetSchema.Check_Para_And_Get_Para(material, Source.MCT[1], Source.MCT[0]) == null)
                    {
                        F_GetSchema.SetDataStorage(material, Source.MCT[1], Source.MCT[0], ma_cong_tac_project);
                    }
                    else ma_cong_tac_project = F_GetSchema.Check_Para_And_Get_Para(material, Source.MCT[1], Source.MCT[0]);

                    string don_vi_project = "Default";
                    if (string.IsNullOrEmpty(F_GetSchema.Check_Para_And_Get_Para(material, Source.DV[1], Source.DV[0])))
                    {
                        F_GetSchema.SetDataStorage(material, Source.DV[1], Source.DV[0], don_vi_project);
                    }
                    else don_vi_project = F_GetSchema.Check_Para_And_Get_Para(material, Source.DV[1], Source.DV[0]);

                    string ton = "2.5";
                    if (string.IsNullOrEmpty(F_GetSchema.Check_Para_And_Get_Para(material, Source.TON[1], Source.TON[0])))
                    {
                        F_GetSchema.SetDataStorage(material, Source.TON[1], Source.TON[0], ton);
                    }
                    else ton = F_GetSchema.Check_Para_And_Get_Para(material, Source.TON[1], Source.TON[0]);

                    string user = "";
                    if (F_GetSchema.Check_Para_And_Get_Para(material, Source.USER[1], Source.USER[0]) == null)
                    {
                        F_GetSchema.SetDataStorage(material, Source.USER[1], Source.USER[0], user);
                    }
                    else user = F_GetSchema.Check_Para_And_Get_Para(material, Source.USER[1], Source.USER[0]);

                    string time = "";
                    if (F_GetSchema.Check_Para_And_Get_Para(material, Source.TIME[1], Source.TIME[0]) == null)
                    {
                        F_GetSchema.SetDataStorage(material, Source.TIME[1], Source.TIME[0], time);
                    }
                    else time = F_GetSchema.Check_Para_And_Get_Para(material, Source.TIME[1], Source.TIME[0]);

                    my_material_project.Add(new data_material_project()
                    {
                        material_project = material,
                        ma_cong_tac_project = ma_cong_tac_project,
                        ten_vat_lieu_project = material.Name,
                        don_vi_project = don_vi_project,
                        user = user,
                        time = time,
                        mau_vat_lieu = Support.material_color(material.Color),
                        do_trong_suot_vat_lieu = material.Transparency,
                        id_surface = material.SurfaceBackgroundPatternId,
                        mau_surface = Support.material_color(material.SurfaceBackgroundPatternColor),
                        id_cut = material.CutBackgroundPatternId,
                        mau_cut = Support.material_color(material.CutBackgroundPatternColor),
                        ton = ton,
                        color = Support.Check_Color(ma_cong_tac_project),
                        color_sort = Support.Check_Color(ma_cong_tac_project).ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + name);
            }
        }
    }
}
