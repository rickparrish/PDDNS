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
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace RandM.PDDNS
{
    public partial class AddPointForm : Form
    {
        public AddPointForm()
        {
            InitializeComponent();

            txtAPIKey.Text = Config.Default.PointAPIKey.GetPlainText();
            txtEmailAddress.Text = Config.Default.PointEmailAddress;
        }

        void cmdRetrieve_Click(object sender, EventArgs e)
        {
            if (!Dialog.ValidateIsEmailAddress(txtEmailAddress)) return;
            if (!Dialog.ValidateIsNotEmpty(txtAPIKey)) return;

            lvZoneRecords.Items.Clear();

            using (RMWebClient WC = new RMWebClient())
            {
                WC.ContentType = "application/xml";
                WC.Headers[HttpRequestHeader.Authorization] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(txtEmailAddress.Text.Trim() + ":" + txtAPIKey.Text.Trim()));
                WC.Headers["Accept"] = "application/xml";
                WC.UseDefaultCredentials = false;

                // Get all zones
                try
                {
                    string ZonesXml = WC.DownloadString("https://pointhq.com/zones");
                    XmlSerializer XS = new XmlSerializer(typeof(Point.zones));
                    Point.zones Zones = null;
                    using (TextReader TR = new StringReader(ZonesXml))
                    {
                        Zones = (Point.zones)XS.Deserialize(TR);
                    }

                    // Now get all records for each zone
                    foreach (Point.zonesZone Zone in Zones.zone)
                    {
                        string ZoneRecordsXml = WC.DownloadString("https://pointhq.com/zones/" + Zone.id.ToString() + "/records");
                        XS = new XmlSerializer(typeof(Point.zonerecords));
                        Point.zonerecords ZoneRecords = null;
                        using (TextReader TR = new StringReader(ZoneRecordsXml))
                        {
                            ZoneRecords = (Point.zonerecords)XS.Deserialize(TR);
                        }

                        foreach (Point.zonerecordsZonerecord ZoneRecord in ZoneRecords.zonerecord)
                        {
                            if (ZoneRecord.recordtype.ToUpper() == "A")
                            {
                                ListViewItem LVI = new ListViewItem(ZoneRecord.name.TrimEnd('.'));
                                LVI.SubItems.Add(ZoneRecord.zoneid.ToString());
                                LVI.SubItems.Add(ZoneRecord.id.ToString());
                                lvZoneRecords.Items.Add(LVI);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.instance.LogException("Error retrieving Point zone records", ex);
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
                HC.Provider = ProviderName.Point;
                HC.ProviderSpecificSettings[HostConfig.POINT_ZONE_ID] = lvZoneRecords.CheckedItems[i].SubItems[1].Text.Trim();
                HC.ProviderSpecificSettings[HostConfig.POINT_RECORD_ID] = lvZoneRecords.CheckedItems[i].SubItems[2].Text.Trim();
                HC.Username = txtEmailAddress.Text.Trim();
                HC.Save();
            }

            if (chkSaveEmailAddressAndAPIKey.Checked)
            {
                Config.Default.PointAPIKey = txtAPIKey.Text;
                Config.Default.PointEmailAddress = txtEmailAddress.Text;
            }
            else
            {
                Config.Default.PointAPIKey = "";
                Config.Default.PointEmailAddress = "";
            }
            Config.Default.Save();

            DialogResult = DialogResult.OK;
        }
    }
}
