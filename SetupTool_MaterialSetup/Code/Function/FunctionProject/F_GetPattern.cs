using Autodesk.Revit.DB;
using SetupTool_MaterialSetup.Data.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_GetPattern
    {
        public static void get_pattern(Document doc, ComboBox name_cut, ComboBox name_surface)
        {
            try
            {
                List<data_fill_pattern> my_fill_pattern = new List<data_fill_pattern>();
                foreach (FillPatternElement pattern in new FilteredElementCollector(doc).OfClass(typeof(FillPatternElement)).ToElements())
                {
                    if (pattern.GetFillPattern().Target == FillPatternTarget.Drafting)
                    {
                        BitmapImage source = F_ImagePattern.BimapToImage(F_ImagePattern.CreateFillPatternImage(pattern.GetFillPattern()));
                        my_fill_pattern.Add(new data_fill_pattern()
                        {
                            name = pattern.GetFillPattern().Name,
                            image = source,
                            pattern_id = pattern.Id
                        });
                    }
                }
                my_fill_pattern.Insert(0, new data_fill_pattern()
                {
                    name = "<None>",
                    image = null,
                    pattern_id = new ElementId(-1)
                });
                name_cut.ItemsSource = my_fill_pattern;
                name_surface.ItemsSource = my_fill_pattern;
                name_cut.SelectedIndex = 0;
                name_surface.SelectedIndex = 0;

                CollectionView view_cut = (CollectionView)CollectionViewSource.GetDefaultView(name_cut.ItemsSource);
                view_cut.SortDescriptions.Add(new System.ComponentModel.SortDescription("name", System.ComponentModel.ListSortDirection.Ascending));
                CollectionView view_surface = (CollectionView)CollectionViewSource.GetDefaultView(name_surface.ItemsSource);
                view_surface.SortDescriptions.Add(new System.ComponentModel.SortDescription("name", System.ComponentModel.ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
