using GZ_SpotGate.Udp;
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
        private Dictionary<string, ITcpConnection> clientCollection = new Dictionary<string, ITcpConnection>();
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
                    var ep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                    var ra = ep.Address.ToString();
                    var rp = ep.Port;

                    ITcpConnection connection = null;
                    if (rp == 1001)
                    {
                        connection = new TcpConnection(ra, tcpClient);
                    }
                    else if (rp == 1002)
                    {
                        connection = new TcpIDConnection(ra, tcpClient);
                    }
                    else if (rp == 1003)
                    {
                        connection = new TcpConnection(ra, tcpClient);
                    }
                    else if (rp == 1004)
                    {
                        connection = new TcpIDConnection(ra, tcpClient);
                    }

                    var key = ep.ToString();
                    Console.WriteLine("来了一个" + key);
                    if (clientCollection.ContainsKey(key))
                    {
                        var old = clientCollection[key];
                        old.Stop();
                        clientCollection.Remove(key);
                    }

                    connection.SetCallback(AcceptData);
                    connection.Start();
                    clientCollection.Add(key, connection);
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
