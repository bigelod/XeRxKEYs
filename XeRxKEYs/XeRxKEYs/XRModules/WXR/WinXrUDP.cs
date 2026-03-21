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
        private int udpPortINFallback = 7873;

        private int udpPortOUT = 7278;
        private string targetIP = "127.0.0.1";

        private UdpClient udpClient;
        private Thread udpReadThread;

        private UdpClient transmitClient;

        private Action<string, float> receiveAction;

        private int lastTickCount;

        public WinXrUDP(Action<string, float> act)
        {
            receiveAction = act;

            transmitClient = new UdpClient();

            udpReadThread = new Thread(new ThreadStart(ReceiveData));
            udpReadThread.IsBackground = true;
            udpReadThread.Start();
        }

        public void SendHapticVibration(float lControllerVibration, float rControllerVibration, string vrFlag = "3", string sbsFlag = "0")
        {
            //V0.2 now takes L Vibration, R Vibration, VR flag, SBS flag, target FOV W, target FOV H
            float lCV = Math.Max(0f, lControllerVibration);
            float rCV = Math.Max(0f, rControllerVibration);

            string lVibe = lCV.ToString("0.00");
            string rVibe = rCV.ToString("0.00");

            if (lCV + rCV == 0)
            {
                lVibe = "0";
                rVibe = "0";
            }

            SendData(lVibe + " " + rVibe + " 1 " + vrFlag + " " + sbsFlag + " 104.5 104.5");
        }

        public void SendData(string data)
        {
            byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(data);

            transmitClient.Send(serverMessageAsByteArray, serverMessageAsByteArray.Length, new IPEndPoint(IPAddress.Parse(targetIP), udpPortOUT));
        }

        private void ReceiveData()
        {
            try
            {
                udpClient = new UdpClient(udpPortIN);
            }
            catch
            {
                udpClient = new UdpClient(udpPortINFallback);
            }

            lastTickCount = Environment.TickCount;

            while (true)
            {
                try
                {
                    IPEndPoint recieveFromAnyIP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpClient.Receive(ref recieveFromAnyIP);

                    int currentTickCount = Environment.TickCount;

                    int elapsedMilliseconds = currentTickCount - lastTickCount;
                    
                    lastTickCount = currentTickCount;

                    float deltaTime = (float)elapsedMilliseconds / 1000.0f;

                    string returnData = Encoding.ASCII.GetString(data);

                    if (deltaTime < 0) deltaTime = 0;

                    if (receiveAction != null)
                    {
                        receiveAction.Invoke(returnData, deltaTime);
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
