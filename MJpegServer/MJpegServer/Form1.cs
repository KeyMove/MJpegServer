using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KMButton;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Net;
using HIDTEST;

namespace MJpegServer
{
    public partial class Form1 : Form
    {

        MJPEGStream MjpegServer=new MJPEGStream();
        HTTP WebServer;
        Bitmap map;
        Image lastImage;
        Graphics g;
        Object lua;

        Bitmap drawmap;
        Graphics draw;
        Size newSize=new Size();
        bool isNewSize = false;

        List<Timer> TimerList = new List<Timer>();
        List<Object> FunList = new List<Object>();

        USBHID HID = new USBHID();

        Timer UpdateImage = new Timer();

        int WindowW= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        int WindowH= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        int updatevalue=0;

        protected override void WndProc(ref Message m)
        {
            if(HID.CheckMessage(m)>0)
                if (HIDCheck.Enabled)
                {
                    Input.TestHID();
                }
            base.WndProc(ref m);
        }

        internal static int GetPort()
        {
            bool inUse = false;

            int port = 80;

            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();

            Random randport = new Random();

            while (true) {
                inUse = false;
                foreach (IPEndPoint endPoint in ipEndPoints)
                {
                    if (endPoint.Port == port)
                    {
                        inUse = true;
                        break;
                    }
                }
                if (!inUse)
                    return port;
                port = randport.Next(1000, 65535);
            }
        }

        public void TestHID()
        {
            Input.TestHID();
        }

        public void KeyClear()
        {
            Input.Clear();
        }

        public void KeyUP(int vk)
        {
            Input.KeyInput(vk, Input.KeyFlag.Up);
        }

        public void KeyDOWN(int vk)
        {
            Input.KeyInput(vk, Input.KeyFlag.Down);
        }

        public void MouseLeftDOWN()
        {
            Input.MouseInput(0, 0, Input.MouseFlag.LeftDown);
        }

        public void MouseLeftUP()
        {
            Input.MouseInput(0, 0, Input.MouseFlag.LeftUp);
        }

        public void MouseRightDOWN()
        {
            Input.MouseInput(0, 0, Input.MouseFlag.RightDown);
        }

        public void MouseRightUP()
        {
            Input.MouseInput(0, 0, Input.MouseFlag.RightUp);
        }

        public void MouseAddMove(int x,int y)
        {
            Input.MouseInput(x, y);
        }

        public void MouseMoveTo(int x,int y)
        {
            Input.setDest(new Point(x, y), Cursor.Position);
        }

        public int MouseOffset(int x,int y,int max=10)
        {
            int xs = x - Cursor.Position.X;
            int ys = y - Cursor.Position.Y;
            xs = (xs > 0) ? (xs > max) ? max : xs : (-xs > max) ? -max : xs;
            ys = (ys > 0) ? (ys > max) ? max : ys : (-ys > max) ? -max : ys;
            return xs << 16 + ys;
        }

        public bool MouseStepInt(int loc, int max = 10)
        {
            int x = loc >> 16;
            int y = loc & 0xffff;
            int xs = x - Cursor.Position.X;
            int ys = y - Cursor.Position.Y;
            int incx, incy;
            if (xs > 0)
                incx = xs > (max / 2 + 1) ? max : 0;
            else
                incx = -xs > (max / 2 + 1) ? -max : 0;
            if (ys > 0)
                incy = ys > (max / 2 + 1) ? max : 0;
            else
                incy = -ys > (max / 2 + 1) ? -max : 0;


            if (incx == 0 && incy == 0) return false;
            Input.MouseInput(incx, incy);
            return true;
        }

        public bool MouseStep(int x,int y,int max=10)
        {
            int xs = x - Cursor.Position.X;
            int ys = y - Cursor.Position.Y;
            int incx, incy;
            if (xs > 0)
                incx = xs > max/2 ? max : 0;
            else
                incx = -xs > max/2 ? -max : 0;
            if (ys > 0)
                incy = ys > max/2 ? max : 0;
            else
                incy = -ys > max/2 ? -max : 0;


            if (incx == 0 && incy == 0) return false;
            Input.MouseInput(incx, incy);
            return true;
        }

        //public Object getDM()
        //{
        //    return Input.getDM();
        //}

        void InitLua()
        {
            try
            {
                NLua.Lua VM = new NLua.Lua();
                VM.LoadCLRPackage();
                VM.RegisterFunction("print", this, this.GetType().GetMethod("msg"));
                VM["Handle"] = this;
                lua = VM;
                LoadLua.Visible = true;
            }
            catch { };
        }

        public int msg(object arg)
        {
            if(arg==null)
                OutputBox.AppendText("nil");
            else
                OutputBox.AppendText(arg.ToString());
            return -1;
        }

        void InitLua(string filepath)
        {
            try
            {
                if(lua!=null)
                    (lua as NLua.Lua).DoFile(filepath);
            }
            catch { }
        }

        public Bitmap UpdateScreen()
        {
            //if (lastImage != null) lastImage.Dispose();
            lastImage = ScreenCap.GetImage();
            return lastImage as Bitmap;
        }
        int[] colorData;
        public void UpdateScreenColor() 
        {
            Size s = ScreenCap.Size;
            if (colorData == null)
                colorData = new int[s.Width*s.Height];
            ScreenCap.GetImageForByte(0, 0, s.Width, s.Height, colorData);
        }


