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

namespace XeRxKEYs.OutputModules.SendKeysModule
{
    //Currently unfinished as it wasn't working as hoped
    public class SendKeysModule : IOutModule
    {
        public string DisplayName { get; set; }

        public void Setup()
        {

        }

        public void Shutdown()
        {

        }

        public void PrepareWindow(IntPtr myHandle, IntPtr targetHandle)
        {

        }

        public void SendInput(List<SendableInput> inputs)
        {
            foreach (SendableInput input in inputs)
            {
                if (input.Type == SendType.Keyboard)
                {
                    PressKey(input.SendKey);
                }
            }
        }

        public void PressKey(SendableKey key)
        {
            SendKeys.SendWait(key.SendValue);
        }
    }
}
