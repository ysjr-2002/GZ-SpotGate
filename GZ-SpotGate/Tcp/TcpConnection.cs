using GZ_SpotGate.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    class TcpConnection
    {
        private string _ipAddress = "";
        private NetworkStream _nws = null;
        private byte[] _buffer = new byte[1024];
        private Action<DataEventArgs> _callback;

        public TcpConnection(string ip, TcpClient tcp)
        {
            _ipAddress = ip;
            _nws = tcp.GetStream();
        }

        public void Work(Action<DataEventArgs> callback)
        {
            _callback = callback;
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
                var code = Encoding.UTF8.GetString(_buffer, 0, len);
                var data = new DataEventArgs
                {
                    Ip = _ipAddress,
                    Data = code,
                };
                _callback.BeginInvoke(data, null, null);
            }
            catch
            {
            }
        }

        public void Stop()
        {
            _nws.Close();
        }
    }
}
