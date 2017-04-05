using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    class MegviiGate
    {
        private const int PORT = 5000;
        private const string COMMAND_OPEN1 = "on1:01";
        private const string COMMAND_OPEN2 = "on2:01";
        private const string COMMAND_CLOSE = "off1";

        private UdpClient socket = null;

        public MegviiGate()
        {
            socket = new UdpClient();
        }

        public void In(string gateIp)
        {
            var buffer = GetOpenPackage(COMMAND_OPEN1);
            Send(gateIp, buffer);
        }

        public void Out(string gateIp)
        {
            var buffer = GetOpenPackage(COMMAND_OPEN2);
            Send(gateIp, buffer);
        }

        private void Send(string gateIp, byte[] buffer)
        {
            var ep = new IPEndPoint(IPAddress.Parse(gateIp), PORT);
            socket.Send(buffer, buffer.Length, ep);
        }

        private byte[] GetOpenPackage(string command)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(command);
            return buffer;
        }

        public void Dispose()
        {
            socket?.Close();
        }
    }
}
