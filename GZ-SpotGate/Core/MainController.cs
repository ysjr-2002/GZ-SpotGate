using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using GZ_SpotGate.XmlParser;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
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
        private readonly int UDP_COM_SERVER_PORT = 0;
        private readonly int TCP_COM_SERVER_PORT = 0;
        private readonly int WEB_SERVER_PORT = 0;

        private UdpComServer _udpServer = null;
        private TcpComServer _tcpServer = null;
        private WebServer _webServer = null;
        private List<ChannelController> _channels = new List<ChannelController>();

        public MainController()
        {
            UDP_COM_SERVER_PORT = ConfigProfile.Current.UdpComListenPort;
            TCP_COM_SERVER_PORT = ConfigProfile.Current.TcpComListenPort;
            WEB_SERVER_PORT = ConfigProfile.Current.WebSocketListenPort;
        }

        public async void Start()
        {
            _udpServer = new UdpComServer(UDP_COM_SERVER_PORT);
            _udpServer.OnMessageInComming += ComServer_OnMessageInComming;
            _udpServer.Start();

            _tcpServer = new TcpComServer(TCP_COM_SERVER_PORT);
            _tcpServer.OnMessageInComming += ComServer_OnMessageInComming;
            _tcpServer.Start();

            _webServer = new WebServer(WEB_SERVER_PORT);
            _webServer.Start();

            ChannelController c = new ChannelController();
            _channels.Add(c);

            c.Init(new ChannelModel
            {
                FaceInIp = "192.168.1.110",
                FaceInCameraIp = "192.168.1.116",

                FaceOutIp = "192.168.1.111",
                FaceOutCameraIp = "192.168.1.116",
            });
        }

        private List<ChannelModel> GetChannelModels()
        {
            List<ChannelModel> list = new List<ChannelModel>();
            return list;
        }

        private void ComServer_OnMessageInComming(object sender, DataEventArgs e)
        {
            var epSendIp = e.Ip;
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
                CheckIntype inType = CheckIntype.BarCode;
                if (e.QRData)
                    inType = CheckIntype.BarCode;
                if (e.ICData)
                    inType = CheckIntype.IC;
                Task.Run(() => channlController.Work(inType, e.Data));
            }
        }

        public void Dispose()
        {
            _udpServer.Stop();
            _tcpServer.Stop();
            _webServer.Stop();
            foreach (var channel in _channels)
            {
                channel.Stop();
            }
        }
    }
}
