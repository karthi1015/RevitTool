using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Color = Autodesk.Revit.DB.Color;
using Color1 = System.Drawing.Color;
using Color2 = System.Windows.Media.Color;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Image = System.Windows.Controls.Image;
using System.Windows.Media.Imaging;
using ListView = System.Windows.Controls.ListView;
using SetupTool_MaterialSetup.Data.Binding;
using System.Diagnostics;

namespace SetupTool_MaterialSetup
{
    class Support
    {
        //----------------------------------------------------------
        public static string RemoveUnicode(string text)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = text.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(text[index]);
                text = text.Replace(text[index], ReplText[index2]);
            }
            return text;
        }

        //----------------------------------------------------------
        public static Brush Check_Color(string data_check)
        {
            Brush color = Source.color;
            try
            {
                var key_company = data_check.Split('.');
                if (string.IsNullOrEmpty(data_check) || (key_company.Count() > 0 && Source.desciplines.Any(x => x.key == key_company[0]) == false))
                {
                    color = Source.color_error;
                }
            }
            catch (Exception)
            {

            }
            return color;
        }

        public static Color material_color(Color color)
        {
            try
            {
                if (color == null)
                    return Source.material_color;
                else if (color.Red == 0 && color.Green == 0 && color.Blue == 0)
                    return Source.material_color;
                else
                    return color;
            }
            catch (Exception)
            {
                return Source.material_color;
            }
        }
    }
}
