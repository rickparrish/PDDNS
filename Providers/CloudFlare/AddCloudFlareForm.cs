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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RandM.PDDNS
{
    public partial class AddCloudFlareForm : Form
    {
        public AddCloudFlareForm()
        {
            InitializeComponent();

            txtAPIKey.Text = Config.Default.CloudFlareAPIKey.GetPlainText();
            txtEmailAddress.Text = Config.Default.CloudFlareEmailAddress;
        }

        void cmdRetrieve_Click(object sender, EventArgs e)
        {
            if (!Dialog.ValidateIsEmailAddress(txtEmailAddress)) return;
            if (!Dialog.ValidateIsNotEmpty(txtAPIKey)) return;

            lvZoneRecords.Items.Clear();

            using (RMWebClient WC = new RMWebClient())
            {
                // Get all zones
                try
                {
                    NameValueCollection Params = new NameValueCollection();
                    Params.Add("a", "zone_load_multi");
                    Params.Add("email", txtEmailAddress.Text.Trim());
                    Params.Add("tkn", txtAPIKey.Text.Trim());
                    Hashtable zone_load_multi_response = (Hashtable)JSON.JsonDecode(Encoding.UTF8.GetString(WC.UploadValues("https://www.cloudflare.com/api_json.html", "POST", Params)));
                    if ((zone_load_multi_response["result"] != null) && (zone_load_multi_response["result"].ToString() == "success"))
                    {
                        Hashtable Response = (Hashtable)zone_load_multi_response["response"];
                        Hashtable Zones = (Hashtable)Response["zones"];
                        ArrayList Objs = (ArrayList)Zones["objs"];

                        // Now get all records for each zone
                        foreach (Hashtable Zone in Objs)
                        {
                            Params = new NameValueCollection();
                            Params.Add("a", "rec_load_all");
                            Params.Add("email", txtEmailAddress.Text.Trim());
                            Params.Add("tkn", txtAPIKey.Text.Trim());
                            Params.Add("z", Zone["zone_name"].ToString());
                            Hashtable rec_load_all_response = (Hashtable)JSON.JsonDecode(Encoding.UTF8.GetString(WC.UploadValues("https://www.cloudflare.com/api_json.html", "POST", Params)));
                            Response = (Hashtable)rec_load_all_response["response"];
                            Hashtable Recs = (Hashtable)Response["recs"];
                            Objs = (ArrayList)Recs["objs"];

                            foreach (Hashtable ZoneRecord in Objs)
                            {
                                if (ZoneRecord["type"].ToString() == "A")
                                {
                                    ListViewItem LVI = new ListViewItem(ZoneRecord["name"].ToString());
                                    LVI.SubItems.Add(ZoneRecord["zone_name"].ToString());
                                    LVI.SubItems.Add(ZoneRecord["rec_id"].ToString());
                                    lvZoneRecords.Items.Add(LVI);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (zone_load_multi_response["msg"] == null)
                        {
                            throw new Exception("Unknown error");
                        }
                        else
                        {
                            throw new Exception(zone_load_multi_response["msg"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("Error retrieving CloudFlare zone records", ex);
                    Dialog.Error("Error retrieving hostnames:\r\n\r\n" + ex.Message, "Error");
                }
            }
        }

        void cmdSave_Click(object sender, EventArgs e)
        {
            if (lvZoneRecords.CheckedItems.Count == 0)
            {
                Dialog.Error("Please select one or more hostnames you want to keep updated", "Error");
                return;
            }

            for (int i = 0; i < lvZoneRecords.CheckedItems.Count; i++)
            {
                HostConfig HC = new HostConfig(lvZoneRecords.CheckedItems[i].SubItems[0].Text.Trim());
                HC.Password = txtAPIKey.Text.Trim();
                HC.Provider = ProviderName.CloudFlare;
                HC.ProviderSpecificSettings[HostConfig.CLOUDFLARE_ZONE_NAME] = lvZoneRecords.CheckedItems[i].SubItems[1].Text.Trim();
                HC.ProviderSpecificSettings[HostConfig.CLOUDFLARE_REC_ID] = lvZoneRecords.CheckedItems[i].SubItems[2].Text.Trim();
                HC.Username = txtEmailAddress.Text.Trim();
                HC.Save();
            }

            if (chkSaveEmailAddressAndAPIKey.Checked)
            {
                Config.Default.CloudFlareAPIKey = txtAPIKey.Text;
                Config.Default.CloudFlareEmailAddress = txtEmailAddress.Text;
            }
            else
            {
                Config.Default.CloudFlareAPIKey = "";
                Config.Default.CloudFlareEmailAddress = "";
            }
            Config.Default.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
