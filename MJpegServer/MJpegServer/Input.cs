using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMButton
{
    public static class Input
    {
        static CDmSoft DM;
        public enum KeyFlag
        {
            Down=0,
            Up=1,
            Press=2,
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
        }

        public delegate void KeySend(int vk,KeyFlag flag);
        public delegate void MouseSend(int x, int y, MouseFlag flag, int data = 0);

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
                MouseInput=((int x, int y, MouseFlag flag,int data)=>{
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
