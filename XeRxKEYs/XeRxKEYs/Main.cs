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
using XeRxKEYs.Properties;

namespace XeRxKEYs
{
    public partial class Main : Form
    {
        private System.Threading.Timer _keySenderTimer;

        private InputHelper inputHelper;

        private bool sendLock = false;

        public List<SendableInput> ActiveSendables = new List<SendableInput>();

        private IXRModule xrModuleInstance = null;
        private Type xrModuleType = typeof(IXRModule);

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;

            FormClosing += Main_FormClosing;
            niTaskbarIcon.MouseUp += niTaskbarIcon_MouseUp;

            //TODO: Update this to send more frequently
            _keySenderTimer = new System.Threading.Timer(TimerCallback, null, 5000, 5000);

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

            if (settings.MinimizeAtStart && settings.GestureProfile != "")
            {
                this.WindowState = FormWindowState.Minimized;
            }
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
            SendKeysToForegroundWindow(myOwnHandle);
        }

        private void SendKeysToForegroundWindow(IntPtr myOwnHandle)
        {
            IntPtr targetHwnd = WindowsAPI.GetForegroundWindow();
            if (sendLock || targetHwnd == myOwnHandle || targetHwnd == IntPtr.Zero ) // || ActiveSendables.Count <= 0)
            {
                return;
            }

            sendLock = true;

            WindowsAPI.SetForegroundWindow(targetHwnd);

            Thread.Sleep(50);

            //foreach (SendableInput input in ActiveSendables)
            //{

            //}

            //ActiveSendables.Clear();
            
            //TODO: Remove DEBUG code below

            if (chkSendInput.Checked)
            {
                inputHelper.PressKey('w', true);
                Thread.Sleep(100);
                inputHelper.PressKey('w', false);
                Thread.Sleep(100);
                inputHelper.PressKey('a', true);
                Thread.Sleep(100);
                inputHelper.PressKey('a', false);

                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;
                int targetX = screenWidth / 2;
                int targetY = screenHeight / 2;

                inputHelper.MoveMouse(targetX, targetY);

                Thread.Sleep(100);

                inputHelper.SendMouseClick(WindowsAPI.MOUSEEVENTF_RIGHTDOWN, WindowsAPI.MOUSEEVENTF_RIGHTUP, 100);

                Thread.Sleep(100);

                inputHelper.ScrollMouseWheel(-120);
            }

            if (chkFallback.Checked)
            {
                inputHelper.PressKeyFallback(new SendableKey("W", "W"));
                inputHelper.PressKeyFallback(new SendableKey("w", "w"));
                inputHelper.PressKeyFallback(new SendableKey("A", "A"));
                inputHelper.PressKeyFallback(new SendableKey("a", "a"));

                inputHelper.ScrollMouseWheelFallback(120);
                Thread.Sleep(1500);
                inputHelper.ScrollMouseWheelFallback(-120);

                inputHelper.SendMouseClickFallback(WindowsAPI.MOUSEEVENTF_MIDDLEDOWN, WindowsAPI.MOUSEEVENTF_MIDDLEUP);
            }

            sendLock = false;
        }

        private void ShowWindow()
        {
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

        private void chkFallback_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
