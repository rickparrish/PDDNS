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
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public partial class AddEditDtDNSForm : Form
    {
        private string _OriginalHostname = "";

        public AddEditDtDNSForm()
        {
            InitializeComponent();
        }

        public AddEditDtDNSForm(string hostname)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(hostname))
            {
                this.Text = this.Text.Replace(" Add ", " Edit ");

                _OriginalHostname = hostname.ToUpper();

                HostConfig HC = new HostConfig(hostname);
                txtHostname.Text = HC.Hostname;
                txtPassword.SecureText = HC.Password.GetSecureText();
            }
        }

        void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Dialog.ValidateIsNotEmpty(txtHostname)) return;
            if (!Dialog.ValidateIsNotEmpty(txtPassword)) return;

            if (txtHostname.Text.Trim().ToUpper() != _OriginalHostname) {
                using (IniFile Ini = new IniFile(Config.Default.FileName))
                {
                    Ini.EraseSection(_OriginalHostname);
                    Ini.Save();
                }
            }

            HostConfig HC = new HostConfig(txtHostname.Text.Trim());
            HC.Password = txtPassword.SecureText.GetSecureText();
            HC.Provider = ProviderName.DtDNS;
            HC.Username = HC.Hostname;
            if (HC.Disabled)
            {
                // Saving changes should reset the disabled state and last update date, so a new update can be attempted right away
                HC.Disabled = false;
                HC.LastUpdateDate = DateTime.MinValue;
            }
            HC.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
