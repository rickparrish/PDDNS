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
    public class NoIPProvider : Provider
    {
        public NoIPProvider(string name)
        {
            this.AddForm = typeof(AddEditNoIPForm).FullName;
            this.EditForm = typeof(AddEditNoIPForm).FullName;
            this.Name = name;
            this.Url = new Uri("http://www.noip.com");
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            if (HC == null) throw new ArgumentNullException("HC");
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            using (RMWebClient WC = new RMWebClient())
            {
                WC.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(HC.Username + ":" + HC.Password.GetPlainText()));
                WC.UseDefaultCredentials = false;
                WC.UserAgent = ProcessUtils.CompanyName + " " + ProcessUtils.ProductName + "/" + ProcessUtils.ProductVersion + " pddns-noip@rickparrish.ca";

                string Url = "https://dynupdate.no-ip.com/nic/update";
                Url += "?hostname=" + Uri.EscapeDataString(HC.Hostname);
                Url += "&myip=" + ipAddress.ToString();

                string ResponseText = WC.DownloadString(Url).Trim().ToLower();
                if (ResponseText.Contains(ipAddress.ToString()))
                {
                    if (ResponseText.Contains("nochg"))
                    {
                        Logging.instance.LogWarning("Host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + ") successfully updated, but returned NOCHG response.  Too many of these could cause your account to be banned by the provider.");
                    }
                }
                else
                {
                    HC.Disabled = true;
                    switch (ResponseText)
                    {
                        case "nohost": HC.DisabledReason = "Hostname supplied does not exist under specified account"; break;
                        case "badauth": HC.DisabledReason = "Invalid username password combination"; break;
                        case "badagent": HC.DisabledReason = "Client disabled."; break;
                        case "!donator": HC.DisabledReason = "An update request was sent including a feature that is not available to that particular user such as offline options."; break;
                        case "abuse": HC.DisabledReason = "Username is blocked due to abuse. Either for not following our update specifications or disabled due to violation of the No-IP terms of service."; break;
                        case "911":
                            Logging.instance.LogWarning("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): A fatal error on the provider's side such as a database outage.  Will retry in 1 hour.");
                            return;
                        default: HC.DisabledReason = "An unknown response code was received: \"" + ResponseText + "\""; break;
                    }
                    HC.Save();

                    Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                }
            }
        }
    }
}