        public List<int> ScanRectRGB(int r, int g, int b, int x, int y, int w, int h,int mode, int offset = 0)
        {
            List<int> plist = new List<int>();
            if (colorData == null) return plist;
            int color = (r << 16) + (g << 8) + b;
            color += 0xff << 24;
            var s = ScreenCap.Size;
            int count = s.Width * s.Height;
            int index = 0;
            int sr = r, sg = g, sb = b;
            int incx, incy;
            incx = (mode & 1) > 0 ? -1 : 1;
            incy = (mode & 2) > 0 ? -1 : 1;
            index = y;
            index *= s.Width;
            index += x;
            if ((mode & 4) > 0)
            {
                if (incy > 0)
                {
                    if (incx > 0)
                        incy = -(s.Width * (h-1) - 1);
                    else
                    {
                        incy = -(s.Width * (h-1) + 1);
                        index += w - 1;
                    }
                    incx = s.Width;
                }
                else
                {
                    index += (h - 1) * s.Width;
                    if (incx > 0)
                        incy = (s.Width * (h - 1) + 1);
                    else
                    {
                        incy = (s.Width * (h - 1) - 1);
                        index += w - 1;
                    }
                    incx = -s.Width;
                }
                int swap;
                swap = h;
                h = w;
                w = swap;
                w--;
            }
            else
            {
                if (incy > 0)
                {
                    if (incx > 0)
                        incy *= s.Width - w;
                    else
                    {
                        incy *= s.Width + w;
                        index += w - 1;
                    }
                }
                else
                {
                    index += (h - 1) * s.Width;
                    if (incx > 0)
                        incy *= s.Width + w;
                    else
                    {
                        incy *= s.Width - w;
                        index += w - 1;
                    }
                }
            }
            if (offset != 0)
            {
                for (int sy = 0; sy < h; sy++)
                {
                    for (int sx = 0; sx < w; sx++)
                    {
                        color = colorData[index];
                        r = sr - 0xff & (color >> 16);
                        g = sg - 0xff & (color >> 8);
                        b = sb - 0xff & color;
                        if (r < 0) r = -r;
                        if (g < 0) g = -g;
                        if (b < 0) b = -b;
                        if (r > offset) continue;
                        if (g > offset) continue;
                        if (b > offset) continue;
                        plist.Add(((index % s.Width) << 16) + (index / s.Width));
                        index+=incx;
                    }
                    index += incy;
                }
            }
            else
            {
                for (int sy = 0; sy < h; sy++)
                {
                    for (int sx = 0; sx < w; sx++)
                    {
                        if (colorData[index] == color)
                        {
                            plist.Add(((index % s.Width) << 16) + (index / s.Width));
                        }
                        index+=incx;
                    }
                    index += incy;
                }
            }
            return plist;
        }

        public void DrawBlock(int loc,int w,int h)
        {
            int x = loc >> 16;
            int y = loc & 0xffff;
            draw.FillRectangle(Brushes.Black, x, y, w, h);
        }

        public void addFillBlock(int x,int y,int w,int h,int value)
        {
            var s = ScreenCap.Size;
            int index = y;
            index *= s.Width;
            index += x;
            for (int sy = 0; sy < h; sy++)
            {
                for (int sx = 0; sx < w; sx++)
                {
                    colorData[index]=value;
                    index++;
                }
                index += s.Width - w;
            }
        }

        public List<int> FindRectRGB(int r,int g,int b,int x,int y,int w,int h,int offset=0)
        {
            List<int> plist = new List<int>();
            if (colorData == null) return plist;
            int color = (r << 16) + (g << 8) + b;
            color += 0xff << 24;
            var s = ScreenCap.Size;
            int count = s.Width * s.Height;
            int index = 0;
            int sr = r, sg = g, sb = b;
            index = y;
            index *= s.Width;
            index += x;
            if (offset != 0)
            {
                for (int sy = 0; sy < h; sy++)
                {
                    for (int sx = 0; sx < w; sx++)
                    {
                        color = colorData[index];
                        r = sr - 0xff & (color >> 16);
                        g = sg - 0xff & (color >> 8);
                        b = sb - 0xff & color;
                        if (r < 0) r = -r;
                        if (g < 0) g = -g;
                        if (b < 0) b = -b;
                        if (r > offset) continue;
                        if (g > offset) continue;
                        if (b > offset) continue;
                        plist.Add(((index % s.Width) << 16) + (index / s.Width));
                        index++;
                    }
                    index += s.Width - w;
                }
            }
            else
            {
                for (int sy = 0; sy < h; sy++)
                {
                    for (int sx = 0; sx < w; sx++)
                    {
                        if (colorData[index] == color)
                        {
                            plist.Add(((index % s.Width) << 16) + (index / s.Width));
                        }
                        index++;
                    }
                    index += s.Width - w;
                }
            }
            return plist;
        }

