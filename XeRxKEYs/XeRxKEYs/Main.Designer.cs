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
            this.cmsPopupMenu.SuspendLayout();
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
            this.chkSendInputs.Location = new System.Drawing.Point(700, 12);
            this.chkSendInputs.Name = "chkSendInputs";
            this.chkSendInputs.Size = new System.Drawing.Size(88, 17);
            this.chkSendInputs.TabIndex = 1;
            this.chkSendInputs.Text = "Send Input";
            this.chkSendInputs.UseVisualStyleBackColor = true;
            this.chkSendInputs.CheckedChanged += new System.EventHandler(this.chkSendInputs_CheckedChanged);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkSendInputs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "XeRxKEYs";
            this.Load += new System.EventHandler(this.Main_Load);
            this.cmsPopupMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon niTaskbarIcon;
        private System.Windows.Forms.ContextMenuStrip cmsPopupMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkSendInputs;
    }
}

