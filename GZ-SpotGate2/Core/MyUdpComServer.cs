using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    class MyUdpComServer
    {
        private int _port = 0;
        private UdpClient _server = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        private const string qr_prefiex = "qr";
        private const string ic_prefiex = "ic";

        private bool init = false;

        public MyUdpComServer(int port)
        {
            _port = port;
            _server = new UdpClient(port);
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

                byte[] buffer = _server.EndReceive(ir, ref epSender);
                BeginReceive();

                if (buffer == null || buffer.Length < 2)
                {
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
                    return;
                }
                var data = new DataEventArgs
                {
                    readerIp = epSender.Address.ToString(),
                    Data = code,
                    ICData = ic,
                    QRData = qr
                };
                OnMessageInComming?.Invoke(null, data);
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
