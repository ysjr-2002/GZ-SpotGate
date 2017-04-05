using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    public interface IReader
    {
        bool OpenPort(string portname);

        void SetCallback(Action<string> callback);

        bool ClosePort();
    }
}
