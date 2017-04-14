using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GZ_SpotGate.Core;
using log4net;
using System.Net;
using System.Threading;

namespace GZ_SpotGate.Tcp
{
    class TcpGateConnection : IGateTcpConnection
    {
        private TcpClient _tcp = null;
        private bool _running = false;
        private NetworkStream _nws = null;
        private IPEndPoint _ipEndPoint = null;
        private Action<DataEventArgs> _callback;
        private Thread _thread = null;
        private static readonly ILog log = LogManager.GetLogger("TcpConnection");

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        public TcpClient Tcp
        {
            get
            {
                return _tcp;
            }
        }

        public TcpGateConnection(IPEndPoint endPoint, TcpClient tcp)
        {
            _ipEndPoint = endPoint;
            _tcp = tcp;
        }

        public void SetCallback(Action<DataEventArgs> act)
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public void EnterOpen(int count)
        {
        }

        public void ExitOpen(int count)
        {
        }
    }
}
