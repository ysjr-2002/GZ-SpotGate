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

        private List<ChannelController> _channels = new List<ChannelController>();
        public void Start()
        {
            _server = new ComServer(1111);
            _server.OnMessageInComming += _server_OnMessageInComming;
            _server.Start();
        }

        private void InitChannelController()
        {

        }

        private void _server_OnMessageInComming(object sender, DataEventArgs e)
        {

        }

        public void Dispose()
        {
            _server.Stop();
        }
    }
}
