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
using System.Net;
using System.Text;

namespace RandM.PDDNS
{
    public class DtDNSProvider : Provider
    {
        public DtDNSProvider(string name)
        {
            this.AddForm = typeof(AddEditDtDNSForm).FullName;
            this.EditForm = typeof(AddEditDtDNSForm).FullName;
            this.Name = name;
            this.Url = new Uri("http://www.dtdns.com");
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            if (HC == null) throw new ArgumentNullException("HC");
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            using (RMWebClient WC = new RMWebClient())
            {
                string Url = "https://www.dtdns.com/api/autodns.cfm";
                Url += "?id=" + Uri.EscapeDataString(HC.Hostname);
                Url += "&pw=" + Uri.EscapeDataString(HC.Password.GetPlainText());
                Url += "&ip=" + ipAddress.ToString();
                Url += "&client=PDDNS";

                string ResponseText = WC.DownloadString(Url).Trim();
                if (!ResponseText.Contains(ipAddress.ToString()))
                {
                    HC.Disabled = true;
                    HC.DisabledReason = ResponseText;
                    HC.Save();

                    Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                }
            }
        }
    }
}
