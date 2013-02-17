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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RandM.RMLibUI;
using RandM.RMLib;
using System.IO;

namespace RandM.PDDNS
{
    public partial class LogWindowForm : Form
    {
        private bool _Disposed = false;
        private object _Lock = new object();

        public LogWindowForm()
        {
            InitializeComponent();

            chkLogToDisk.Checked = Config.Default.LogToDisk;
            if (Config.Default.LogToDisk)
            {
                try
                {
                    if (File.Exists(Path.ChangeExtension(Config.Default.FileName, ".log")))
                    {
                        rtbLog.Text = FileUtils.FileReadAllText(Path.ChangeExtension(Config.Default.FileName, ".log"));
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("Unable to load previous log entries from \"" + Path.ChangeExtension(Config.Default.FileName, ".log") + "\"", ex);
                }
            }
        }

        public void AddToLog(string message, Color colour)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new MethodInvoker(delegate { AddToLog(message, colour); }));
            }
            else
            {
                lock (_Lock)
                {
                    string DateAndTime = "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "]  ";
                    rtbLog.SelectionHangingIndent = 50;
                    rtbLog.AppendText(DateAndTime + message + "\r\n", colour);
                    rtbLog.SelectionStart = rtbLog.Text.Length;
                    rtbLog.ScrollToCaret();
                }
            }
        }

        private void chkLogToDisk_CheckedChanged(object sender, EventArgs e)
        {
            Config.Default.LogToDisk = chkLogToDisk.Checked;
            Config.Default.Save();
        }

        private void cmdEmptyLog_Click(object sender, EventArgs e)
        {
            lock (_Lock)
            {
                rtbLog.Clear();
            }

            FileUtils.FileDelete(Path.ChangeExtension(Config.Default.FileName, ".log"));
        }

        private void tmrSaveLog_Tick(object sender, EventArgs e)
        {
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
                catch (Exception ex)
                {
                    Logging.instance.LogException("Unabled to save log entries to \"" + Path.ChangeExtension(Config.Default.FileName, ".log") + "\"", ex);
                }
            }
        }
    }
}
