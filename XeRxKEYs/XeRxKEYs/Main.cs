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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace XeRxKEYs
{
    public partial class Main : Form
    {
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

            RefreshMainUI();
        }

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

        private void btnInputSelectPopupTest_Click(object sender, EventArgs e)
        {
            SelectInput inputSel = new SelectInput();

            inputSel.SendInputTo(ReceiveInputTest);

            inputSel.ShowDialog(this);

            inputSel.Dispose();
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

        #endregion

        #region PAGES_UI

        private void SetTabPage(TabPage page, bool forceRefresh = false)
        {
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
                RefreshProfilesUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabMotionGestures)
            {
                RefreshGesturesUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabEditTriggerActions)
            {
                RefreshActionsUI();
            }
            else if (tcDisplayTabs.SelectedTab == tabTriggerConditions)
            {
                RefreshConditionsUI();
            }
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

            settings.Save();

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
        #endregion

        #region MAIN_UI
        private void RefreshMainUI(bool skipGestureLoad = true)
        {
            var settings = Properties.Settings.Default;

            if (!skipGestureLoad)
            {
                string gestureProfile = settings.GestureProfile;

                if (gestureProfile != "")
                {
                    foreach (GestureProfile profile in allGestureProfiles)
                    {
                        if (profile.Name == gestureProfile)
                        {
                            ActiveGestureProfile = profile;
                        }
                    }
                }
            }

            cmbActiveGestureProfile.Items.Clear();

            cmbActiveGestureProfile.Items.Add(" ");

            int index = 1;
            int currIndex = -1;

            foreach (GestureProfile profile in allGestureProfiles)
            {
                cmbActiveGestureProfile.Items.Add(profile.Name);

                if (ActiveGestureProfile != null && profile.Name == ActiveGestureProfile.Name)
                {
                    currIndex = index;
                }

                index += 1;
            }

            if (currIndex > -1)
            {
                cmbActiveGestureProfile.SelectedIndex = currIndex;
            }

            //TODO: Load values into the main UI

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
            if (cmbActiveGestureProfile.SelectedIndex <= 0)
            {
                ActiveGestureProfile = null;
                txtActiveGestureProfileDesc.Text = "";
                picActiveGestureProfile.Image = null;
            }
            else
            {
                foreach (GestureProfile profile in allGestureProfiles)
                {
                    if (profile.Name == cmbActiveGestureProfile.Text)
                    {
                        ActiveGestureProfile = profile;

                        txtActiveGestureProfileDesc.Text = profile.Description;

                        if (profile.Image != "")
                        {
                            string imagePath = Path.Combine(Application.StartupPath, "Images", profile.Image);

                            picActiveGestureProfile.Image = Image.FromFile(imagePath);
                        }
                        break;
                    }
                }
            }
        }
        #endregion

        #region PROFILES_UI
        private void RefreshProfilesUI()
        {
            //TODO: Load values into the Gesture Profiles UI
        }
        #endregion

        #region GESTURES_UI
        private void RefreshGesturesUI()
        {
            //TODO: Load values into the Motion Gestures UI
        }
        #endregion

        #region ACTIONS_UI
        private void RefreshActionsUI()
        {
            //TODO: Load values into the Trigger Actions UI
        }
        #endregion

        #region CONDITIONS_UI
        private void RefreshConditionsUI()
        {
            //TODO: Load values into the Trigger Conditions UI
        }
        #endregion
    }
}
