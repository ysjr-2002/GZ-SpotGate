using log4net;
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

        public string OpenGateUrl { get; set; }

        public string RebootGateUrl { get; set; }

        public string AutoRestartTime { get; set; }

        public string ShutdownTime { get; set; }

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
            OpenGateUrl = GetKey("clientGateUrl");
            RebootGateUrl = GetKey("rebootGateUrl");
            ShutdownTime = GetKey("gateshutdownTime");
        }

        public void Save()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["auto"].Value = AutoRun.ToString();
            cfg.AppSettings.Settings["checkInServerUrl"].Value = CheckInServerUrl;
            cfg.AppSettings.Settings["clientGateUrl"].Value = OpenGateUrl;
            cfg.AppSettings.Settings["rebootGateUrl"].Value = RebootGateUrl;
            cfg.AppSettings.Settings["gateshutdownTime"].Value = ShutdownTime;
            cfg.Save();
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
