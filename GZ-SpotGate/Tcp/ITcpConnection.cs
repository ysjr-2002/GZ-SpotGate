using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    internal interface ITcpConnection
    {
        TcpClient Tcp { get; }

        bool Running { get; }

        void Start();

        void SetCallback(Action<DataEventArgs> act);

        void Stop();
    }
}
