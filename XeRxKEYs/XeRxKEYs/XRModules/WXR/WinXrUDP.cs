using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace XeRxKEYs.XRModules.WXR
{
    //Based on this tutorial by Chiad Dogan: https://cihaddogan.medium.com/udp-communication-between-unity-and-matlab-simulink-d4a62921936d
    public class WinXrUDP
    {
        private int udpPortIN = 7872;

        private int udpPortOUT = 7278;
        private string targetIP = "127.0.0.1";

        private UdpClient udpClient;
        private Thread udpReadThread;

        private UdpClient transmitClient;

        private Action<string> receiveAction;

        public WinXrUDP(Action<string> act)
        {
            receiveAction = act;

            transmitClient = new UdpClient();

            udpReadThread = new Thread(new ThreadStart(ReceiveData));
            udpReadThread.IsBackground = true;
            udpReadThread.Start();
        }

        public void SendHapticVibration(float lControllerVibration, float rControllerVibration, string vrFlag = "2", string sbsFlag = "0")
        {
            //V0.2 now takes L Vibration, R Vibration, VR flag, SBS flag, target FOV W, target FOV H
            float lCV = Math.Max(0f, lControllerVibration);
            float rCV = Math.Max(0f, rControllerVibration);

            if (lCV + rCV == 0)
            {
                SendData("0 0 " + vrFlag + " " + sbsFlag + " 104.5 104.5");
            }
            else
            {
                string vibeData = lCV.ToString("0.00") + " " + rCV.ToString("0.00");

                SendData(vibeData + " 1 " + vrFlag + " " + sbsFlag + " 104.5 104.5");
            }
        }

        public void SendData(string data)
        {
            byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(data);

            transmitClient.Send(serverMessageAsByteArray, serverMessageAsByteArray.Length, new IPEndPoint(IPAddress.Parse(targetIP), udpPortOUT));
        }

        private void ReceiveData()
        {
            udpClient = new UdpClient(udpPortIN);

            while (true)
            {
                try
                {
                    IPEndPoint recieveFromAnyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref recieveFromAnyIP);

                    string returnData = Encoding.ASCII.GetString(data);

                    if (receiveAction != null)
                    {
                        receiveAction.Invoke(returnData);
                    }
                }
                catch (Exception e)
                {
                    
                }
            }
        }

        public void Destroy()
        {
            try
            {
                udpReadThread.Abort();
                udpReadThread = null;
                udpClient.Close();
                transmitClient.Close();
            }
            catch (Exception e)
            {
                
            }
        }
    }
}
