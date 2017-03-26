using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class DataEventArgs : EventArgs
    {
        public string Ip { get; set; }

        public bool IsHeart { get; set; }

        public bool IsQrcode { get; set; }

        public string Data { get; set; }
    }
}
