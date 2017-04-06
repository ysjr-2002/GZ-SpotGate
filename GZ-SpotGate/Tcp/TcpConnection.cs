using GZ_SpotGate.Udp;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    class TcpConnection : ITcpConnection
    {
        private string _ipAddress = "";
        private TcpClient _tcp = null;
        private bool _running = false;
        private NetworkStream _nws = null;
        private byte[] _buffer = new byte[1024];
        private Action<DataEventArgs> _callback;

        private static readonly ILog log = LogManager.GetLogger("TcpConnection");

        public TcpClient Tcp
        {
            get
            {
                return _tcp;
            }
        }

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        public TcpConnection(string ipAddress, TcpClient tcp)
        {
            _ipAddress = ipAddress;
            _tcp = tcp;
            _nws = tcp.GetStream();
        }

        public void SetCallback(Action<DataEventArgs> callback)
        {
            _callback = callback;
        }

        public void Start()
        {
            if (_running)
                return;

            BeginRead();
        }

        private void BeginRead()
        {
            _nws.BeginRead(_buffer, 0, _buffer.Length, EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            var len = 0;
            try
            {
                len = _nws.EndRead(ir);
                if (len == 0)
                {
                    Stop();
                    return;
                }
                var code = Encoding.UTF8.GetString(_buffer, 0, len);
                var data = new DataEventArgs
                {
                    Ip = _ipAddress,
                    Data = code,
                };
                Console.WriteLine("data->" + code);
                if (code == "hello")
                {
                    Stop();
                    return;
                }
                BeginRead();
                _callback.BeginInvoke(data, null, null);
            }
            catch (Exception ex)
            {
                log.Debug("连接关闭->" + ex.Message);
            }
        }

        public void Stop()
        {
            _running = false;
            _nws.Close();
            _tcp.Close();
            _tcp = null;
        }
    }
}
