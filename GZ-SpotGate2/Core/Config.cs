using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    class Config
    {
        private static Config instance = new Config();

        public static Config Instance
        {
            get
            {
                return instance;
            }
        }

        public string Auto { get; set; }

        public string CheckInServerUrl { get; set; }
        public string PWServer { get; internal set; }
        public string FaceServer { get; internal set; }
        public int PadDelay { get; internal set; }

        public string LocalIp { get; set; }
        public string Account { get; set; }
        public string Pwd { get; set; }
        public int Interval { get; set; }

        public void Read()
        {
            Auto = GetKey("auto");
            PWServer = GetKey("pwserver");
            FaceServer = GetKey("faceserver");
            PadDelay = GetKey("paddelay").ToInt32();
            LocalIp = GetKey("localIp");
            Account = GetKey("account");
            Pwd = GetKey("pwd");
            Interval = GetKey("interval").ToInt32();
        }

        public void Save()
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfg.AppSettings.Settings["auto"].Value = Auto.ToString();
            cfg.AppSettings.Settings["pwserver"].Value = PWServer;
            cfg.AppSettings.Settings["faceserver"].Value = FaceServer;
            cfg.AppSettings.Settings["paddelay"].Value = PadDelay.ToString();
            cfg.AppSettings.Settings["localIp"].Value = LocalIp;
            cfg.AppSettings.Settings["account"].Value = Account;
            cfg.AppSettings.Settings["pwd"].Value = Pwd;
            cfg.AppSettings.Settings["interval"].Value = Interval.ToString();
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
