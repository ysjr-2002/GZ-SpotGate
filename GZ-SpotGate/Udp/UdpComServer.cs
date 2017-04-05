using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    class UdpComServer
    {
        private int _port = 0;
        private bool _stop = false;
        private UdpClient _server = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        public UdpComServer(int port)
        {
            _port = port;
            _server = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void Start()
        {
            BeginReceive();
        }

        private void BeginReceive()
        {
            _server.BeginReceive(EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            try
            {
                IPEndPoint epSender = null;
                if (_server.Client != null)
                {
                    byte[] recBuffer = _server.EndReceive(ir, ref epSender);
                    BeginReceive();

                    var code = Encoding.UTF8.GetString(recBuffer);
                    DataEventArgs args = new DataEventArgs
                    {
                        Ip = epSender.Address.ToString(),
                        Data = code
                    };
                    OnMessageInComming?.Invoke(null, args);
                }
                else
                {
                }
            }
            catch (Exception)
            {
            }
        }

        public void Stop()
        {
            _stop = true;
            if (_server != null)
            {
                _server.Close();
            }
        }
    }
}
