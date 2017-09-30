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
        private const int Gate_Port = 1005;
        private static Dictionary<string, IGateTcpConnection> gateClientCollection = new Dictionary<string, IGateTcpConnection>();

        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="gatecomIp"></param>
        /// <param name="entercount"></param>
        public static void EnterOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                if (entercount == 1)
                    entercount = 0;
                tcp.EnterOpen(entercount);
            }
        }

        /// <summary>
        /// 出向开闸
        /// </summary>
        /// <param name="gatecomIp"></param>
        /// <param name="entercount"></param>
        public static void ExitOpen(string gatecomIp, byte entercount)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                if (entercount == 1)
                    entercount = 0;

                tcp.ExitOpen(entercount);
            }
        }

        public static void EnterHoldOpen(string gatecomIp)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.EnterHoldOpen();
            }
        }

        public static void ExitHoldOpen(string gatecomIp)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.ExitHoldOpen();
            }
        }

        public static void EnterClose(string gatecomIp)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.EnterClose();
            }
        }

        public static void ExitClose(string gatecomIp)
        {
            var key = string.Concat(gatecomIp, ":", Gate_Port);
            if (gateClientCollection.ContainsKey(key))
            {
                var tcp = gateClientCollection[key];
                tcp.ExitClose();
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

        /// <summary>
        /// 关闭闸机连接
        /// </summary>
        public static void Dispose()
        {
            foreach (var item in gateClientCollection)
            {
                item.Value.StopAsync();
            }
        }
    }
}
