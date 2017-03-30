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
        private ComServer _server = null;
        private const int LISTEN_PORT = 9876;
        private List<ChannelController> _channels = new List<ChannelController>();

        private ILog log = null;
        public MainController()
        {
            var name = this.GetType().FullName;
            log = log4net.LogManager.GetLogger(name);
        }

        public async void Start()
        {
            log.Debug("debug");
            log.Info("MainController start");
            log.Warn("warn");
            log.Error("error");
            log.Fatal("fatal");

            _server = new ComServer(LISTEN_PORT);
            _server.OnMessageInComming += _server_OnMessageInComming;
            _server.Start();

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

        private void _server_OnMessageInComming(object sender, DataEventArgs e)
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
                Task.Run(() => channlController.Work(CheckIntype.IC, e.Data));
        }

        public void Dispose()
        {
            _server.Stop();
            foreach (var channel in _channels)
            {
                channel.Stop();
            }
        }
    }
}
