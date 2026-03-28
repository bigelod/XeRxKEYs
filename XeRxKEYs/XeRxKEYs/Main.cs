using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;
using XeRxKEYs.Gestures.GestureProfiles;
using XeRxKEYs.Gestures.MotionGestures;
using XeRxKEYs.Gestures.Triggers;
using XeRxKEYs.Gestures.Triggers.Actions;

namespace XeRxKEYs
{
    public partial class Main : Form
    {
        private const int inputSendTime = 5000;
        private System.Threading.Timer _keySenderTimer;

        private const int cooldownTimeMS = 100;
        private System.Threading.Timer _cooldownTimer;

        private bool sendLock = false;

        public GestureProfile ActiveGestureProfile = null;
        public List<SendableInput> ActiveSendables = new List<SendableInput>();

        private IXRModule xrModuleInstance = null;
        private Type xrModuleType = typeof(IXRModule);

        private List<IOutModule> activeOutModules = new List<IOutModule>();
        private List<Type> activeOutModuleTypes = new List<Type>();

        private List<TriggerAction> allTriggerActions = new List<TriggerAction>();
        private List<MotionGesture> allMotionGestures = new List<MotionGesture>();
        private List<GestureProfile> allGestureProfiles = new List<GestureProfile>();

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;

            Resize += Main_Resize;
            FormClosing += Main_FormClosing;
            niTaskbarIcon.MouseUp += niTaskbarIcon_MouseUp;

            _keySenderTimer = new System.Threading.Timer(TimerCallback, null, inputSendTime, inputSendTime);
            _cooldownTimer = new System.Threading.Timer(CooldownCallback, null, cooldownTimeMS, cooldownTimeMS);

            InputHelper.GenerateSendableInputs();

            var settings = Properties.Settings.Default;

            string xrMod = settings.XRModule;
            string outMods = settings.OutModules;
            string gestureProfile = settings.GestureProfile;
            string launchOnStart = "";

