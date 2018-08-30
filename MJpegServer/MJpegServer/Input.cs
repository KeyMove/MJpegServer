using HIDTEST;
using MJpegServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
//using System.Windows.Forms;

namespace KMButton
{
    public static class Input
    {
        static CDmSoft DM;
        static bool _hid = false;
        static int _hidsw = 0;
        static Timer timer;
        static USBHID hid;

        static volatile byte[][] datalist = new byte[16][];
        static volatile int ringcount;
        static volatile int ringpos;
        static volatile int ringreadpos;

        static byte[] usbvk = { 4, 5, 6, };

        public static int VID = 2211;
        public static int PID = 4433;
        static byte[] keydata = new byte[7];
        static byte[] mousedata = new byte[7];
        static byte mousekeystats = 0x08;
        static List<byte> keylist = new List<byte>();

        static List<byte> mouselist = new List<byte>();

        static int destx;
        static int desty;

        static int xcount;
        static int ycount;
        static int discount;
        static int dis;
        static int incx;
        static int incy;
        static int ix;
        static int iy;
        static int incscal = 5;
        static int maxscal = 8;
        static double scalvalue =0.1;
        static double scalcount;
        static int add, nor, dec;
        static int loop;

        public static bool HIDIdle
        {
            get
            {
                return dis<=0;
            }
            private set { }
        }

        public static void setDest(Point p, Point px)
        {
            ix = p.X - px.X;
            iy = p.Y - px.Y;
            ix /= 10;
            iy /= 10;
            incx = 10;
            incy = 10;
            if (ix < 0)
            {
                ix = -ix;
                incx = -10;
            }
            if (iy < 0)
            {
                iy = -iy;
                incy = -10;
            }
            discount = dis = ix > iy ? ix : iy;
            scalcount = 0;
            add = (int)((maxscal - incscal) / scalvalue);
            if(discount>(add*2))
            {
                dec = add;
                nor = discount - add * 2;
            }
            else
            {
                dec = add = discount / 2;
                if ((discount % 2)!=0)
                    nor = 1;
            }
        }

        public static void Clear()
        {
            mousekeystats = 0;
            keylist.Clear();
        }

        public static void MoveTo(Point x,Point y)
        {
            setDest(x, y);
        }

        public static Point UpdateCursor()
        {
            int xv, yv;
            if (dis <= 0) return Point.Empty;
            int scal = incscal + (int)scalcount;
            int dc = scal;
            do
            {
                if (add > 0)
                {
                    add -= dc;
                    scalcount += dc * scalvalue;
                    if (add < 0)
                        dc = -add;
                    else
                        dc = 0;
                }
                else if (nor > 0)
                {
                    nor -= dc;
                    if (nor < 0)
                        dc = -nor;
                    else
                        dc = 0;
                }
                else if (dec > 0)
                {
                    dec -= dc;
                    scalcount -= dc * scalvalue;
                    if (nor < 0)
                        dc = 0;
                }
                else
                    dc = 0;
            } while (dc > 0);

            dis -= scal;
            xcount += ix * scal;
            ycount += iy * scal;
            xv = xcount / discount;
            yv = ycount / discount;
            xcount -= xv * discount;
            ycount -= yv * discount;
            return new Point(xv*incx, yv*incy);
        }

        public static Action<bool> HIDStatsChange;

        static public USBHID HID
        {
            get { return hid; }
            set { hid = value; }
        }

        static public bool HIDState
        {
            get { return _hidsw != 0; }
            private set { }
        }

        static public void TestHID()
        {
            if (hid != null&&_hidsw==1)
            {
                try
                {
                    hid.WriteData(new byte[] { 1, 0, 0, 0, 0, 0, 0 });
                }
                catch
                {
                    hid.Close();
                    timer.Interval = 400;
                    _hidsw = 0;
                    if(HIDStatsChange!=null)
                        HIDStatsChange(false);
                }
            }
        }

