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

namespace MJpegServer
{
    public partial class Form1 : Form
    {

        MJPEGStream MjpegServer=new MJPEGStream();
        HTTP WebServer = new HTTP(80);
        Bitmap map;
        Graphics g;

        Timer UpdateImage = new Timer();

        int WindowW= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        int WindowH= System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        int updatevalue=0;

        public Form1()
        {
            InitializeComponent();
            map = new Bitmap(WindowW, WindowH);
            g = Graphics.FromImage(map);
            g.Clear(Color.Black);
            g.Dispose();
            ScreenCap.Init(WindowW, WindowH, true);
            if (Input.Init())
                ButtonMode.Text = "DM模式";
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
                        ScreenCap.Init((int)(WindowW * sc), (int)(WindowH * sc), true);
                    }
                    catch { }
                }
                if (list[3][0] == 'F')
                {
                    try
                    {
                        int value = int.Parse(list[3].Substring(2));
                        if (value < 1) value = 1;
                        if (value > 30) value = 30;
                        updatevalue = 1000/value;
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
            WebServer.RegisterGet((string v, Socket s) => {
                if(v.Contains("Stream"))
                {
                    MjpegServer.SendImage(s, map);
                    MjpegServer.AddSocket(s);
                }
            });

            UpdateImage.Tick += (object s, EventArgs ev) => {
                //g.Clear(Color.White);
                //g.DrawString(DateTime.Now.ToString().Replace(" ", "\r\n"), Font, Brushes.Black, 0, 0);
                if (MjpegServer.Clients > 0)
                {
                    try { 
                    MjpegServer.WriteImage(ScreenCap.GetImage());
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
        }

        byte GetVkCode(string s)
        {
            string str = s.ToUpper();
            switch (str)
            {
                case "TAB": return 9;
                case "BACK": return 8;
                case "RETURN": return 0x0d;
                case "SHIFT": return 0x10;
                case "CTRL": return 0x11;
                case "ALT": return 0x12;
                case "END": return 0x23;
                case "HOME": return 0x24;
                case "ARROWLEFT": return 0x25;
                case "ARROWUP": return 0x26;
                case "ARROWRIGHT": return 0x27;
                case "ARROWDOWN": return 0x28;
                case "INSERT": return 0x2d;
                case "DELETE": return 0x2E;
                default: return (byte)str.ToCharArray()[0];
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
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
    }
}
