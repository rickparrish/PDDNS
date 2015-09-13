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
    public partial class AddEditNoIPForm : Form
    {
        private string _OriginalHostname = "";

        public AddEditNoIPForm()
        {
            InitializeComponent();
        }

        public AddEditNoIPForm(string hostname)
        {
            InitializeComponent();

            this.Text = this.Text.Replace(" Add ", " Edit ");

            _OriginalHostname = hostname;
            
            HostConfig HC = new HostConfig(hostname);
            txtHostname.Text = HC.Hostname;
            txtUsername.Text = HC.Username;
            txtPassword.SecureText = HC.Password.GetSecureText();
        }

        void cmdSave_Click(object sender, EventArgs e)
        {
            if (!Dialog.ValidateIsNotEmpty(txtHostname)) return;
            if (!Dialog.ValidateIsNotEmpty(txtUsername)) return;
            if (!Dialog.ValidateIsNotEmpty(txtPassword)) return;

            if (txtHostname.Text.Trim().ToUpper() != _OriginalHostname)
            {
                using (IniFile Ini = new IniFile(Config.Default.FileName))
                {
                    Ini.EraseSection(_OriginalHostname);
                    Ini.Save();
                }
            }


            HostConfig HC = new HostConfig(txtHostname.Text.Trim());
            HC.LastUpdateDate = DateTime.MinValue;
            HC.Password = txtPassword.SecureText.GetSecureText();
            HC.Provider = ProviderName.NoIP;
            HC.Username = txtUsername.Text.Trim();
            if (HC.Disabled)
            {
                // Saving changes should reset the disabled state and last update date, so a new update can be attempted right away
                HC.Disabled = false;
            }
            HC.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
