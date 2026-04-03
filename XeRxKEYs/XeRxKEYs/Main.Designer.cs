namespace XeRxKEYs
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.niTaskbarIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsPopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkSendInputs = new System.Windows.Forms.CheckBox();
            this.tcDisplayTabs = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.picActiveGestureProfile = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtActiveGestureProfileDesc = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbActiveGestureProfile = new System.Windows.Forms.ComboBox();
            this.btnInputSelectPopupTest = new System.Windows.Forms.Button();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.chkSaveAssetsOnClose = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnImportAction = new System.Windows.Forms.Button();
            this.btnExportActions = new System.Windows.Forms.Button();
            this.btnImportGesture = new System.Windows.Forms.Button();
            this.btnExportGestures = new System.Windows.Forms.Button();
            this.btnImportProfile = new System.Windows.Forms.Button();
            this.btnExportProfiles = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.chkMinimizeAtStart = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.clbDefaultOutModules = new System.Windows.Forms.CheckedListBox();
            this.cmbDefaultGestureProfile = new System.Windows.Forms.ComboBox();
            this.cmbDefaultXRModule = new System.Windows.Forms.ComboBox();
            this.btnSettingsSave = new System.Windows.Forms.Button();
            this.tabGestureProfiles = new System.Windows.Forms.TabPage();
            this.tabMotionGestures = new System.Windows.Forms.TabPage();
            this.tabEditTriggerActions = new System.Windows.Forms.TabPage();
            this.tabTriggerConditions = new System.Windows.Forms.TabPage();
            this.oFDSelectFile = new System.Windows.Forms.OpenFileDialog();
            this.fbdSelectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnIconSelectPopup = new System.Windows.Forms.Button();
            this.cmsPopupMenu.SuspendLayout();
            this.tcDisplayTabs.SuspendLayout();
            this.tabMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picActiveGestureProfile)).BeginInit();
            this.tabSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // niTaskbarIcon
            // 
            this.niTaskbarIcon.ContextMenuStrip = this.cmsPopupMenu;
            this.niTaskbarIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("niTaskbarIcon.Icon")));
            this.niTaskbarIcon.Text = "XeRxKEYs";
            this.niTaskbarIcon.Visible = true;
            this.niTaskbarIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.niTaskbarIcon_MouseDoubleClick);
            // 
            // cmsPopupMenu
            // 
            this.cmsPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.cmsPopupMenu.Name = "cmsPopupMenu";
            this.cmsPopupMenu.Size = new System.Drawing.Size(104, 48);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // chkSendInputs
            // 
            this.chkSendInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSendInputs.AutoSize = true;
            this.chkSendInputs.Checked = true;
            this.chkSendInputs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendInputs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSendInputs.Location = new System.Drawing.Point(616, 6);
            this.chkSendInputs.Name = "chkSendInputs";
            this.chkSendInputs.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSendInputs.Size = new System.Drawing.Size(152, 17);
            this.chkSendInputs.TabIndex = 1;
            this.chkSendInputs.Text = ":Enable Input Sending";
            this.chkSendInputs.UseVisualStyleBackColor = true;
            this.chkSendInputs.CheckedChanged += new System.EventHandler(this.chkSendInputs_CheckedChanged);
            // 
            // tcDisplayTabs
            // 
            this.tcDisplayTabs.Controls.Add(this.tabMain);
            this.tcDisplayTabs.Controls.Add(this.tabSettings);
            this.tcDisplayTabs.Controls.Add(this.tabGestureProfiles);
            this.tcDisplayTabs.Controls.Add(this.tabMotionGestures);
            this.tcDisplayTabs.Controls.Add(this.tabEditTriggerActions);
            this.tcDisplayTabs.Controls.Add(this.tabTriggerConditions);
            this.tcDisplayTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDisplayTabs.Location = new System.Drawing.Point(0, 0);
            this.tcDisplayTabs.Name = "tcDisplayTabs";
            this.tcDisplayTabs.SelectedIndex = 0;
            this.tcDisplayTabs.Size = new System.Drawing.Size(784, 441);
            this.tcDisplayTabs.TabIndex = 2;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.btnIconSelectPopup);
            this.tabMain.Controls.Add(this.btnExit);
            this.tabMain.Controls.Add(this.btnSettings);
            this.tabMain.Controls.Add(this.picActiveGestureProfile);
            this.tabMain.Controls.Add(this.label14);
            this.tabMain.Controls.Add(this.txtActiveGestureProfileDesc);
            this.tabMain.Controls.Add(this.label13);
            this.tabMain.Controls.Add(this.cmbActiveGestureProfile);
            this.tabMain.Controls.Add(this.btnInputSelectPopupTest);
            this.tabMain.Controls.Add(this.chkSendInputs);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(776, 415);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(644, 371);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(124, 32);
            this.btnExit.TabIndex = 15;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(644, 333);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(124, 32);
            this.btnSettings.TabIndex = 14;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // picActiveGestureProfile
            // 
            this.picActiveGestureProfile.Location = new System.Drawing.Point(365, 6);
            this.picActiveGestureProfile.Name = "picActiveGestureProfile";
            this.picActiveGestureProfile.Size = new System.Drawing.Size(81, 77);
            this.picActiveGestureProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picActiveGestureProfile.TabIndex = 13;
            this.picActiveGestureProfile.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(68, 39);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Description:";
            // 
            // txtActiveGestureProfileDesc
            // 
            this.txtActiveGestureProfileDesc.Location = new System.Drawing.Point(149, 34);
            this.txtActiveGestureProfileDesc.Multiline = true;
            this.txtActiveGestureProfileDesc.Name = "txtActiveGestureProfileDesc";
            this.txtActiveGestureProfileDesc.ReadOnly = true;
            this.txtActiveGestureProfileDesc.Size = new System.Drawing.Size(210, 49);
            this.txtActiveGestureProfileDesc.TabIndex = 11;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(8, 10);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(135, 13);
            this.label13.TabIndex = 10;
            this.label13.Text = "Active Gesture Profile:";
            // 
            // cmbActiveGestureProfile
            // 
            this.cmbActiveGestureProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbActiveGestureProfile.FormattingEnabled = true;
            this.cmbActiveGestureProfile.Location = new System.Drawing.Point(149, 7);
            this.cmbActiveGestureProfile.Name = "cmbActiveGestureProfile";
            this.cmbActiveGestureProfile.Size = new System.Drawing.Size(210, 21);
            this.cmbActiveGestureProfile.TabIndex = 9;
            this.cmbActiveGestureProfile.SelectedIndexChanged += new System.EventHandler(this.cmbActiveGestureProfile_SelectedIndexChanged);
            // 
            // btnInputSelectPopupTest
            // 
            this.btnInputSelectPopupTest.Location = new System.Drawing.Point(616, 29);
            this.btnInputSelectPopupTest.Name = "btnInputSelectPopupTest";
            this.btnInputSelectPopupTest.Size = new System.Drawing.Size(152, 23);
            this.btnInputSelectPopupTest.TabIndex = 2;
            this.btnInputSelectPopupTest.Text = "InputSelectPopupTest";
            this.btnInputSelectPopupTest.UseVisualStyleBackColor = true;
            this.btnInputSelectPopupTest.Click += new System.EventHandler(this.btnInputSelectPopupTest_Click);
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.chkSaveAssetsOnClose);
            this.tabSettings.Controls.Add(this.label12);
            this.tabSettings.Controls.Add(this.label11);
            this.tabSettings.Controls.Add(this.label10);
            this.tabSettings.Controls.Add(this.label9);
            this.tabSettings.Controls.Add(this.label8);
            this.tabSettings.Controls.Add(this.label7);
            this.tabSettings.Controls.Add(this.btnImportAction);
            this.tabSettings.Controls.Add(this.btnExportActions);
            this.tabSettings.Controls.Add(this.btnImportGesture);
            this.tabSettings.Controls.Add(this.btnExportGestures);
            this.tabSettings.Controls.Add(this.btnImportProfile);
            this.tabSettings.Controls.Add(this.btnExportProfiles);
            this.tabSettings.Controls.Add(this.label6);
            this.tabSettings.Controls.Add(this.chkMinimizeAtStart);
            this.tabSettings.Controls.Add(this.label5);
            this.tabSettings.Controls.Add(this.label4);
            this.tabSettings.Controls.Add(this.label3);
            this.tabSettings.Controls.Add(this.label2);
            this.tabSettings.Controls.Add(this.label1);
            this.tabSettings.Controls.Add(this.clbDefaultOutModules);
            this.tabSettings.Controls.Add(this.cmbDefaultGestureProfile);
            this.tabSettings.Controls.Add(this.cmbDefaultXRModule);
            this.tabSettings.Controls.Add(this.btnSettingsSave);
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(776, 415);
            this.tabSettings.TabIndex = 1;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            this.tabSettings.Click += new System.EventHandler(this.tabSettings_Click);
            // 
            // chkSaveAssetsOnClose
            // 
            this.chkSaveAssetsOnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSaveAssetsOnClose.AutoSize = true;
            this.chkSaveAssetsOnClose.Checked = true;
            this.chkSaveAssetsOnClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveAssetsOnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSaveAssetsOnClose.Location = new System.Drawing.Point(588, 70);
            this.chkSaveAssetsOnClose.Name = "chkSaveAssetsOnClose";
            this.chkSaveAssetsOnClose.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSaveAssetsOnClose.Size = new System.Drawing.Size(155, 17);
            this.chkSaveAssetsOnClose.TabIndex = 26;
            this.chkSaveAssetsOnClose.Text = ":Save Assets On Close";
            this.chkSaveAssetsOnClose.UseVisualStyleBackColor = true;
            this.chkSaveAssetsOnClose.CheckedChanged += new System.EventHandler(this.chkSaveAssetsOnClose_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(416, 321);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(311, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Run XeRxKEYs with these to configure it at launch automatically";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(539, 254);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(188, 52);
            this.label11.TabIndex = 24;
            this.label11.Text = "-module \"XR_Module_Name\"\r\n-outmod \"Out,Module,Names,As,CSV\"\r\n-profile \"Gesture_Pr" +
    "ofile_Name\"\r\n-launch \"Path_To_Some_Other_EXE\"";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(413, 254);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(120, 13);
            this.label10.TabIndex = 23;
            this.label10.Text = "Launch Parameters:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(85, 74);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(289, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "The profile to load at startup if not set via launch parameters";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(413, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "Import Single File:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(588, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Export Entire Data Set:";
            // 
            // btnImportAction
            // 
            this.btnImportAction.Location = new System.Drawing.Point(416, 198);
            this.btnImportAction.Name = "btnImportAction";
            this.btnImportAction.Size = new System.Drawing.Size(152, 32);
            this.btnImportAction.TabIndex = 19;
            this.btnImportAction.Text = "Import Trigger Action";
            this.btnImportAction.UseVisualStyleBackColor = true;
            this.btnImportAction.Click += new System.EventHandler(this.btnImportAction_Click);
            // 
            // btnExportActions
            // 
            this.btnExportActions.Location = new System.Drawing.Point(591, 198);
            this.btnExportActions.Name = "btnExportActions";
            this.btnExportActions.Size = new System.Drawing.Size(152, 32);
            this.btnExportActions.TabIndex = 18;
            this.btnExportActions.Text = "Export Trigger Actions";
            this.btnExportActions.UseVisualStyleBackColor = true;
            this.btnExportActions.Click += new System.EventHandler(this.btnExportActions_Click);
            // 
            // btnImportGesture
            // 
            this.btnImportGesture.Location = new System.Drawing.Point(416, 160);
            this.btnImportGesture.Name = "btnImportGesture";
            this.btnImportGesture.Size = new System.Drawing.Size(152, 32);
            this.btnImportGesture.TabIndex = 17;
            this.btnImportGesture.Text = "Import Motion Gesture";
            this.btnImportGesture.UseVisualStyleBackColor = true;
            this.btnImportGesture.Click += new System.EventHandler(this.btnImportGesture_Click);
            // 
            // btnExportGestures
            // 
            this.btnExportGestures.Location = new System.Drawing.Point(591, 160);
            this.btnExportGestures.Name = "btnExportGestures";
            this.btnExportGestures.Size = new System.Drawing.Size(152, 32);
            this.btnExportGestures.TabIndex = 16;
            this.btnExportGestures.Text = "Export Motion Gestures";
            this.btnExportGestures.UseVisualStyleBackColor = true;
            this.btnExportGestures.Click += new System.EventHandler(this.btnExportGestures_Click);
            // 
            // btnImportProfile
            // 
            this.btnImportProfile.Location = new System.Drawing.Point(416, 122);
            this.btnImportProfile.Name = "btnImportProfile";
            this.btnImportProfile.Size = new System.Drawing.Size(152, 32);
            this.btnImportProfile.TabIndex = 15;
            this.btnImportProfile.Text = "Import Gesture Profile";
            this.btnImportProfile.UseVisualStyleBackColor = true;
            this.btnImportProfile.Click += new System.EventHandler(this.btnImportProfile_Click);
            // 
            // btnExportProfiles
            // 
            this.btnExportProfiles.Location = new System.Drawing.Point(591, 122);
            this.btnExportProfiles.Name = "btnExportProfiles";
            this.btnExportProfiles.Size = new System.Drawing.Size(152, 32);
            this.btnExportProfiles.TabIndex = 14;
            this.btnExportProfiles.Text = "Export Gesture Profiles";
            this.btnExportProfiles.UseVisualStyleBackColor = true;
            this.btnExportProfiles.Click += new System.EventHandler(this.btnExportProfiles_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(459, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(284, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Will be ignored if no default or launch Gesture Profile is set!";
            // 
            // chkMinimizeAtStart
            // 
            this.chkMinimizeAtStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMinimizeAtStart.AutoSize = true;
            this.chkMinimizeAtStart.Checked = true;
            this.chkMinimizeAtStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMinimizeAtStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMinimizeAtStart.Location = new System.Drawing.Point(603, 17);
            this.chkMinimizeAtStart.Name = "chkMinimizeAtStart";
            this.chkMinimizeAtStart.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkMinimizeAtStart.Size = new System.Drawing.Size(140, 17);
            this.chkMinimizeAtStart.TabIndex = 12;
            this.chkMinimizeAtStart.Text = ":Minimize At Launch";
            this.chkMinimizeAtStart.UseVisualStyleBackColor = true;
            this.chkMinimizeAtStart.CheckedChanged += new System.EventHandler(this.chkMinimizeAtStart_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 286);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Note:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(62, 286);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(276, 39);
            this.label4.TabIndex = 10;
            this.label4.Text = "Actions will be sent to ALL selected modules at once\r\nInput will be handled if an" +
    " Out Module supports it,\r\neg: Some modules can only handle KEY or MOUSE input";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Active Out Module(s):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Default Gesture Profile:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Active XR Module:";
            // 
            // clbDefaultOutModules
            // 
            this.clbDefaultOutModules.FormattingEnabled = true;
            this.clbDefaultOutModules.Location = new System.Drawing.Point(21, 126);
            this.clbDefaultOutModules.Name = "clbDefaultOutModules";
            this.clbDefaultOutModules.ScrollAlwaysVisible = true;
            this.clbDefaultOutModules.Size = new System.Drawing.Size(353, 154);
            this.clbDefaultOutModules.TabIndex = 6;
            this.clbDefaultOutModules.SelectedIndexChanged += new System.EventHandler(this.clbDefaultOutModules_SelectedIndexChanged);
            // 
            // cmbDefaultGestureProfile
            // 
            this.cmbDefaultGestureProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultGestureProfile.FormattingEnabled = true;
            this.cmbDefaultGestureProfile.Location = new System.Drawing.Point(164, 44);
            this.cmbDefaultGestureProfile.Name = "cmbDefaultGestureProfile";
            this.cmbDefaultGestureProfile.Size = new System.Drawing.Size(210, 21);
            this.cmbDefaultGestureProfile.TabIndex = 5;
            this.cmbDefaultGestureProfile.SelectedIndexChanged += new System.EventHandler(this.cmbDefaultGestureProfile_SelectedIndexChanged);
            // 
            // cmbDefaultXRModule
            // 
            this.cmbDefaultXRModule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultXRModule.FormattingEnabled = true;
            this.cmbDefaultXRModule.Location = new System.Drawing.Point(164, 17);
            this.cmbDefaultXRModule.Name = "cmbDefaultXRModule";
            this.cmbDefaultXRModule.Size = new System.Drawing.Size(143, 21);
            this.cmbDefaultXRModule.TabIndex = 4;
            this.cmbDefaultXRModule.SelectedIndexChanged += new System.EventHandler(this.cmbDefaultXRModule_SelectedIndexChanged);
            // 
            // btnSettingsSave
            // 
            this.btnSettingsSave.Location = new System.Drawing.Point(619, 360);
            this.btnSettingsSave.Name = "btnSettingsSave";
            this.btnSettingsSave.Size = new System.Drawing.Size(124, 32);
            this.btnSettingsSave.TabIndex = 3;
            this.btnSettingsSave.Text = "Save and Close";
            this.btnSettingsSave.UseVisualStyleBackColor = true;
            this.btnSettingsSave.Click += new System.EventHandler(this.btnSettingsSave_Click);
            // 
            // tabGestureProfiles
            // 
            this.tabGestureProfiles.Location = new System.Drawing.Point(4, 22);
            this.tabGestureProfiles.Name = "tabGestureProfiles";
            this.tabGestureProfiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabGestureProfiles.Size = new System.Drawing.Size(776, 415);
            this.tabGestureProfiles.TabIndex = 2;
            this.tabGestureProfiles.Text = "GestureProfiles";
            this.tabGestureProfiles.UseVisualStyleBackColor = true;
            // 
            // tabMotionGestures
            // 
            this.tabMotionGestures.Location = new System.Drawing.Point(4, 22);
            this.tabMotionGestures.Name = "tabMotionGestures";
            this.tabMotionGestures.Padding = new System.Windows.Forms.Padding(3);
            this.tabMotionGestures.Size = new System.Drawing.Size(776, 415);
            this.tabMotionGestures.TabIndex = 3;
            this.tabMotionGestures.Text = "MotionGestures";
            this.tabMotionGestures.UseVisualStyleBackColor = true;
            // 
            // tabEditTriggerActions
            // 
            this.tabEditTriggerActions.Location = new System.Drawing.Point(4, 22);
            this.tabEditTriggerActions.Name = "tabEditTriggerActions";
            this.tabEditTriggerActions.Padding = new System.Windows.Forms.Padding(3);
            this.tabEditTriggerActions.Size = new System.Drawing.Size(776, 415);
            this.tabEditTriggerActions.TabIndex = 4;
            this.tabEditTriggerActions.Text = "TriggerActions";
            this.tabEditTriggerActions.UseVisualStyleBackColor = true;
            // 
            // tabTriggerConditions
            // 
            this.tabTriggerConditions.Location = new System.Drawing.Point(4, 22);
            this.tabTriggerConditions.Name = "tabTriggerConditions";
            this.tabTriggerConditions.Padding = new System.Windows.Forms.Padding(3);
            this.tabTriggerConditions.Size = new System.Drawing.Size(776, 415);
            this.tabTriggerConditions.TabIndex = 5;
            this.tabTriggerConditions.Text = "TriggerConditions";
            this.tabTriggerConditions.UseVisualStyleBackColor = true;
            // 
            // oFDSelectFile
            // 
            this.oFDSelectFile.DefaultExt = "json";
            this.oFDSelectFile.FileName = "Select File";
            this.oFDSelectFile.Filter = "JSON files (*.json)|*.json";
            // 
            // fbdSelectFolder
            // 
            this.fbdSelectFolder.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // btnIconSelectPopup
            // 
            this.btnIconSelectPopup.Location = new System.Drawing.Point(616, 58);
            this.btnIconSelectPopup.Name = "btnIconSelectPopup";
            this.btnIconSelectPopup.Size = new System.Drawing.Size(152, 23);
            this.btnIconSelectPopup.TabIndex = 16;
            this.btnIconSelectPopup.Text = "IconSelectPopupTest";
            this.btnIconSelectPopup.UseVisualStyleBackColor = true;
            this.btnIconSelectPopup.Click += new System.EventHandler(this.btnIconSelectPopup_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.tcDisplayTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "XeRxKEYs";
            this.Load += new System.EventHandler(this.Main_Load);
            this.cmsPopupMenu.ResumeLayout(false);
            this.tcDisplayTabs.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picActiveGestureProfile)).EndInit();
            this.tabSettings.ResumeLayout(false);
            this.tabSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niTaskbarIcon;
        private System.Windows.Forms.ContextMenuStrip cmsPopupMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSendInputs;
        private System.Windows.Forms.TabControl tcDisplayTabs;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabGestureProfiles;
        private System.Windows.Forms.TabPage tabMotionGestures;
        private System.Windows.Forms.TabPage tabEditTriggerActions;
        private System.Windows.Forms.TabPage tabTriggerConditions;
        private System.Windows.Forms.Button btnInputSelectPopupTest;
        private System.Windows.Forms.Button btnSettingsSave;
        private System.Windows.Forms.ComboBox cmbDefaultXRModule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox clbDefaultOutModules;
        private System.Windows.Forms.ComboBox cmbDefaultGestureProfile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkMinimizeAtStart;
        private System.Windows.Forms.Button btnImportProfile;
        private System.Windows.Forms.Button btnExportProfiles;
        private System.Windows.Forms.Button btnImportAction;
        private System.Windows.Forms.Button btnExportActions;
        private System.Windows.Forms.Button btnImportGesture;
        private System.Windows.Forms.Button btnExportGestures;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.OpenFileDialog oFDSelectFile;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.FolderBrowserDialog fbdSelectFolder;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox cmbActiveGestureProfile;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtActiveGestureProfileDesc;
        private System.Windows.Forms.PictureBox picActiveGestureProfile;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.CheckBox chkSaveAssetsOnClose;
        private System.Windows.Forms.Button btnIconSelectPopup;
    }
}

