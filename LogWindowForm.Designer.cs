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
using RandM.RMLib;
using System;
using System.IO;
namespace RandM.PDDNS
{
    partial class LogWindowForm
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

                if (!_Disposed)
                {
                    // Dispose managed resources.
                    if (Config.Default.LogToDisk)
                    {
                        string LogText = "";

                        lock (_Lock)
                        {
                            LogText = rtbLog.Text;
                        }

                        try
                        {
                            FileUtils.FileWriteAllText(Path.ChangeExtension(Config.Default.FileName, ".log"), LogText);
                        }
                        catch (Exception)
                        {
                            // Can't do much about it at this point, so just ignore it
                        }
                    }
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogWindowForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkLogToDisk = new System.Windows.Forms.CheckBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdEmptyLog = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.tmrSaveLog = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkLogToDisk);
            this.panel1.Controls.Add(this.cmdClose);
            this.panel1.Controls.Add(this.cmdEmptyLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 396);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(624, 45);
            this.panel1.TabIndex = 0;
            // 
            // chkLogToDisk
            // 
            this.chkLogToDisk.AutoSize = true;
            this.chkLogToDisk.Location = new System.Drawing.Point(103, 16);
            this.chkLogToDisk.Name = "chkLogToDisk";
            this.chkLogToDisk.Size = new System.Drawing.Size(397, 17);
            this.chkLogToDisk.TabIndex = 2;
            this.chkLogToDisk.Text = "Log to disk (will persist log entries when R&&M PDDNS is stopped and restarted)";
            this.chkLogToDisk.UseVisualStyleBackColor = true;
            this.chkLogToDisk.CheckedChanged += new System.EventHandler(this.chkLogToDisk_CheckedChanged);
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(537, 12);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 1;
            this.cmdClose.Text = "&Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            // 
            // cmdEmptyLog
            // 
            this.cmdEmptyLog.Location = new System.Drawing.Point(12, 12);
            this.cmdEmptyLog.Name = "cmdEmptyLog";
            this.cmdEmptyLog.Size = new System.Drawing.Size(75, 23);
            this.cmdEmptyLog.TabIndex = 0;
            this.cmdEmptyLog.Text = "&Erase Log";
            this.cmdEmptyLog.UseVisualStyleBackColor = true;
            this.cmdEmptyLog.Click += new System.EventHandler(this.cmdEmptyLog_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.Black;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.LightGray;
            this.rtbLog.Location = new System.Drawing.Point(0, 0);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(624, 396);
            this.rtbLog.TabIndex = 1;
            this.rtbLog.Text = "";
            // 
            // tmrSaveLog
            // 
            this.tmrSaveLog.Enabled = true;
            this.tmrSaveLog.Interval = 60000;
            this.tmrSaveLog.Tick += new System.EventHandler(this.tmrSaveLog_Tick);
            // 
            // LogWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "LogWindowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "R&M PDDNS Log Window";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdEmptyLog;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Timer tmrSaveLog;
        private System.Windows.Forms.CheckBox chkLogToDisk;

    }
}