using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    /// <summary>
    /// 二维码服务
    /// </summary>
    class QRUdpComServer
    {
        private readonly int port = 9876;
        private readonly UdpClient server = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        private const string qr_prefiex = "qr";
        private const string ic_prefiex = "ic";

        public QRUdpComServer()
        {
            server = new UdpClient(this.port);
        }

        public void ReceiveAsync()
        {
            BeginReceive();
        }

        private void BeginReceive()
        {
            server?.BeginReceive(EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            try
            {
                IPEndPoint epSender = null;
                byte[] buffer = server.EndReceive(ir, ref epSender);
                BeginReceive();
                //if (buffer == null || buffer.Length < 2)
                //{
                //    return;
                //}
                //var len = buffer.Length;
                //var code = Encoding.UTF8.GetString(buffer);
                //code = code.Replace('\r', ' ').Replace('\n', ' ').Trim();
                //var prefix = code.Substring(0, 2);
                //code = code.Substring(2);
                //var ic = false;
                //var qr = false;
                //if (prefix == qr_prefiex)
                //{
                //    //二维码数据
                //    qr = true;
                //    ic = false;
                //}
                //else if (prefix == ic_prefiex)
                //{
                //    //IC卡
                //    qr = false;
                //    ic = true;
                //}
                //else
                //{
                //    return;
                //}

                var len = buffer.Length;
                var code = Encoding.UTF8.GetString(buffer);
                code = code.Replace('\r', ' ').Replace('\n', ' ').Trim();
                var qr = true;
                var ic = false;
                var data = new DataEventArgs
                {
                    readerIp = epSender.Address.ToString(),
                    Data = code,
                    QRData = qr,
                    ICData = ic
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
            if (server != null)
            {
                server.Close();
            }
        }
    }
}
