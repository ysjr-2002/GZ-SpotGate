using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    class TcpComServer
    {
        private int _port = 0;
        private bool _stop = false;
        private TcpListener _tcpListener = null;
        private Dictionary<string, TcpConnection> clientCollection = new Dictionary<string, TcpConnection>();
        public event EventHandler<DataEventArgs> OnMessageInComming;

        private static ILog log = LogManager.GetLogger("TcpComServer");

        public TcpComServer(int port)
        {
            _port = port;
        }

        public async void Start()
        {
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, _port));
            _tcpListener.Start();
            while (!_stop)
            {
                try
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    var remoteAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
                    TcpConnection tcpConnection = new TcpConnection(remoteAddress, tcpClient);
                    tcpConnection.Work(AcceptData);
                    clientCollection.Add(remoteAddress, tcpConnection);
                }
                catch (Exception ex)
                {
                    if (_stop)
                        log.Fatal("发送异常->" + ex.Message);
                    else
                        log.Debug("服务器关闭");
                }
            }
        }

        private void AcceptData(DataEventArgs data)
        {
            OnMessageInComming?.Invoke(null, data);
        }

        public void Stop()
        {
            _stop = true;
            _tcpListener.Stop();
            foreach (var item in clientCollection)
            {
                item.Value.Stop();
            }
        }
    }
}
