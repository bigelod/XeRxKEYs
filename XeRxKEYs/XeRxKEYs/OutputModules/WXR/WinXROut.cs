using System;
using System.Collections.Generic;
using System.IO;
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
        private int udpPortIN = 7287;
        private string targetIP = "127.0.0.1";

        private UdpClient transmitClient;

        private string dataDirectory = "Z:/tmp/xr";

        public void Setup()
        {
            transmitClient = new UdpClient();

            if (dataDirectory == "Z:/tmp/xr" && !Directory.Exists(dataDirectory))
            {
                if (Directory.Exists("Z:/"))
                {
                    //Try to create the temp directory if it doesn't exist
                    if (!Directory.Exists("Z:/tmp"))
                    {
                        Directory.CreateDirectory("Z:/tmp");
                    }
                }
                else
                {
                    //Fallback to a temp directory on whatever drive we are running on instead
                    dataDirectory = Path.GetFullPath("./").Substring(0, 1) + ":/xrtemp";
                }
            }

            if (!Directory.Exists(dataDirectory)) Directory.CreateDirectory(dataDirectory);

            if (!Directory.Exists(dataDirectory))
            {
                return;
            }

            string inputVersionFile = dataDirectory + "/input_version";

            if (!File.Exists(inputVersionFile))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(inputVersionFile))
                    {
                        sw.WriteLine("0.1");
                    }
                }
                catch
                {

                }
            }
        }
        public void Shutdown()
        {
            try
            {
                string inputVersionFile = dataDirectory + "/input_version";

                if (File.Exists(inputVersionFile))
                {
                    File.Delete(inputVersionFile);
                }

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
