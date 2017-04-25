using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    /// <summary>
    /// 串口服务器，接收11条通道的串口连接
    /// </summary>
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

        public void Start()
        {
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, _port));
            _tcpListener.Start();

            BeginAccept();
        }

        private void EndAccept(IAsyncResult ir)
        {
            TcpClient tcpClient = null;
            try
            {
                tcpClient = _tcpListener.EndAcceptTcpClient(ir);
            }
            catch (Exception)
            {
                return;
            }
            BeginAccept();

            var ep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            var rp = ep.Port;

            ITcpConnection connection = null;
            if (rp == 1001)
            {
                connection = new TcpConnection(ep, tcpClient);
            }
            else if (rp == 1002)
            {
                connection = new TcpIDConnection(ep, tcpClient);
            }
            else if (rp == 1003)
            {
                connection = new TcpConnection(ep, tcpClient);
            }
            else if (rp == 1004)
            {
                connection = new TcpIDConnection(ep, tcpClient);
            }

            var key = ep.ToString();
            log.Debug("端口连接->" + key);
            if (rp == 1005)
            {
                connection = new TcpGateConnection(ep, tcpClient);
                if (GateConnectionPool.ContainsKey(key))
                {
                    var old = GateConnectionPool.GetGateTcp(key);
                    GateConnectionPool.RemoveGateTcp(key);
                    old.StopAsync();
                }
                connection.SetCallback(AcceptData);
                connection.Start();
                GateConnectionPool.Add(key, (IGateTcpConnection)connection);
            }
            else
            {
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
        }

        private void BeginAccept()
        {
            _tcpListener.BeginAcceptTcpClient(EndAccept, null);
        }

        [Obsolete("同步接收方法")]
        private void SyncAccept()
        {
            while (!_stop)
            {
                try
                {
                    var tcpClient = _tcpListener.AcceptTcpClient();
                    var ep = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                    var ra = ep.Address.ToString();
                    var rp = ep.Port;

                    ITcpConnection connection = null;
                    if (rp == 1001)
                    {
                        connection = new TcpConnection(ep, tcpClient);
                    }
                    else if (rp == 1002)
                    {
                        connection = new TcpIDConnection(ep, tcpClient);
                    }
                    else if (rp == 1003)
                    {
                        connection = new TcpConnection(ep, tcpClient);
                    }
                    else if (rp == 1004)
                    {
                        connection = new TcpIDConnection(ep, tcpClient);
                    }

                    var key = ep.ToString();
                    log.Debug(key);

                    if (rp == 1005)
                    {
                        connection = new TcpGateConnection(ep, tcpClient);
                        if (GateConnectionPool.ContainsKey(key))
                        {
                            var old = GateConnectionPool.GetGateTcp(key);
                            old.Stop();
                            GateConnectionPool.RemoveGateTcp(key);
                        }
                        connection.SetCallback(AcceptData);
                        connection.Start();
                        GateConnectionPool.Add(key, (IGateTcpConnection)connection);
                    }
                    else
                    {
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
            _tcpListener?.Stop();
            _tcpListener = null;
            foreach (var item in clientCollection)
            {
                item.Value.Stop();
            }
        }
    }
}
