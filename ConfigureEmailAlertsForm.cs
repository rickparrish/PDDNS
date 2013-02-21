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
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public partial class ConfigureEmailAlertsForm : Form
    {
        public ConfigureEmailAlertsForm()
        {
            InitializeComponent();

            chkIPChange.Checked = Config.Default.EmailAlertIPChange;
            chkUpdateError.Checked = Config.Default.EmailAlertUpdateError;
            chkNewVersion.Checked = Config.Default.EmailAlertNewVersion;
            txtHostname.Text = Config.Default.SmtpHostname;
            txtPort.Text = Config.Default.SmtpPort.ToString();
            chkSsl.Checked = Config.Default.SmtpSsl;
            txtUsername.Text = Config.Default.SmtpUsername;
            txtPassword.SecureText = Config.Default.SmtpPassword.GetSecureText();
            txtFrom.Text = Config.Default.EmailFrom;
            txtTo.Text = Config.Default.EmailTo;
        }

        private void chkSsl_CheckedChanged(object sender, EventArgs e)
        {
            txtPort.Text = chkSsl.Checked ? "587" : "25";
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            if (chkIPChange.Checked || chkUpdateError.Checked || chkNewVersion.Checked)
            {
                if (!ValidateInputs()) return;
            }

            Config.Default.EmailAlertIPChange = chkIPChange.Checked;
            Config.Default.EmailAlertUpdateError = chkUpdateError.Checked;
            Config.Default.EmailAlertNewVersion = chkNewVersion.Checked;
            Config.Default.SmtpHostname = txtHostname.Text.Trim();
            Config.Default.SmtpPassword = txtPassword.SecureText.GetSecureText();
            Config.Default.SmtpPort = int.Parse(txtPort.Text.Trim());
            Config.Default.SmtpSsl = chkSsl.Checked;
            Config.Default.SmtpUsername = txtUsername.Text.Trim();
            Config.Default.EmailFrom = txtFrom.Text.Trim();
            Config.Default.EmailTo = txtTo.Text.Trim();
            Config.Default.Save();

            DialogResult = DialogResult.OK;
        }

        private void cmdTest_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            try
            {
                WebUtils.Email(txtHostname.Text.Trim(), int.Parse(txtPort.Text.Trim()), new MailAddress(txtFrom.Text.Trim()), new MailAddress(txtTo.Text.Trim()), new MailAddress(txtFrom.Text.Trim()), "R&M PDDNS Test Message", "If you are reading this, then you've entered correct settings into PDDNS", false, txtUsername.Text.Trim(), txtPassword.SecureText, chkSsl.Checked);
                Dialog.Information("A test message has been sent to " + txtFrom.Text.Trim(), "Message sent");
            }
            catch (Exception ex)
            {
                Dialog.Error("An error occurred while sending your test message:\r\n\r\n" + ex.Message, "Error");
            }
        }

        private bool ValidateInputs()
        {
            if (!Dialog.ValidateIsNotEmpty(txtHostname)) return false;
            if (!Dialog.ValidateIsInRange(txtPort, 1, 65535)) return false;
            if (txtPort.Text.Trim() == "465")
            {
                if (Dialog.CancelOK("Port 465 is normally used for 'implicit' ssl, which is not supported by PDDNS\r\n\r\nIf your SMTP server supports 'explicit' ssl (using STARTTLS) on this port then click OK, otherwise please click Cancel and try port 587 instead.", "Confirm port") == DialogResult.Cancel) return false;
            }
            if (!Dialog.ValidateIsEmailAddress(txtFrom)) return false;
            if (!Dialog.ValidateIsEmailAddress(txtTo)) return false;
            return true;
        }
    }
}