        public List<int> FindRGB(int r, int g, int b, int offset = 0)
        {
            List<int> plist = new List<int>();
            if (colorData == null) return plist;
            int color = (r << 16) + (g << 8) + b;
            color += 0xff << 24;
            var s = ScreenCap.Size;
            int count = s.Width * s.Height;
            int index = 0;
            int sr=r, sg=g, sb=b;
            if (offset!=0)
                for (int i = 0; i < count; i++)
                {
                    color = colorData[index++];
                    r = sr - (0xff & (color >> 16));
                    g = sg - (0xff & (color >> 8));
                    b = sb - (0xff & color);
                    if (r < 0) r = -r;
                    if (g < 0) g = -g;
                    if (b < 0) b = -b;
                    if (r > offset) continue;
                    if (g > offset) continue;
                    if (b > offset) continue;
                    plist.Add(((i % s.Width) << 16) + (i / s.Width));
                }
            else
                for (int i = 0; i < count; i++)
                {
                    if (colorData[index++] == color)
                    {
                        plist.Add(((i % s.Width) << 16) + (i / s.Width));
                    }
                }
            return plist;
        }

        public void DrawMark(int x,int y)
        {
            draw.DrawEllipse(Pens.Red, x - 5, y - 5, 10, 10);
        }

        public void DrawMark(int b)
        {
            int x=b>>16, y=b&0xffff;
            draw.DrawEllipse(Pens.Red, x - 5, y - 5, 10, 10);
            DrawTest.Image = drawmap;
        }

        public void DrawScreen()
        {
            var s = ScreenCap.Size;
            drawmap = (Bitmap)ScreenCap.GetImageFor(0, 0, s.Width, s.Height);
            DrawTest.Size = s;
            DrawTest.Image = drawmap;
            draw = Graphics.FromImage(drawmap);
        }

        public int[] LuaTable(NLua.LuaTable tab)
        {
            int[] val = new int[tab.Values.Count];
            int index = 0;
            foreach(double i in tab.Values)
            {
                val[index++] = (int)i;
            }
            return val;
        }

        public int[] matchOffsetList(int[] px,int r,int g,int b,int offset,List<int> plist,bool mult = false) 
        {
            List<int> matchpos = new List<int>();
            for (int i = 0; i < plist.Count; i++)
            {
                int bz = plist[i];
                int lx = bz >> 16;
                int ly = bz & 0xffff;
                int j = 0;
                int i1 = 0;
                bool e = true;
                for (j = 0; j < px.Length / 2; j++)
                {
                    int v = ((lx + px[i1]) << 16) + ((ly + px[i1 + 1]) & 0xffff);
                    if (!matchRGBInt(v, r, g, b, offset))
                    {
                        e = false;
                        break;
                    }
                    i1 += 2;
                }
                if (e)
                {
                    if (!mult)
                        return new int[] { bz };
                    else
                        matchpos.Add(bz);
                }
            }
            if (matchpos.Count == 0) return null;
            return matchpos.ToArray();
        }

        public List<int> removeRectPointInt(int loc, int w, int h, List<int> plist, int start = 0)
        {
            int x = loc >> 16;
            int y = loc & 0xffff;
            return removeRectPoint(x, y, w, h, plist, start);
        }

        public List<int> removeRectPoint(int x,int y,int w,int h, List<int> plist,int start=0)
        {
            if (start != 0)
            {
                if(plist.Contains(start))
                plist.RemoveRange(0, plist.IndexOf(start));
            }
            int count = plist.Count;
            int index = 0;
            Rectangle r = new Rectangle(x, y, w, h);
            while (count > 0)
            {
                int px = plist[index] >> 16;
                int py = plist[index] & 0xffff;

                if (r.Contains(px, py))
                    plist.RemoveAt(index);
                else
                    index++;
                count--;
            }
            return plist;
        }

        public List<int> getRectPointInt(int loc, int w, int h, List<int> plist, int start=0)
        {
            int x = loc >> 16;
            int y = loc & 0xffff;
            return getRectPoint(x, y, w, h, plist, start);
        }

        public List<int> getRectPoint(int x,int y,int w,int h,List<int> plist,int start=0)
        {
            List<int> newplist = new List<int>();
            if (start != 0)
            {
                if (plist.Contains(start))
                    plist.RemoveRange(0, plist.IndexOf(start));
            }
            int count = plist.Count;
            int index = 0;
            Rectangle r = new Rectangle(x, y, w, h);
            while (count > 0)
            {
                int px = plist[index] >> 16;
                int py = plist[index] & 0xffff;
                if (r.Contains(px, py))
                    newplist.Add(plist[index]);
                index++;
                count--;
            }
            return newplist;
        }

        public int[] matchPointList(int[] px, int[] py, int r,int g,int b,bool mult=false)
        {
            return matchPointList(px, py, FindRGB(r, g, b),mult);
        }

        public int[] matchPointList(int[] px,int[] py, List<int> plist,bool mult=false)
        {
            List<int> matchpos = new List<int>();
            for(int i = 0; i < plist.Count; i++)
            {
                int b = plist[i];
                int lx = b >> 16;
                int ly = b & 0xffff;
                int j = 0;
                int i1=0;
                bool e=true;
                for (j = 0; j < px.Length/2; j++)
                {
                    int v = ((lx + px[i1]) << 16) + ((ly + px[i1 + 1]) & 0xffff);
                    if (!plist.Contains(v))
                    {
                        e = false;
                        break;
                    }
                    i1 += 2;
                }
                i1 = 0;
                for (j = 0; j < py.Length/2; j++)
                {
                    int v = ((lx + py[i1]) << 16) + ((ly + py[i1 + 1]) & 0xffff);
                    if (plist.Contains(v))
                    {
                        e = false;
                        break;
                    }
                    i1 += 2;
                }
                if (e)
                {
                    if (!mult)
                        return new int[] { b };
                    else
                        matchpos.Add(b);
                }
            }
            if (matchpos.Count == 0) return null;
            return matchpos.ToArray();
        }

