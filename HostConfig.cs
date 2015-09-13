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
    public class HostConfig : ConfigHelper
    {
        public const string CLOUDFLARE_REC_ID = "rec_id";
        public const string CLOUDFLARE_ZONE_NAME = "zone_name";
        public const string POINT_RECORD_ID = "RecordId";
        public const string POINT_ZONE_ID = "ZoneId";

        public bool Disabled { get; set; }
        public string DisabledReason { get; set; }
        public string Hostname { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string LastUpdateIP { get; set; }
        public RMSecureString Password { get; set; }
        public ProviderName Provider { get; set; }
        public StringDictionary ProviderSpecificSettings { get; set; }
        public string Username { get; set; }

        public HostConfig(string hostname) : base()
        {
            Disabled = false;
            DisabledReason = "";
            Hostname = hostname;
            LastUpdateDate = DateTime.MinValue;
            LastUpdateIP = "";
            Password = "";
            Provider = ProviderName.None;
            ProviderSpecificSettings = new StringDictionary();
            Username = "";

            base.Load(hostname);
        }

        public new void Delete()
        {
            base.Delete();
        }

        public static string[] GetHostnames()
        {
            List<string> Result = new List<string>();

            using (IniFile Ini = new IniFile(Config.Default.FileName))
            {
                // Get section names (but not the CONFIGURATION section, since that's not a host)
                string[] Sections = Ini.ReadSections();
                foreach (string Section in Sections)
                {
                    if (Section.ToUpper() != "CONFIGURATION") Result.Add(Section);
                }
            }

            Result.Sort();
            return Result.ToArray();
        }

        public IPAddress[] GetRemoteIPs()
        {
            return Providers.GetProvider(Provider).GetRemoteIPs(this);
        }

        public new void Save()
        {
            base.Save();
        }

        public string Status
        {
            get
            {
                if (Disabled)
                {
                    return "Disabled: " + DisabledReason;
                }
                else
                {
                    return "Last Updated: " + (LastUpdateDate == DateTime.MinValue ? "Never" : LastUpdateDate.ToShortDateString() + " " + LastUpdateDate.ToLongTimeString());
                }
            }
        }
    }
}
