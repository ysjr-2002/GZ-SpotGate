using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    static class GateConnectionPool
    {
        private static Dictionary<string, IGateTcpConnection> clientCollection = new Dictionary<string, IGateTcpConnection>();

        private const int Gate_In_Port = 1003;
        private const int Gate_Out_Port = 1006;

        public static void EnterOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":" + Gate_In_Port);
            if (clientCollection.ContainsKey(key))
            {
                var tcp = clientCollection[key];
                tcp.EnterOpen(entercount);
            }
        }

        public static void ExitOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":" + Gate_Out_Port);
            if (clientCollection.ContainsKey(key))
            {
                var tcp = clientCollection[key];
                tcp.ExitOpen(entercount);
            }
        }

        public static bool ContainsKey(string key)
        {
            return clientCollection.ContainsKey(key);
        }

        public static IGateTcpConnection GetGateTcp(string key)
        {
            return clientCollection[key];
        }

        public static void RemoveGateTcp(string key)
        {
            clientCollection.Remove(key);
        }

        public static void Add(string key, IGateTcpConnection gate)
        {
            clientCollection.Add(key, gate);
        }
    }
}
