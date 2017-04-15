using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    internal class DataEventArgs : EventArgs
    {
        public IPEndPoint IPEndPoint { get; set; }

        public bool ICData { get; set; }

        public bool QRData { get; set; }

        public bool IDData { get; set; }

        public bool FaceData { get; set; }

        public string Name { get; set; }

        public string Data { get; set; }

        public bool GateOpen { get; set; }

        public bool PersonIn { get; set; }
    }
}
