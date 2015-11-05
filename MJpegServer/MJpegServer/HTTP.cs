using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MJpegServer
{
    class HTTP
    {
        TcpListener myList;

        //string HTTPHead = "HTTP/1.1 200 OK\r\n";

        Thread ServerThread;
        List<Socket> newSocket = new List<Socket>();
        List<Socket> ClineList = new List<Socket>();

        byte[] RecvBuff = new byte[1500];
        string DirPath;

        byte[] HttpHearNone;
        byte[] Error404Array;
        byte[] indexPageArray;
        Encoding ServerCoding = Encoding.ASCII;

        //string IndexPageCode = "<html>\r\n<head><title>test</title></head>\r\n<body bgcolor=\"#d0d0d0\" onkeydown=\"if(sk!=event.key){post('/key:1,'+event.key);sk=event.key}\" onkeyup=\"if(sk==event.key)sk=null;post('/key:0,'+event.key);\" onmouseup=\"post('/mouse:u,'+event.button)\" onmousedown=\"if(checkmouse(event))post('/mouse:d,'+event.button)\" onmousemove=\"if(first&&checkmouse(event))post('/mouse:m,'+(document.body.scrollLeft+event.clientX)+'|'+(document.body.scrollTop+event.clientY-45))\">\r\n<script>var sk;var issend=false;var isrect=false;var w=0;var h=0;var first=false;document.oncopy=function(){return false};document.oncontextmenu=function(){return false};function post(val,flag){\r\nvar x=document.getElementById(\"postdata\");\r\nx.action=val;\r\nif(!first){\r\nvar img=document.getElementById(\"stream\")\r\nw=img.width;\r\nh=img.height;\r\nif(w<=64&&h<=64){\r\n//alert(\"未能获取图片,刷新页面\");\r\nreturn;}\r\nfirst=true;\r\nissend=true;\r\nimg.width=w;\r\nimg.height=h;\r\n//alert(\"x:\"+w+\"y:\"+h);\r\n}\r\nif(issend)\r\nx.submit();\r\n}\r\nfunction checkmouse(e){\r\nvar y=e.clientY+document.body.scrollTop;\r\nvar x=e.clientX+document.body.scrollLeft;\r\nconsole.log(\"x=\"+x+\" w=\"+w+\" y=\"+y+\" h=\"+h)\r\nif(x<w&&y>45&&y<(45+h)){isrect=true;return true}\r\nisrect=false;\r\nreturn false\r\n}\r\nfunction showset(){\r\nvar x=document.getElementById(\"setinfo\");\r\nif(parseInt(x.style.left)<0)\r\n{x.style.left='0';issend=false}\r\nelse\r\n{x.style.left='-430';issend=true}\r\n}\r\n</script>\r\n<div style=\"position: absolute;left:0;top:45\"><img id=\"stream\" src=\"/?Stream\"/></div>\r\n<div id=\"setinfo\" style=\"position: absolute;left:-430;top:0\">\r\n<iframe style=\"width:0; height:0; margin-top:-10px;\" name=\"submitFrame\" src=\"about:blank\"></iframe>\r\n<form id=\"postdata\" target=\"submitFrame\" method=\"post\"></form>\r\n质量:<input id=\"qt\" type=\"text\" size=\"3\" maxlength=\"2\" value=\"70\"/>\r\n尺寸:<input id=\"sc\" type=\"text\" size=\"3\" maxlength=\"3\" value=\"100\"/>%\r\n刷新帧率:<input id=\"fq\" type=\"text\" size=\"3\" maxlength=\"2\" value=\"1\"/>\r\n<input id=\"md\" type=\"checkbox\"/>图片模式\r\n<input type=\"button\" value=\"提交\" onclick=\"postdata.action='/cmd:o1,Q:'+qt.value+',S:'+sc.value+',F:'+fq.value+',M:'+md.checked;postdata.submit()\"/>\r\n<input type=\"button\" value=\">>展开设置\" onclick=\"showset()\"/>\r\n</div>\r\n</body>\r\n</html>";

        bool isRun = false;

        Dictionary<string, SocketPostCallBack> postMap = new Dictionary<string, SocketPostCallBack>();

        SocketGetCallBack getCall = null;

        public delegate void SocketPostCallBack(string message,Socket sk);
        public delegate void SocketGetCallBack(string message, Socket sk);
        public HTTP(int port)
        {
            myList = new TcpListener(IPAddress.Any, port);
            DirPath = System.IO.Directory.GetCurrentDirectory()+"\\html";
            Error404Array = ServerCoding.GetBytes("HTTP/1.1 404 Not Found\r\nContent-type: text/plain\r\nContent-Length: 17\r\n\r\n404: Not Found!\r\n");
            HttpHearNone = ServerCoding.GetBytes("HTTP/1.1 200 OK\r\nContent-type: text/html\r\nContent-Length: 0\r\n\r\n");
            if (!loadIndexFile())
            {
                StringBuilder sb = new StringBuilder("HTTP/1.1 200 OK\r\nContent-type: text/html\r\nContent-Length: ");
                byte[] pagearray = HttpIndex.IndexPageArray;
                sb.Append(pagearray.Length);
                sb.Append("\r\n\r\n");
                indexPageArray = new byte[pagearray.Length + sb.Length];
                byte[] headarray = ServerCoding.GetBytes(sb.ToString());
                Buffer.BlockCopy(headarray, 0, indexPageArray, 0, sb.Length);
                Buffer.BlockCopy(pagearray, 0, indexPageArray, sb.Length, pagearray.Length);
            }
            isRun = false;
        }

        public bool loadIndexFile()
        {
            string indexfile = DirPath + "\\index.html";
            if (File.Exists(indexfile))
            {
                StringBuilder sb = new StringBuilder("HTTP/1.1 200 OK\r\nContent-type: text/html\r\nContent-Length: ");
                FileStream fs = new FileStream(indexfile, FileMode.Open);
                sb.Append(fs.Length);
                sb.Append("\r\n\r\n");
                indexPageArray = new byte[fs.Length + sb.Length];
                fs.Read(indexPageArray, sb.Length, (int)fs.Length);
                byte[] headarray = ServerCoding.GetBytes(sb.ToString());
                Buffer.BlockCopy(headarray, 0, indexPageArray, 0, sb.Length);
                fs.Close();
                fs.Dispose();
                return true;
            }
            return false;
        }

        public void RegisterGet(SocketGetCallBack call)
        {
            getCall = call;
        }

        public void RegisterPost(string flag,SocketPostCallBack call)
        {
            postMap.Add(flag, call);
        }

        public void unRegisterPost(string flag)
        {
            postMap.Remove(flag);
        }

        void PostCall(string message,Socket sk)
        {
            foreach(string v in postMap.Keys)
            {
                if (message.IndexOf(v) == 0)
                {
                    postMap[v](message,sk);
                }
            }
        }

        Stream GetData(string path,Socket sk)
        {
            if (path == string.Empty)
            {
                MemoryStream ms = new MemoryStream(indexPageArray);
                return ms;
            }
            if (path[0] == '?')
            {
                getCall(path, sk);
                return null;
            }
            if (!File.Exists(DirPath + path))
            {
                sk.Send(Error404Array);
                return null;
            }
            FileStream fs = new FileStream(DirPath + path, FileMode.Open);
            return fs;
        }

        void StreamSend(Socket sk,Stream s)
        {
            if (s.Length == 0)
            {
                s.Close();
                s.Dispose();
            }
            byte[] buff = new byte[2048];
            int len;
            do
            {
                len = s.Read(buff, 0, 2048);
                sk.Send(buff);
            } while (len == 2048);
            if(len!=0)
            sk.Send(buff,len,SocketFlags.None);
            s.Close();
            s.Dispose();
        }

        bool CheckHead(Socket e)
        {
            Stream s;
            byte[] rv = new byte[1024];
            int len = e.Receive(rv);
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                if (rv[i] == (byte)'\r')
                    break;
                b.Append((char)rv[i]);
            }
            string head = b.ToString();
            int index = head.IndexOf("GET /");
            int pos = head.IndexOf(" HTTP");
            if (pos == -1) return false;
            if (index != -1)
            {
                index += 5;
                pos -= index;
                s = GetData(head.Substring(index, pos), e);
                if (s != null)
                {
                    StreamSend(e, s);
                }
                return true;
            }
            index = head.IndexOf("POST /");
            if (index != -1)
            {
                index += 6;
                pos -= index;
                PostCall(head.Substring(index, pos), e);
                //e.Send(HttpHearNone);
            }
            return true;
        }

        public void ServerStart()
        {
            myList.Start();
            isRun = true;
            ServerThread = new Thread(() => {
                while (isRun) { 
                Socket sk = myList.AcceptSocket();
                    try { 
                    CheckHead(sk);
                    }
                    catch { }
                   // newSocket.Add(sk);
                }
            });
            ServerThread.Start();
        }
    }
}
