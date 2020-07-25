using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using Tool_CheckUpdateModel.Data;
using Button = System.Windows.Controls.Button;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using ToggleButton = System.Windows.Controls.Primitives.ToggleButton;

namespace Tool_CheckUpdateModel.Function
{
    class F_Button
    {
        public static void State_Control(string name, ToggleButton column, ToggleButton framing, ToggleButton floor, ToggleButton wall)
        {
            try
            {
                List<ToggleButton> list_control = new List<ToggleButton>() { column, framing, floor, wall };

                foreach (ToggleButton button in list_control)
                {
                    if (button.Name != name) button.IsChecked = false;
                    else button.IsChecked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
