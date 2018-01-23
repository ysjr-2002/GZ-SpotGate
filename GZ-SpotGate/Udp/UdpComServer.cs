using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    internal class UdpComServer
    {
        private int _port = 0;
        private UdpClient _server = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        private const string qr_prefiex = "qr";
        private const string ic_prefiex = "ic";

        private static readonly ILog log = LogManager.GetLogger("UdpComServer");

        private bool init = false;

        public UdpComServer(int port)
        {
            _port = port;
            _server = new UdpClient(port);
            MyConsole.Current.Log("Udp server start=" + port);
        }

        public void ReceiveAsync()
        {
            BeginReceive();
        }

        private void BeginReceive()
        {
            _server?.BeginReceive(EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            try
            {
                IPEndPoint epSender = null;
                if (_server?.Client != null)
                {
                    byte[] buffer = _server.EndReceive(ir, ref epSender);
                    BeginReceive();

                    if (buffer == null || buffer.Length < 2)
                    {
                        log.Error("无效Udp包数据");
                        return;
                    }
                    var len = buffer.Length;
                    var code = Encoding.UTF8.GetString(buffer);
                    code = code.Replace('\r', ' ').Replace('\n', ' ').Trim();
                    var prefix = code.Substring(0, 2);
                    code = code.Substring(2);
                    var ic = false;
                    var qr = false;
                    if (prefix == qr_prefiex)
                    {
                        //二维码数据
                        qr = true;
                        ic = false;
                    }
                    else if (prefix == ic_prefiex)
                    {
                        //IC卡
                        qr = false;
                        ic = true;
                    }
                    else
                    {
                        log.Error("非法二维码数据");
                        return;
                    }
                    var data = new DataEventArgs
                    {
                        IPEndPoint = epSender,
                        Data = code,
                        ICData = ic,
                        QRData = qr
                    };
                    OnMessageInComming?.Invoke(null, data);
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public void Stop()
        {
            if (_server != null)
            {
                _server.Close();
            }
        }
    }
}
