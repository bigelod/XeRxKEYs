using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs
{
    interface IOutModule
    {
        string DisplayName { get; set; }

        void Setup();

        void Shutdown();

        void PrepareWindow(IntPtr myHandle, IntPtr targetHandle);

        void SendInput(List<SendableInput> inputs);
    }
}
