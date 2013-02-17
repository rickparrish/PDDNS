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
    public class DynProvider : Provider
    {
        public DynProvider(string name)
        {
            this.AddForm = typeof(AddEditDynForm).FullName;
            this.EditForm = typeof(AddEditDynForm).FullName;
            this.Name = name;
            this.Url = new Uri("http://www.dyn.com");
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            if (HC == null) throw new ArgumentNullException("HC");
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            using (RMWebClient WC = new RMWebClient())
            {
                WC.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(HC.Username + ":" + HC.Password.GetPlainText()));
                WC.UseDefaultCredentials = false;
                WC.UserAgent = ProcessUtils.CompanyName + " - " + ProcessUtils.ProductName + " - " + ProcessUtils.ProductVersion;

                string Url = "https://members.dyndns.org/nic/update";
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
                        case "badauth": HC.DisabledReason = "The username and password pair do not match a real user."; break;
                        case "!donator": HC.DisabledReason = "An option available only to credited users (such as offline URL) was specified, but the user is not a credited user."; break;
                        case "notfqdn": HC.DisabledReason = "The hostname specified is not a fully-qualified domain name (not in the form hostname.dyndns.org or domain.com)."; break;
                        case "nohost": HC.DisabledReason = "The hostname specified does not exist in this user account (or is not in the service specified in the system parameter)."; break;
                        case "numhost": HC.DisabledReason = "Too many hosts (more than 20) specified in an update. Also returned if trying to update a round robin (which is not allowed)."; break;
                        case "abuse": HC.DisabledReason = "The hostname specified is blocked for update abuse."; break;
                        case "badagent": HC.DisabledReason = "The user agent was not sent or HTTP method is not permitted (we recommend use of GET request method)."; break;
                        case "good 127.0.0.1": HC.DisabledReason = "Request was ignored because of agent that does not follow our specifications."; break;
                        case "dnserr":
                            Logging.instance.LogWarning("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): DNS error encountered on provider's side.  Will retry in 1 hour.");
                            return;
                        case "911":
                            Logging.instance.LogWarning("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): There is a problem or scheduled maintenance on the provider's side.  Will retry in 1 hour.");
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
