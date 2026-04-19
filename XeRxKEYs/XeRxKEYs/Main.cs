using Newtonsoft.Json;
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
using XeRxKEYs.Properties;

namespace XeRxKEYs
{
    public partial class Main : Form
    {
        #region VARIABLES
        private const int inputSendTime = 5000; //TODO: Send input more frequently than once every 5 seconds once testing is further along
        private System.Threading.Timer _keySenderTimer;

        private const int cooldownTimeMS = 100;
        private System.Threading.Timer _cooldownTimer;

        private bool sendLock = false;

        public GestureProfile ActiveGestureProfile = null;
        public List<SendableInput> ActiveSendables = new List<SendableInput>();

        private Type xrModuleType = typeof(IXRModule);
        private List<Type> activeOutModuleTypes = new List<Type>();

        private List<string> allXRModuleNames = new List<string>();
        private List<string> allOutModuleNames = new List<string>();

        private IXRModule xrModuleInstance = null;

        private List<IOutModule> activeOutModules = new List<IOutModule>();

        private List<TriggerAction> allTriggerActions = new List<TriggerAction>();
        private List<MotionGesture> allMotionGestures = new List<MotionGesture>();
        private List<GestureProfile> allGestureProfiles = new List<GestureProfile>();

        private Action TabLeaveAction;

        private bool StopImageLoads = false;

        private Image noIconImg;

        private string imageDirectory = "";

        private int _editingGestureProfile = -1;
        private int _editingMotionGesture = -1;
        private int _editingTriggerAction = -1;
        private int _editingTriggerCondition = -1;

        private string _editingGestureProfileImage = "";
        private string _editingMotionGestureImage = "";

        private bool _GestureProfileChanged = false;
        private bool _MotionGestureChanged = false;
        private bool _TriggerActionChanged = false;
        private bool _TriggerConditionChanged = false;

        private bool listViewLock = false;

        private List<TriggerCondition> _motionGestureTriggerConditions = new List<TriggerCondition>();
        #endregion

        #region STARTUP
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

            tcDisplayTabs.SelectedIndexChanged += tcDisplayTabs_SelectedIndexChanged;

            _keySenderTimer = new System.Threading.Timer(TimerCallback, null, inputSendTime, inputSendTime);
            _cooldownTimer = new System.Threading.Timer(CooldownCallback, null, cooldownTimeMS, cooldownTimeMS);

            imageDirectory = Path.Combine(Application.StartupPath, "Images");

            lvwProfileEnabledMotionGestures.ItemChecked += lvwProfileEnabledMotionGestures_ItemChecked;

            tcDisplayTabs.Appearance = TabAppearance.FlatButtons;
            tcDisplayTabs.ItemSize = new Size(0, 1);
            tcDisplayTabs.SizeMode = TabSizeMode.Fixed;
            tcDisplayTabs.TabStop = false;

            tcConditionTabs.Appearance = TabAppearance.FlatButtons;
            tcConditionTabs.ItemSize = new Size(0, 1);
            tcConditionTabs.SizeMode = TabSizeMode.Fixed;
            tcConditionTabs.TabStop = false;

            //tcDisplayTabs.KeyDown += tcDisplayTabs_KeyDown;
            //tcDisplayTabs.KeyPress += tcDisplayTabs_KeyPress;
            //tcDisplayTabs.KeyUp += tcDisplayTabs_KeyUp;

            //tcConditionTabs.KeyDown += tcConditionTabs_KeyDown;
            //tcConditionTabs.KeyPress += tcConditionTabs_KeyPress;
            //tcConditionTabs.KeyUp += tcConditionTabs_KeyUp;

