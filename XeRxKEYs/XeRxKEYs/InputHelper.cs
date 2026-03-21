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
            AllSendableKeys.Add(new SendableKey("ALT", "%", (ushort)Keys.Menu));

            //Letter keys
            AllSendableKeys.Add(new SendableKey("A", "a", (ushort)Keys.A));
            AllSendableKeys.Add(new SendableKey("B", "b", (ushort)Keys.B));
            AllSendableKeys.Add(new SendableKey("C", "c", (ushort)Keys.C));
            AllSendableKeys.Add(new SendableKey("D", "d", (ushort)Keys.D));
            AllSendableKeys.Add(new SendableKey("E", "e", (ushort)Keys.E));
            AllSendableKeys.Add(new SendableKey("F", "f", (ushort)Keys.F));
            AllSendableKeys.Add(new SendableKey("G", "g", (ushort)Keys.G));
            AllSendableKeys.Add(new SendableKey("H", "h", (ushort)Keys.H));
            AllSendableKeys.Add(new SendableKey("I", "i", (ushort)Keys.I));
            AllSendableKeys.Add(new SendableKey("J", "j", (ushort)Keys.J));
            AllSendableKeys.Add(new SendableKey("K", "k", (ushort)Keys.K));
            AllSendableKeys.Add(new SendableKey("L", "l", (ushort)Keys.L));
            AllSendableKeys.Add(new SendableKey("M", "m", (ushort)Keys.M));
            AllSendableKeys.Add(new SendableKey("N", "n", (ushort)Keys.N));
            AllSendableKeys.Add(new SendableKey("O", "o", (ushort)Keys.O));
            AllSendableKeys.Add(new SendableKey("P", "p", (ushort)Keys.P));
            AllSendableKeys.Add(new SendableKey("Q", "q", (ushort)Keys.Q));
            AllSendableKeys.Add(new SendableKey("R", "r", (ushort)Keys.R));
            AllSendableKeys.Add(new SendableKey("S", "s", (ushort)Keys.S));
            AllSendableKeys.Add(new SendableKey("T", "t", (ushort)Keys.T));
            AllSendableKeys.Add(new SendableKey("U", "u", (ushort)Keys.U));
            AllSendableKeys.Add(new SendableKey("V", "v", (ushort)Keys.V));
            AllSendableKeys.Add(new SendableKey("W", "w", (ushort)Keys.W));
            AllSendableKeys.Add(new SendableKey("X", "x", (ushort)Keys.X));
            AllSendableKeys.Add(new SendableKey("Y", "y", (ushort)Keys.Y));
            AllSendableKeys.Add(new SendableKey("Z", "z", (ushort)Keys.Z));

            //Number keys
            AllSendableKeys.Add(new SendableKey("0", "0", (ushort)Keys.D0));
            AllSendableKeys.Add(new SendableKey("1", "1", (ushort)Keys.D1));
            AllSendableKeys.Add(new SendableKey("2", "2", (ushort)Keys.D2));
            AllSendableKeys.Add(new SendableKey("3", "3", (ushort)Keys.D3));
            AllSendableKeys.Add(new SendableKey("4", "4", (ushort)Keys.D4));
            AllSendableKeys.Add(new SendableKey("5", "5", (ushort)Keys.D5));
            AllSendableKeys.Add(new SendableKey("6", "6", (ushort)Keys.D6));
            AllSendableKeys.Add(new SendableKey("7", "7", (ushort)Keys.D7));
            AllSendableKeys.Add(new SendableKey("8", "8", (ushort)Keys.D8));
            AllSendableKeys.Add(new SendableKey("9", "9", (ushort)Keys.D9));

            //Extra symbols
            AllSendableKeys.Add(new SendableKey("!", "{!}", 0));
            AllSendableKeys.Add(new SendableKey("@", "+2", "{@}", 0));
            AllSendableKeys.Add(new SendableKey("#", "+3", "{#}", 0));
            AllSendableKeys.Add(new SendableKey("*", "+8", "{MULTIPLY}", 0));
            AllSendableKeys.Add(new SendableKey("(", "+9", "{(}", 0));
            AllSendableKeys.Add(new SendableKey(")", "+0", "{)}", 0));
            AllSendableKeys.Add(new SendableKey("-", "-", "{SUBTRACT}", 0));
            AllSendableKeys.Add(new SendableKey("_", "_", "{_}", 0));
            AllSendableKeys.Add(new SendableKey("=", "=", "{=}", 0));
            AllSendableKeys.Add(new SendableKey("+", "{+}", "{ADD}", 0));
            AllSendableKeys.Add(new SendableKey("[", "[", "{[}", 0));
            AllSendableKeys.Add(new SendableKey("]", "]", "{]}", 0));
            AllSendableKeys.Add(new SendableKey("{", "+[", "{{}", 0));
            AllSendableKeys.Add(new SendableKey("}", "+]", "{}}", 0));
            AllSendableKeys.Add(new SendableKey(";", ";", "{;}", 0));
            AllSendableKeys.Add(new SendableKey(":", "+;", "{:}", 0));
            AllSendableKeys.Add(new SendableKey("'", "'", "{'}", 0));
            AllSendableKeys.Add(new SendableKey("\"", "+'", "{\"}", 0));
            AllSendableKeys.Add(new SendableKey("`", "`", "{`}", 0));
            AllSendableKeys.Add(new SendableKey("~", "+`", "{~}", 0));
            AllSendableKeys.Add(new SendableKey("/", "/", "{/}", 0));
            AllSendableKeys.Add(new SendableKey("\\", "\\", "{//}", 0));
            AllSendableKeys.Add(new SendableKey("|", "+/", "{|}", 0));
            AllSendableKeys.Add(new SendableKey(",", ",", "{,}", 0));
            AllSendableKeys.Add(new SendableKey(".", ".", "{.}", 0));
            AllSendableKeys.Add(new SendableKey("<", "+,", "{<}", 0));
            AllSendableKeys.Add(new SendableKey(">", "+.", "{>}", 0));
            AllSendableKeys.Add(new SendableKey("?", "+\\", "{?}", 0));

            //Special keys
            AllSendableKeys.Add(new SendableKey("BACKSPACE", "{BS}", (ushort)Keys.Back));
            AllSendableKeys.Add(new SendableKey("PAGE DOWN", "{PGDN}", (ushort)Keys.PageDown));
            AllSendableKeys.Add(new SendableKey("PAGE UP", "{PGUP}", (ushort)Keys.PageUp));
            AllSendableKeys.Add(new SendableKey("TAB", "{TAB}", (ushort)Keys.Tab));
            AllSendableKeys.Add(new SendableKey("DELETE", "{DEL}", (ushort)Keys.Delete));
            AllSendableKeys.Add(new SendableKey("END", "{END}", (ushort)Keys.End));
            AllSendableKeys.Add(new SendableKey("HOME", "{HOME}", (ushort)Keys.Home));
            AllSendableKeys.Add(new SendableKey("INSERT", "{INS}", (ushort)Keys.Insert));

            //Rarely used special keys
            AllSendableKeys.Add(new SendableKey("CAPS LOCK", "{CAPSLOCK}", (ushort)Keys.CapsLock));
            AllSendableKeys.Add(new SendableKey("NUM LOCK", "{NUMLOCK}", (ushort)Keys.NumLock));
            AllSendableKeys.Add(new SendableKey("PRINT SCREEN", "{PRTSC}", (ushort)Keys.PrintScreen));
            AllSendableKeys.Add(new SendableKey("SCROLL LOCK", "{SCROLLLOCK}", (ushort)Keys.Scroll));
            AllSendableKeys.Add(new SendableKey("BREAK", "{BREAK}", (ushort)Keys.Pause));
            AllSendableKeys.Add(new SendableKey("HELP", "{HELP}", (ushort)Keys.Help));
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

        public static SendableInput GetSendableInputByAltValue(string value)
        {
            SendableInput sendableInput = null;

            foreach (SendableInput input in AllSendableInputs)
            {
                if (input.Type == SendType.Keyboard && input.SendKey.AltValue == value)
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

        public static SendableKey GetSendableKeyByAltValue(string combo)
        {
            SendableKey sendableKey = null;

            foreach (SendableKey input in AllSendableKeys)
            {
                if (input.AltValue == combo)
                {
                    sendableKey = input;
                    break;
                }
            }

            return sendableKey;
        }
    }
}
