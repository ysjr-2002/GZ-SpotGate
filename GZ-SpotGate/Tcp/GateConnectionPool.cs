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
        private static Dictionary<string, IGateTcpConnection> gateClientCollection = new Dictionary<string, IGateTcpConnection>();

        private const int Gate_In_Port = 1005;

        public static void EnterOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":" + Gate_In_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.EnterOpen(entercount);
            }
        }

        public static void ExitOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":" + Gate_In_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.ExitOpen(entercount);
            }
        }

        public static bool ContainsKey(string key)
        {
            return gateClientCollection.ContainsKey(key);
        }

        public static IGateTcpConnection GetGateTcp(string key)
        {
            return gateClientCollection[key];
        }

        public static void RemoveGateTcp(string key)
        {
            gateClientCollection.Remove(key);
        }

        public static void Add(string key, IGateTcpConnection gate)
        {
            gateClientCollection.Add(key, gate);
        }

        public static void Dispose()
        {
            foreach (var item in gateClientCollection)
            {
                item.Value.Stop();
            }
        }
    }
}