            LoadAllXRModules();
            LoadAllOutputModules();

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
                MessageBox.Show(this, "XR module not found! Setting back to default", "XeRxKEYs - Error loading XR module!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                settings.XRModule = "WinXRApi";
                settings.Save();

                if (!SetupInputModule("WinXRApi"))
                {
                    MessageBox.Show(this, "Catastrophic error! Closing!", "XeRxKEYs - Error loading XR module!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }

            SetupOutputModules(outMods);

            LoadAssets();

            //TODO: Remove call to AssetTest
            AssetTest();

            if (gestureProfile != "")
            {
                bool profileFound = false;

                foreach (GestureProfile profile in allGestureProfiles)
                {
                    if (profile.Name == gestureProfile)
                    {
                        profileFound = true;
                        ActiveGestureProfile = profile;

                        break;
                    }
                }

                if (!profileFound && settings.GestureProfile != "")
                {
                    settings.GestureProfile = "";
                    settings.Save();
                }

                if (settings.MinimizeAtStart)
                {
                    StopImageLoads = true;
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

            noIconImg = Properties.Resources.NoImg;

            RefreshMainUI();
        }
        #endregion

        #region BACKEND_CORE

        private void LoadAllXRModules()
        {
            allXRModuleNames.Clear();

            foreach (Type moduleType in Assembly.GetExecutingAssembly().GetTypes()
                 .Where(moduleType => moduleType.GetInterfaces().Contains(typeof(IXRModule))))
            {
                allXRModuleNames.Add(moduleType.Name);
            }
        }

        private void LoadAllOutputModules()
        {
            allOutModuleNames.Clear();

            foreach (Type moduleType in Assembly.GetExecutingAssembly().GetTypes()
                 .Where(moduleType => moduleType.GetInterfaces().Contains(typeof(IOutModule))))
            {
                allOutModuleNames.Add(moduleType.Name);
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

            DeDuplicateAssets();
        }

        private void DeDuplicateAssets()
        {
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

        private void DeleteGestureProfileFile(GestureProfile profile)
        {
            if (profile.OriginalFileName == "") return;

            try
            {
                File.Delete(profile.OriginalFileName);
                profile.OriginalFileName = "";
            }
            catch
            {

            }
        }

        private void DeleteMotionGestureFile(MotionGesture gesture)
        {
            if (gesture.OriginalFileName == "") return;

            try
            {
                File.Delete(gesture.OriginalFileName);
                gesture.OriginalFileName = "";
            }
            catch
            {

            }
        }

        private void DeleteTriggerActionFile(TriggerAction action)
        {
            if (action.OriginalFileName == "") return;

            try
            {
                File.Delete(action.OriginalFileName);
                action.OriginalFileName = "";
            }
            catch
            {

            }
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
                StopImageLoads = true;
                Hide();
            }
            else
            {
                StopImageLoads = false;
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
            if (Properties.Settings.Default.SaveOnClose) SaveAssets();

            foreach (IOutModule module in activeOutModules)
            {
                module.Shutdown();
            }

            if (xrModuleInstance != null)
            {
                xrModuleInstance.Shutdown();
            }

            _keySenderTimer?.Dispose();

            _cooldownTimer?.Dispose();
        }

        private void TimerCallback(object state)
        {
            try
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
            catch (Exception er)
            {

            }
        }

        private void CooldownCallback(object state)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (ActiveGestureProfile != null)
                    {
                        if (ActiveGestureProfile.Gestures.Count > 0)
                        {
                            foreach (MotionGesture gesture in ActiveGestureProfile.Gestures)
                            {
                                gesture.UpdateCooldown(cooldownTimeMS);

                                if (gesture.TriggerActions.Count > 0)
                                {
                                    foreach (TriggerAction act in gesture.TriggerActions)
                                    {
                                        act.UpdateWaitTimer(cooldownTimeMS);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception er)
            {

            }
        }

        private void ShowWindow()
        {
            Show();

            if (this.WindowState == FormWindowState.Minimized)
            {
                StopImageLoads = false;
                this.WindowState = FormWindowState.Normal;
            }

            TriggerTabRefresh();

            BringToFront();
            this.TopMost = true;
            this.TopMost = false;
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

        private string MinLenStr(string inStr, int len = 50, bool spaceAhead = false)
        {
            string ans = "";

            if ((len - inStr.Length) > 0)
            {
                while (ans.Length < (len - inStr.Length))
                {
                    ans += " ";
                }
            }

            if (spaceAhead)
            {
                return ans + inStr;
            }
            else
            {
                return inStr + ans;
            }
        }

        #endregion BACKEND_CORE

        #region TESTS
        private void AssetTest()
        {
            //Create and save a Test Profile
            GestureProfile a = new GestureProfile("GestureTest");
            MotionGesture b = new MotionGesture("MotionTest");
            TriggerAction c = new TriggerAction("TriggerTest");

            a.Image = "Icon.1_01.png";
            b.Image = "Icon.1_05.png";

            a.Description = "Gesture Profile Test";
            b.Description = "Motion Gesture Test";

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
            g.ProximityEvent.Trigger_When = new ChangeAmount(0, 10);

            //f.Disable_If_Trigger_Conditions.Add(g);

            b.TriggerConditions.Add(f);
            b.TriggerConditions.Add(g);

            b.TriggerOnAnyCondition = true;

            AddTriggerAction(c);

            b.TriggerActions.Add(c);
            AddMotionGesture(b);

            a.Gestures.Add(b);

            ActiveGestureProfile = a;
            AddGestureProfile(ActiveGestureProfile);

            SaveAssets();
        }

        private void btnInputSelectPopupTest_Click(object sender, EventArgs e)
        {
            if (btnInputSelectPopupTest.Visible)
            {
                SelectInput inputSel = new SelectInput();

                inputSel.SendInputTo(ReceiveInputTest);

                inputSel.ShowDialog(this);

                inputSel.Dispose();
            }
        }

        public void ReceiveInputTest(SendableInput input)
        {
            if (input != null)
            {
                MessageBox.Show(input.Name + " SELECTED!");
            }
            else
            {
                MessageBox.Show("NO INPUT SELECTED!");
            }
        }

        private void btnIconSelectPopup_Click(object sender, EventArgs e)
        {
            if (btnIconSelectPopup.Visible)
            {
                SelectImage imageSel = new SelectImage();

                imageSel.SendIconTo(ReceiveIconName);

                imageSel.ShowDialog(this);

                imageSel.Dispose();
            }
        }

        public void ReceiveIconName(string name)
        {
            if (name == "") name = "NONE";

            MessageBox.Show(name);
        }

        #endregion

        #region PAGES_UI

        private void SetTabPage(TabPage page, bool forceRefresh = false)
        {
            listViewLock = false;

            if (tcDisplayTabs.SelectedTab != page)
            {
                tcDisplayTabs.SelectedTab = page;
            }

            if (forceRefresh)
            {
                if (TabLeaveAction != null) TabLeaveAction.Invoke();
                TabLeaveAction = null;

                TriggerTabRefresh();
            }
        }

        private void tcDisplayTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabLeaveAction != null) TabLeaveAction.Invoke();
            TabLeaveAction = null;

            TriggerTabRefresh();
        }

        private void TriggerTabRefresh()
        {
            if (tcDisplayTabs.SelectedTab == tabSettings)
            {
                RefreshSettingsUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabMain)
            {
                RefreshMainUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabGestureProfiles)
            {
                RefreshGestureProfilesUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabMotionGestures)
            {
                RefreshMotionGesturesUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabEditTriggerActions)
            {
                RefreshTriggerActionsUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabTriggerConditions)
            {
                RefreshTriggerConditionsUI();
            }
        }

        private void tcConditionTabs_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void tcConditionTabs_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void tcConditionTabs_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void tcDisplayTabs_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void tcDisplayTabs_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void tcDisplayTabs_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region SETTINGS_UI
        private void RefreshSettingsUI()
        {
            TabLeaveAction = TabLeaveSettingsUI;

            int index = 0;
            int currIndex = -1;

            cmbDefaultXRModule.Items.Clear();

            foreach (string moduleName in allXRModuleNames)
            {
                cmbDefaultXRModule.Items.Add(moduleName);

                if (moduleName == Properties.Settings.Default.XRModule)
                {
                    currIndex = index;
                }

                index += 1;
            }

            if (currIndex > -1)
            {
                cmbDefaultXRModule.SelectedIndex = currIndex;
            }

            cmbDefaultGestureProfile.Items.Clear();
            cmbDefaultGestureProfile.Items.Add(" ");

            index = 1;
            currIndex = -1;

            foreach (GestureProfile profile in allGestureProfiles)
            {
                cmbDefaultGestureProfile.Items.Add(profile.Name);

                if (profile.Name == Properties.Settings.Default.GestureProfile)
                {
                    currIndex = index;
                }

                index += 1;
            }

            if (currIndex > -1)
            {
                cmbDefaultGestureProfile.SelectedIndex = currIndex;
            }

            clbDefaultOutModules.Items.Clear();

            foreach (string moduleName in allOutModuleNames)
            {
                CheckState isChecked = CheckState.Unchecked;

                foreach (IOutModule module in activeOutModules)
                {
                    if (module.DisplayName == moduleName)
                    {
                        isChecked = CheckState.Checked;
                    }
                }

                clbDefaultOutModules.Items.Add(moduleName, isChecked);
            }

            chkMinimizeAtStart.Checked = Properties.Settings.Default.MinimizeAtStart;

            chkSaveAssetsOnClose.Checked = Properties.Settings.Default.SaveOnClose;

        }

        private void btnSettingsSave_Click(object sender, EventArgs e)
        {
            var settings = Properties.Settings.Default;

            settings.XRModule = cmbDefaultXRModule.Text;

            if (cmbDefaultGestureProfile.SelectedIndex > 0)
            {
                settings.GestureProfile = cmbDefaultGestureProfile.Text;
            }
            else
            {
                settings.GestureProfile = "";
            }

            string outModules = "";

            foreach (object item in clbDefaultOutModules.CheckedItems)
            {
                string itemName = item.ToString();

                if (outModules != "") outModules += ",";

                outModules += itemName;
            }

            if (outModules == "")
            {
                outModules = "WinXROut";
            }

            settings.OutModules = outModules;

            settings.MinimizeAtStart = chkMinimizeAtStart.Checked;
            settings.SaveOnClose = chkSaveAssetsOnClose.Checked;

            settings.Save();

            if (cmbDefaultXRModule.Text != "" && xrModuleInstance != null && cmbDefaultXRModule.Text != xrModuleInstance.DisplayName)
            {
                if (!SetupInputModule(cmbDefaultXRModule.Text))
                {
                    xrModuleInstance.Shutdown();

                    MessageBox.Show(this, "XR module not found! Setting back to default", "XeRxKEYs - Error loading XR module!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    settings.XRModule = "WinXRApi";
                    settings.Save();

                    if (!SetupInputModule("WinXRApi"))
                    {
                        MessageBox.Show(this, "Catastrophic error! Closing!", "XeRxKEYs - Error loading XR module!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
                }
            }

            string currActiveOutModules = "";

            foreach (IOutModule module in activeOutModules)
            {
                if (currActiveOutModules != "") currActiveOutModules += ",";

                currActiveOutModules += module.DisplayName;
            }

            if (currActiveOutModules != outModules)
            {
                foreach (IOutModule module in activeOutModules)
                {
                    module.Shutdown();
                }

                activeOutModules.Clear();

                SetupOutputModules(outModules);
            }
            SetTabPage(tabMain);
        }

        private void TabLeaveSettingsUI()
        {
            //TODO: Consider if users will ever be able to leave a tab except by closing the app or saving?
            //Settings may be the only screen that needs to worry about this
        }

        private void cmbDefaultXRModule_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbDefaultGestureProfile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void clbDefaultOutModules_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabSettings_Click(object sender, EventArgs e)
        {

        }

        private void btnImportProfile_Click(object sender, EventArgs e)
        {
            oFDSelectFile.Title = "Select A Gesture Profile JSON File";
            oFDSelectFile.FileName = "";
            DialogResult foundIt = oFDSelectFile.ShowDialog(this);

            if (foundIt == DialogResult.Cancel || oFDSelectFile.FileName == "")
            {
                MessageBox.Show(this, "Gesture Profile Not Found!", "XeRxKEYs - Unable To Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (File.Exists(oFDSelectFile.FileName))
                {
                    if (GestureLoadSave.LoadProfile(ref allGestureProfiles, ref xrModuleInstance, oFDSelectFile.FileName))
                    {
                        DeDuplicateAssets();

                        MessageBox.Show(this, "Gesture Profile Imported!", "XeRxKEYs - Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnImportGesture_Click(object sender, EventArgs e)
        {
            oFDSelectFile.Title = "Select A Motion Gesture JSON File";
            oFDSelectFile.FileName = "";
            DialogResult foundIt = oFDSelectFile.ShowDialog(this);

            if (foundIt == DialogResult.Cancel || oFDSelectFile.FileName == "")
            {
                MessageBox.Show(this, "Motion Gesture Not Found!", "XeRxKEYs - Unable To Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (File.Exists(oFDSelectFile.FileName))
                {
                    if (MotionLoadSave.LoadProfile(ref allMotionGestures, ref xrModuleInstance, oFDSelectFile.FileName))
                    {
                        DeDuplicateAssets();

                        MessageBox.Show(this, "Motion Gesture Imported!", "XeRxKEYs - Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnImportAction_Click(object sender, EventArgs e)
        {
            oFDSelectFile.Title = "Select A Trigger Action JSON File";
            oFDSelectFile.FileName = "";
            DialogResult foundIt = oFDSelectFile.ShowDialog(this);

            if (foundIt == DialogResult.Cancel || oFDSelectFile.FileName == "")
            {
                MessageBox.Show(this, "Trigger Action Not Found!", "XeRxKEYs - Unable To Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (File.Exists(oFDSelectFile.FileName))
                {
                    if (ActionLoadSave.LoadProfile(ref allTriggerActions, oFDSelectFile.FileName))
                    {
                        DeDuplicateAssets();

                        MessageBox.Show(this, "Trigger Actions Imported!", "XeRxKEYs - Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnExportActions_Click(object sender, EventArgs e)
        {
            if (allTriggerActions.Count > 0)
            {
                fbdSelectFolder.Description = "Select A Folder To Export Trigger Actions To";
                fbdSelectFolder.ShowDialog();

                if (fbdSelectFolder.SelectedPath != null && fbdSelectFolder.SelectedPath != "" && Directory.Exists(fbdSelectFolder.SelectedPath))
                {
                    ActionLoadSave.SaveProfiles(allTriggerActions, fbdSelectFolder.SelectedPath);

                    MessageBox.Show(this, "Trigger Actions Exported!", "XeRxKEYs - Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Folder not selected!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this, "No Trigger Actions To Export!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExportGestures_Click(object sender, EventArgs e)
        {
            if (allMotionGestures.Count > 0)
            {
                fbdSelectFolder.Description = "Select A Folder To Export Motion Gestures To";
                fbdSelectFolder.ShowDialog();

                if (fbdSelectFolder.SelectedPath != null && fbdSelectFolder.SelectedPath != "" && Directory.Exists(fbdSelectFolder.SelectedPath))
                {
                    MotionLoadSave.SaveProfiles(allMotionGestures, fbdSelectFolder.SelectedPath);

                    MessageBox.Show(this, "Motion Gestures Exported!", "XeRxKEYs - Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Folder not selected!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this, "No Motion Gestures To Export!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExportProfiles_Click(object sender, EventArgs e)
        {
            if (allGestureProfiles.Count > 0)
            {
                fbdSelectFolder.Description = "Select A Folder To Export Gesture Profiles To";
                fbdSelectFolder.ShowDialog();

                if (fbdSelectFolder.SelectedPath != null && fbdSelectFolder.SelectedPath != "" && Directory.Exists(fbdSelectFolder.SelectedPath))
                {
                    GestureLoadSave.SaveProfiles(allGestureProfiles, fbdSelectFolder.SelectedPath);

                    MessageBox.Show(this, "Gesture Profiles Exported!", "XeRxKEYs - Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Folder not selected!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(this, "No Gesture Profiles To Export!", "XeRxKEYs - Unable To Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void chkMinimizeAtStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkSaveAssetsOnClose_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region MAIN_UI
        private void RefreshMainUI()
        {
            cmbActiveGestureProfile.Items.Clear();

            cmbActiveGestureProfile.Items.Add(" ");

            foreach (GestureProfile profile in allGestureProfiles)
            {
                cmbActiveGestureProfile.Items.Add(profile.Name);
            }

            if (ActiveGestureProfile != null)
            {
                bool profileFound = false;
                int index = 0;

                foreach (GestureProfile profile in allGestureProfiles)
                {
                    if (profile.Name == ActiveGestureProfile.Name)
                    {
                        cmbActiveGestureProfile.SelectedIndex = index + 1;
                        profileFound = true;
                        break;
                    }

                    index += 1;
                }

                if (!profileFound)
                {
                    cmbActiveGestureProfile.SelectedIndex = 0;
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SetTabPage(tabSettings);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbActiveGestureProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateActiveGestureProfile();
        }

        private void UpdateActiveGestureProfile()
        {
            lvwActiveMotionGestures.Items.Clear();
            lvwActiveMotionGestures.SmallImageList = null;

            if (cmbActiveGestureProfile.SelectedIndex <= 0)
            {
                ActiveGestureProfile = null;
                txtActiveGestureProfileDesc.Text = "";
                picActiveGestureProfile.Image = noIconImg;
            }
            else
            {
                foreach (GestureProfile profile in allGestureProfiles)
                {
                    if (profile.Name == cmbActiveGestureProfile.Text)
                    {
                        ActiveGestureProfile = profile;

                        txtActiveGestureProfileDesc.Text = profile.Description;

                        if (profile.Image != "" && !StopImageLoads)
                        {
                            string imagePath = Path.Combine(imageDirectory, profile.Image);

                            picActiveGestureProfile.Image = Image.FromFile(imagePath);
                        }

                        ImageList smallImageList = new ImageList();
                        smallImageList.ImageSize = new Size(32, 32);
                        smallImageList.ColorDepth = ColorDepth.Depth32Bit;

                        lvwActiveMotionGestures.SmallImageList = smallImageList;
                        lvwActiveMotionGestures.View = View.SmallIcon;

                        foreach (MotionGesture gesture in profile.Gestures)
                        {
                            int imgIndex = -1;

                            if (gesture.Image != "" && !StopImageLoads)
                            {
                                try
                                {
                                    Image gestureIcon = Image.FromFile(Path.Combine(imageDirectory, gesture.Image));
                                    smallImageList.Images.Add(gestureIcon);
                                    imgIndex = smallImageList.Images.Count - 1;
                                }
                                catch
                                {

                                }
                            }

                            string itemName = MinLenStr(gesture.Name + ": " + gesture.Description, 80);

                            if (gesture.Description == "")
                            {
                                itemName = MinLenStr(gesture.Name + ": [No Description]", 80);
                            }

                            lvwActiveMotionGestures.Items.Add(new ListViewItem(itemName, imgIndex));
                        }

                        break;
                    }
                }
            }
        }

        private void btnEditGestureProfiles_Click(object sender, EventArgs e)
        {
            SetTabPage(tabGestureProfiles);
        }

        private void btnEditTriggerActions_Click(object sender, EventArgs e)
        {
            SetTabPage(tabEditTriggerActions);
        }

        private void btnEditMotionGestures_Click(object sender, EventArgs e)
        {
            SetTabPage(tabMotionGestures);
        }

        private void lvwActiveMotionGestures_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region GESTURE_PROFILES_UI
        private void RefreshGestureProfilesUI()
        {
            ClearGestureProfileEdit();

            lvwAllGestureProfiles.SmallImageList = null;
            lvwAllGestureProfiles.Items.Clear();

            ImageList smallImageList = new ImageList();
            smallImageList.ImageSize = new Size(32, 32);
            smallImageList.ColorDepth = ColorDepth.Depth32Bit;

            lvwAllGestureProfiles.SmallImageList = smallImageList;
            lvwAllGestureProfiles.View = View.SmallIcon;

            foreach (GestureProfile profile in allGestureProfiles)
            {
                int imgIndex = -1;

                if (profile.Image != "" && !StopImageLoads)
                {
                    try
                    {
                        Image gestureIcon = Image.FromFile(Path.Combine(imageDirectory, profile.Image));
                        smallImageList.Images.Add(gestureIcon);
                        imgIndex = smallImageList.Images.Count - 1;
                    }
                    catch
                    {

                    }
                }

                lvwAllGestureProfiles.Items.Add(new ListViewItem(MinLenStr(profile.Name), imgIndex));
            }
        }
        private void lvwAllGestureProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listViewLock)
            {
                listViewLock = true;

                if (_editingGestureProfile > -1)
                {
                    ConfirmProfileEditorSave();
                    _editingGestureProfile = -1;
                }

                lvwProfileEnabledMotionGestures.SmallImageList = null;
                lvwProfileEnabledMotionGestures.Items.Clear();

                if (lvwAllGestureProfiles.Items.Count > 0 && lvwAllGestureProfiles.SelectedIndices.Count > 0 && lvwAllGestureProfiles.SelectedIndices[0] > -1)
                {
                    txtEditGestureProfileName.Enabled = true;
                    txtEditGestureProfileDescription.Enabled = true;
                    btnDeleteGestureProfile.Enabled = true;

                    _editingGestureProfile = lvwAllGestureProfiles.SelectedIndices[0];

                    GestureProfile profile = allGestureProfiles[_editingGestureProfile];

                    txtEditGestureProfileName.Text = profile.Name;
                    txtEditGestureProfileDescription.Text = profile.Description;

                    if (profile.Image != "")
                    {
                        _editingGestureProfileImage = profile.Image;

                        if (!StopImageLoads)
                        {
                            string imagePath = Path.Combine(imageDirectory, profile.Image);

                            picEditGestureProfileIcon.Image = Image.FromFile(imagePath);
                        }
                    }

                    ImageList smallImageList = new ImageList();
                    smallImageList.ImageSize = new Size(32, 32);
                    smallImageList.ColorDepth = ColorDepth.Depth32Bit;

                    lvwProfileEnabledMotionGestures.SmallImageList = smallImageList;
                    lvwProfileEnabledMotionGestures.View = View.SmallIcon;

                    foreach (MotionGesture gesture in allMotionGestures)
                    {
                        int imgIndex = -1;

                        if (gesture.Image != "" && !StopImageLoads)
                        {
                            try
                            {
                                Image gestureIcon = Image.FromFile(Path.Combine(imageDirectory, gesture.Image));
                                smallImageList.Images.Add(gestureIcon);
                                imgIndex = smallImageList.Images.Count - 1;
                            }
                            catch
                            {

                            }
                        }

                        string itemName = MinLenStr(gesture.Name + ": " + gesture.Description, 110);

                        if (gesture.Description == "")
                        {
                            itemName = MinLenStr(gesture.Name + ": [No Description]", 110);
                        }

                        lvwProfileEnabledMotionGestures.Items.Add(new ListViewItem(itemName, imgIndex));

                        foreach (MotionGesture activeGesture in profile.Gestures)
                        {
                            if (activeGesture.Name == gesture.Name)
                            {
                                lvwProfileEnabledMotionGestures.Items[lvwProfileEnabledMotionGestures.Items.Count - 1].Checked = true;

                                break;
                            }
                        }
                    }

                    _GestureProfileChanged = false;
                    UpdateGestureProfileEditor();
                }
                else
                {
                    _editingGestureProfile = -1;
                    ClearGestureProfileEdit();
                }

                listViewLock = false;
            }
        }

        private void ClearGestureProfileEdit()
        {
            _editingGestureProfileImage = "";
            _editingGestureProfile = -1;
            _GestureProfileChanged = false;
            btnSaveEditedGestureProfile.Enabled = false;
            btnDeleteGestureProfile.Enabled = false;
            txtEditGestureProfileName.Text = "";
            picEditGestureProfileIcon.Image = noIconImg;
            txtEditGestureProfileDescription.Text = "";

            txtEditGestureProfileName.Enabled = false;
            txtEditGestureProfileDescription.Enabled = false;

            lvwProfileEnabledMotionGestures.SmallImageList = null;
            lvwProfileEnabledMotionGestures.Items.Clear();
        }

        private void ConfirmProfileEditorSave()
        {
            if (_editingGestureProfile >= 0 && _GestureProfileChanged)
            {
                DialogResult result = MessageBox.Show(this, "Changes have been made to current profile, save changes?", "Gesture Profile Changed - Unsaved Changes Will Be Lost!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SaveGestureProfileEditor();
                }
            }
        }

        private void SaveGestureProfileEditor()
        {
            if (_editingGestureProfile >= 0 && allGestureProfiles.Count > _editingGestureProfile)
            {
                if (txtEditGestureProfileName.Text != "")
                {
                    if (allGestureProfiles[_editingGestureProfile].Name != txtEditGestureProfileName.Text)
                    {
                        //Name change, delete old file
                        DeleteGestureProfileFile(allGestureProfiles[_editingGestureProfile]);
                        allGestureProfiles[_editingGestureProfile].Name = txtEditGestureProfileName.Text;
                    }

                    allGestureProfiles[_editingGestureProfile].Description = txtEditGestureProfileDescription.Text;
                    allGestureProfiles[_editingGestureProfile].Image = _editingGestureProfileImage;

                    List<MotionGesture> activeMotionGestures = new List<MotionGesture>();

                    foreach (ListViewItem item in lvwProfileEnabledMotionGestures.Items)
                    {
                        if (item.Checked && allMotionGestures.Count > item.Index)
                        {
                            activeMotionGestures.Add(allMotionGestures[item.Index]);
                        }
                    }

                    allGestureProfiles[_editingGestureProfile].Gestures = activeMotionGestures;

                    DeDuplicateAssets();

                    RefreshGestureProfilesUI();
                }
                else
                {
                    MessageBox.Show(this, "Unable to save, Gesture Profile must have a non-blank name! Please change it and try again", "Gesture Profile Name Cannot Be Blank!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReturnToMainGP_Click(object sender, EventArgs e)
        {
            ConfirmProfileEditorSave();

            SetTabPage(tabMain);
        }

        private void btnSaveEditedGestureProfile_Click(object sender, EventArgs e)
        {
            SaveGestureProfileEditor();
        }

        private void txtEditGestureProfileName_TextChanged(object sender, EventArgs e)
        {
            if (_editingGestureProfile >= 0 && txtEditGestureProfileName.Text != "") _GestureProfileChanged = true;

            UpdateGestureProfileEditor();
        }

        private void txtEditGestureProfileDescription_TextChanged(object sender, EventArgs e)
        {
            if (_editingGestureProfile >= 0) _GestureProfileChanged = true;

            UpdateGestureProfileEditor();
        }

        private void picEditGestureProfileIcon_Click(object sender, EventArgs e)
        {
            if (_editingGestureProfile >= 0)
            {
                SelectImage imageSel = new SelectImage();

                imageSel.SendIconTo(UpdateEditedGestureProfileImage);

                imageSel.ShowDialog(this);

                imageSel.Dispose();
            }
        }

        public void UpdateEditedGestureProfileImage(string img)
        {
            if (img == null || img == "")
            {
                //Clear
                _editingGestureProfileImage = "";

                if (picEditGestureProfileIcon.Image != noIconImg)
                {
                    picEditGestureProfileIcon.Image = noIconImg;
                    _GestureProfileChanged = true;
                }
            }
            else
            {
                //Load
                _editingGestureProfileImage = img;

                try
                {
                    picEditGestureProfileIcon.Image = Image.FromFile(Path.Combine(imageDirectory, img));
                }
                catch
                {
                    picEditGestureProfileIcon.Image = noIconImg;
                }

                _GestureProfileChanged = true;
            }

            UpdateGestureProfileEditor();
        }

        public void UpdateGestureProfileEditor()
        {
            if (_GestureProfileChanged && _editingGestureProfile >= 0)
            {
                if (txtEditGestureProfileName.Text != "")
                {
                    btnSaveEditedGestureProfile.Enabled = true;
                }
                else
                {
                    btnSaveEditedGestureProfile.Enabled = false;
                }
            }
            else
            {
                _GestureProfileChanged = false;
                btnSaveEditedGestureProfile.Enabled = false;
            }
        }

        private void btnCreateNewGestureProfile_Click(object sender, EventArgs e)
        {
            ConfirmProfileEditorSave();

            allGestureProfiles.Add(new GestureProfile("NewProfile-" + DateTime.Now.ToString("ddHHmmssffff")));

            RefreshGestureProfilesUI();
        }

        private void btnDeleteGestureProfile_Click(object sender, EventArgs e)
        {
            if (lvwAllGestureProfiles.Items.Count > 0 && lvwAllGestureProfiles.SelectedIndices.Count > 0)
            {
                DialogResult confirmDelete = MessageBox.Show(this, "Delete this Gesture Profile? This cannot be undone!", "Confirm Profile Delete?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (confirmDelete == DialogResult.OK)
                {
                    int index = lvwAllGestureProfiles.SelectedIndices[0];

                    if (index < lvwAllGestureProfiles.Items.Count)
                    {
                        DeleteGestureProfileFile(allGestureProfiles[index]);

                        allGestureProfiles.RemoveAt(index);
                    }

                    RefreshGestureProfilesUI();
                }
            }
        }

        private void lvwProfileEnabledMotionGestures_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwProfileEnabledMotionGestures_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_editingGestureProfile >= 0)
            {
                _GestureProfileChanged = true;
            }

            UpdateGestureProfileEditor();
        }
        #endregion

        #region MOTION_GESTURES_UI
        private void RefreshMotionGesturesUI()
        {
            ClearMotionGestureEdit();

            lvwAllMotionGestures.SmallImageList = null;
            lvwAllMotionGestures.Items.Clear();

            ImageList smallImageList = new ImageList();
            smallImageList.ImageSize = new Size(32, 32);
            smallImageList.ColorDepth = ColorDepth.Depth32Bit;

            lvwAllMotionGestures.SmallImageList = smallImageList;
            lvwAllMotionGestures.View = View.SmallIcon;

            foreach (MotionGesture motion in allMotionGestures)
            {
                int imgIndex = -1;

                if (motion.Image != "" && !StopImageLoads)
                {
                    try
                    {
                        Image gestureIcon = Image.FromFile(Path.Combine(imageDirectory, motion.Image));
                        smallImageList.Images.Add(gestureIcon);
                        imgIndex = smallImageList.Images.Count - 1;
                    }
                    catch
                    {

                    }
                }

                lvwAllMotionGestures.Items.Add(new ListViewItem(MinLenStr(motion.Name), imgIndex));
            }
        }

        private void lvwAllMotionGestures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listViewLock)
            {
                listViewLock = true;

                if (_editingMotionGesture > -1)
                {
                    ConfirmMotionGestureSave();
                    _editingMotionGesture = -1;
                }

                if (lvwAllMotionGestures.Items.Count > 0 && lvwAllMotionGestures.SelectedIndices.Count > 0 && lvwAllMotionGestures.SelectedIndices[0] > -1)
                {
                    txtEditMotionGestureName.Enabled = true;
                    txtEditMotionGestureDescription.Enabled = true;
                    btnDeleteMotionGesture.Enabled = true;

                    _editingMotionGesture = lvwAllMotionGestures.SelectedIndices[0];

                    MotionGesture motion = allMotionGestures[_editingMotionGesture];

                    txtEditMotionGestureName.Text = motion.Name;
                    txtEditMotionGestureDescription.Text = motion.Description;

                    chkEditMotionProfileTriggerOnAny.Checked = motion.TriggerOnAnyCondition;

                    if (motion.Image != "")
                    {
                        _editingMotionGestureImage = motion.Image;

                        if (!StopImageLoads)
                        {
                            string imagePath = Path.Combine(imageDirectory, motion.Image);

                            picEditMotionGestureIcon.Image = Image.FromFile(imagePath);
                        }
                    }

                    lstTriggerConditionPreview.Items.Clear();
                    _motionGestureTriggerConditions = new List<TriggerCondition>();

                    foreach (TriggerCondition cond in motion.TriggerConditions)
                    {
                        _motionGestureTriggerConditions.Add(cond);
                        lstTriggerConditionPreview.Items.Add(cond.Type.ToString().Replace("_", " "));
                    }

                    clbEnabledTriggerActions.Items.Clear();

                    foreach (TriggerAction action in allTriggerActions)
                    {
                        clbEnabledTriggerActions.Items.Add(action.Name);

                        foreach (TriggerAction enabledAction in motion.TriggerActions)
                        {
                            if (enabledAction.Name == action.Name)
                            {
                                clbEnabledTriggerActions.SetItemChecked(clbEnabledTriggerActions.Items.Count - 1, true);
                            }
                        }
                    }

                    _MotionGestureChanged = false;
                    UpdateMotionGestureEditor();
                }
                else
                {
                    _editingMotionGesture = -1;
                    ClearMotionGestureEdit();
                }

                listViewLock = false;
            }
        }

        private void ClearMotionGestureEdit()
        {
            _editingMotionGestureImage = "";
            _editingMotionGesture = -1;
            _MotionGestureChanged = false;
            btnSaveEditedMotionGesture.Enabled = false;
            btnDeleteMotionGesture.Enabled = false;
            txtEditMotionGestureName.Text = "";
            picEditMotionGestureIcon.Image = noIconImg;
            txtEditMotionGestureDescription.Text = "";

            txtEditMotionGestureName.Enabled = false;
            txtEditMotionGestureDescription.Enabled = false;

            clbEnabledTriggerActions.Items.Clear();
            lstTriggerConditionPreview.Items.Clear();
        }

        private void ConfirmMotionGestureSave()
        {
            if (_editingMotionGesture >= 0 && _MotionGestureChanged)
            {
                DialogResult result = MessageBox.Show(this, "Changes have been made to current gesture, save changes?", "Motion Gesture Changed - Unsaved Changes Will Be Lost!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SaveMotionGestureProfileEditor();
                }
            }
        }

        private void SaveMotionGestureProfileEditor()
        {
            if (_editingMotionGesture >= 0 && allMotionGestures.Count > _editingMotionGesture)
            {
                if (txtEditMotionGestureName.Text != "")
                {
                    if (allMotionGestures[_editingMotionGesture].Name != txtEditMotionGestureName.Text)
                    {
                        //Name change, delete old file
                        DeleteMotionGestureFile(allMotionGestures[_editingMotionGesture]);
                        allMotionGestures[_editingMotionGesture].Name = txtEditMotionGestureName.Text;
                    }

                    allMotionGestures[_editingMotionGesture].Description = txtEditMotionGestureDescription.Text;
                    allMotionGestures[_editingMotionGesture].Image = _editingMotionGestureImage;

                    allMotionGestures[_editingMotionGesture].TriggerOnAnyCondition = chkEditMotionProfileTriggerOnAny.Checked;

                    List<TriggerAction> activeTriggerActions = new List<TriggerAction>();

                    if (clbEnabledTriggerActions.CheckedItems.Count > 0)
                    {
                        for (int i = 0; i < allTriggerActions.Count; i++)
                        {
                            if (clbEnabledTriggerActions.Items.Count > i)
                            {
                                if (clbEnabledTriggerActions.GetItemChecked(i))
                                {
                                    activeTriggerActions.Add(allTriggerActions[i]);
                                }
                            }
                        }
                    }

                    allMotionGestures[_editingMotionGesture].TriggerActions = activeTriggerActions;
                    allMotionGestures[_editingMotionGesture].TriggerConditions = new List<TriggerCondition>();

                    foreach (TriggerCondition cond in _motionGestureTriggerConditions)
                    {
                        allMotionGestures[_editingMotionGesture].TriggerConditions.Add(cond);
                    }

                    DeDuplicateAssets();

                    RefreshMotionGesturesUI();
                }
                else
                {
                    MessageBox.Show(this, "Unable to save, Motion Gesture must have a non-blank name! Please change it and try again", "Motion Gesture Name Cannot Be Blank!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCreateNewMotionGesture_Click(object sender, EventArgs e)
        {
            ConfirmMotionGestureSave();

            allMotionGestures.Add(new MotionGesture("NewProfile-" + DateTime.Now.ToString("ddHHmmssffff")));

            RefreshMotionGesturesUI();
        }

        private void btnDeleteMotionGesture_Click(object sender, EventArgs e)
        {
            if (lvwAllMotionGestures.Items.Count > 0 && lvwAllMotionGestures.SelectedIndices.Count > 0)
            {
                DialogResult confirmDelete = MessageBox.Show(this, "Delete this Motion Gesture? This cannot be undone!", "Confirm Profile Delete?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                if (confirmDelete == DialogResult.OK)
                {
                    int index = lvwAllMotionGestures.SelectedIndices[0];

                    if (index < lvwAllMotionGestures.Items.Count)
                    {
                        DeleteMotionGestureFile(allMotionGestures[index]);

                        allMotionGestures.RemoveAt(index);
                    }

                    RefreshMotionGesturesUI();
                }
            }
        }

        private void btnReturnToMainMG_Click(object sender, EventArgs e)
        {
            SetTabPage(tabMain);
        }

        private void btnSaveEditedMotionGesture_Click(object sender, EventArgs e)
        {
            SaveMotionGestureProfileEditor();
        }

        private void txtEditMotionGestureName_TextChanged(object sender, EventArgs e)
        {
            if (_editingMotionGesture >= 0 && txtEditMotionGestureName.Text != "") _MotionGestureChanged = true;

            UpdateMotionGestureEditor();
        }

        private void txtEditMotionGestureDescription_TextChanged(object sender, EventArgs e)
        {
            if (_editingMotionGesture >= 0) _MotionGestureChanged = true;

            UpdateMotionGestureEditor();
        }

        private void picEditMotionGestureIcon_Click(object sender, EventArgs e)
        {
            SelectImage imageSel = new SelectImage();

            imageSel.SendIconTo(UpdateEditedMotionGestureImage);

            imageSel.ShowDialog(this);

            imageSel.Dispose();
        }

        public void UpdateEditedMotionGestureImage(string img)
        {
            if (img == null || img == "")
            {
                //Clear
                _editingMotionGestureImage = "";

                if (picEditMotionGestureIcon.Image != noIconImg)
                {
                    picEditMotionGestureIcon.Image = noIconImg;
                    _MotionGestureChanged = true;
                }
            }
            else
            {
                //Load
                _editingMotionGestureImage = img;

                try
                {
                    picEditMotionGestureIcon.Image = Image.FromFile(Path.Combine(imageDirectory, img));
                }
                catch
                {
                    picEditMotionGestureIcon.Image = noIconImg;
                }

                _MotionGestureChanged = true;
            }

            UpdateMotionGestureEditor();
        }

        private void UpdateMotionGestureEditor()
        {
            if (_MotionGestureChanged && _editingMotionGesture >= 0)
            {
                if (txtEditMotionGestureName.Text != "")
                {
                    btnSaveEditedMotionGesture.Enabled = true;
                }
                else
                {
                    btnSaveEditedMotionGesture.Enabled = false;
                }
            }
            else
            {
                _MotionGestureChanged = false;
                btnSaveEditedMotionGesture.Enabled = false;
            }
        }

        private void chkEditMotionProfileTriggerOnAny_CheckedChanged(object sender, EventArgs e)
        {
            _MotionGestureChanged = true;
            UpdateMotionGestureEditor();
        }

        private void btnEditTriggerConditions_Click(object sender, EventArgs e)
        {
            SetTabPage(tabTriggerConditions);
        }
        private void pnlMotionGesturesRight_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lstTriggerConditionPreview_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void IngestTriggerConditions()
        {
            lstTriggerConditionPreview.Items.Clear();

            foreach (TriggerCondition cond in _motionGestureTriggerConditions)
            {
                lstTriggerConditionPreview.Items.Add(cond.Type.ToString().Replace("_", " "));
            }

            SaveMotionGestureProfileEditor();
        }
        #endregion

        #region TRIGGER_ACTIONS_UI
        private void RefreshTriggerActionsUI()
        {
            //TODO: Load values into the Trigger Actions UI
        }

        private void lvwAllTriggerActions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCreateNewTriggerAction_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteTriggerAction_Click(object sender, EventArgs e)
        {

        }

        private void pnlTriggerActionsRight_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnReturnToMainTA_Click(object sender, EventArgs e)
        {
            SetTabPage(tabMain);
        }

        private void btnSaveEditedTriggerAction_Click(object sender, EventArgs e)
        {

        }

        private void btnAddNewInput_Click(object sender, EventArgs e)
        {
            UpdateTriggerActionEditor();
        }

        private void btnDeleteInput_Click(object sender, EventArgs e)
        {
            UpdateTriggerActionEditor();
        }

        private void lstEditTriggerActionSendInputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTriggerActionEditor();
        }

        private void txtEditTriggerActionDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateTriggerActionEditor();
        }

        private void txtEditTriggerActionName_TextChanged(object sender, EventArgs e)
        {
            UpdateTriggerActionEditor();
        }

        private void UpdateTriggerActionEditor()
        {

        }

        private void pnlTriggerActionsBottom_Paint(object sender, PaintEventArgs e)
        {

        }
        #endregion

        #region TRIGGER_CONDITIONS_UI
        //_motionGestureTriggerConditions
        private void RefreshTriggerConditionsUI()
        {
            ClearTriggerConditionEdit();

            lvwCurrentTriggerConditions.Items.Clear();

            foreach (TriggerCondition cond in _motionGestureTriggerConditions)
            {
                lvwCurrentTriggerConditions.Items.Add(new ListViewItem(MinLenStr(cond.Type.ToString().Replace("_", " "), 110)));
            }

            cmbTriggerType.Items.Clear();

            foreach (string item in Enum.GetNames(typeof(TriggerConditionType)))
            {
                cmbTriggerType.Items.Add(item.Replace("_", " "));
            }
        }

        private void lvwCurrentTriggerConditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listViewLock)
            {
                listViewLock = true;

                if (_editingTriggerCondition > -1)
                {
                    ConfirmTriggerConditionSave();
                    _editingTriggerCondition = -1;
                }

                clbObjectGroupA.Items.Clear();
                clbObjectGroupA.Items.Add("Head");
                clbObjectGroupA.Items.Add("Left Hand");
                clbObjectGroupA.Items.Add("Right Hand");

                clbObjectGroupB.Items.Clear();
                clbObjectGroupB.Items.Add("Head");
                clbObjectGroupB.Items.Add("Left Hand");
                clbObjectGroupB.Items.Add("Right Hand");

                clbTriggerForObjects.Items.Clear();
                clbTriggerForObjects.Items.Add("Head");
                clbTriggerForObjects.Items.Add("Left Hand");
                clbTriggerForObjects.Items.Add("Right Hand");

                if (lvwCurrentTriggerConditions.Items.Count > 0 && lvwCurrentTriggerConditions.SelectedIndices.Count > 0 && lvwCurrentTriggerConditions.SelectedIndices[0] > -1)
                {
                    pnlTriggerConditionsRight.Enabled = true;
                    btnDeleteTriggerCondition.Enabled = true;

                    _editingTriggerCondition = lvwCurrentTriggerConditions.SelectedIndices[0];

                    TriggerCondition cond = _motionGestureTriggerConditions[_editingTriggerCondition];

                    cmbTriggerType.SelectedIndex = (int)cond.Type;
                    cmbTriggerType.Enabled = true;

                    if (cond.Type == TriggerConditionType.Proximity)
                    {
                        if (cond.ProximityEvent != null)
                        {
                            foreach (TrackedObject obj in cond.ProximityEvent.Device_Group_A)
                            {
                                if (obj.Name == "Head")
                                {
                                    clbObjectGroupA.SetItemChecked(0, true);
                                }
                                else if (obj.Name == "Left Hand")
                                {
                                    clbObjectGroupA.SetItemChecked(1, true);
                                }
                                else if (obj.Name == "Right Hand")
                                {
                                    clbObjectGroupA.SetItemChecked(2, true);
                                }
                            }

                            foreach (TrackedObject obj in cond.ProximityEvent.Device_Group_B)
                            {
                                if (obj.Name == "Head")
                                {
                                    clbObjectGroupB.SetItemChecked(0, true);
                                }
                                else if (obj.Name == "Left Hand")
                                {
                                    clbObjectGroupB.SetItemChecked(1, true);
                                }
                                else if (obj.Name == "Right Hand")
                                {
                                    clbObjectGroupB.SetItemChecked(2, true);
                                }
                            }

                            chkProxEventTriggerOnce.Checked = cond.ProximityEvent.TriggerOnce;
                            chkInvertProxEventTrigger.Checked = cond.ProximityEvent.Invert;

                            if (cond.ProximityEvent.Trigger_When != null)
                            {
                                txtMinDistProxEvent.Text = cond.ProximityEvent.Trigger_When.MinValue.ToString();
                                txtMaxDistProxEvent.Text = cond.ProximityEvent.Trigger_When.MaxValue.ToString();
                            }
                        }
                    }
                    else
                    {
                        if (cond.ShakeEvent != null)
                        {
                            foreach (TrackedObject obj in cond.ShakeEvent.Trigger_For_Objects)
                            {
                                if (obj.Name == "Head")
                                {
                                    clbTriggerForObjects.SetItemChecked(0, true);
                                }
                                else if (obj.Name == "Left Hand")
                                {
                                    clbTriggerForObjects.SetItemChecked(1, true);
                                }
                                else if (obj.Name == "Right Hand")
                                {
                                    clbTriggerForObjects.SetItemChecked(2, true);
                                }
                            }
                        }
                    }

                    _TriggerConditionChanged = false;
                    UpdateTriggerConditionEditor();
                }
                else
                {
                    ClearTriggerConditionEdit();
                    _editingTriggerCondition = -1;
                }

                listViewLock = false;
            }
        }

        private void ClearTriggerConditionEdit()
        {
            _editingTriggerCondition = -1;
            _TriggerConditionChanged = false;
            btnDeleteTriggerCondition.Enabled = false;

            cmbTriggerType.Enabled = false;
            cmbTriggerType.SelectedIndex = -1;

            tcConditionTabs.Enabled = false;
            tcConditionTabs.Visible = false;
        }

        private void ConfirmTriggerConditionSave()
        {
            //if (_editingTriggerCondition >= 0 && _TriggerConditionChanged)
            //{
            //DialogResult result = MessageBox.Show(this, "Changes have been made to current Trigger Condition, save changes?", "Trigger Condition Changed - Unsaved Changes Will Be Lost!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (result == DialogResult.Yes)
            //{
            //}

            //}
            if (_TriggerConditionChanged) SaveTriggerConditionEditor();
        }

        private void SaveTriggerConditionEditor()
        {
            if (_editingTriggerCondition >= 0 && _motionGestureTriggerConditions.Count > _editingTriggerCondition)
            {
                if (_motionGestureTriggerConditions[_editingTriggerCondition].Type == TriggerConditionType.Proximity)
                {
                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_A.Clear();

                    foreach (int index in clbObjectGroupA.CheckedIndices)
                    {
                        if (index == 0)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_A.Add(xrModuleInstance.Head);
                        }
                        else if (index == 1)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_A.Add(xrModuleInstance.L_Hand);
                        }
                        else if (index == 2)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_A.Add(xrModuleInstance.R_Hand);
                        }
                    }

                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_B.Clear();

                    foreach (int index in clbObjectGroupB.CheckedIndices)
                    {
                        if (index == 0)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_B.Add(xrModuleInstance.Head);
                        }
                        else if (index == 1)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_B.Add(xrModuleInstance.L_Hand);
                        }
                        else if (index == 2)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Device_Group_B.Add(xrModuleInstance.R_Hand);
                        }
                    }

                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Invert = chkInvertProxEventTrigger.Checked;
                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.TriggerOnce = chkProxEventTriggerOnce.Checked;

                    float minVal = 0.0f;
                    float maxVal = 0.0f;

                    float.TryParse(txtMinDistProxEvent.Text, out minVal);
                    float.TryParse(txtMaxDistProxEvent.Text, out maxVal);

                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Trigger_When.MinValue = minVal;
                    _motionGestureTriggerConditions[_editingTriggerCondition].ProximityEvent.Trigger_When.MaxValue = maxVal;
                }
                else
                {
                    _motionGestureTriggerConditions[_editingTriggerCondition].ShakeEvent.Trigger_For_Objects.Clear();

                    foreach (int index in clbTriggerForObjects.CheckedIndices)
                    {
                        if (index == 0)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ShakeEvent.Trigger_For_Objects.Add(xrModuleInstance.Head);
                        }
                        else if (index == 1)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ShakeEvent.Trigger_For_Objects.Add(xrModuleInstance.L_Hand);
                        }
                        else if (index == 2)
                        {
                            _motionGestureTriggerConditions[_editingTriggerCondition].ShakeEvent.Trigger_For_Objects.Add(xrModuleInstance.R_Hand);
                        }
                    }
                }
            }

            IngestTriggerConditions();

            RefreshTriggerConditionsUI();
        }

        private void btnCreateNewTriggerCondition_Click(object sender, EventArgs e)
        {
            _motionGestureTriggerConditions.Add(new TriggerCondition(TriggerConditionType.Shake_Vertical));
            RefreshTriggerConditionsUI();
        }

        private void btnDeleteTriggerCondition_Click(object sender, EventArgs e)
        {
            if (lvwCurrentTriggerConditions.Items.Count > 0 && lvwCurrentTriggerConditions.SelectedIndices.Count > 0)
            {
                //DialogResult confirmDelete = MessageBox.Show(this, "Delete this Trigger Condition? This cannot be undone!", "Confirm Profile Delete?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                //if (confirmDelete == DialogResult.OK)
                //{
                int index = lvwCurrentTriggerConditions.SelectedIndices[0];

                if (index < lvwCurrentTriggerConditions.Items.Count)
                {
                    _motionGestureTriggerConditions.RemoveAt(index);
                }

                RefreshTriggerConditionsUI();
                //}
            }
        }

        private void UpdateTriggerConditionEditor()
        {
            if (cmbTriggerType.SelectedIndex >= 0)
            {
                tcConditionTabs.Enabled = true;

                if (cmbTriggerType.SelectedIndex == (int)TriggerConditionType.Proximity)
                {
                    tcConditionTabs.SelectedTab = tabProxEvent;
                }
                else
                {
                    tcConditionTabs.SelectedTab = tabShakeEvent;
                }
            }
            else
            {
                tcConditionTabs.Enabled = false;
            }
        }

        private void cmbTriggerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_editingTriggerCondition > -1 && _motionGestureTriggerConditions.Count > _editingTriggerCondition)
            {
                tcConditionTabs.Visible = true;

                if ((int)_motionGestureTriggerConditions[_editingTriggerCondition].Type != cmbTriggerType.SelectedIndex)
                {
                    _motionGestureTriggerConditions[_editingTriggerCondition] = new TriggerCondition((TriggerConditionType)cmbTriggerType.SelectedIndex);
                    _TriggerConditionChanged = true;
                }
            }

            UpdateTriggerConditionEditor();
        }

        private void btnCloseTriggerConditionEditor_Click(object sender, EventArgs e)
        {
            SaveTriggerConditionEditor();
            SetTabPage(tabMotionGestures);
        }

        private void clbTriggerForObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void clbObjectGroupA_SelectedIndexChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void clbObjectGroupB_SelectedIndexChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void chkInvertProxEventTrigger_CheckedChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void chkProxEventTriggerOnce_CheckedChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void txtMaxDistProxEvent_TextChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }

        private void txtMinDistProxEvent_TextChanged(object sender, EventArgs e)
        {
            _TriggerConditionChanged = true;
            UpdateTriggerConditionEditor();
        }
        #endregion
    }
}
