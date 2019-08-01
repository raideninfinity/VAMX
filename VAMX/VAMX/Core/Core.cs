using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Dynamic;
using System.Reflection;

namespace VAMX
{
    public static class Core
    {
        static string map_path;
        static string graphics_path;

        public static string MapPath { get { return map_path; } set { map_path = value; } }
        public static string GraphicsPath { get { return graphics_path; } set { graphics_path = value; } }

        static MapX map;
        public static MapX Map { get { return map; } }
        public static void SetMap(MapX m) { map = m; }

        public static void LoadMap()
        {
            map = RubyCore.LoadMap(map_path);
        }

        public static void SaveMap()
        {
            RubyCore.SaveMap(map, map_path);
        }

        public static void UnloadMap()
        {
            map_path = "";
            graphics_path = "";
            map = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        static float zoom = 1.0f;
        public static float Zoom { get { return zoom; } }
        public static void SetZoom(float z) { zoom = z; }

        #region Editor Loading

        static Editor ed;
        public static Editor Editor { get { return ed; } }

        private static List<Bitmap> flag_images = new List<Bitmap>();
        private static Bitmap berase;
        private static Bitmap bauto;
        public static List<Bitmap> FlagImages { get { return flag_images; } }
        public static Bitmap BErase { get { return berase; } }
        public static Bitmap BAuto { get { return bauto; } }

        public static void InitEditor(MainForm form)
        {
            ed = new Editor(form);
            flag_images.Clear();
            flag_images.Add(((Bitmap)Image.FromFile(Path.Combine(RubyCore.SelfPath, "res", "pass.png"))).ChangeImageOpacity(0.8f));
            flag_images.Add(((Bitmap)Image.FromFile(Path.Combine(RubyCore.SelfPath, "res", "flags.png"))).ChangeImageOpacity(0.8f));
            flag_images.Add(((Bitmap)Image.FromFile(Path.Combine(RubyCore.SelfPath, "res", "regions.png"))).ChangeImageOpacity(0.8f));
            bauto = new Bitmap(ResizeImage((Bitmap)Image.FromFile(RubyCore.SelfPath + @"\res\auto.png"), 32, 32));
            berase = new Bitmap(ResizeImage((Bitmap)Image.FromFile(RubyCore.SelfPath + @"\res\erase.png"), 32, 32));
            PrepareGrid();
            PrepareIndic();
        }

        static Dictionary<float, Bitmap> grid_cache = new Dictionary<float, Bitmap>();
        static Dictionary<float, Bitmap> indic_cache = new Dictionary<float, Bitmap>();

        public static Dictionary<float, Bitmap> GridCache { get { return grid_cache; } }
        public static Dictionary<float, Bitmap> IndicCache { get { return indic_cache; } }

        public static void PrepareGrid()
        {
            grid_cache.Clear();
            var b = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetGrid(g, 32, 32);
            }
            grid_cache.Add(1.0f, (Bitmap)ChangeImageOpacity(b, 0.4f));
            b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetGrid(g, 16, 16);
            }
            grid_cache.Add((1.0f / 2.0f), (Bitmap)ChangeImageOpacity(b, 0.4f));
            b = new Bitmap(8, 8);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetGrid(g, 8, 8);
            }
            grid_cache.Add((1.0f / 4.0f), (Bitmap)ChangeImageOpacity(b, 0.4f));
        }

        public static void GetGrid(Graphics g, int w, int h)
        {
            g.FillRectangle(Brushes.Black, 0, 0, w, 1);
            g.FillRectangle(Brushes.Black, 0, 0, 1, h);
            g.FillRectangle(Brushes.Black, 0, h - 1, w, 1);
            g.FillRectangle(Brushes.Black, w - 1, 0, 1, h);
        }

        public static void PrepareIndic()
        {
            float op = 0.8f;
            indic_cache.Clear();
            var b = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetIndic(g, 32, 32);
            }
            indic_cache.Add(1.0f, (Bitmap)ChangeImageOpacity(b, op));
            b = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetIndic(g, 16, 16);
            }
            indic_cache.Add((1.0f / 2.0f), (Bitmap)ChangeImageOpacity(b, op));
            b = new Bitmap(8, 8);
            using (Graphics g = Graphics.FromImage(b))
            {
                GetIndic(g, 8, 8);
            }
            indic_cache.Add((1.0f / 4.0f), (Bitmap)ChangeImageOpacity(b, op));
        }

        public static void GetIndic(Graphics g, int w, int h)
        {
            g.FillRectangle(Brushes.White, 0, 0, w, 2);
            g.FillRectangle(Brushes.White, 0, 0, 2, h);
            g.FillRectangle(Brushes.White, 0, h - 2, w, 2);
            g.FillRectangle(Brushes.White, w - 2, 0, 2, h);
        }


        #endregion

        #region Data Operations

        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static int div(this int a, int b)
        {
            return ((a - a % b) / b);
        }

        #endregion

        #region Image Operations

        public static Bitmap ResizeImage(this Bitmap image, int width, int height)
        {
            if (image.Width == width && image.Height == height) { return image; }

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public static Bitmap ChangeImageOpacity(this Bitmap originalImage, float opacity)
        {
            if ((originalImage.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return originalImage;
            }

            Bitmap bmp = (Bitmap)originalImage.Clone();

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * 4;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += 4)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + 4 - 1] == 0)
                    continue;

                int pos = 0;
                pos++; // B value
                pos++; // G value
                pos++; // R value

                argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        public static Bitmap CreateBack32(int w, int h)
        {
            int imgw = w * 32; int imgh = h * 32;
            if (imgw == 0) { imgw = 1; }
            if (imgh == 0) { imgh = 1; }
            Bitmap basebmp = new Bitmap(imgw, imgh);
            return basebmp;
        }

        #endregion

    }
}
