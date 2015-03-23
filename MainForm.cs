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
using RandM.RMLibUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public partial class MainForm : Form
    {
        private string _EmailTemplateIPChange = "";
        private string _EmailTemplateNewVersion = "";
        private string _EmailTemplateUpdateError = "";
        private bool _FirstPaint = true;
        private FormWindowState _LastWindowState = FormWindowState.Normal;
        private bool _NewVersionAvailable = false;

        public MainForm()
        {
            InitializeComponent();

            LoadEmailTemplates();
            LoadProviders();
            LoadHosts();
        }

        private bool CheckHosts(IPAddress localIPAddress)
        {
            bool Result = true;

            List<string> Hostnames = new List<string>();

            foreach (ListViewItem LVI in lvHosts.Items)
            {
                Hostnames.Add(LVI.SubItems[0].Text);
            }

            foreach (string Hostname in Hostnames)
            {
                try
                {
                    HostConfig HC = new HostConfig(Hostname);
                    if (HC.Loaded)
                    {
                        bool HostNeedsUpdate = true;

                        // Get current IP for host (to see if it differs from detected IP)
                        try
                        {
                            IPHostEntry HostEnt = Dns.GetHostEntry(HC.Hostname);
                            foreach (IPAddress RemoteIPAddress in HostEnt.AddressList)
                            {
                                SetCurrentIP(HC.Hostname, RemoteIPAddress.ToString());
                                if (RemoteIPAddress.ToString() == localIPAddress.ToString())
                                {
                                    HostNeedsUpdate = false;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SetCurrentIP(HC.Hostname, "Unable to resolve");
                            Logging.instance.LogException("Unable to resolve \"" + HC.Hostname + "\"", ex);
                        }

                        // Also check if it has been > 7 days since we updated, and if so, force an update to ensure our host doesn't expire
                        if (DateTime.Now.Subtract(HC.LastUpdateDate).TotalDays >= 7.0)
                        {
                            HostNeedsUpdate = true;
                        }

                        if (HostNeedsUpdate && !HC.Disabled)
                        {
                            // If we get here it means the IP retrieved from DNS doesn't match our detected external IP, so
                            // in theory we need to send an update to the provider.  The problem is if the provider has a
                            // large TTL value then we may have already sent an update and just got a cached IP from the DNS 
                            // lookup -- and in that case we don't want to send another update.  So we'll check if the last 
                            // IP we updated with matches the current IP.  If it does, then we won't send another update 
                            // unless one hour has passed since the last update
                            if ((localIPAddress.ToString() != HC.LastUpdateIP) || (DateTime.Now.Subtract(HC.LastUpdateDate).TotalHours > 1))
                            {
                                if (!Debugger.IsAttached || (Dialog.YesNo("Do you want to update " + HC.Hostname + "?", "Confirm Update") == DialogResult.Yes))
                                {
                                    Logging.instance.LogMessage("Updating \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + ") to " + localIPAddress);
                                    HC.LastUpdateIP = localIPAddress.ToString();
                                    HC.LastUpdateDate = DateTime.Now;
                                    HC.Save();

                                    Provider P = Providers.GetProvider(HC.Provider);
                                    P.Update(HC, localIPAddress);
                                    SetStatus(HC.Hostname, HC.Status);
                                }
                            }
                        }

                        if (HC.Disabled) Result = false;
                    }
                }
                catch (Exception ex)
                {
                    // Don't let a single host fail the whole update process
                    Logging.instance.LogException("Error checking \"" + Hostname + "\"", ex);
                    if (Debugger.IsAttached) Dialog.Error("Error checking " + Hostname + ":\r\n\r\n" + ex.Message, "Error");
                    Result = false;
                }
            }

            return Result;
        }

        private void GetExternalIPAndCheckHosts()
        {
            if (!tmrGetExternalIP.Enabled)
            {
                Logging.instance.LogWarning("GetExternalIPAndCheckHosts() called while GetExternalIPAndCheckHosts() already running -- this should not happen!");
                return;
            }

            tmrGetExternalIP.Stop();

            this.Icon = Properties.Resources.Fatcow_Farm_Fresh_World_go;
            Tray.Icon = this.Icon;

            lblIPAddress.Text = "Checking...";
            lblIPAddress.ForeColor = Color.Orange;

            IPAddress NewIPAddress = WebUtils.GetExternalIPv4(Config.Default.GetExternalIPMethodOrder);
            bool IsValidIP = (!WebUtils.IsPrivateIP(NewIPAddress) && (NewIPAddress.ToString() != IPAddress.Any.ToString()) && (NewIPAddress.ToString() != IPAddress.None.ToString()));

            lblIPAddress.Text = NewIPAddress.ToString();
            lblIPAddress.ForeColor = (IsValidIP ? Color.Green : Color.Red);

            if (IsValidIP)
            {
                if (Config.Default.LastIPAddress != NewIPAddress.ToString())
                {
                    SendEmailIPChange(Config.Default.LastIPAddress, NewIPAddress.ToString());
                    Config.Default.LastIPAddress = NewIPAddress.ToString();
                    Config.Default.Save();
                }
                if (CheckHosts(NewIPAddress))
                {
                    this.Icon = Properties.Resources.Fatcow_Farm_Fresh_World;
                    Tray.Icon = this.Icon;
                }
                else
                {
                    this.Icon = Properties.Resources.Fatcow_Farm_Fresh_World_delete;
                    Tray.Icon = this.Icon;
                }
            }
            else
            {
                this.Icon = Properties.Resources.Fatcow_Farm_Fresh_World_delete;
                Tray.Icon = this.Icon;
            }

            tmrGetExternalIP.Interval = 5 * 60 * 1000; // 5 minutes
            tmrGetExternalIP.Start();
        }

        private void lblIPAddress_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(lblIPAddress.Text);
            this.WindowState = FormWindowState.Minimized;
        }

        void LoadEmailTemplates()
        {
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
            using (StreamReader SR = new StreamReader(CurrentAssembly.GetManifestResourceStream("RandM.PDDNS.EmailTemplates.IPChange.html")))
            {
                _EmailTemplateIPChange = SR.ReadToEnd();
            }
            using (StreamReader SR = new StreamReader(CurrentAssembly.GetManifestResourceStream("RandM.PDDNS.EmailTemplates.NewVersion.html")))
            {
                _EmailTemplateNewVersion = SR.ReadToEnd();
            }
            using (StreamReader SR = new StreamReader(CurrentAssembly.GetManifestResourceStream("RandM.PDDNS.EmailTemplates.UpdateError.html")))
            {
                _EmailTemplateUpdateError = SR.ReadToEnd();
            }
        }

        void LoadHosts()
        {
            lvHosts.Items.Clear();
            string[] Hostnames = HostConfig.GetHostnames();
            foreach (string Hostname in Hostnames)
            {
                HostConfig HC = new HostConfig(Hostname);
                ListViewItem LVI = new ListViewItem(Hostname);
                LVI.SubItems.Add(HC.Provider.ToString());
                LVI.SubItems.Add("Waiting to check...");
                LVI.SubItems.Add(HC.Status);
                lvHosts.Items.Add(LVI);
            }
        }

        private void LoadProviders()
        {
            string[] ProviderNames = Providers.GetProviderNames();
            foreach (string ProviderName in ProviderNames)
            {
                ToolStripMenuItem TSMI = new ToolStripMenuItem("Add " + ProviderName + " hostname");
                TSMI.Click += mnuAdd_Click;
                TSMI.Tag = ProviderName;
                mnuAdd.DropDownItems.Add(TSMI);
            }
        }

        private void lvHosts_DoubleClick(object sender, EventArgs e)
        {
            if (lvHosts.SelectedItems.Count > 0)
            {
                Provider P = Providers.GetProviderByName(lvHosts.SelectedItems[0].SubItems[1].Text);
                if (P == null)
                {
                    Logging.instance.LogError("Unable to load provider \"" + lvHosts.SelectedItems[0].SubItems[1].Text + "\"");
                    Dialog.Error("Sorry, there was an error loading that particular Edit Hostname form", "Error");
                }
                else
                {
                    if (P.Edit(lvHosts.SelectedItems[0].SubItems[0].Text))
                    {
                        LoadHosts();
                        if (tmrGetExternalIP.Enabled)
                        {
                            Logging.instance.LogMessage("Initiating  update (user edited a host)");
                            GetExternalIPAndCheckHosts();
                        }
                    }
                }
            }
        }

        private void lvHosts_KeyUp(object sender, KeyEventArgs e)
        {
            if (lvHosts.SelectedItems.Count > 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    if (Dialog.NoYes("Really delete " + lvHosts.SelectedItems[0].Text + "?", "Confirm Delete") == DialogResult.Yes)
                    {
                        // Delete from ini
                        HostConfig HC = new HostConfig(lvHosts.SelectedItems[0].Text);
                        HC.Delete();

                        // Delete from interface
                        lvHosts.Items.RemoveAt(lvHosts.SelectedIndices[0]);
                    }
                }
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (_FirstPaint)
            {
                _FirstPaint = false;
                if (!Debugger.IsAttached) this.WindowState = FormWindowState.Minimized;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                Tray.Visible = true;
            }
            else
            {
                _LastWindowState = this.WindowState;
            }
        }

        private void mnuAdd_Click(object sender, EventArgs e)
        {
            Provider P = Providers.GetProviderByName(((ToolStripMenuItem)sender).Tag.ToString());
            if (P == null)
            {
                Logging.instance.LogError("Unable to load provider \"" + ((ToolStripMenuItem)sender).Tag.ToString() + "\"");
                Dialog.Error("Sorry, there was an error loading that particular Add Hostname form", "Error");
            }
            else
            {
                if (P.Add())
                {
                    LoadHosts();
                    if (tmrGetExternalIP.Enabled)
                    {
                        Logging.instance.LogMessage("Initiating  update (user added a host)");
                        GetExternalIPAndCheckHosts();
                    }
                }
            }
        }

        private void mnuFileConfigureEmailAlerts_Click(object sender, EventArgs e)
        {
            using (ConfigureEmailAlertsForm F = new ConfigureEmailAlertsForm())
            {
                F.ShowDialog();
            }
        }

        private void mnuFileConfigureIPDetection_Click(object sender, EventArgs e)
        {
            using (ConfigureIPDetectionForm F = new ConfigureIPDetectionForm())
            {
                F.ShowDialog();
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuFileUpdate_Click(object sender, EventArgs e)
        {
            if (tmrGetExternalIP.Enabled)
            {
                Logging.instance.LogMessage("Initiating  update (user clicked File -> Update hosts now)");
                GetExternalIPAndCheckHosts();
            }
            else
            {
                Dialog.Error("An update is already in progress", "Unable to update now");
            }
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            Dialog.Information(ProcessUtils.Title + "\r\nversion " + ProcessUtils.ProductVersion + "\r\nby Rick Parrish of R&M Software\r\nhttp://www.randm.ca/pddns/\r\n\r\nProgram icons by Fatcow Web Hosting\r\nhttp://www.fatcow.com/free-icons", "Help -> About");
        }

        private void mnuHelpCheckForUpdate_Click(object sender, EventArgs e)
        {
            if (AutoUpdateUI.Update(new Uri("http://www.randm.ca/autoupdate.ini"), true))
            {
                Close();
            }
        }

        private void mnuHelpSupportWebsite_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.randm.ca/pddns/");
        }

        private void mnuViewLogWindow_Click(object sender, EventArgs e)
        {
            Logging.instance.ShowDialog();
        }

        private void popTrayCopyIPAddress_Click(object sender, EventArgs e)
        {
            lblIPAddress_Click(null, EventArgs.Empty);
        }

        private void popTrayExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void popTrayShowForm_Click(object sender, EventArgs e)
        {
            Tray_DoubleClick(null, EventArgs.Empty);
        }

        private void SendEmailIPChange(string oldIPAddress, string newIPAddress)
        {
            if (Config.Default.EmailAlertIPChange)
            {
                try
                {
                    if (string.IsNullOrEmpty(Config.Default.SmtpHostname))
                    {
                        Logging.instance.LogError("Unable to send IP change email (SMTP hostname is blank)");
                    }
                    else
                    {
                        Logging.instance.LogMessage("Sending IP change email");

                        string Body = _EmailTemplateIPChange;
                        Body = Body.Replace("[DATETIME]", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                        Body = Body.Replace("[OLDIP]", oldIPAddress);
                        Body = Body.Replace("[NEWIP]", newIPAddress);
                        if (_NewVersionAvailable) Body = Body.Replace("<!--NEWVERSION", "").Replace("NEWVERSION-->", "");

                        WebUtils.Email(Config.Default.SmtpHostname, Config.Default.SmtpPort, new MailAddress(Config.Default.EmailFrom), new MailAddress(Config.Default.EmailTo), new MailAddress(Config.Default.EmailFrom), "R&M PDDNS | External IP Address Change Notification", Body, true, Config.Default.SmtpUsername, Config.Default.SmtpPassword, Config.Default.SmtpSsl);
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("SendEmailIPChange(\"" + oldIPAddress + "\", \"" + newIPAddress + "\")", ex);
                }
            }
        }

        private void SendEmailNewVersion()
        {
            if (Config.Default.EmailAlertNewVersion)
            {
                try
                {
                    if (string.IsNullOrEmpty(Config.Default.SmtpHostname))
                    {
                        Logging.instance.LogError("Unable to send new version email (SMTP hostname is blank)");
                    }
                    else
                    {
                        Logging.instance.LogMessage("Sending new version email");

                        string Body = _EmailTemplateNewVersion;
                        Body = Body.Replace("[DATETIME]", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                        if (_NewVersionAvailable) Body = Body.Replace("<!--NEWVERSION", "").Replace("NEWVERSION-->", "");

                        WebUtils.Email(Config.Default.SmtpHostname, Config.Default.SmtpPort, new MailAddress(Config.Default.EmailFrom), new MailAddress(Config.Default.EmailTo), new MailAddress(Config.Default.EmailFrom), "R&M PDDNS | New Version Notification", Body, true, Config.Default.SmtpUsername, Config.Default.SmtpPassword, Config.Default.SmtpSsl);
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("SendEmailNewVersion()", ex);
                }
            }
        }

        private void SendEmailUpdateError(string hostname, string error)
        {
            if (Config.Default.EmailAlertUpdateError)
            {
                try
                {
                    if (string.IsNullOrEmpty(Config.Default.SmtpHostname))
                    {
                        Logging.instance.LogError("Unable to send update error email (SMTP hostname is blank)");
                    }
                    else
                    {
                        Logging.instance.LogMessage("Sending update error email");

                        string Body = _EmailTemplateUpdateError;
                        Body = Body.Replace("[DATETIME]", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
                        Body = Body.Replace("[HOSTNAME]", hostname);
                        Body = Body.Replace("[ERROR]", error);
                        if (_NewVersionAvailable) Body = Body.Replace("<!--NEWVERSION", "").Replace("NEWVERSION-->", "");

                        WebUtils.Email(Config.Default.SmtpHostname, Config.Default.SmtpPort, new MailAddress(Config.Default.EmailFrom), new MailAddress(Config.Default.EmailTo), new MailAddress(Config.Default.EmailFrom), "R&M PDDNS | Hostname Update Error Notification", Body, true, Config.Default.SmtpUsername, Config.Default.SmtpPassword, Config.Default.SmtpSsl);
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("SendEmailUpdateError(\"" + hostname + "\", \"" + error + "\")", ex);
                }
            }
        }

        private void SetCurrentIP(string hostname, string ipAddress)
        {
            ListViewItem LVI = lvHosts.FindItemWithText(hostname, true, 0);
            if (LVI != null)
            {
                LVI.SubItems[2].Text = ipAddress.ToString();
                Application.DoEvents();
            }
        }

        private void SetStatus(string hostname, string statusText)
        {
            ListViewItem LVI = lvHosts.FindItemWithText(hostname, true, 0);
            if (LVI != null)
            {
                LVI.SubItems[3].Text = statusText;
                Application.DoEvents();
            }
        }

        private void tmrCheckForUpdate_Tick(object sender, EventArgs e)
        {
            tmrCheckForUpdate.Stop();

            _NewVersionAvailable = AutoUpdate.Available(new Uri("http://www.randm.ca/autoupdate.ini"));

            if (_NewVersionAvailable)
            {
                Logging.instance.LogMessage("A new version is available at http://www.randm.ca/pddns/");
            }
            else
            {
                // Only restart the timer if there isn't a new version found yet
                tmrCheckForUpdate.Interval = 1 * 24 * 60 * 60 * 1000; // 1 day
                tmrCheckForUpdate.Start();
            }
        }

        void tmrGetExternalIP_Tick(object sender, EventArgs e)
        {
            Logging.instance.LogMessage("Initiating  update (timer interval elapsed)");
            GetExternalIPAndCheckHosts();
        }

        private void Tray_DoubleClick(object sender, EventArgs e)
        {
            if (!this.Visible) this.Show();
            if (!this.Focused) this.Activate();
            this.WindowState = _LastWindowState;
            Tray.Visible = false;
        }
    }
}