        public int[] matchPointListInt(int[] px, int[] py, List<int> plist, bool mult = false)
        {
            List<int> matchpos = new List<int>();
            for (int i = 0; i < plist.Count; i++)
            {
                int b = plist[i];
                int lx = b >> 16;
                int ly = b & 0xffff;
                int j = 0;
                int i1 = 0;
                bool e = true;
                for (j = 0; j < px.Length / 2; j++)
                {
                    int v = b+px[i1];
                    if (!plist.Contains(v))
                    {
                        e = false;
                        break;
                    }
                    i1 ++;
                }
                i1 = 0;
                for (j = 0; j < py.Length / 2; j++)
                {
                    int v = b + py[i1];
                    if (plist.Contains(v))
                    {
                        e = false;
                        break;
                    }
                    i1 ++;
                }
                if (e)
                {
                    if (!mult)
                        return new int[] { b };
                    else
                        matchpos.Add(b);
                }
            }
            if (matchpos.Count == 0) return null;
            return matchpos.ToArray();
        }

        public bool matchRGBInt(int loc, int r, int g, int b, int offset = 0)
        {
            var s = ScreenCap.Size;
            int p = (loc >> 16) + (loc & 0xffff) * s.Width;
            if (p > s.Width * s.Height) return false;

            if (offset == 0)
            {
                if ((0xffffff & colorData[p]) != ((r << 16) | (g << 8) | b)) return false;
            }
            else
            {
                r -= 0xff&(colorData[p]>>16);
                g -= 0xff & (colorData[p]>>8);
                b -= 0xff & colorData[p];
                if (r < 0) r = -r;
                if (g < 0) g = -g;
                if (b < 0) b = -b;
                if (r > offset) return false;
                if (g > offset) return false;
                if (b > offset) return false;
            }
            return true;
        }

        public int offset(int loc,int x,int y)
        {
            return (((loc >> 16) + x) << 16) + ((loc & 0xffff) + y);
        }

