using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SetupTool_TypeSetup.Code.Function
{
    class F_ImageFamily
    {
        //----------------------------------------------------------
        public static BitmapImage ConvertBitmapToBitmapSource(ElementType type)
        {
            Size imgSize = new Size(256, 256);
            Bitmap bm = type.GetPreviewImage(imgSize);
            bm.MakeTransparent(System.Drawing.Color.FromArgb(120,120,120));

            MemoryStream ms = new MemoryStream();
            (bm).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }
    }
}
