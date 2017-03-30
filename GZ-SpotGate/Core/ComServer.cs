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
        private UdpClient server = null;
        public event EventHandler<DataEventArgs> OnMessageInComming;

        public ComServer(int port)
        {
            this.port = port;
            server = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void Start()
        {
            server.BeginReceive(EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            var udp = ir.AsyncState as UdpClient;
            IPEndPoint epSender = null;
            if (server.Client != null)
            {
                byte[] data = server.EndReceive(ir, ref epSender);
                var txt = Encoding.UTF8.GetString(data);
                Console.WriteLine("data coming->" + txt + " " + epSender + " " + Thread.CurrentThread.IsThreadPoolThread + " " + Thread.CurrentThread.ManagedThreadId);
                server.BeginReceive(EndReceive, server);
            }
            else
            {
                Console.WriteLine("udp close");
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
