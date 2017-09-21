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
    /// 二合一设备连接
    /// </summary>
    class TcpConnection : ITcpConnection
    {
        private TcpClient _tcp = null;
        private bool _running = false;
        private NetworkStream _nws = null;
        private IPEndPoint _ipEndPoint = null;
        private Action<DataEventArgs> _callback;
        private Thread _thread = null;

        private const string qr_prefiex = "qr";
        private const string ic_prefiex = "ic";
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

        public TcpConnection(IPEndPoint endPoint, TcpClient tcp)
        {
            _ipEndPoint = endPoint;
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

            _running = true;
            _thread = new Thread(Work);
            _thread.Start();
        }

        private void Work()
        {
            while (_running)
            {
                try
                {
                    byte[] buffer = new byte[256];
                    var len = _nws.Read(buffer, 0, buffer.Length);
                    if (len > 0)
                    {
                        var data = new byte[len - 1];
                        Array.Copy(buffer, data, data.Length);
                        NotifySubscribe(data);
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }

            //客户端关闭
            Stop();
        }

        private void NotifySubscribe(byte[] buffer)
        {
            if (buffer.Length < 2)
                return;

            var len = buffer.Length;
            var code = "";
            var prefix = Encoding.UTF8.GetString(buffer, 0, 2);
            var ic = false;
            var qr = false;
            if (prefix == qr_prefiex)
            {
                //二维码数据
                qr = true;
                ic = false;
                code = Encoding.UTF8.GetString(buffer, 2, len - 2);
            }
            else if (prefix == ic_prefiex)
            {
                //IC卡
                qr = false;
                ic = true;
                code = BitConverter.ToInt32(buffer, 2).ToString();
            }
            else
            {
                return;
            }
            var data = new DataEventArgs
            {
                IPEndPoint = _ipEndPoint,
                Data = code,
                ICData = ic,
                QRData = qr
            };
            _callback?.BeginInvoke(data, null, null);
        }

        public void Stop()
        {
            try
            {
                _running = false;
                _nws?.Close();
                _tcp?.Close();
                _tcp = null;
                _thread?.Join(50);
                _thread = null;
            }
            catch (Exception ex)
            {
            }
        }
    }
}
