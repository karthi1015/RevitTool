using SetupTool_MaterialSetup.Code.Function;
using Autodesk.Revit.DB;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_SelectMaterialProject
    {
        public static void select_material(Document doc, ListView thong_tin_vat_lieu_project,
            TextBox ten_vat_lieu, TextBox ma_cong_tac, ComboBox don_vi,
            Button color_shading, Button color_surface, Button color_cut,
            Slider tranparency_bar, TextBox ton_value,
            ComboBox name_surface, ComboBox name_cut)
        {
            try
            {
                if (thong_tin_vat_lieu_project.SelectedItem != null)
                {
                    data_material_project item = (data_material_project)thong_tin_vat_lieu_project.SelectedItem;

                    string surface_name = "<None>";
                    if (item.id_surface.IntegerValue != -1)
                    {
                        surface_name = (doc.GetElement(item.id_surface) as FillPatternElement).GetFillPattern().Name;
                    }

                    string cut_name = "<None>";
                    if (item.id_cut.IntegerValue != -1)
                    {
                        cut_name = (doc.GetElement(item.id_cut) as FillPatternElement).GetFillPattern().Name;
                    }

                    Material material = item.material_project;

                    ten_vat_lieu.Text = item.ten_vat_lieu_project;
                    ma_cong_tac.Text = item.ma_cong_tac_project;
                    don_vi.Text = item.don_vi_project;
                    color_shading.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_vat_lieu.Red, item.mau_vat_lieu.Green, item.mau_vat_lieu.Blue));
                    tranparency_bar.Value = item.do_trong_suot_vat_lieu;
                    color_surface.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_surface.Red, item.mau_surface.Green, item.mau_surface.Blue));
                    name_surface.SelectedItem = CollectionViewSource.GetDefaultView(name_surface.ItemsSource).Cast<data_fill_pattern>().ToList().First(x => x.name == surface_name);
                    color_cut.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, item.mau_cut.Red, item.mau_cut.Green, item.mau_cut.Blue));
                    name_cut.SelectedItem = CollectionViewSource.GetDefaultView(name_cut.ItemsSource).Cast<data_fill_pattern>().ToList().First(x => x.name == cut_name);

                    ton_value.Text = item.ton;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
