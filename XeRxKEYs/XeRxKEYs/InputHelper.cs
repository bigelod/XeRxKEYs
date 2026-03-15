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

namespace XeRxKEYs
{
    public class InputHelper
    {

        public List<SendableInput> AllSendableInputs = new List<SendableInput>();
        public List<SendableKey> AllSendableKeys = new List<SendableKey>();

        public InputHelper()
        {
            GenerateSendableInputs();
        }

        private void GenerateSendableInputs()
        {
            GenerateSendableKeys();

            foreach (SendableKey key in AllSendableKeys)
            {
                AllSendableInputs.Add(new SendableInput(key));
            }

            AllSendableInputs.Add(new SendableInput(0));
            AllSendableInputs.Add(new SendableInput(1));
            AllSendableInputs.Add(new SendableInput(2));

            AllSendableInputs.Add(new SendableInput(120, true));
            AllSendableInputs.Add(new SendableInput(120, false));
        }

        private void GenerateSendableKeys()
        {
            //TODO
            //Letter keys

            //TODO
            //Number keys

            //Common gaming keys
            AllSendableKeys.Add(new SendableKey("UP ARROW", "{UP}", (ushort)Keys.Up));
            AllSendableKeys.Add(new SendableKey("DOWN ARROW", "{DOWN}", (ushort)Keys.Down));
            AllSendableKeys.Add(new SendableKey("LEFT ARROW", "{LEFT}", (ushort)Keys.Left));
            AllSendableKeys.Add(new SendableKey("RIGHT ARROW", "{RIGHT}", (ushort)Keys.Right));
            AllSendableKeys.Add(new SendableKey("ENTER", "{ENTER}", (ushort)Keys.Enter));
            AllSendableKeys.Add(new SendableKey("ESCAPE", "{ESC}", (ushort)Keys.Escape));
            AllSendableKeys.Add(new SendableKey("SHIFT", "+", (ushort)Keys.ShiftKey));
            AllSendableKeys.Add(new SendableKey("CTRL", "^", (ushort)Keys.ControlKey));
            AllSendableKeys.Add(new SendableKey("SPACEBAR", " ", (ushort)Keys.Space));

            //TODO
            //Symbol keys

            //Special keys
            AllSendableKeys.Add(new SendableKey("ALT", "%", (ushort)Keys.Menu));
            AllSendableKeys.Add(new SendableKey("BACKSPACE", "{BS}", (ushort)Keys.Back));
            AllSendableKeys.Add(new SendableKey("PAGE DOWN", "{PGDN}", (ushort)Keys.PageDown));
            AllSendableKeys.Add(new SendableKey("PAGE UP", "{PGUP}", (ushort)Keys.PageUp));
            AllSendableKeys.Add(new SendableKey("TAB", "{TAB}", (ushort)Keys.Tab));
            AllSendableKeys.Add(new SendableKey("DELETE", "{DEL}", (ushort)Keys.Delete));
            AllSendableKeys.Add(new SendableKey("END", "{END}", (ushort)Keys.End));
            AllSendableKeys.Add(new SendableKey("HOME", "{HOME}", (ushort)Keys.Home));
            AllSendableKeys.Add(new SendableKey("INSERT", "{INS}", (ushort)Keys.Insert));

            //Rarely used special keys
            AllSendableKeys.Add(new SendableKey("BREAK", "{BREAK}", (ushort)Keys.Pause));
            AllSendableKeys.Add(new SendableKey("CAPS LOCK", "{CAPSLOCK}", (ushort)Keys.CapsLock));
            AllSendableKeys.Add(new SendableKey("HELP", "{HELP}", (ushort)Keys.Help));
            AllSendableKeys.Add(new SendableKey("NUM LOCK", "{NUMLOCK}", (ushort)Keys.NumLock));
            AllSendableKeys.Add(new SendableKey("PRINT SCREEN", "{PRTSC}", (ushort)Keys.PrintScreen));
            AllSendableKeys.Add(new SendableKey("SCROLL LOCK", "{SCROLLLOCK}", (ushort)Keys.Scroll));
        }

        #region FallbackInputHelperFunctions
        public void PressKeyFallback(SendableKey key)
        {
            SendKeys.SendWait(key.SendValue);
        }

        public void SendMouseClickFallback(uint down_flag, uint up_flag, int holdTime = 0)
        {
            WindowsAPI.mouse_event(down_flag, 0, 0, 0, 0);

            if (holdTime > 0) {
                Thread.Sleep(holdTime);
            }

            WindowsAPI.mouse_event(up_flag, 0, 0, 0, 0);
        }

        public void ScrollMouseWheelFallback(int scrollAmount)
        {
            WindowsAPI.mouse_event(WindowsAPI.MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
        }
        #endregion

        #region SendInputHelperFunctions
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
        #endregion
    }
}
