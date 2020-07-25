using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;


namespace Naviswork_ClashComment
{
    public class Support
    {
        public static BitmapImage Image_Base64(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                if (path != "[]")
                {
                    path = path.Replace("[\"", "").Replace("\"]", "").Split(',')[1];
                    var bytes = Convert.FromBase64String(path);
                    bitmap.BeginInit();
                    bitmap.StreamSource = new MemoryStream(bytes);
                    bitmap.EndInit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return bitmap;
        }
    }
}
