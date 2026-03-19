using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WinApi;

namespace XeRxKEYs
{
    public static class InputHelper
    {
        public static List<SendableInput> AllSendableInputs = new List<SendableInput>();
        public static List<SendableInput> AllSendableMouseInputs = new List<SendableInput>();
        public static List<SendableKey> AllSendableKeys = new List<SendableKey>();

        public static void GenerateSendableInputs()
        {
            AllSendableInputs.Clear();

            GenerateSendableKeys();

            GenerateSendableMouseInputs();

            //Keys
            foreach (SendableKey key in AllSendableKeys)
            {
                AllSendableInputs.Add(new SendableInput(key));
            }

            //Mouse
            foreach (SendableInput input in AllSendableMouseInputs)
            {
                AllSendableInputs.Add(input);
            }
        }

        private static void GenerateSendableKeys()
        {
            AllSendableKeys.Clear();

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
        private static void GenerateSendableMouseInputs()
        {
            AllSendableMouseInputs.Clear();

            //Mouse buttons
            SendableInput mouseLeftClick = new SendableInput(0);
            SendableInput mouseRightClick = new SendableInput(1);
            SendableInput mouseMiddleClick = new SendableInput(2);

            //Mouse scroll events
            SendableInput mouseScrollUp = new SendableInput(120, false);
            SendableInput mouseScrollDown = new SendableInput(120, true);

            AllSendableMouseInputs.Add(mouseLeftClick);
            AllSendableMouseInputs.Add(mouseRightClick);
            AllSendableMouseInputs.Add(mouseMiddleClick);
            AllSendableMouseInputs.Add(mouseScrollUp);
            AllSendableMouseInputs.Add(mouseScrollDown);
        }

        public static SendableInput GetSendableInputByName(string name)
        {
            SendableInput sendableInput = null;

            foreach (SendableInput input in AllSendableInputs)
            {
                if (input.Name == name)
                {
                    sendableInput = input;
                    break;
                }
            }

            return sendableInput;
        }

        public static SendableInput GetSendableMouseInputByName(string name)
        {
            SendableInput sendableInput = null;

            foreach (SendableInput input in AllSendableMouseInputs)
            {
                if (input.Name == name)
                {
                    sendableInput = input;
                    break;
                }
            }

            return sendableInput;
        }

        public static SendableInput GetSendableInputBySendKey(string sendKey)
        {
            SendableInput sendableInput = null;

            foreach (SendableInput input in AllSendableInputs)
            {
                if (input.Type == SendType.Keyboard && input.SendKey.SendValue == sendKey)
                {
                    sendableInput = input;
                    break;
                }
            }

            return sendableInput;
        }

        public static SendableInput GetSendableInputByRawKeyValue(ushort value)
        {
            SendableInput sendableInput = null;

            foreach (SendableInput input in AllSendableInputs)
            {
                if (input.Type == SendType.Keyboard && input.SendKey.RawValue == value)
                {
                    sendableInput = input;
                    break;
                }
            }

            return sendableInput;
        }

        public static SendableKey GetSendableKeyByName(string name)
        {
            SendableKey sendableKey = null;

            foreach (SendableKey input in AllSendableKeys)
            {
                if (input.DisplayName == name)
                {
                    sendableKey = input;
                    break;
                }
            }

            return sendableKey;
        }

        public static SendableKey GetSendableKeyBySendKey(string sendKey)
        {
            SendableKey sendableKey = null;

            foreach (SendableKey input in AllSendableKeys)
            {
                if (input.SendValue == sendKey)
                {
                    sendableKey = input;
                    break;
                }
            }

            return sendableKey;
        }

        public static SendableKey GetSendableKeyByRawKeyValue(ushort value)
        {
            SendableKey sendableKey = null;

            foreach (SendableKey input in AllSendableKeys)
            {
                if (input.RawValue == value)
                {
                    sendableKey = input;
                    break;
                }
            }

            return sendableKey;
        }
    }
}
