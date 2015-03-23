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
    public class UnsupportedProvider : Provider
    {
        public UnsupportedProvider(string name)
        {
            this.AddForm = null;
            this.EditForm = null;
            this.Name = name;
            this.Url = null;
        }

        public override void Update(HostConfig HC, IPAddress ipAddress)
        {
            HC.Disabled = true;
            HC.DisabledReason = "Unsupported provider: " + this.Name;
        }
    }
}
