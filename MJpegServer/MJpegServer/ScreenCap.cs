using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MJpegServer
{
    class ScreenCap
    {
        static Size size = new Size(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
        static Bitmap srcImage;
        static Graphics srcGraphics;
        static bool scaled;
        static Bitmap dstImage;
        static Graphics dstGraphics;
        static bool init=false;
        static int windowwidth;
        static int windowheight;
        static bool showcur;
        static Rectangle src;
        static Rectangle dst;
        static Size curSize;
        static Point cp;
        public static void Init(int width, int height, bool showCursor)
        {
            if (windowheight == height && windowwidth == width)
            {
                return;
            }
            if (init)
            {
                srcGraphics.Dispose();
                dstGraphics.Dispose();

                srcImage.Dispose();
                dstImage.Dispose();
                System.GC.Collect();
            }
            showcur = showCursor;
            windowwidth = width;
            windowheight = height;
            srcImage = new Bitmap(size.Width, size.Height);
            srcGraphics = Graphics.FromImage(srcImage);

            scaled = (width != size.Width || height != size.Height);

            dstImage = srcImage;
            dstGraphics = srcGraphics;

            if (scaled)
            {
                dstImage = new Bitmap(windowwidth, windowheight);
                dstGraphics = Graphics.FromImage(dstImage);
            }

            src = new Rectangle(0, 0, size.Width, size.Height);
            dst = new Rectangle(0, 0, windowwidth, windowheight);
            curSize = new Size(32, 32);
            init = true;
        }

        public static Image GetImage()
        {
            srcGraphics.CopyFromScreen(0, 0, 0, 0, size);
            if (showcur)
            {
                cp = Cursor.Position;
                //cp.X -= 32;
                //cp.Y -= 32;
                Cursors.Default.Draw(srcGraphics, new Rectangle(cp, curSize));
            }
            if (scaled)
                dstGraphics.DrawImage(srcImage, dst, src, GraphicsUnit.Pixel);
            return dstImage;
        }
    }
}
