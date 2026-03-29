using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;

namespace XeRxKEYs.OutputModules.SendInputModule
{
    //Currently unfinished as it wasn't working as hoped
    public class SendInputModule : IOutModule
    {
        public string DisplayName
        {
            get
            {
                return "SendInputModule";
            }
            set
            {

            }
        }

        public void Setup()
        {

        }

        public void Shutdown()
        {

        }

        public void PrepareWindow(IntPtr myHandle, IntPtr targetHandle)
        {
            WindowsAPI.SetForegroundWindow(targetHandle);

            Thread.Sleep(100);

            uint temp;
            uint currentThreadId = WindowsAPI.GetWindowThreadProcessId(myHandle, out temp);
            uint foregroundThreadId = WindowsAPI.GetWindowThreadProcessId(targetHandle, out temp);
            WindowsAPI.AttachThreadInput(currentThreadId, foregroundThreadId, true);

            Thread.Sleep(100);
        }

        public void SendInput(List<SendableInput> inputs)
        {
            foreach (SendableInput input in inputs)
            {
                if (input.Type == SendType.Keyboard)
                {
                    //TODO: Update this to support more than just a character send, eg: symbols and arrows
                    PressKey(char.Parse(input.SendKey.DisplayName.Substring(0,1)), false);
                }
                else if (input.Type == SendType.MouseMove)
                {
                    MoveMouse(input.MouseX, input.MouseY);
                }
                else if (input.Type == SendType.MouseScroll)
                {
                    ScrollMouseWheel(input.MouseScrollAmount);
                }
                else if (input.Type == SendType.MouseClick)
                {
                    SendMouseClick(input.MouseButtonDownFlag, input.MouseButtonUpFlag);
                }
            }
        }
        public void PressKey(char ch, bool capital, int holdTime = 0)
        {
            if (holdTime > 0)
            {
                SendKeyDown(ch, capital);
                Thread.Sleep(holdTime);
                SendKeyUp(ch, capital);
            }
            else
            {
                byte vk = WindowsAPI.VkKeyScan(ch);
                ushort vkey = (ushort)(vk & 0xff);

                bool shift_required = (vk >> 8) == 1;

                if (capital)
                {
                    shift_required = true;
                }

                var inputs = new List<INPUT>();

                if (shift_required)
                {
                    INPUT shiftDown = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                    shiftDown.ki.wVk = (ushort)Keys.ShiftKey;
                    shiftDown.ki.dwFlags = 0;
                    inputs.Add(shiftDown);
                }

                INPUT keyDown = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                keyDown.ki.wVk = vkey;
                keyDown.ki.dwFlags = 0;
                inputs.Add(keyDown);

                INPUT keyUp = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                keyUp.ki.wVk = vkey;
                keyUp.ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
                inputs.Add(keyUp);

                if (shift_required)
                {
                    INPUT shiftUp = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                    shiftUp.ki.wVk = (ushort)Keys.ShiftKey;
                    shiftUp.ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
                    inputs.Add(shiftUp);
                }
                WindowsAPI.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
            }
        }

        public void SendKeyDown(char ch, bool capital = false)
        {
            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort vkey = (ushort)(vk & 0xff);
            bool shift_required = (vk >> 8) == 1 || capital;
            var inputs = new List<INPUT>();

            if (shift_required)
            {
                INPUT shiftDown = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                shiftDown.ki.wVk = (ushort)Keys.ShiftKey;
                shiftDown.ki.dwFlags = 0;
                inputs.Add(shiftDown);
            }

            INPUT keyDown = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
            keyDown.ki.wVk = vkey;
            keyDown.ki.dwFlags = 0;
            inputs.Add(keyDown);
            WindowsAPI.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
        public void SendKeyUp(char ch, bool capital = false)
        {
            byte vk = WindowsAPI.VkKeyScan(ch);
            ushort vkey = (ushort)(vk & 0xff);
            bool shift_required = (vk >> 8) == 1 || capital;
            var inputs = new List<INPUT>();

            INPUT keyUp = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
            keyUp.ki.wVk = vkey;
            keyUp.ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
            inputs.Add(keyUp);

            if (shift_required)
            {
                INPUT shiftUp = new INPUT { type = WindowsAPI.INPUT_KEYBOARD };
                shiftUp.ki.wVk = (ushort)Keys.ShiftKey;
                shiftUp.ki.dwFlags = WindowsAPI.KEYEVENTF_KEYUP;
                inputs.Add(shiftUp);
            }
            WindowsAPI.SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }

        public void MoveMouse(int pixelX, int pixelY)
        {
            Point absolutePos = ConvertPixelsToAbsolute(pixelX, pixelY);
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_MOUSE;
            inputs[0].mi.dx = absolutePos.X;
            inputs[0].mi.dy = absolutePos.Y;
            inputs[0].mi.dwFlags = WindowsAPI.MOUSEEVENTF_MOVE | WindowsAPI.MOUSEEVENTF_ABSOLUTE;
            WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void SendMouseClick(uint buttonDownFlag, uint buttonUpFlag, int holdTime = 0)
        {
            if (holdTime > 0)
            {
                SendMouseDown(buttonDownFlag);
                Thread.Sleep(holdTime);
                SendMouseUp(buttonUpFlag);
            }
            else
            {
                INPUT[] inputs = new INPUT[2];
                inputs[0].type = WindowsAPI.INPUT_MOUSE;
                inputs[0].mi.dwFlags = buttonDownFlag;
                inputs[1].type = WindowsAPI.INPUT_MOUSE;
                inputs[1].mi.dwFlags = buttonUpFlag;
                WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            }
        }

        public void SendMouseDown(uint buttonDownFlag)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_MOUSE;
            inputs[0].mi.dwFlags = buttonDownFlag;
            WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void SendMouseUp(uint buttonUpFlag)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_MOUSE;
            inputs[0].mi.dwFlags = buttonUpFlag;
            WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public void ScrollMouseWheel(int scrollAmount)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = WindowsAPI.INPUT_MOUSE;
            inputs[0].mi.mouseData = (uint)scrollAmount;
            inputs[0].mi.dwFlags = WindowsAPI.MOUSEEVENTF_WHEEL;
            WindowsAPI.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public Point ConvertPixelsToAbsolute(int pixelX, int pixelY)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            int absoluteX = (pixelX * 65535) / screenWidth;
            int absoluteY = (pixelY * 65535) / screenHeight;
            return new Point(absoluteX, absoluteY);
        }
    }
}
