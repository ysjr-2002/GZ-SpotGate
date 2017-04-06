using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
   internal class DataEventArgs : EventArgs
    {
        public string Ip { get; set; }

        public bool ICData { get; set; }

        public bool QRData { get; set; }

        public bool IDData { get; set; }

        public string Name { get; set; }

        public string Data { get; set; }
    }
}
