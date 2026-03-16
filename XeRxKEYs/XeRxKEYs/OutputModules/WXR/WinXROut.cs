using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs.OutputModules.WXRModule
{
    public class WinXROut : IOutModule
    {
        public string DisplayName { get; set; }

        private int udpPortOUT = 7728;
        private string targetIP = "127.0.0.1";

        private UdpClient transmitClient;

        public void Setup()
        {
            transmitClient = new UdpClient();
        }
        public void Shutdown()
        {
            try
            {
                transmitClient.Close();
            }
            catch (Exception e)
            {

            }
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
                    //PressKey
                }
                else if (input.Type == SendType.MouseMove)
                {
                    //MoveMouse
                }
                else if (input.Type == SendType.MouseScroll)
                {
                    //ScrollMouseWheel
                }
                else if (input.Type == SendType.MouseClick)
                {
                    //SendMouseClick
                }
            }
        }

        public void SendData(string data)
        {
            byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(data);

            transmitClient.Send(serverMessageAsByteArray, serverMessageAsByteArray.Length, new IPEndPoint(IPAddress.Parse(targetIP), udpPortOUT));
        }
    }
}
