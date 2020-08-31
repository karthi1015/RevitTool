using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_ChooseColor
    {
        public static Brush choose_color(Button color_shading, Button color_surface, Button color_cut, ColorDialog colorDialog)
        {
            Brush color = new SolidColorBrush(Color.FromRgb(120, 120, 120));
            try
            {
                Color shading = ((SolidColorBrush)color_shading.Background).Color;
                Color surface = ((SolidColorBrush)color_surface.Background).Color;
                Color cut = ((SolidColorBrush)color_cut.Background).Color;
                colorDialog.CustomColors = new int[]
                {
                    BitConverter.ToInt32(new byte[4] { shading.R, shading.G , shading.B , 0}, 0),
                    BitConverter.ToInt32(new byte[4] { surface.R, surface.G , surface.B , 0}, 0),
                    BitConverter.ToInt32(new byte[4] { cut.R, cut.G , cut.B , 0}, 0)
                };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    color = new SolidColorBrush(Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return color;
        }
    }
}
