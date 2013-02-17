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
    public static class Providers
    {
        private static Dictionary<ProviderName, Provider> _Providers = new Dictionary<ProviderName, Provider>();

        public static Provider GetProvider(ProviderName name)
        {
            if (_Providers.ContainsKey(name)) return _Providers[name];
            return null;
        }

        public static Provider GetProviderByName(string name)
        {
            return GetProvider((ProviderName)Enum.Parse(typeof(ProviderName), name));
        }

        public static string[] GetProviderNames()
        {
            List<string> Result = new List<string>();
            foreach (KeyValuePair<ProviderName, Provider> KVP in _Providers)
            {
                Result.Add(KVP.Key.ToString());
            }
            return Result.ToArray();
        }

        public static void Init()
        {
            _Providers.Add(ProviderName.DtDNS, new DtDNSProvider(ProviderName.DtDNS.ToString()));
            _Providers.Add(ProviderName.Dyn, new DynProvider(ProviderName.Dyn.ToString()));
            _Providers.Add(ProviderName.NoIP, new NoIPProvider(ProviderName.NoIP.ToString()));
            _Providers.Add(ProviderName.Point, new PointProvider(ProviderName.Point.ToString()));
        }
    }
}
