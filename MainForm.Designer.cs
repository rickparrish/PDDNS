/*
  R&M PDDNS: A Promiscuous Dynamic DNS Client
  Copyright (C) 2013  Rick Parrish, R&M Software

  This file is part of PDDNS.

  PDDNS is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  any later version.

  PDDNS is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with PDDNS.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace RandM.PDDNS
{
    partial class MainForm
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
                if (Logging.instance != null) Logging.instance.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.Tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.tmrGetExternalIP = new System.Windows.Forms.Timer(this.components);
            this.lvHosts = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileConfigureEmailAlerts = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileConfigureIPDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewLogWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpSupportWebsite = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpCheckForUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrCheckForUpdate = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblIPAddress);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 388);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(624, 53);
            this.panel2.TabIndex = 1;
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblIPAddress.Font = new System.Drawing.Font("Comic Sans MS", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIPAddress.ForeColor = System.Drawing.Color.Orange;
            this.lblIPAddress.Location = new System.Drawing.Point(3, 5);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(194, 45);
            this.lblIPAddress.TabIndex = 0;
            this.lblIPAddress.Text = "Checking...";
            this.lblIPAddress.Click += new System.EventHandler(this.lblIPAddress_Click);
            // 
            // Tray
            // 
            this.Tray.Icon = ((System.Drawing.Icon)(resources.GetObject("Tray.Icon")));
            this.Tray.Text = "R&&&M PDDNS";
            this.Tray.DoubleClick += new System.EventHandler(this.Tray_DoubleClick);
            // 
            // tmrGetExternalIP
            // 
            this.tmrGetExternalIP.Enabled = true;
            this.tmrGetExternalIP.Interval = 1000;
            this.tmrGetExternalIP.Tick += new System.EventHandler(this.tmrGetExternalIP_Tick);
            // 
            // lvHosts
            // 
            this.lvHosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvHosts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvHosts.FullRowSelect = true;
            this.lvHosts.GridLines = true;
            this.lvHosts.Location = new System.Drawing.Point(0, 24);
            this.lvHosts.Name = "lvHosts";
            this.lvHosts.Size = new System.Drawing.Size(624, 364);
            this.lvHosts.TabIndex = 2;
            this.lvHosts.UseCompatibleStateImageBehavior = false;
            this.lvHosts.View = System.Windows.Forms.View.Details;
            this.lvHosts.DoubleClick += new System.EventHandler(this.lvHosts_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Hostname";
            this.columnHeader1.Width = 146;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Provider";
            this.columnHeader2.Width = 115;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Current IP";
            this.columnHeader3.Width = 109;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 224;
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuAdd,
            this.mnuHelp});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(624, 24);
            this.MainMenu.TabIndex = 3;
            this.MainMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileConfigureEmailAlerts,
            this.mnuFileConfigureIPDetection,
            this.toolStripMenuItem2,
            this.mnuFileUpdate,
            this.toolStripMenuItem1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "&File";
            // 
            // mnuFileConfigureEmailAlerts
            // 
            this.mnuFileConfigureEmailAlerts.Name = "mnuFileConfigureEmailAlerts";
            this.mnuFileConfigureEmailAlerts.Size = new System.Drawing.Size(193, 22);
            this.mnuFileConfigureEmailAlerts.Text = "Configure &email alerts";
            this.mnuFileConfigureEmailAlerts.Click += new System.EventHandler(this.mnuFileConfigureEmailAlerts_Click);
            // 
            // mnuFileConfigureIPDetection
            // 
            this.mnuFileConfigureIPDetection.Name = "mnuFileConfigureIPDetection";
            this.mnuFileConfigureIPDetection.Size = new System.Drawing.Size(193, 22);
            this.mnuFileConfigureIPDetection.Text = "Configure &IP detection";
            this.mnuFileConfigureIPDetection.Click += new System.EventHandler(this.mnuFileConfigureIPDetection_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(190, 6);
            // 
            // mnuFileUpdate
            // 
            this.mnuFileUpdate.Name = "mnuFileUpdate";
            this.mnuFileUpdate.Size = new System.Drawing.Size(193, 22);
            this.mnuFileUpdate.Text = "&Update hosts now";
            this.mnuFileUpdate.Click += new System.EventHandler(this.mnuFileUpdate_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(190, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(193, 22);
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewLogWindow});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "&View";
            // 
            // mnuViewLogWindow
            // 
            this.mnuViewLogWindow.Name = "mnuViewLogWindow";
            this.mnuViewLogWindow.Size = new System.Drawing.Size(139, 22);
            this.mnuViewLogWindow.Text = "&Log window";
            this.mnuViewLogWindow.Click += new System.EventHandler(this.mnuViewLogWindow_Click);
            // 
            // mnuAdd
            // 
            this.mnuAdd.Name = "mnuAdd";
            this.mnuAdd.Size = new System.Drawing.Size(99, 20);
            this.mnuAdd.Text = "&Add Hostname";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpSupportWebsite,
            this.mnuHelpCheckForUpdate,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpSupportWebsite
            // 
            this.mnuHelpSupportWebsite.Name = "mnuHelpSupportWebsite";
            this.mnuHelpSupportWebsite.Size = new System.Drawing.Size(165, 22);
            this.mnuHelpSupportWebsite.Text = "&Support Website";
            this.mnuHelpSupportWebsite.Click += new System.EventHandler(this.mnuHelpSupportWebsite_Click);
            // 
            // mnuHelpCheckForUpdate
            // 
            this.mnuHelpCheckForUpdate.Name = "mnuHelpCheckForUpdate";
            this.mnuHelpCheckForUpdate.Size = new System.Drawing.Size(165, 22);
            this.mnuHelpCheckForUpdate.Text = "&Check for update";
            this.mnuHelpCheckForUpdate.Click += new System.EventHandler(this.mnuHelpCheckForUpdate_Click);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(165, 22);
            this.mnuHelpAbout.Text = "&About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tmrCheckForUpdate
            // 
            this.tmrCheckForUpdate.Enabled = true;
            this.tmrCheckForUpdate.Interval = 1000;
            this.tmrCheckForUpdate.Tick += new System.EventHandler(this.tmrCheckForUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.lvHosts);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.MainMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "R&M PDDNS";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NotifyIcon Tray;
        private System.Windows.Forms.Timer tmrGetExternalIP;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.ListView lvHosts;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileConfigureIPDetection;
        private System.Windows.Forms.ToolStripMenuItem mnuFileUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuAdd;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpSupportWebsite;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuFileConfigureEmailAlerts;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpCheckForUpdate;
        private System.Windows.Forms.Timer tmrCheckForUpdate;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewLogWindow;

    }
}

