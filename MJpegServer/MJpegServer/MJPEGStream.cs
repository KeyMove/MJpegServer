using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJpegServer
{
    class MJPEGStream
    {
        string MJPEGHead= "HTTP/1.1 200 OK\r\nContent-Type: multipart/x-mixed-replace;boundary=img\r\n";
        string MJPEGDataHead= "\r\n--img\r\nContent-Type: image/jpeg\r\nContent-Length: ";
        string MJPEGLast = "--img\r\n";
        string JPEGHead = "HTTP/1.1 200 OK\r\nContent-type: image/jpeg\r\n\r\n";
        TcpListener myList;
        Thread RecvThread;

        byte[] HeadArray;
        byte[] JPEGHeadArray;
        byte[] MJPEGLastArray;
        //byte[] DataHeadArray;
        byte[] recvbuff = new byte[1500];

        List<Socket> LinkList = new List<Socket>();

        public bool isRun { get; private set; }

        Encoding ServerCoding = Encoding.ASCII;

        ImageCodecInfo Jpeginfo = null;
        EncoderParameters Parmeter = new EncoderParameters(1);

        //int ImageWidth = -1;
        //int ImageHeight = -1;

        public MJPEGStream(int Port)
        {
            Jpeginfo = GetJpegInfo();
            Quality = 70;
            myList = new TcpListener(IPAddress.Any, Port);
            HeadArray = ServerCoding.GetBytes(MJPEGHead);
            MJPEGLastArray = ServerCoding.GetBytes(MJPEGLast);
            JPEGHeadArray = ServerCoding.GetBytes(JPEGHead);
            isRun = false;
        }

        public MJPEGStream()
        {
            Jpeginfo = GetJpegInfo();
            Quality = 70;
            HeadArray = ServerCoding.GetBytes(MJPEGHead);
            MJPEGLastArray = ServerCoding.GetBytes(MJPEGLast);
            JPEGHeadArray = ServerCoding.GetBytes(JPEGHead);
            isRun = false;
        }

        int _quality = -1;
        public int Quality
        {
            get
            {
                return _quality;
            }
            set
            {
                if (value == _quality) return;
                _quality = value;
                if (_quality > 100) _quality = 100;
                if (_quality < 0) _quality = 0;
                Parmeter.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)_quality);
            }
        }

        bool isScale = false;
        double _Scale;
        public double Scale
        {
            set {
                _Scale = value;

            }
            get { return _Scale; }
        }

        public int Clients
        {
            get {
                return LinkList.Count;
            }
        }

        static ImageCodecInfo GetJpegInfo()
        {
            foreach (ImageCodecInfo v in ImageCodecInfo.GetImageEncoders())
            {
                if (v.MimeType == "image/jpeg")
                {
                    return v;
                }
            }
            return null;
        }

        public void AddSocket(Socket sk)
        {
            if (sk.Connected)
            {
                if (LinkList.IndexOf(sk) == -1)
                {
                    sk.Send(HeadArray);
                    sk.SendTimeout = 2000;
                    LinkList.Add(sk);
                }
            }
        }
        public void RemoveSocket(Socket sk)
        {
            if (LinkList.IndexOf(sk) != -1)
            {
                LinkList.Remove(sk);
                sk.Close();
            }
        }
        public void StartServer()
        {
            myList.Start();
            isRun = true;
            RecvThread = new Thread(() => {
                while (isRun)
                {
                    Socket sk = myList.AcceptSocket();
                    int len = sk.Receive(recvbuff);
                    if (LinkList.IndexOf(sk) == -1)
                    {
                        sk.Send(HeadArray);
                        LinkList.Add(sk);
                    }
                }
            });
            RecvThread.Start();
        }

        byte[] GetImageArray(Image img)
        {
            MemoryStream ms = new MemoryStream();            
            img.Save(ms, Jpeginfo,Parmeter);
            //img.Save(ms, GetCodecInfo)
            byte[] array = ms.ToArray();
            return array;
        }

        public void SendJpeg(Socket sk, Image img)
        {
            sk.Send(JPEGHeadArray);
            sk.Send(GetImageArray(img));
        }
        public void SendImage(Socket sk, Image img)
        {
            if (LinkList.Count == 0) return;
            byte[] array = GetImageArray(img);
            StringBuilder sb = new StringBuilder(MJPEGDataHead);
            sb.Append(array.Length);
            sb.Append("\r\n\r\n");
            string head = sb.ToString();
            byte[] headdata = Encoding.ASCII.GetBytes(head);
            int len = array.Length + headdata.Length;
            byte[] packet = new byte[len];
            Buffer.BlockCopy(headdata, 0, packet, 0, headdata.Length);
            Buffer.BlockCopy(array, 0, packet, headdata.Length, array.Length);
            sk.SendTimeout = 3000;
            sk.Send(HeadArray);
            sk.Send(packet);
        }

        public void WriteImage(Image img)
        {
            if (LinkList.Count == 0) return;
            byte[] array = GetImageArray(img);
            StringBuilder sb = new StringBuilder(MJPEGDataHead);
            sb.Append(array.Length);
            sb.Append("\r\n\r\n");
            string head = sb.ToString();
            byte[] headdata = Encoding.ASCII.GetBytes(head);
            //int len = array.Length + headdata.Length + MJPEGLastArray.Length;
            int len = array.Length + headdata.Length;
            byte[] packet = new byte[len];
            Buffer.BlockCopy(headdata, 0, packet, 0, headdata.Length);
            Buffer.BlockCopy(array, 0, packet, headdata.Length, array.Length);
            //Buffer.BlockCopy(MJPEGLastArray, 0, packet, headdata.Length + array.Length, MJPEGLastArray.Length);
            List<Socket> remove = new List<Socket>();
            for (int i = 0; i < LinkList.Count; i++)
            {
                Socket s = LinkList[i];
                if (s.Connected)
                {
                    try
                    {
                        s.Send(HeadArray);
                        s.Send(packet);
                    }
                    catch
                    {
                        remove.Add(s);
                    }
                }
                else
                {
                    remove.Add(s);
                }
            }
            foreach (Socket s in remove)
            {
                LinkList.Remove(s);
                s.Close();
            }
        }
    }
}
