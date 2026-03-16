using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApi;
using XeRxKEYs.Gestures.MotionGestures;
using XeRxKEYs.Gestures.GestureProfiles;
using XeRxKEYs.Gestures.Triggers.Actions;

namespace XeRxKEYs
{
    public partial class Main : Form
    {
        private const int inputSendTime = 5000;
        private System.Threading.Timer _keySenderTimer;

        private InputHelper inputHelper;

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

            //TODO: Update this to send more frequently
            _keySenderTimer = new System.Threading.Timer(TimerCallback, null, inputSendTime, inputSendTime);

            inputHelper = new InputHelper();

            var settings = Properties.Settings.Default;

            if (!SetupInputModule(settings.XRModule))
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

            SetupOutputModules(Properties.Settings.Default.OutModules);

            LoadTriggerActions();

            LoadMotionGestures();

            LoadGestureProfiles();

            //Create and save a Test Profile
            TriggerAction c = new TriggerAction();
            MotionGesture b = new MotionGesture("MotionTest");
            GestureProfile a = new GestureProfile("GestureTest");

            c.Name = "TriggerTest";
            allTriggerActions.Add(c);

            b.TriggerActions.Add(c);
            allMotionGestures.Add(b);

            a.Gestures.Add(b);
            allGestureProfiles.Add(a);

            SaveTriggerActions();

            SaveMotionGestures();

            SaveGestureProfiles();

            if (settings.GestureProfile != "")
            {
                //Try to enable the active Gesture Profile

                if (settings.MinimizeAtStart)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
            }
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

            foreach (IOutModule module in activeOutModules)
            {
                module.SendInput(ActiveSendables);
            }

            //ActiveSendables.Clear();

            sendLock = false;
        }

        private void LoadTriggerActions()
        {
            ActionLoadSave.LoadProfiles(ref allTriggerActions);
        }

        private void LoadMotionGestures()
        {
            MotionLoadSave.LoadProfiles(ref allMotionGestures);
        }

        private void LoadGestureProfiles()
        {
            GestureLoadSave.LoadProfiles(ref allGestureProfiles);
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