        static void SafeWriteData(byte[] data)
        {
            try
            {
                int count = 0;
                data[2] = 0;
                for (int i = 0; i < 4; i++)
                {
                    byte b = data[3 + i];
                    byte xb = 0x80;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((b & xb) != 0)
                        {
                            if (++count == 6)
                            {
                                data[3 + i] ^= 0x55;
                                data[2] |= (byte)(0x80 >> i);
                                count = 0;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                        xb >>= 1;
                    }
                }
                datalist[ringpos++] = data;
                ringcount++;
                ringpos &= 0xf;
                //hid.WriteData(data);
            }
            catch { }
        }

        static public bool HIDMODE
        {
            get
            {
                return _hid;
            }
            set
            {
                _hid = value;
                if (value)
                {
                    if (timer == null)
                    {
                        if (hid == null) return;
                        //hid = new USBHID();
                        //var v = hid.getHandle(2211, 4433);
                        //if (v.Count != 0)
                        //{
                        //    if (hid.OpenUSBHID(v[0]))
                        //    {
                        //        _hidsw = 1;
                        //    }
                        //}
                        KeyInputbackup = KeyInput;
                        KeyInput = USBKeySend;
                        MouseInputbackup = MouseInput;
                        MouseInput = USBMouseSend;
                        timer = new Timer();
                        timer.Interval = 400;
                        timer.Elapsed += (object send, ElapsedEventArgs e) => {
                            try
                            {
                                switch (_hidsw)
                                {
                                    case 0:
                                        //var v = hid.getHandle(2211, 4433);
                                        //if (v.Count != 0)
                                        //{

                                        //}
                                        if (hid.OpenUSBHID())
                                        {
                                            _hidsw = 1;
                                            hid.WriteData(new byte[] { 1, 0, 0, 0, 0, 0, 0 });
                                            timer.Interval = 1;
                                            if (HIDStatsChange != null)
                                                HIDStatsChange(true);
                                        }
                                        break;
                                    case 1:
                                        if (ringcount != 0)
                                        {
                                            byte[] data = datalist[ringreadpos++];
                                            ringcount--;
                                            ringreadpos &= 0xf;
                                            try
                                            {
                                                hid.WriteData(data);
                                            }
                                            catch
                                            {
                                                hid.Close();
                                                timer.Interval = 400;
                                                _hidsw = 0;
                                                if (HIDStatsChange != null)
                                                    HIDStatsChange(false);
                                            }
                                        }
                                        if (loop == 0)
                                        {
                                            Point p;
                                            if ((p = UpdateCursor()) != Point.Empty)
                                            {
                                                USBMouseSend(p.X, p.Y);
                                                loop = 3;
                                            }
                                        }
                                        else
                                            loop--;
                                        break;
                                }
                            }
                            catch (Exception ex) { }

                        };
                        timer.Start();
                    }
                    else
                    {
                        timer.Start();
                    }
                }
                else
                {
                    KeyInput = KeyInputbackup;
                    MouseInput = MouseInputbackup;
                    if (timer != null)
                    {
                        timer.Stop();
                    }
                }
            }
        }

        static byte scanvk(int vk)
        {
            if (vk >= 'A' && vk <= 'Z')
                return (byte)(vk - 'A' + 4);
            if (vk >= '0' && vk <= '1')
                return (byte)(vk - '0' + 30);
            if (vk >= 0x70 && vk <= 0x7b)
                return (byte)(0x3a + (vk - 0x70));
            switch (vk)
            {
                case 9: return 43;
                case 8: return 42;
                case 0x0d: return 40;
                case 0x10: return 0xff - 1;
                case 0x11: return 0xff - 2;
                case 0x12: return 0xff - 3;
                case 0x1b:return 0x29;
                case 0x20:return 0x2C;
                case 0x23: return 77;
                case 0x24: return 74;
                case 0x25: return 80;
                case 0x26: return 82;
                case 0x27: return 79;
                case 0x28: return 81;
                case 0x2d: return 76;
            }
            return 0xff;
        }

        enum MouseKey{
            Left=1,
            Right=2,
            Middle=4,
        }
        static void USBMouseSend(int x, int y, MouseFlag flag=MouseFlag.AddTo, int data = 0)
        {
            switch (flag)
            {
                case MouseFlag.LeftDown:
                    if ((mousekeystats & 0x01) == 0)
                        mousekeystats += (byte)MouseKey.Left;
                    break;
                case MouseFlag.LeftUp:
                    if ((mousekeystats & 0x01) != 0)
                        mousekeystats -= (byte)MouseKey.Left;
                    break;
                case MouseFlag.RightDown:
                    if ((mousekeystats & 0x02) == 0)
                        mousekeystats += (byte)MouseKey.Right;
                    break;
                case MouseFlag.RightUp:
                    if ((mousekeystats & 0x02) != 0)
                        mousekeystats -= (byte)MouseKey.Right;
                    break;
                case MouseFlag.Reset:
                    mousekeystats = 0x08;
                    break;
            }


            mousedata[0] = 1;
            mousedata[1] = 0xAA;
            mousedata[2] = 0;
            mousedata[3] = (byte)x;
            mousedata[4] = (byte)y;
            mousedata[5] = mousekeystats;
            mousedata[6] = (byte)data;
            if (_hidsw == 1)
                SafeWriteData(mousedata);
        }

        static void USBKeySend(int vk, KeyFlag flag)
        {
            if (vk > 127) return;
            byte hidvk = scanvk(vk);
            if (hidvk == 0xff) return;
            bool update = false;
            switch (flag)
            {
                case KeyFlag.Down:
                    if (keylist.IndexOf(hidvk) == -1)
                    {
                        keylist.Add(hidvk);
                        update = true;
                    }
                    break;
                case KeyFlag.Up:
                    if (keylist.IndexOf(hidvk) != -1)
                    {
                        keylist.Remove(hidvk);
                        update = true;
                    }
                    break;
                case KeyFlag.Press: break;

            }
            if (update)
            {
                if (_hidsw == 1)
                {
                    keydata[0] = 1;
                    keydata[1] = 0x5A;
                    keydata[2] = 0;
                    int i = 0;
                    int j = keylist.Count > 4 ? 4 : keylist.Count;
                    for (i = 0; i < j; i++)
                        keydata[3 + i] = keylist[i];
                    for (; i < 4; i++)
                        keydata[3 + i] = 0;

                    SafeWriteData(keydata);
                }
            }
        }

        public enum KeyFlag
        {
            Down = 0,
            Up = 1,
            Press = 2,
        }

        public enum MouseFlag
        {
            MoveTo = 0,
            AddTo = 1,
            Wheel = 2,
            LeftDown = 3,
            LeftUp = 4,
            RightDown = 5,
            RightUp = 6,
            Reset=7,
        }

        public delegate void KeySend(int vk, KeyFlag flag);
        public delegate void MouseSend(int x, int y, MouseFlag flag=MouseFlag.AddTo, int data = 0);

        public static KeySend KeyInputbackup;
        public static MouseSend MouseInputbackup;

        public static KeySend KeyInput;
        public static MouseSend MouseInput;

        public static bool Init()
        {
            try
            {
                DM = new CDmSoft();
                KeyInput = ((int vk, KeyFlag flag) => {
                    switch (flag)
                    {
                        case KeyFlag.Down: DM.KeyDown(vk); break;
                        case KeyFlag.Up: DM.KeyUp(vk); break;
                        case KeyFlag.Press: DM.KeyPress(vk); break;
                    }
                });
                MouseInput = ((int x, int y, MouseFlag flag, int data) => {
                    switch (flag)
                    {
                        case MouseFlag.MoveTo: DM.MoveTo(x, y); break;
                        case MouseFlag.AddTo: DM.MoveR(x, y); break;
                        case MouseFlag.Wheel:
                            if (data > 0)
                                DM.WheelUp();
                            else
                                DM.WheelDown();
                            break;
                        case MouseFlag.LeftDown: DM.LeftDown(); break;
                        case MouseFlag.LeftUp: DM.LeftUp(); break;
                        case MouseFlag.RightDown: DM.RightDown(); break;
                        case MouseFlag.RightUp: DM.RightUp(); break;
                    }
                });
            }
            catch
            {
                KeyInput = KeyEvent.KeySend;
                MouseInput = KeyEvent.MouseSend;
                return false;
            }
            return true;
        }

        //public void KeyInput(int vk,KeyFlag mode)
        //{
        //    switch (mode)
        //    {
        //        case KeyFlag.Down: 

        //            break;
        //        case KeyFlag.Up: break;
        //        case KeyFlag.Press: break;
        //    }
        //}
    }
}
