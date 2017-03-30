using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class MainController
    {
        private ComServer _server = null;
        private const int LISTEN_PORT = 9876;
        private List<ChannelController> _channels = new List<ChannelController>();

        public async void Start()
        {
            _server = new ComServer(LISTEN_PORT);
            _server.OnMessageInComming += _server_OnMessageInComming;
            _server.Start();
        }

        private void InitChannels()
        {
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
