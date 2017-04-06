using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using GZ_SpotGate.XmlParser;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    /// <summary>
    /// DEBUG
    /// INFO
    /// WARN
    /// ERROR
    /// FATAL
    /// </summary>
    class MainController
    {
        private readonly ILog log = LogManager.GetLogger("MainController");
        private readonly int TCP_COM_SERVER_PORT = 0;
        private readonly int WEB_SERVER_PORT = 0;

        private TcpComServer _tcpServer = null;
        private WebServer _webServer = null;
        private List<ChannelController> _channels = new List<ChannelController>();

        public MainController()
        {
            TCP_COM_SERVER_PORT = ConfigProfile.Current.TcpComListenPort;
            WEB_SERVER_PORT = ConfigProfile.Current.WebSocketListenPort;
        }

        public async void Start()
        {
            _tcpServer = new TcpComServer(TCP_COM_SERVER_PORT);
            _tcpServer.OnMessageInComming += ComServer_OnMessageInComming;
            _tcpServer.Start();

            _webServer = new WebServer(WEB_SERVER_PORT);
            _webServer.Start();

            ChannelController c = new ChannelController();
            _channels.Add(c);

            //c.Init(new ChannelModel
            //{
            //    FaceInIp = "192.168.1.110",
            //    FaceInCameraIp = "192.168.1.116",
            //    FaceOutIp = "192.168.1.111",
            //    FaceOutCameraIp = "192.168.1.116",
            //});
        }

        private List<ChannelModel> GetChannelModels()
        {
            List<ChannelModel> list = new List<ChannelModel>();
            return list;
        }

        private void ComServer_OnMessageInComming(object sender, DataEventArgs e)
        {
            var epSendIp = e.IPEndPoint.Address.ToString();
            ChannelController channlController = null;
            foreach (var channel in _channels)
            {
                if (channel.ContainIp(epSendIp))
                {
                    channlController = channel;
                    break;
                }
            }

            if (channlController != null)
            {
                channlController.Work(e);
            }
        }

        public void Dispose()
        {
            _tcpServer.Stop();
            _webServer.Stop();
            foreach (var channel in _channels)
            {
                channel.Stop();
            }
        }
    }
}