            //Parse the input arguments
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-module":
                        if (i + 1 < args.Length)
                        {
                            xrMod = args[i + 1];
                            i++;
                        }
                        break;
                    case "-outmod":
                        if (i + 1 < args.Length)
                        {
                            outMods = args[i + 1];
                            i++;
                        }
                        break;
                    case "-profile":
                        if (i + 1 < args.Length)
                        {
                            gestureProfile = args[i + 1];
                            i++;
                        }
                        break;
                    case "-launch":
                        if (i + 1 < args.Length)
                        {
                            launchOnStart = args[i + 1];
                            i++;
                        }
                        break;
                }
            }

            if (!SetupInputModule(xrMod))
            {
                MessageBox.Show(this, "Input module not found! Setting back to default", "XeRxKEYs - Error loading Input module!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                settings.XRModule = "WinXRApi";
                settings.Save();

                if (!SetupInputModule("WinXRApi"))
                {
                    MessageBox.Show(this, "Catastrophic error! Closing!", "XeRxKEYs - Error loading Input module!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }

            SetupOutputModules(outMods);

            LoadAssets();

            AssetTest();

            if (gestureProfile != "")
            {
                //Try to enable the active Gesture Profile


                if (settings.MinimizeAtStart)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }

            if (launchOnStart != "")
            {
                try
                {
                    string exeDir = Path.GetDirectoryName(launchOnStart);

                    if (Directory.Exists(exeDir))
                    {
                        if (File.Exists(launchOnStart))
                        {
                            Process.Start(launchOnStart);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void AssetTest()
        {
            //Create and save a Test Profile
            GestureProfile a = new GestureProfile("GestureTest");
            MotionGesture b = new MotionGesture("MotionTest");
            TriggerAction c = new TriggerAction("TriggerTest");
            
            SendableInputCombo d = new SendableInputCombo();

            d.ComboInputs.Add(InputHelper.GetSendableInputByName("UP ARROW"));
            d.ComboInputs.Add(InputHelper.GetSendableInputByName("RIGHT ARROW"));

            c.InputCombos.Add(d);

            SendableInputCombo e = new SendableInputCombo();

            e.ComboInputs.Add(InputHelper.GetSendableInputByName("MOUSE LEFT CLICK"));
            e.ComboInputs.Add(InputHelper.GetSendableInputByName("BACKSPACE"));

            c.InputCombos.Add(e);

            TriggerCondition f = new TriggerCondition(TriggerConditionType.Shake_Vertical);

            f.ShakeEvent.Trigger_For_Objects = new List<TrackedObject>();
            f.ShakeEvent.Trigger_For_Objects.Add(xrModuleInstance.GetTrackedObject(new SerializableTrackedObject("Left Hand")));
            f.ShakeEvent.Trigger_When = new ChangeAmount();

            TriggerCondition g = new TriggerCondition(TriggerConditionType.Proximity);
            g.ProximityEvent.Device_Group_A.Add(xrModuleInstance.GetTrackedObject(new SerializableTrackedObject("Head")));
            g.ProximityEvent.Device_Group_B.Add(xrModuleInstance.GetTrackedObject(new SerializableTrackedObject("Right Hand")));
            //g.ProximityEvent.Invert = true;
            g.ProximityEvent.Trigger_When = new ChangeAmount();

            //f.Disable_If_Trigger_Conditions.Add(g);

            b.TriggerConditions.Add(f);
            b.TriggerConditions.Add(g);

            b.TriggerOnAnyCondition = true;

            AddTriggerAction(c);

            b.TriggerActions.Add(c);
            AddMotionGesture(b);

            a.Gestures.Add(b);
            AddGestureProfile(a);

            SaveAssets();

            ActiveGestureProfile = a;
        }

        private void SetupOutputModules(string modules)
        {
            string[] moduleNames = modules.Split(',');

            foreach (string module in moduleNames)
            {
                if (!string.IsNullOrEmpty(module))
                {
                    if (!SetupOutputModule(module))
                    {
                        MessageBox.Show(this, "Output Module '" + module + "' failed to load, continuing without it.", "XeRxKEYs - Error loading Output module!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private bool SetupOutputModule(string moduleName)
        {
            IOutModule outModuleInstance = null;
            Type outModuleType = typeof(IOutModule);

            bool result = GetOutputModule(moduleName, ref outModuleType);

            if (result)
            {
                outModuleInstance = (IOutModule)Activator.CreateInstance(outModuleType);
                outModuleInstance.Setup();

                activeOutModules.Add(outModuleInstance);
                activeOutModuleTypes.Add(outModuleType);
            }

            return result;
        }

        private bool GetOutputModule(string moduleName, ref Type outModuleType)
        {
            bool found = false;

            foreach (Type moduleType in Assembly.GetExecutingAssembly().GetTypes()
                 .Where(moduleType => moduleType.GetInterfaces().Contains(typeof(IOutModule))))
            {
                if (moduleType.Name == moduleName)
                {
                    outModuleType = moduleType;
                    found = true;
                    break;
                }
            }

            return found;
        }

        private bool SetupInputModule(string moduleName)
        {
            bool result = GetInputModule(moduleName);

            if (result)
            {
                xrModuleInstance = (IXRModule)Activator.CreateInstance(xrModuleType);
                xrModuleInstance.Setup();
            }

            return result;
        }

        private bool GetInputModule(string moduleName)
        {
            bool found = false;

            foreach (Type moduleType in Assembly.GetExecutingAssembly().GetTypes()
                 .Where(moduleType => moduleType.GetInterfaces().Contains(typeof(IXRModule))))
            {
                if (moduleType.Name == moduleName)
                {
                    xrModuleType = moduleType;
                    found = true;
                    break;
                }
            }

            return found;
        }
        private void SendKeysToForegroundWindow(IntPtr myOwnHandle)
        {
            IntPtr targetHwnd = WindowsAPI.GetForegroundWindow();
            if (sendLock || targetHwnd == myOwnHandle || targetHwnd == IntPtr.Zero) // || ActiveSendables.Count <= 0)
            {
                return;
            }

            sendLock = true;

            //Check all Trigger Actions to prepare the active sendables list, then send them
            if (ActiveGestureProfile != null)
            {
                foreach (MotionGesture gesture in ActiveGestureProfile.Gestures)
                {
                    ActiveSendables.AddRange(gesture.GatherInputs());
                }
            }

            //Finished checking all Trigger Actions
            if (xrModuleInstance != null)
            {
                xrModuleInstance.TriggersComplete();
            }

            foreach (IOutModule module in activeOutModules)
            {
                module.SendInput(ActiveSendables);
            }

            ActiveSendables.Clear();

            sendLock = false;
        }

        public void AddTriggerAction(TriggerAction action)
        {
            for (int i = allTriggerActions.Count - 1; i >= 0; i--)
            {
                if (allTriggerActions[i].Name == action.Name)
                {
                    allTriggerActions.RemoveAt(i);
                    break;
                }
            }

            allTriggerActions.Add(action);
        }

        public void AddMotionGesture(MotionGesture motion)
        {
            for (int i = allMotionGestures.Count - 1; i >= 0; i--)
            {
                if (allMotionGestures[i].Name == motion.Name)
                {
                    allMotionGestures.RemoveAt(i);
                    break;
                }
            }

            allMotionGestures.Add(motion);
        }

        public void AddGestureProfile(GestureProfile gesture)
        {
            for (int i = allGestureProfiles.Count - 1; i >= 0; i--)
            {
                if (allGestureProfiles[i].Name == gesture.Name)
                {
                    allGestureProfiles.RemoveAt(i);
                    break;
                }
            }

            allGestureProfiles.Add(gesture);
        }

        public List<TriggerAction> GetTriggerActions()
        {
            return allTriggerActions;
        }

        public List<MotionGesture> GetMotionGestures()
        {
            return allMotionGestures;
        }

        public List<GestureProfile> GetGestureProfiles()
        {
            return allGestureProfiles;
        }

        private void LoadAssets()
        {
            LoadTriggerActions();

            LoadMotionGestures();

            LoadGestureProfiles();

            foreach (MotionGesture motion in allMotionGestures)
            {
                for (int i = 0; i < motion.TriggerActions.Count; i++)
                {
                    TriggerAction loadedAct = motion.TriggerActions[i];
                    string actName = loadedAct.Name;

                    bool actFound = false;

                    foreach (TriggerAction globalAct in allTriggerActions)
                    {
                        if (actName == globalAct.Name)
                        {
                            actFound = true;
                            motion.TriggerActions[i] = globalAct;
                            break;
                        }
                    }

                    if (!actFound)
                    {
                        allTriggerActions.Add(loadedAct);
                    }
                }
            }

            foreach (GestureProfile profile in allGestureProfiles)
            {
                for (int i = 0; i < profile.Gestures.Count; i++)
                {
                    MotionGesture loadedGesture = profile.Gestures[i];
                    string gestureName = loadedGesture.Name;

                    bool gestureFound = false;

                    foreach (MotionGesture globalGesture in allMotionGestures)
                    {
                        if (gestureName == globalGesture.Name)
                        {
                            gestureFound = true;
                            profile.Gestures[i] = globalGesture;
                            break;
                        }
                    }

                    if (!gestureFound)
                    {
                        allMotionGestures.Add(loadedGesture);
                    }
                }
            }
        }

        private void SaveAssets()
        {
            SaveTriggerActions();

            SaveMotionGestures();

            SaveGestureProfiles();
        }

        private void LoadTriggerActions()
        {
            ActionLoadSave.LoadProfiles(ref allTriggerActions);
        }

        private void LoadMotionGestures()
        {
            MotionLoadSave.LoadProfiles(ref allMotionGestures, ref xrModuleInstance);
        }

        private void LoadGestureProfiles()
        {
            GestureLoadSave.LoadProfiles(ref allGestureProfiles, ref xrModuleInstance);
        }

        private void SaveTriggerActions()
        {
            ActionLoadSave.SaveProfiles(allTriggerActions);
        }

        private void SaveMotionGestures()
        {
            MotionLoadSave.SaveProfiles(allMotionGestures);
        }

        private void SaveGestureProfiles()
        {
            GestureLoadSave.SaveProfiles(allGestureProfiles);
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void niTaskbarIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(niTaskbarIcon, null);
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (IOutModule module in activeOutModules)
            {
                module.Shutdown();
            }

            if (xrModuleInstance != null)
            {
                xrModuleInstance.Shutdown();
            }

            _keySenderTimer?.Dispose();
        }

        private void TimerCallback(object state)
        {
            IntPtr myOwnHandle = IntPtr.Zero;
            this.Invoke((MethodInvoker)delegate
            {
                myOwnHandle = this.Handle;
            });

            if (chkSendInputs.Checked)
            {
                SendKeysToForegroundWindow(myOwnHandle);
            }
        }

        private void CooldownCallback(object state)
        {
            this.Invoke((MethodInvoker)delegate
            {
                foreach (MotionGesture gesture in allMotionGestures)
                {
                    gesture.UpdateCooldown(cooldownTimeMS);
                }
            });
        }

        private void ShowWindow()
        {
            Show();

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
            }

            BringToFront();
            Focus();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        private void niTaskbarIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowWindow();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkSendInputs_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
