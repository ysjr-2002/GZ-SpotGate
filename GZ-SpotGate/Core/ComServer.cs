using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class ComServer
    {
        private int port = 0;
        private bool isRunning = true;
        private UdpClient server = null;
        private Thread workThread = null;
        private IPEndPoint remoteEndPoint = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        public ComServer(int port)
        {
            this.port = port;
            server = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void Start()
        {
            workThread = new Thread(Listen);
            workThread.Start();
            //LogHelper.Info("启动通讯服务->" + port);
        }

        private void Listen()
        {
            while (isRunning)
            {
                try
                {
                    var buffer = server.Receive(ref remoteEndPoint);
                    var remoteIp = remoteEndPoint.Address.ToString();
                    var data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    ThreadPool.QueueUserWorkItem((s) =>
                    {
                        if (OnMessageInComming != null)
                        {
                            OnMessageInComming(this,
                            new DataEventArgs
                            {
                                Ip = remoteIp,
                                IsHeart = data.Length < 10,
                                IsQrcode = data.Length > 10,
                                Data = data
                            });
                        }
                    });
                }
                catch
                {
                }
            }
        }

        public void Stop()
        {
            isRunning = false;
            if (server != null)
            {
                server.Close();
            }
            workThread?.Abort();
            workThread = null;
        }
    }
}
