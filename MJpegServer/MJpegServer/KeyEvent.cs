using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KMButton
{
    static public class KeyEvent
    {
        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Ansi, EntryPoint = "MapVirtualKeyA")]
        public static extern UInt32 MapVirtualKey(UInt32 nCode, UInt32 uMapType);

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(0)]
            public Int32 type;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 mouseData;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }
        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int KEYEVENTF_SCANCODE = 0x0008;
        public const int MOUSEEVENTF_MOVE = 1;
        public const int MOUSEEVENTF_LEFTDOWN = 2;
        public const int MOUSEEVENTF_LEFTUP = 4;
        public const int MOUSEEVENTF_RIGHTDOWN = 8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_WHEEL = 0x800;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        static int W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        static int H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        public static void KeySend(int key, Input.KeyFlag flag)
        {
            INPUT[] input = new INPUT[2];
            input[0] = new INPUT();
            input[1] = new INPUT();
            input[0].type = input[1].type = INPUT_KEYBOARD;
            input[0].ki.wVk = input[1].ki.wVk = (short)key;
            short scan = (short)MapVirtualKey((uint)key, 0);
            input[0].ki.wScan = scan;
            input[1].ki.wScan = (short)(scan + 128);
            input[0].ki.dwFlags = KEYEVENTF_SCANCODE;
            input[1].ki.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_SCANCODE;
            switch (key)
            {
                case 37:
                case 38:
                case 39:
                case 40:
                    input[0].ki.dwFlags |= 1;
                    input[1].ki.dwFlags |= 1;
                    break;
            }
            switch ((int)flag)
            {
                case 0:
                    SendInput(1, input, Marshal.SizeOf(input[0]));
                    break;
                case 1:
                    input[0] = input[1];
                    SendInput(1, input, Marshal.SizeOf(input[0]));
                    break;
                case 2:
                    SendInput(2, input, Marshal.SizeOf(input[0]));
                    break;
            }
        }
        public static void MouseSend(int x, int y, Input.MouseFlag flag, int data)
        {
            INPUT[] input = new INPUT[1];
            int value = (int)flag;
            if (flag == Input.MouseFlag.MoveTo)
            {
                value = ((int)(MOUSEEVENTF_ABSOLUTE) | (int)(MOUSEEVENTF_MOVE));
                x = 65536 * x / W;
                y = 65536 * y / H;
            }
            if (flag == Input.MouseFlag.AddTo)
            {
                x = 65536 * x / W;
                y = 65536 * y / H;
            }
            input[0] = new INPUT();
            input[0].mi.dx = x;
            input[0].mi.dy = y;
            input[0].mi.dwExtraInfo = (IntPtr)0;
            input[0].mi.time = 0;
            input[0].mi.mouseData = data;

            input[0].mi.dwFlags = value;
            input[0].type = INPUT_MOUSE;
            SendInput(1, input, Marshal.SizeOf(input[0]));
        }

    }
}
