using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.WS
{
    class Udp
    {
        public static void SendToAndroid(string androidip, AndroidMessage message)
        {
            UdpClient udp = new UdpClient();
            try
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] data = Encoding.UTF8.GetBytes(json);
                IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse(androidip), 9872);
                udp.Send(data, data.Length, remoteIp);
            }
            finally
            {
                udp.Close();
            }
        }
    }
}
