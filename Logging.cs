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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RandM.PDDNS
{
    public class Logging
    {
        public static Logging instance = new Logging();

        private LogWindowForm _LogWindow = new LogWindowForm();

        ~Logging()
        {
            _LogWindow.Dispose();
        }

        public void LogDebug(string message)
        {
            _LogWindow.AddToLog("DEBUG: " + message, Color.Cyan);
        }

        public void LogError(string message)
        {
            _LogWindow.AddToLog("ERROR: " + message, Color.Red);
        }

        public void LogException(string message, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");

            _LogWindow.AddToLog("EXCEPTION: " + message + ": " + ex.ToString(), Color.Red);
        }

        public void LogMessage(string message)
        {
            _LogWindow.AddToLog(message, Color.LightGray);
        }

        public void LogWarning(string message)
        {
            _LogWindow.AddToLog("WARNING: " + message, Color.Yellow);
        }

        public DialogResult ShowDialog()
        {
            return _LogWindow.ShowDialog();
        }
    }
}
