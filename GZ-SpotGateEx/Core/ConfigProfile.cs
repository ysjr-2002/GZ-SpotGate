﻿using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.Core
{
    class ConfigProfile
    {
        private static ConfigProfile _current = new ConfigProfile();

        public int AutoRun { get; set; }

        public string CheckInServerUrl { get; set; }

        public string ClientOpenGateUrl { get; set; }

        public string AutoRestartTime { get; set; }

        private ILog log = LogManager.GetLogger("ConfigProfile");

        private ConfigProfile()
        {
        }

        public static ConfigProfile Current
        {
            get
            {
                return _current;
            }
        }

        public void ReadConfig()
        {
            AutoRun = GetKey("auto").ToInt32();
            CheckInServerUrl = GetKey("checkInServerUrl");
            ClientOpenGateUrl = GetKey("clientGateUrl");
        }

        private string GetKey(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                var val = ConfigurationManager.AppSettings[key];
                return val;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