        public List<int> RGBNearPoint(int x, int y, int w, int h, int min = 5, int max = int.MaxValue,int rx=8)
        {
            int[] values = RGBSortRect(x, y, w, h, min, max);
            List<int> tlist = new List<int>();
            Dictionary<int, int> rvalue = new Dictionary<int, int>();
            Dictionary<int, int> tvalue = new Dictionary<int, int>();
            for (int i = 0; i < values.Length; i += 2)
            {
                int r = (values[i] >> 16) & 0xff;
                int g = (values[i] >> 8) & 0xff;
                int b = (values[i] >> 0) & 0xff;
                var points=FindRectRGB(r,g,b,x,y,w,h);
                int len = points.Count;
                len = len > 5 ? len - 5:5;
                rvalue.Clear();
                for(int j = 0; j < points.Count; j++)
                {
                    int locs = offset(points[j], -rx/2, -rx/2);
                    for(int ly=0;ly<rx;ly++)
                        for(int lx=0;lx<rx;lx++)
                        {
                            int locv = locs + ly + (lx << 16);
                            if (!rvalue.ContainsKey(locv))
                                rvalue.Add(locv, 1);
                            else
                                rvalue[locv]++;
                        }
                }
                var xy = rvalue.OrderBy(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
                var ps=xy.Values.ToArray();
                var pa=xy.Keys.ToArray();
                if (ps[ps.Length - 1] > 5)
                    if(!tvalue.ContainsKey(pa[pa.Length-1]))
                        tvalue.Add(pa[pa.Length - 1], ps[ps.Length - 1]);
            }
            var txy = tvalue.OrderBy(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            int[] rvalues = txy.Keys.ToArray();
            int[] rcounts = txy.Values.ToArray();
            //List<int> kvs = new List<int>();
            for (int i = 0; i < rvalues.Length; i++)
            {
                if ((rcounts[i] >= 5) && (rcounts[i] <= 9))
                {
                    tlist.Add(rvalues[i]);
                    tlist.Add(rcounts[i]);
                }

            }
            return tlist;
        }

        public int[] RGBSortRect(int x,int y,int w,int h,int min=3,int max=int.MaxValue)
        {
            var s = ScreenCap.Size;
            int index = x + y * s.Width;
            int incw = s.Width - w;
            Dictionary<int, int> rgbmap = new Dictionary<int, int>();
            for (int sy = 0; sy < h; sy++)
            {
                for (int sx = 0; sx < w; sx++)
                {
                    if (!rgbmap.ContainsKey(colorData[index]))
                        rgbmap.Add(colorData[index], 1);
                    else
                        rgbmap[colorData[index]]++;
                    index++;
                }
                index += incw;
            }
            var xy = rgbmap.OrderBy(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            int[] values = xy.Keys.ToArray();
            int[] counts = xy.Values.ToArray();
            List<int> kvs = new List<int>();
            for(int i = 0; i < values.Length; i++)
            {
                if ((counts[i] >= min) && (counts[i] <= max))
                {
                    kvs.Add(values[i]&0x00ffffff);
                    kvs.Add(counts[i]);
                }

            }
            return kvs.ToArray();
        }

        public int[] RGBSort(int count)
        {
            Dictionary<int, int> rgbmap=new Dictionary<int, int>();
            for (int i = 0; i < colorData.Length; i++)
                if (!rgbmap.ContainsKey(colorData[i]))
                    rgbmap.Add(colorData[i], 1);
                else
                    rgbmap[colorData[i]]++;
            var x= rgbmap.OrderBy(o => o.Value).ToDictionary(p=>p.Key,o=>o.Value);
            int[] values = rgbmap.Keys.ToArray();
            int[] counts = rgbmap.Values.ToArray();
            int[] rgbsort = new int[values.Length*2];
            for(int i = 0; i < values.Length; i++)
            {
                rgbsort[i*2] = values[i];
                rgbsort[i*2 + 1] = counts[i];
            }
            return rgbsort;
        }

        public int matchNextRGB(int loc,int max,int r,int g,int b,int offset = 0)
        {
            var s = ScreenCap.Size;
            int p = (loc >> 16) + (loc & 0xffff) * s.Width;
            int index = 0;
            int lr = r, lg = g, lb = b;
            int inc = 1;
            if (max < 0)
            {
                max = -max;inc = -1;
                if (p < max) max = p;
            }
            else
            {
                max = (p + max) > (s.Width * s.Height) ? s.Width * s.Height - p : max;
            }
            while (max-- > 0)
            {
                if (offset == 0)
                {
                    if ((0xffffff & colorData[p]) != ((r << 16) | (g << 8) | b)) return index;
                }
                else
                {
                    r = lr;
                    g = lg;
                    b = lb;
                    int targetcolor = colorData[p];
                    r -= 0xff & (targetcolor >> 16);
                    g -= 0xff & (targetcolor >> 8);
                    b -= 0xff & targetcolor;
                    if (r < 0) r = -r;
                    if (g < 0) g = -g;
                    if (b < 0) b = -b;
                    if (r > offset) return index;
                    if (g > offset) return index;
                    if (b > offset) return index;
                }
                index++;
                p+=inc;
            }
            return -1;
        }

        public bool matchRGBOffset(int loc,int x,int y,int r,int g,int b,int offset = 0)
        {
            return matchRGB(loc >> 16 + x, loc & 0xffff + y, r, g, b, offset);
        }

        public bool matchRGB(int x,int y,int r,int g,int b,int offset=0)
        {
            var s = ScreenCap.Size;
            int p = (s.Width * y + x);
            if (offset == 0)
            {
                if ((0xffffff&colorData[p]) != ((r << 16) | (g << 8) | b)) return false;
            }
            else
            {
                int targetcolor = colorData[p];
                r -= 0xff & (targetcolor >> 16);
                g -= 0xff & (targetcolor >> 8);
                b -= 0xff & targetcolor;
                if (r < 0) r = -r;
                if (g < 0) g = -g;
                if (b < 0) b = -b;
                if (r > offset) return false;
                if (g > offset) return false;
                if (b > offset) return false;
            }
            return true;
        }

        public Bitmap GetScreen(int x,int y,int ex,int ey)
        {
            lastImage = ScreenCap.GetImageFor(x, y, ex, ey);
            return lastImage as Bitmap;
        }

        public void SetImg(Image img)
        {
            TestPic.Image = img;
        }

        public void StartTimer(int id,int time,NLua.LuaFunction lf)
        {
            for (int i = 0; i < TimerList.Count; i++)
            {
                if ((int)TimerList[i].Tag == id)
                {
                    TimerList[i].Interval=time;
                    FunList[i] = lf;
                    return;
                }
            }
            Timer t = new Timer();
            t.Interval = time;
            t.Tag = id;
            FunList.Add(lf);
            TimerList.Add(t);
            t.Start();
            t.Tick += T_Tick;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            try
            {
                (FunList[TimerList.IndexOf((Timer)sender)] as NLua.LuaFunction).Call();
            }
            catch(Exception ex) {
                OutputBox.AppendText(ex.ToString());
            }
        }

        public void StopTimer(int id)
        {
            for(int i = 0; i < TimerList.Count; i++)
            {
                if ((int)TimerList[i].Tag == id)
                {
                    TimerList[i].Stop();
                    TimerList[i].Dispose();
                    FunList.RemoveAt(i);
                    TimerList.RemoveAt(i);
                    return;
                }
            }
        }


        public Form1(params string[] slist)
        {
            //UpdateScreen().GetPixel()
            //var x = HID.getHandle(2211, 4433);
            //if (x.Count > 0)
            //{
            //    HID.OpenUSBHID(x[0]);
            //}



            InitializeComponent();

            Input.HID = HID;
            drawmap = new Bitmap(DrawTest.Width, DrawTest.Height);
            draw = Graphics.FromImage(drawmap);

            int port = 80;
            WebServer = new HTTP(port = GetPort(), Encoding.Default.GetBytes(MJpegServer.Properties.Resources.index));
            PortLable.Text = " Port:" + port;
            map = new Bitmap(WindowW, WindowH);
            g = Graphics.FromImage(map);
            g.Clear(Color.Black);
            g.Dispose();
            ScreenCap.Init(WindowW, WindowH, true);
            if (Input.Init())
                ButtonMode.Text = "DM模式";
            Input.HIDStatsChange = (bool stats) => {
                if (HIDCheck.Enabled)
                {
                    Invoke(new MethodInvoker(() => {
                        if (stats)
                            ButtonMode.Text = "HID模式";
                        else
                            ButtonMode.Text = "HID断开";
                    }));
                }
            };
            WebServer.ServerStart();
            WebServer.RegisterPost("cmd:", (string v,Socket s) => {
                s.Disconnect(true);
                s.Close();
                s.Dispose();
                string[] list=v.Split(',');
                if (list[1][0] == 'Q')
                {
                    try
                    {
                        int value=int.Parse(list[1].Substring(2));
                        MjpegServer.Quality = value;
                    }
                    catch { }
                }
                if (list[2][0] == 'S')
                {
                    try
                    {
                        int value = int.Parse(list[2].Substring(2));
                        double sc = (double)value / 100;
                        newSize.Width =(int) (WindowW * sc);
                        newSize.Height =(int)( WindowH * sc);
                        isNewSize = true;
                    }
                    catch { }
                }
                if (list[3][0] == 'F')
                {
                    try
                    {
                        double value = double.Parse(list[3].Substring(2));
                        if (value < 0.05) value = 0.05;
                        if (value > 30) value = 30;
                        updatevalue = (int)((1.0/value)*1000);
                    }
                    catch { }
                }
            });
            WebServer.RegisterPost("mouse:", (string v, Socket s) => {
                s.Disconnect(true);
                s.Close();
                s.Dispose();
                int px, py;
                try
                {
                    switch (v[6])
                    {
                        case 'd':
                            switch (v[8] - '0')
                            {
                                case 0:
                                    Input.MouseInput(0, 0, Input.MouseFlag.LeftDown);
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    Input.MouseInput(0, 0, Input.MouseFlag.RightDown);
                                    break;
                            }
                            break;
                        case 'u':
                            switch (v[8] - '0')
                            {
                                case 0:
                                    Input.MouseInput(0, 0, Input.MouseFlag.LeftUp);
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    Input.MouseInput(0, 0, Input.MouseFlag.RightUp);
                                    break;
                            }
                            break;
                        case 'm':
                            string[] pv = v.Substring(8).Split('|');

                            px = int.Parse(pv[0]);
                            py = int.Parse(pv[1]);
                            double wx = int.Parse(pv[2]);
                            double wy = int.Parse(pv[3]);
                            wx = WindowW / wx;
                            wy = WindowH / wy;
                            px = (int)(px * wx);
                            py = (int)(py * wy);
                            if (Input.HIDState)
                                MouseStep(px, py);
                            else
                                Input.MouseInput(px, py, Input.MouseFlag.MoveTo);
                            break;
                    }
                }
                catch { }
            });
            WebServer.RegisterPost("key:", (string v, Socket s) => {
                s.Disconnect(true);
                s.Close();
                s.Dispose();
                int vk = GetVkCode(v.Substring(6));
                if (v[4] == '1')
                {
                    Input.KeyInput(vk, Input.KeyFlag.Down);
                }
                else
                {
                    Input.KeyInput(vk, Input.KeyFlag.Up);
                }
            });
            WebServer.RegisterPost("lua:", (string v, Socket s) => {
                s.Disconnect(true);
                s.Close();
                s.Dispose();
                Invoke(new MethodInvoker(() => {
                    v=Uri.UnescapeDataString(v.Substring(4));
                    //v=Uri.EscapeDataString(v.Substring(4));
                    OutputBox.AppendText("[WEBLUA]"+v+"\r\n");
                    try
                    {
                        (lua as NLua.Lua).DoString(v);
                    }
                    catch (Exception ex)
                    {
                        OutputBox.AppendText(ex.ToString());
                    }
                }));
            });
            WebServer.RegisterGet((string v, Socket s) => {
                if(v.Contains("?Stream"))
                {
                    MjpegServer.SendImage(s, map);
                    MjpegServer.AddSocket(s);
                }
                if (v.IndexOf("?img")==0)
                {
                    if(MjpegServer.Clients > 0)
                        MjpegServer.SendJpeg(s, lastImage);
                    else
                        MjpegServer.SendJpeg(s, ScreenCap.GetImage());
                }
                if (v.Contains("Config"))
                {
                    WebServer.SendHttpData("Q:"+MjpegServer.Quality+"S:"+((int)(MjpegServer.Scale*100))+"F:"+updatevalue, s);
                }
            });

            UpdateImage.Tick += (object s, EventArgs ev) => {
                //g.Clear(Color.White);
                //g.DrawString(DateTime.Now.ToString().Replace(" ", "\r\n"), Font, Brushes.Black, 0, 0);
                if (MjpegServer.Clients > 0)
                {
                    try {
                        if (isNewSize)
                        {
                            isNewSize = false;
                            ScreenCap.Init(newSize.Width, newSize.Height, true);
                        }
                    MjpegServer.WriteImage(lastImage=ScreenCap.GetImage());
                    }
                    catch { }
                }
                if (updatevalue != 0)
                {
                    ((Timer)s).Interval = updatevalue;
                    updatevalue = 0;
                }
            };
            UpdateImage.Interval = 1000/5;
            UpdateImage.Start();

            base.FormClosed +=(object sx,FormClosedEventArgs ec) =>
            {
                System.Environment.Exit(0);
            };
            try
            {
                updateturnonstart();
            }
            catch
            {
                checkBox1.Enabled = false;
                checkBox1.Text = "(没有权限)" + checkBox1.Text;
            }

            InitLua();
        }

        public byte GetVkCode(string s)
        {
            string str = s.ToUpper();
            switch (str)
            {
                case "TAB": return 9;
                case "BACKSPACE": return 8;
                case "RETURN": return 0x0d;
                case "SHIFT": return 0x10;
                case "CTRL": return 0x11;
                case "ALT": return 0x12;
                case "ESCAPE":return 0x1b;
                case "END": return 0x23;
                case "HOME": return 0x24;
                case "ARROWLEFT": return 0x25;
                case "ARROWUP": return 0x26;
                case "ARROWRIGHT": return 0x27;
                case "ARROWDOWN": return 0x28;
                case "INSERT": return 0x2d;
                case "DELETE": return 0x2E;
                case "F1":return 0x70;
                case "F2": return 0x71;
                case "F3": return 0x72;
                case "F4": return 0x73;
                case "F5": return 0x74;
                case "F6": return 0x75;
                case "F7": return 0x76;
                case "F8": return 0x77;
                case "F9": return 0x78;
                case "F10": return 0x79;
                case "F11": return 0x7a;
                case "F12": return 0x7b;
                //case ">":
                //case ".":return 190;
                //case "<":
                //case ",":return 188;
                //case "?":
                //case "/":return 191;
                default: return str.Length==0?(byte)' ':(byte)str.ToCharArray()[0];
            }
        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            int width = Screen.PrimaryScreen.Bounds.Width;

            int height = Screen.PrimaryScreen.Bounds.Height;

            string path = System.IO.Directory.GetCurrentDirectory() + "\\test.jpg";

            Bitmap a = new Bitmap(width, height);
            Graphics gc = Graphics.FromImage(a);
            gc.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
            EncoderParameters p = new EncoderParameters(1);
            p.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
            //p.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 18L);
            //p.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionCCITT4);
            ImageCodecInfo info=null;
            foreach (ImageCodecInfo v in ImageCodecInfo.GetImageEncoders())
            {
                if (v.MimeType == "image/jpeg")
                {
                    info = v;
                    break;
                }
            }
            try {
                a.Save(path,info,p);
            }
            catch(Exception ex) { }
            //a.UnlockBits(data);
            a.Dispose();
            //b.Dispose();
            gc.Dispose();
        }

        private void SendImg_Click(object sender, EventArgs e)
        {
            WebServer.loadIndexFile();
        }



        bool setcheck = false;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (setcheck)
            {
                setcheck = false;
                return;
            }
            if (checkBox1.Checked) //设置开机自启动  
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("MjpegServer", path);
                rk2.Close();
                rk.Close();
            }
            else //取消开机自启动  
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("MjpegServer", false);
                rk2.Close();
                rk.Close();
            }
        }

