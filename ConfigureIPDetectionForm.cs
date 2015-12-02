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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public partial class ConfigureIPDetectionForm : Form
    {
        private bool _FirstPaint = true;

        public ConfigureIPDetectionForm()
        {
            InitializeComponent();

            cmdSave.Click += cmdSave_Click;
            this.Paint += ConfigureForm_Paint;
        }

        void cmdSave_Click(object sender, EventArgs e)
        {
            if (lvResults.CheckedItems.Count == 0)
            {
                Dialog.Error("Please select at least one IP detection method", "Error");
                return;
            }

            int Index = 0;
            GetExternalIPv4Methods[] NewMethodOrder = new GetExternalIPv4Methods[lvResults.CheckedItems.Count];
            if (lvResults.Items[0].Checked) NewMethodOrder[Index++] = GetExternalIPv4Methods.UnicastAddress;
            if (lvResults.Items[1].Checked) NewMethodOrder[Index++] = GetExternalIPv4Methods.NatPmp;
            if (lvResults.Items[2].Checked) NewMethodOrder[Index++] = GetExternalIPv4Methods.Upnp;
            if (lvResults.Items[3].Checked) NewMethodOrder[Index++] = GetExternalIPv4Methods.Http8880;
            if (lvResults.Items[4].Checked) NewMethodOrder[Index++] = GetExternalIPv4Methods.Http80;
            Config.Default.GetExternalIPMethodOrder = NewMethodOrder;
            Config.Default.Save();

            DialogResult = DialogResult.OK;
        }

        void ConfigureForm_Paint(object sender, PaintEventArgs e)
        {
            if (_FirstPaint)
            {
                _FirstPaint = false;

                IPAddress ExternalIP = IPAddress.None;
                ListViewItem LVI = null;

                try
                {
                    LVI = new ListViewItem("UnicastAddress");
                    LVI.SubItems.Add("Checking...");
                    LVI.SubItems.Add("Checking...");
                    lvResults.Items.Add(LVI);
                    Application.DoEvents();
                    DateTime Start = DateTime.Now;
                    IPAddress IP = WebUtils.GetExternalIPv4ByUnicastAddress();
                    DateTime End = DateTime.Now;
                    if (ExternalIP == IPAddress.None) ExternalIP = IP;
                    LVI.SubItems[1].Text = IP.ToString().Replace("255.255.255.255", "Private network");
                    LVI.SubItems[2].Text = ((int)End.Subtract(Start).TotalMilliseconds).ToString() + " milliseconds";
                }
                catch (Exception ex)
                {
                    LVI.SubItems[1].Text = "Error: " + ex.Message;
                }

                try
                {
                    LVI = new ListViewItem("NAT-PMP");
                    LVI.SubItems.Add("Checking...");
                    LVI.SubItems.Add("Checking...");
                    lvResults.Items.Add(LVI);
                    Application.DoEvents();
                    DateTime Start = DateTime.Now;
                    IPAddress IP = WebUtils.GetExternalIPv4ByNatPmp();
                    DateTime End = DateTime.Now;
                    if (ExternalIP == IPAddress.None) ExternalIP = IP;
                    LVI.SubItems[1].Text = IP.ToString().Replace("255.255.255.255", "Not supported");
                    LVI.SubItems[2].Text = ((int)End.Subtract(Start).TotalMilliseconds).ToString() + " milliseconds";
                }
                catch (Exception ex)
                {
                    LVI.SubItems[1].Text = "Error: " + ex.Message;
                }

                try
                {
                    LVI = new ListViewItem("UPnP");
                    LVI.SubItems.Add("Checking...");
                    LVI.SubItems.Add("Checking...");
                    lvResults.Items.Add(LVI);
                    Application.DoEvents();
                    DateTime Start = DateTime.Now;
                    IPAddress IP = WebUtils.GetExternalIPv4ByUpnp();
                    DateTime End = DateTime.Now;
                    if (ExternalIP == IPAddress.None) ExternalIP = IP;
                    LVI.SubItems[1].Text = IP.ToString().Replace("255.255.255.255", "Not supported");
                    LVI.SubItems[2].Text = ((int)End.Subtract(Start).TotalMilliseconds).ToString() + " milliseconds";
                }
                catch (Exception ex)
                {
                    LVI.SubItems[1].Text = "Error: " + ex.Message;
                }

                try
                {
                    LVI = new ListViewItem("HTTP(8880)");
                    LVI.SubItems.Add("Checking...");
                    LVI.SubItems.Add("Checking...");
                    lvResults.Items.Add(LVI);
                    Application.DoEvents();
                    DateTime Start = DateTime.Now;
                    IPAddress IP = WebUtils.GetExternalIPv4ByHttp(8880);
                    DateTime End = DateTime.Now;
                    if (ExternalIP == IPAddress.None) ExternalIP = IP;
                    LVI.SubItems[1].Text = IP.ToString().Replace("255.255.255.255", "randm.ca unreachable");
                    LVI.SubItems[2].Text = ((int)End.Subtract(Start).TotalMilliseconds).ToString() + " milliseconds";
                }
                catch (Exception ex)
                {
                    LVI.SubItems[1].Text = "Error: " + ex.Message;
                }

                try
                {
                    LVI = new ListViewItem("HTTP(80)");
                    LVI.SubItems.Add("Checking...");
                    LVI.SubItems.Add("Checking...");
                    lvResults.Items.Add(LVI);
                    Application.DoEvents();
                    DateTime Start = DateTime.Now;
                    IPAddress IP = WebUtils.GetExternalIPv4ByHttp(80);
                    DateTime End = DateTime.Now;
                    if (ExternalIP == IPAddress.None) ExternalIP = IP;
                    LVI.SubItems[1].Text = IP.ToString().Replace("255.255.255.255", "randm.ca unreachable");
                    LVI.SubItems[2].Text = ((int)End.Subtract(Start).TotalMilliseconds).ToString() + " milliseconds";
                }
                catch (Exception ex)
                {
                    LVI.SubItems[1].Text = "Error: " + ex.Message;
                }

                for (int i = 0; i < lvResults.Items.Count; i++)
                {
                    lvResults.Items[i].Checked = (lvResults.Items[i].SubItems[1].Text == ExternalIP.ToString());
                }
            }
        }
    }
}
