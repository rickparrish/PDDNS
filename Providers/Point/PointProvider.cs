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
    public class PointProvider : Provider
    {
        public PointProvider(string name)
        {
            this.AddForm = typeof(AddPointForm).FullName;
            this.EditForm = typeof(EditPointForm).FullName;
            this.Name = name;
            this.Url = "http://www.pointhq.com";
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            using (RMWebClient WC = new RMWebClient())
            {
                WC.ContentType = "application/xml";
                WC.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(HC.Username + ":" + HC.Password.GetPlainText()));
                WC.Headers["Accept"] = "application/xml";
                WC.UseDefaultCredentials = false;

                try
                {
                    string Url = "https://pointhq.com";
                    Url += "/zones/" + HC.ProviderSpecificSettings[HostConfig.POINT_ZONE_ID];
                    Url += "/records/" + HC.ProviderSpecificSettings[HostConfig.POINT_RECORD_ID];

                    string ResponseText = WC.UploadString(Url, "PUT", "<zone-record><data>" + ipAddress.ToString() + "</data><ttl type=\"integer\">60</ttl></zone-record>").Trim();
                    if (!ResponseText.Contains(ipAddress.ToString()))
                    {
                        HC.Disabled = true;
                        HC.DisabledReason = "Reason not known.";
                        HC.Save();

                        Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                    }
                }
                catch (WebException wex)
                {
                    HC.Disabled = true;
                    switch (((HttpWebResponse)wex.Response).StatusCode)
                    {
                        case HttpStatusCode.Forbidden: HC.DisabledReason = "Current user does not have access to requested resource."; break;
                        case HttpStatusCode.NotFound: HC.DisabledReason = "Record not found."; break;
                        default: HC.DisabledReason = "An unknown response code was received: \"" + ((HttpWebResponse)wex.Response).StatusCode + "\""; break;
                    }
                    HC.Save();

                    Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                }
                catch (Exception ex)
                {
                    HC.Disabled = true;
                    HC.DisabledReason = "Unhandled exception (" + ex.Message + ")";
                    HC.Save();

                    Logging.instance.LogException("Unable to update host \"" + HC.Hostname + "\"", ex);
                }
            }
        }
    }
}