        void updateturnonstart()
        {
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            if (rk2.GetValue("MjpegServer") != null)
            {
                setcheck = true;
                checkBox1.Checked = true;
            }
        }

        private void HIDCheck_CheckedChanged(object sender, EventArgs e)
        {
            Input.HIDMODE = HIDCheck.Enabled;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            System.Environment.Exit(0);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            notifyIcon1.Visible = false;
            base.OnClosing(e);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SizeChanged += Form1_SizeChanged;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }

        OpenFileDialog of = new OpenFileDialog();
        private void LoadLua_Click(object sender, EventArgs e)
        {
            of.Filter = "Lua脚本文件|*.lua";
            if (of.ShowDialog() == DialogResult.OK)
            {
                InitLua(of.FileName);
            }
        }

        private void InputButton_Click(object sender, EventArgs e)
        {
            if (lua == null) return;
            try
            {
                (lua as NLua.Lua).DoString(InputBOX.Text);
            }catch(Exception ex)
            {
                OutputBox.AppendText(ex.ToString());
            }
        }

        Point lastPoint;
        private void DrawTest_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                lastPoint = new Point(e.X, e.Y);
            }
            if (e.Button == MouseButtons.Right)
            {
                draw.Clear(Color.White);
                Input.setDest(new Point(e.X, e.Y), lastPoint);
                Point p;
                Point start = new Point(lastPoint.X, lastPoint.Y);
                bool b = true;
                while ((p = Input.UpdateCursor()) != Point.Empty)
                {
                    int x1=start.X, y1=start.Y;
                    start.Offset(p);
                    if(b)
                        draw.DrawLine(Pens.Red, x1, y1, start.X, start.Y);
                    else
                        draw.DrawLine(Pens.Blue, x1, y1, start.X, start.Y);
                    b = !b;
                }
                draw.FillEllipse(Brushes.Black, lastPoint.X-4, lastPoint.Y-4, 8, 8);
                draw.FillEllipse(Brushes.Black, start.X-4, start.Y-4, 8, 8);
                DrawTest.Image = drawmap;
            }
            if (e.Button == MouseButtons.Middle)
            {
                

            }
        }

        void updateSelectDraw()
        {
            if (DataDraw == null) DataDraw = Graphics.FromImage(DataBitmap); ;
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    DataDraw.FillRectangle(new SolidBrush(selectcolor[x, y]), x * 10, y * 10, 10, 10);
                    if (selectcolor[x, y].ToArgb() == selectcolor[startPoint.X, startPoint.Y].ToArgb())
                        DataDraw.DrawRectangle(Pens.Yellow, x * 10+2, y * 10+2, 5, 5);
                    if (selectcolorenb[x,y])
                        DataDraw.DrawRectangle(Pens.Red, x * 10 , y * 10 , 9, 9);
                    if(startPoint.X==x&&startPoint.Y==y)
                        DataDraw.DrawRectangle(Pens.Blue, x * 10+1, y * 10+1, 7, 7);
                }
            DataImage.Image = DataBitmap;
        }

        double[] rgb2hsv(double r, double g, double b)
        {

            r /= 255.0;
            g /= 255.0;
            b /= 255.0;
            double mx = Math.Max(r, Math.Max(g, b));
            double mn = Math.Min(r, Math.Min(g, b));
            double diff = mx - mn;
            double h=0, s=0, v=0;

            if (mx == mn)
                h = 0;
            else if (mx == r)
            {
                if (g >= b)
                    h = 60 * ((g - b) / diff) + 0;
                else
                    h = 60 * ((g - b) / diff) + 360;
            }
            else if (mx == g)
                h = 60 * ((b - r) / diff) + 120;
            else if (mx == b)
                h = 60 * ((r - g) / diff) + 240;

//# 先计算L
            v = (mx + mn) / 2.0;

            //  # 再计算S
            if (mx == mn)
                s = 0;
            else if (v > 0 && v <= 0.5)
                s = (diff / v) / 2.0;
            else if (v > 0.5)
                s = (diff / (1 - v)) / 2.0;

            return new double[] { h, s, v };

            //double K = 0.0;
            //double tmp;

            //if (g < b)
            //{
            //    tmp = g;
            //    g = b;
            //    b = tmp;

            //    K = -1.0f;
            //}

            //if (r < g)
            //{
            //    tmp = r;
            //    r = g;
            //    g = tmp;

            //    K = -2.0f / 6.0f - K;
            //}

            //double chroma = r - g>b?b:g;
            //double[] hsv = new double[3];
            //hsv[0] = (K + (g - b) / (6.0 * chroma + 1e-20));
            //hsv[1] = chroma / (r + 1e-20);
            //hsv[2] = r;

            //return hsv;
        }


        Color[,] selectcolor = new Color[16, 16];
        bool[,] selectcolorenb = new bool[16, 16];
        Point startPoint=Point.Empty;
        Bitmap DataBitmap = new Bitmap(160, 160);
        Graphics DataDraw;
        private void DataImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (DataDraw == null)
                DataDraw = Graphics.FromImage(DataBitmap);
            if (e.Button == MouseButtons.Left)
            {
                if (e.X < 160 && e.Y < 160)
                {
                    int dx = e.X / 10;
                    int dy = e.Y / 10;
                    selectcolorenb[dx, dy] = !selectcolorenb[dx, dy];
                    valR.Text = "R:" + selectcolor[dx, dy].R;
                    valG.Text = "G:" + selectcolor[dx, dy].G;
                    valB.Text = "B:" + selectcolor[dx, dy].B;
                    var hsv = rgb2hsv(selectcolor[dx, dy].R, selectcolor[dx, dy].G, selectcolor[dx, dy].B);
                    valH.Text = "H:" + hsv[0];
                    valS.Text = "S:" + hsv[1];
                    valV.Text = "V:" + hsv[2];
                    updateSelectDraw();
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (e.X < 160 && e.Y < 160)
                {
                    int dx = e.X / 10;
                    int dy = e.Y / 10;
                    startPoint = new Point(dx, dy);
                    valR.Text = "R:" + selectcolor[dx, dy].R;
                    valG.Text = "G:" + selectcolor[dx, dy].G;
                    valB.Text = "B:" + selectcolor[dx, dy].B;
                    var hsv = rgb2hsv(selectcolor[dx, dy].R, selectcolor[dx, dy].G, selectcolor[dx, dy].B);
                    valH.Text = "H:" + hsv[0];
                    valS.Text = "S:" + hsv[1];
                    valV.Text = "V:" + hsv[2];
                    updateSelectDraw();
                }
            }
            
        }

        private void copyscreenbutton_Click(object sender, EventArgs e)
        {
            DrawScreen();
        }

        private void DrawTest_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int lx = e.X, ly = e.Y;
            if (lx < 8) lx = 8;
            if (ly < 8) ly = 8;
            lx -= 8;
            ly -= 8;
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    selectcolor[x, y] = ((Bitmap)(DrawTest.Image)).GetPixel(lx + x, ly + y);
                    selectcolorenb[x, y] = false;
                }
            updateSelectDraw();
        }

        private void getposbutton_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                {
                    if(selectcolorenb[x, y])
                    {
                        sb.Append("" + (x - startPoint.X));
                        sb.Append(",");
                        sb.Append("" + (y - startPoint.Y));
                        sb.Append(",");
                    }
                }
            sb.Append("}");
            OutputBox.Text = sb.ToString();
        }

        private void clearposbutton_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < 16; y++)
                for (int x = 0; x < 16; x++)
                    selectcolorenb[x, y] = false;
            updateSelectDraw();
        }
    }
}
