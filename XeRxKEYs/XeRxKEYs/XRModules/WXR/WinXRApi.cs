using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs.XRModules.WXR
{
    public class WinXRApi : IXRModule
    {
        public string DisplayName
        {
            get
            {
                return "WinXRApi";
            }
            set
            {

            }
        }

        public TrackedObject Head { get; set; }
        public TrackedObject L_Hand { get; set; }
        public TrackedObject R_Hand { get; set; }

        private WinXrUDP xrUDP;

        private List<HMDSpecificAdjustment> HMDAdjusts = new List<HMDSpecificAdjustment>();
        private string currentHMD = "META";
        private string currentHMDModel = "EUREKA";

        private float LControllerVibration = 0f;
        private float RControllerVibration = 0f;

        private bool ForceHandsFix = false;

        private float currIPD = 0f;
        private float currHMDQX = 0f;
        private float currHMDQY = 0f;
        private float currHMDQZ = 0f;
        private float currHMDQW = 0f;
        private float currHMDX = 0f;
        private float currHMDY = 0f;
        private float currHMDZ = 0f;
        private float currLThumbX = 0f;
        private float currLThumbY = 0f;
        private float currRThumbX = 0f;
        private float currRThumbY = 0f;
        private float currLHandQX = 0f;
        private float currLHandQY = 0f;
        private float currLHandQZ = 0f;
        private float currLHandQW = 0f;
        private float currLHandX = 0f;
        private float currLHandY = 0f;
        private float currLHandZ = 0f;
        private float currRHandQX = 0f;
        private float currRHandQY = 0f;
        private float currRHandQZ = 0f;
        private float currRHandQW = 0f;
        private float currRHandX = 0f;
        private float currRHandY = 0f;
        private float currRHandZ = 0f;
        private float currFOVH = 110f;
        private float currFOVV = 96f;
        private int currFrameID = 0;
        private bool btnLGrip = false;
        private bool btnLMenu = false;
        private bool btnLThumbClick = false;
        private bool btnLThumbLeft = false;
        private bool btnLThumbRight = false;
        private bool btnLThumbUp = false;
        private bool btnLThumbDown = false;
        private bool btnLTrigger = false;
        private bool btnX = false;
        private bool btnY = false;
        private bool btnA = false;
        private bool btnB = false;
        private bool btnRGrip = false;
        private bool btnRThumbClick = false;
        private bool btnRThumbLeft = false;
        private bool btnRThumbRight = false;
        private bool btnRThumbUp = false;
        private bool btnRThumbDown = false;
        private bool btnRTrigger = false;
        private bool isImmersive = false;
        private bool isSBS = false;
        private bool lockFrameData = false;

        private string dataDirectory = "Z:/tmp/xr";

        public WinXRApi()
        {
            Head = new TrackedObject("Head");
            L_Hand = new TrackedObject("Left Hand");
            R_Hand = new TrackedObject("Right Hand");

            TrackedObject.hmdObj = Head;
        }

        public void Setup()
        {
            if (HMDAdjusts.Count <= 0)
            {
                //Default HMD adjustments
                HMDSpecificAdjustment picoHMD = new HMDSpecificAdjustment("PICO");
                picoHMD.FlipHandZRot = true;
                HMDAdjusts.Add(picoHMD);

                //EUREKA - Quest 3 - Works, assumed default
                //PANTHER - Quest 3 S - Works, assumed same as Quest 3
                //SEACLIFF - Quest Pro - Untested, assuming hands upside down
                //HOLLYWOOD - Quest 2 - Works! Hands upside down, performance is OK (better with Turnip driver)
                //MONTEREY - Quest 1 - Untested, unsupported
                HMDSpecificAdjustment questProHMD = new HMDSpecificAdjustment("META", "SEACLIFF");
                questProHMD.FlipHandZRot = true;
                HMDAdjusts.Add(questProHMD);

                HMDSpecificAdjustment quest2HMD = new HMDSpecificAdjustment("META", "HOLLYWOOD");
                quest2HMD.FlipHandZRot = true;
                HMDAdjusts.Add(quest2HMD);

                //Quest 3 and 3 S don't need any adjustments
                HMDSpecificAdjustment quest3HMD = new HMDSpecificAdjustment("META", "EUREKA");
                HMDAdjusts.Add(quest3HMD);

                HMDSpecificAdjustment quest3SHMD = new HMDSpecificAdjustment("META", "PANTHER");
                HMDAdjusts.Add(quest3SHMD);

                //Play For Dream:
                HMDSpecificAdjustment pfdHMD = new HMDSpecificAdjustment("PLAY FOR DREAM");
                pfdHMD.FlipHandZRot = true;
                HMDAdjusts.Add(pfdHMD);
            }

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

            string versionFile = dataDirectory + "/version";

            if (!File.Exists(versionFile))
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(versionFile))
                    {
                        sw.WriteLine("0.5");
                    }
                }
                catch
                {

                }
            }

            string systemFile = dataDirectory + "/system";

            if (File.Exists(systemFile))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(systemFile))
                    {
                        currentHMD = sr.ReadLine();
                        currentHMDModel = sr.ReadLine();
                    }
                }
                catch
                {
                    currentHMD = "META";
                    currentHMDModel = "EUREKA";
                }

                if (currentHMD.ToUpper() == "OCULUS") currentHMD = "META"; //Hard fix for new brand
            }

            if (File.Exists(Path.Combine(Application.StartupPath, "handsfix.txt")))
            {
                ForceHandsFix = true;
            }

            xrUDP = new WinXrUDP(ReceiveData);

            xrUDP.SendHapticVibration(0.0f, 0.0f); //Initialize the tracking
        }

        public void Shutdown()
        {
            if (xrUDP != null)
            {
                xrUDP.Destroy();
            }
        }

        public void SendCommand(string cmd)
        {
            //TODO: Consider ways to send haptic feedback to the user?
        }

        public string RequestData(string request)
        {
            string ans = "";

            //TODO: Consider ways to read input values from controllers beyond Vector3 Position and Quaternion Rotation? eg: button inputs

            return ans;
        }

        public void ReceiveData(string data, float deltaTime)
        {
            if (!lockFrameData)
            {
                lockFrameData = true;

                string[] parts = data.Split(' ');

                string btnbools = "";

                int i = 1;
                int maxParts = parts.Length - 1;

                if (maxParts >= i) UpdateValue(ref currLHandQX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandQY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandQZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandQW, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLThumbX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLThumbY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currLHandZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandQX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandQY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandQZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandQW, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRThumbX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRThumbY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currRHandZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDQX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDQY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDQZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDQW, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDX, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDY, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currHMDZ, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currIPD, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currFOVH, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValue(ref currFOVV, TryParseStr(parts[i]));
                i++;
                if (maxParts >= i) UpdateValueInt(ref currFrameID, TryParseStrToInt(parts[i]));
                i++;
                if (maxParts >= i) btnbools = parts[i].Replace(" ", "");

                if (btnbools != "")
                {
                    btnLGrip = ParseBtnBool(btnbools, 0);
                    btnLMenu = ParseBtnBool(btnbools, 1);
                    btnLThumbClick = ParseBtnBool(btnbools, 2);
                    btnLThumbLeft = ParseBtnBool(btnbools, 3);
                    btnLThumbRight = ParseBtnBool(btnbools, 4);
                    btnLThumbUp = ParseBtnBool(btnbools, 5);
                    btnLThumbDown = ParseBtnBool(btnbools, 6);
                    btnLTrigger = ParseBtnBool(btnbools, 7);
                    btnX = ParseBtnBool(btnbools, 8);
                    btnY = ParseBtnBool(btnbools, 9);
                    btnA = ParseBtnBool(btnbools, 10);
                    btnB = ParseBtnBool(btnbools, 11);
                    btnRGrip = ParseBtnBool(btnbools, 12);
                    btnRThumbClick = ParseBtnBool(btnbools, 13);
                    btnRThumbLeft = ParseBtnBool(btnbools, 14);
                    btnRThumbRight = ParseBtnBool(btnbools, 15);
                    btnRThumbUp = ParseBtnBool(btnbools, 16);
                    btnRThumbDown = ParseBtnBool(btnbools, 17);
                    btnRTrigger = ParseBtnBool(btnbools, 18);
                    isImmersive = ParseBtnBool(btnbools, 19);
                    isSBS = ParseBtnBool(btnbools, 20);
                }

                Vector3 newHeadPos = new Vector3(currHMDX, currHMDY, currHMDZ);
                Quaternion newHeadRot = new Quaternion(currHMDQX, currHMDQY, currHMDQZ, currHMDQW);
                Vector3 newLHandPos = new Vector3(currLHandX, currLHandY, currLHandZ);
                Quaternion newLHandRot = new Quaternion(currLHandQX, currLHandQY, currLHandQZ, currLHandQW);
                Vector3 newRHandPos = new Vector3(currRHandX, currRHandY, currRHandZ);
                Quaternion newRHandRot = new Quaternion(currRHandQX, currRHandQY, currRHandQZ, currRHandQW);

                Head.Update(newHeadPos, newHeadRot, deltaTime);
                L_Hand.Update(newLHandPos, newLHandRot, deltaTime);
                R_Hand.Update(newRHandPos, newRHandRot, deltaTime);

                lockFrameData = false;
            }
        }

        public void TriggersComplete()
        {
            Head.ClearTriggers();
            L_Hand.ClearTriggers();
            R_Hand.ClearTriggers();
        }

        public TrackedObject GetTrackedObject(SerializableTrackedObject serializedTrackedObj)
        {
            TrackedObject ans = null;

            string shortName = serializedTrackedObj.TrackedObjectName.ToUpper().Substring(0, 1);

            if (shortName == "H")
            {
                return Head;
            }

            if (shortName == "L")
            {
                return L_Hand;
            }

            if (shortName == "R")
            {
                return R_Hand;
            }

            return ans;
        }

        private void UpdateValue(ref float currValue, float newValue)
        {
            currValue = newValue;
        }

        private void UpdateValueInt(ref int currValue, int newValue)
        {
            currValue = newValue;
        }

        private float TryParseStr(string str)
        {
            float ans = 0f;

            float.TryParse(str, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out ans);

            return ans;
        }

        private int TryParseStrToInt(string str)
        {
            int ans = 0;

            int.TryParse(str, out ans);

            return ans;
        }

        private bool ParseBtnBool(string boolstring, int index)
        {
            if (boolstring.Length > index && boolstring.ToUpper().Substring(index, 1) == "T")
            {
                return true;
            }

            return false;
        }
    }

    public class HMDSpecificAdjustment
    {
        public string HMDName = "";
        public string HMDModel = "ANY";
        public bool FlipHandXRot = false;
        public bool FlipHandYRot = false;
        public bool FlipHandZRot = false;

        public HMDSpecificAdjustment(string name = "HMD")
        {
            HMDName = name;
            HMDModel = "ANY";
        }

        public HMDSpecificAdjustment(string name = "HMD", string model = "ANY")
        {
            HMDName = name;
            HMDModel = model;
        }
    }
}
