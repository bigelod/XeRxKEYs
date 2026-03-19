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

namespace XeRxKEYs.OutputModules.MouseEvent
{
    //Works in WinlatorXR testing, but unused due to alternative method (InputAPI)
    public class MouseEvent : IOutModule
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
                if (input.Type == SendType.MouseMove)
                {
                    //MoveMouse? Currently unsupported
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

        public void SendMouseClick(uint down_flag, uint up_flag, int holdTime = 0)
        {
            WindowsAPI.mouse_event(down_flag, 0, 0, 0, 0);

            if (holdTime > 0)
            {
                Thread.Sleep(holdTime);
            }

            WindowsAPI.mouse_event(up_flag, 0, 0, 0, 0);
        }

        public void ScrollMouseWheel(int scrollAmount)
        {
            WindowsAPI.mouse_event(WindowsAPI.MOUSEEVENTF_WHEEL, 0, 0, scrollAmount, 0);
        }
    }
}
