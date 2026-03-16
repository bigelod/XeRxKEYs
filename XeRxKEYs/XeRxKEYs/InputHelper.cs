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

            //Keys
            foreach (SendableKey key in AllSendableKeys)
            {
                AllSendableInputs.Add(new SendableInput(key));
            }

            //Mouse buttons
            AllSendableInputs.Add(new SendableInput(0));
            AllSendableInputs.Add(new SendableInput(1));
            AllSendableInputs.Add(new SendableInput(2));

            //Mouse scroll events
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
    }
}
