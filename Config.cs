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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace RandM.PDDNS
{
    class Config : ConfigHelper
    {
        public bool EmailAlertIPChange { get; set; }
        public bool EmailAlertNewVersion { get; set; }
        public bool EmailAlertUpdateError { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public GetExternalIPv4Methods[] GetExternalIPMethodOrder { get; set; }
        public string LastIPAddress { get; set; }
        public bool LogToDisk { get; set; }
        public string SmtpHostname { get; set; }
        public RMSecureString SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpSsl { get; set; }
        public string SmtpUsername { get; set; }

        static public Config Default = new Config();

        public Config() : base()
        {
            LogToDisk = false;
            EmailAlertIPChange = true;
            EmailAlertNewVersion = true;
            EmailAlertUpdateError = true;
            EmailFrom = "";
            EmailTo = "";
            GetExternalIPMethodOrder = new GetExternalIPv4Methods[] { GetExternalIPv4Methods.UnicastAddress,
                                                                      GetExternalIPv4Methods.NatPmp,
                                                                      GetExternalIPv4Methods.Upnp,
                                                                      GetExternalIPv4Methods.Http81,
                                                                      GetExternalIPv4Methods.Http80 };
            LastIPAddress = IPAddress.None.ToString();
            SmtpHostname = "";
            SmtpPassword = new RMSecureString();
            SmtpPort = 587;
            SmtpSsl = true;
            SmtpUsername = "";
            if (!Load()) Save(); // Load (and save default config if not found)
        }

        private void CorrectEnumArrays()
        {
            // ConfigHelper returns enum arrays as int arrays, so we need to fix that
            GetExternalIPv4Methods[] NewMethodOrder = new GetExternalIPv4Methods[GetExternalIPMethodOrder.Length];
            for (int i = 0; i < GetExternalIPMethodOrder.Length; i++)
            {
                NewMethodOrder[i] = (GetExternalIPv4Methods)((int)GetExternalIPMethodOrder[i]);
            }
            GetExternalIPMethodOrder = NewMethodOrder;
        }

        private new bool Load()
        {
            bool Result = base.Load();
            CorrectEnumArrays();
            return Result;
        }

        public new void Save()
        {
            base.Save();
        }
    }
}
