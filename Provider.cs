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
using RandM.RMLibUI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public abstract class Provider
    {
        public string Name;
        public string Url;

        protected string AddForm;
        protected string EditForm;

        public bool Add()
        {
            if (AddForm == null)
            {
                Dialog.Error("Sorry, you cannot add hostnames for that service provider", "Add not allowed");
                return false;
            }
            else
            {
                using (Form F = (Form)Activator.CreateInstance(Type.GetType(AddForm)))
                {
                    return (F.ShowDialog() == DialogResult.OK);
                }
            }
        }

        public bool Edit(string hostname)
        {
                if (EditForm == null)
                {
                    Dialog.Error("Sorry, you cannot edit hostnames for that service provider", "Edit not allowed");
                    return false;
                }
                else
                {
                    using (Form F = (Form)Activator.CreateInstance(Type.GetType(EditForm), hostname))
                    {
                        return (F.ShowDialog() == DialogResult.OK);
                    }
                }
        }

        public abstract void Update(HostConfig HC, IPAddress ipAddress);
    }
}
