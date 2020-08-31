using Autodesk.Revit.DB;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using Rectangle = System.Drawing.Rectangle;
using Matrix = System.Drawing.Drawing2D.Matrix;
using Pen = System.Drawing.Pen;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Interop;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_ImagePattern
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
        public static BitmapImage BimapToImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            (src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        public static Bitmap CreateFillPatternImage(FillPattern fillPattern)
        {
            var width = 30;
            var height = 30;
            Bitmap fillPatternImg = new Bitmap((int)width, (int)height);
            try
            {
                using (var g = Graphics.FromImage(fillPatternImg))
                {
                    var rect = new System.Drawing.Rectangle(0, 0, (int)width, (int)height);
                    g.FillRectangle(System.Drawing.Brushes.Transparent, rect);
                    DrawFillPattern(g, fillPattern);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return fillPatternImg;
        }

        private static void DrawFillPattern(Graphics g, FillPattern fillPattern)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (fillPattern == null)
                return;

            float matrixScale = 1000;

            try
            {
                var width = 30;

                var height = 30;

                var rect = new System.Drawing.Rectangle(0, 0, (int)width, (int)height);

                var rect_border = new System.Drawing.Rectangle(0, 0, (int)width - 1, (int)height - 1);
                Pen myPen = new Pen(System.Drawing.Color.FromArgb(0, 255, 220), 1);

                g.DrawRectangle(myPen, rect_border);

                var centerX = (rect.Left + rect.Left + rect.Width) / 2;

                var centerY = (rect.Top + rect.Top + rect.Height) / 2;

                g.TranslateTransform(centerX, centerY);

                var rectF = new System.Drawing.Rectangle(-1, -1, 2, 2);

                g.FillRectangle(System.Drawing.Brushes.Transparent, rectF);

                g.ResetTransform();

                var fillGrids = fillPattern.GetFillGrids();

                foreach (var fillGrid in fillGrids)
                {
                    var degreeAngle = (float)fillGrid.Angle * 180 / Math.PI;

                    var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(0, 255, 220))
                    {
                        Width = 1f / matrixScale
                    };

                    float dashLength = 1;

                    var segments = fillGrid.GetSegments();

                    if (segments.Count > 0)
                    {
                        pen.DashPattern = segments
                           .Select(s => Math.Max(float.Epsilon, Convert.ToSingle(s)))
                            .ToArray();

                        dashLength = pen.DashPattern.Sum();
                    }

                    g.ResetTransform();

                    var rotateMatrix = new System.Drawing.Drawing2D.Matrix();
                    rotateMatrix.Rotate((float)degreeAngle);

                    var matrix = new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1,
                      centerX, centerY);

                    matrix.Scale(matrixScale, matrixScale);

                    matrix.Translate((float)fillGrid.Origin.U,
                      (float)fillGrid.Origin.V);

                    var backMatrix = matrix.Clone();
                    backMatrix.Multiply(rotateMatrix);
                    matrix.Multiply(rotateMatrix);

                    var offset = (-10) * dashLength;
                    matrix.Translate(offset, 0);
                    backMatrix.Translate(offset, 0);

                    bool moving_forward = true;
                    bool moving_back = true;
                    int safety = 500;
                    double alternator = 0;
                    while (moving_forward || moving_back) // draw segments shifting and offsetting each time
                    {
                        var rectF1 = new RectangleF(-2 / matrixScale, -2 / matrixScale, 4 / matrixScale, 4 / matrixScale);

                        if (moving_forward && LineIntersectsRect(matrix, rect))
                        {
                            g.Transform = matrix;
                            g.DrawLine(pen, new PointF(0, 0), new PointF(100, 0));
                        }
                        else
                        {
                            moving_forward = false;
                        }

                        if (moving_back && LineIntersectsRect(backMatrix, rect))
                        {
                            g.Transform = backMatrix;
                            g.DrawLine(pen, new PointF(0, 0), new PointF(100, 0));
                        }
                        else
                        {
                            moving_back = false;
                        }

                        if (safety == 0)
                        {
                            break;
                        }
                        else --safety;

                        matrix.Translate((float)fillGrid.Shift, (float)fillGrid.Offset);
                        backMatrix.Translate(-(float)fillGrid.Shift, -(float)fillGrid.Offset);

                        alternator += fillGrid.Shift;
                        if (Math.Abs(alternator) > Math.Abs(offset))
                        {
                            matrix.Translate(offset, 0);
                            backMatrix.Translate(offset, 0);
                            alternator = 0d;
                        }
                    }
                }

                sw.Stop();
                g.ResetTransform();

                Pen p = new Pen(System.Drawing.Color.Teal);
                p.Width = 1f / matrixScale;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static bool LineIntersectsRect(Matrix rayMatrix, Rectangle r)
        {
            Matrix m = rayMatrix.Clone();
            m.Translate(200, 0);
            return LineIntersectsRect(new System.Drawing.Point((int)rayMatrix.OffsetX, (int)rayMatrix.OffsetY), new System.Drawing.Point((int)m.OffsetX, (int)m.OffsetY), r);
        }

        public static bool LineIntersectsRect(
          System.Drawing.Point p1,
          System.Drawing.Point p2,
          Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new System.Drawing.Point(r.X, r.Y), new System.Drawing.Point(r.X + r.Width, r.Y))
              || LineIntersectsLine(p1, p2, new System.Drawing.Point(r.X + r.Width, r.Y), new System.Drawing.Point(r.X + r.Width, r.Y + r.Height))
              || LineIntersectsLine(p1, p2, new System.Drawing.Point(r.X + r.Width, r.Y + r.Height), new System.Drawing.Point(r.X, r.Y + r.Height))
              || LineIntersectsLine(p1, p2, new System.Drawing.Point(r.X, r.Y + r.Height), new System.Drawing.Point(r.X, r.Y))
              || (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(
          System.Drawing.Point l1p1,
          System.Drawing.Point l1p2,
          System.Drawing.Point l2p1,
          System.Drawing.Point l2p2)
        {
            try
            {
                Int64 d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);
                if (d == 0) return false;

                Int64 q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
                Int64 r = q / d;

                Int64 q1 = (Int64)(l1p1.Y - l2p1.Y) * (Int64)(l1p2.X - l1p1.X);
                Int64 q2 = (Int64)(l1p1.X - l2p1.X) * (Int64)(l1p2.Y - l1p1.Y);

                q = q1 - q2;
                Int64 s = q / d;

                if (r < 0 || r > 1 || s < 0 || s > 1)
                    return false;

                return true;
            }
            catch (OverflowException err)
            {
                return false;
            }
        }
    }
}
