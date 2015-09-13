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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Linq;

namespace RandM.PDDNS
{
    public class CloudFlareProvider : Provider
    {
        public CloudFlareProvider(string name)
        {
            this.AddForm = typeof(AddCloudFlareForm).FullName;
            this.EditForm = typeof(EditCloudFlareForm).FullName;
            this.Name = name;
            this.Url = new Uri("http://www.cloudflare.com");
        }

        public override IPAddress[] GetRemoteIPs(HostConfig HC) {
            if (HC == null) throw new ArgumentNullException("HC");

            using (RMWebClient WC = new RMWebClient())
            {
                try
                {
                    NameValueCollection Params = new NameValueCollection();
                    Params.Add("a", "rec_load_all");
                    Params.Add("email", HC.Username);
                    Params.Add("tkn", HC.Password.GetPlainText());
                    Params.Add("z", HC.ProviderSpecificSettings[HostConfig.CLOUDFLARE_ZONE_NAME]);
                    Hashtable rec_load_all_response = (Hashtable)JSON.JsonDecode(Encoding.UTF8.GetString(WC.UploadValues("https://www.cloudflare.com/api_json.html", "POST", Params)));
                    if (rec_load_all_response["result"].ToString() == "success")
                    {
                        Hashtable Response = (Hashtable)rec_load_all_response["response"];
                        Hashtable Recs = (Hashtable)Response["recs"];
                        ArrayList Objs = (ArrayList)Recs["objs"];

                        foreach (Hashtable Obj in Objs)
                        {
                            if (Obj["name"].ToString().ToLower() == HC.Hostname.ToLower())
                            {
                                return new IPAddress[] {
                                    IPAddress.Parse(Obj["content"].ToString())
                                };
                            }
                        }
                    }
                    else
                    {
                        Logging.instance.LogError("Unable to get remote IPs \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + (rec_load_all_response["msg"] == null ? "Unknown reason" : rec_load_all_response["msg"].ToString()));
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("Unable to get remote IPs \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + ")", ex);
                }
            }

            return null;
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            if (HC == null) throw new ArgumentNullException("HC");
            if (ipAddress == null) throw new ArgumentNullException("ipAddress");

            using (RMWebClient WC = new RMWebClient())
            {
                try
                {
                    NameValueCollection Params = new NameValueCollection();
                    Params.Add("a", "rec_edit");
                    Params.Add("content", ipAddress.ToString());
                    Params.Add("email", HC.Username);
                    Params.Add("id", HC.ProviderSpecificSettings[HostConfig.CLOUDFLARE_REC_ID]);
                    int HostnamePeriodCount = HC.Hostname.Count(x => x == '.'); // TODO Do this for other providers too
                    switch (HostnamePeriodCount) {
                        case 0: throw new Exception("Hostname has 0 periods?");
                        case 1: Params.Add("name", HC.Hostname); break; // One period == full domain name (ie github.com)
                        default: Params.Add("name", string.Join(".", HC.Hostname.Split('.'), 0, HostnamePeriodCount - 1)); break; // More than one period == all but last element (ie www.github.com or a.b.c.d.github.com)
                    }
                    Params.Add("tkn", HC.Password.GetPlainText());
                    Params.Add("ttl", "1");
                    Params.Add("type", "A");
                    Params.Add("z", HC.ProviderSpecificSettings[HostConfig.CLOUDFLARE_ZONE_NAME]);
                    Hashtable rec_edit_response = (Hashtable)JSON.JsonDecode(Encoding.UTF8.GetString(WC.UploadValues("https://www.cloudflare.com/api_json.html", "POST", Params)));
                    if (rec_edit_response["result"].ToString() == "success")
                    {
                        Hashtable Response = (Hashtable)rec_edit_response["response"];
                        Hashtable Rec = (Hashtable)Response["rec"];
                        Hashtable Obj = (Hashtable)Rec["obj"];

                        if (!Obj["content"].ToString().Contains(ipAddress.ToString()))
                        {
                            HC.Disabled = true;
                            HC.DisabledReason = rec_edit_response["msg"].ToString();
                            HC.Save();

                            Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                        }
                    }
                    else 
                    {
                        HC.Disabled = true;
                        HC.DisabledReason = (rec_edit_response["msg"] == null) ? "Unknown reason" : rec_edit_response["msg"].ToString();
                        HC.Save();

                        Logging.instance.LogError("Unable to update host \"" + HC.Hostname + "\" (" + HC.Provider.ToString() + "): " + HC.DisabledReason);
                    }
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
