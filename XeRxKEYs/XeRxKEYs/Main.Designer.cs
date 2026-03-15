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
            this.chkFallback = new System.Windows.Forms.CheckBox();
            this.chkSendInput = new System.Windows.Forms.CheckBox();
            this.cmsPopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            // chkFallback
            // 
            this.chkFallback.AutoSize = true;
            this.chkFallback.Location = new System.Drawing.Point(609, 63);
            this.chkFallback.Name = "chkFallback";
            this.chkFallback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkFallback.Size = new System.Drawing.Size(128, 17);
            this.chkFallback.TabIndex = 0;
            this.chkFallback.Text = ":Alternate Input Mode";
            this.chkFallback.UseVisualStyleBackColor = true;
            this.chkFallback.CheckedChanged += new System.EventHandler(this.chkFallback_CheckedChanged);
            // 
            // chkSendInput
            // 
            this.chkSendInput.AutoSize = true;
            this.chkSendInput.Checked = true;
            this.chkSendInput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSendInput.Location = new System.Drawing.Point(617, 40);
            this.chkSendInput.Name = "chkSendInput";
            this.chkSendInput.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkSendInput.Size = new System.Drawing.Size(120, 17);
            this.chkSendInput.TabIndex = 1;
            this.chkSendInput.Text = ":Default Input Mode";
            this.chkSendInput.UseVisualStyleBackColor = true;
            // 
            // cmsPopupMenu
            // 
            this.cmsPopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.cmsPopupMenu.Name = "cmsPopupMenu";
            this.cmsPopupMenu.Size = new System.Drawing.Size(181, 70);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chkSendInput);
            this.Controls.Add(this.chkFallback);
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
        private System.Windows.Forms.CheckBox chkFallback;
        private System.Windows.Forms.CheckBox chkSendInput;
        private System.Windows.Forms.ContextMenuStrip cmsPopupMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
    }
}

