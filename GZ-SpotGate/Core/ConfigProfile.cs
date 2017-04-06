using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    class ConfigProfile
    {
        private static ConfigProfile _current = new ConfigProfile();

        public string CheckInServerUrl { get; set; }

        public int UdpComListenPort { get; set; }

        public int TcpComListenPort { get; set; }

        public int WebSocketListenPort { get; set; }

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
            CheckInServerUrl = GetKey("checkInServerUrl");
            TcpComListenPort = Int32.Parse(GetKey("tcpComListenPort"));
            WebSocketListenPort = Int32.Parse(GetKey("webSocketListenPort"));
        }

        private string GetKey(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                var val = ConfigurationManager.AppSettings[key];
                log.Debug(string.Format("参数[{0}]={1}", key, val));
                return val;
            }
            else
            {
                log.Debug(string.Format("参数[{0}]不存在", key));
                return string.Empty;
            }
        }
    }
}
