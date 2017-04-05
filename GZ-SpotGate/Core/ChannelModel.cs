using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    class ChannelModel
    {
        public string ComServerIp { get; set; }

        public byte InQRAdd { get; set; }

        public byte InIDAdd { get; set; }

        public byte OutQRAdd { get; set; }

        public byte OutIDAdd { get; set; }

        public string AndroidInIp { get; set; }

        public string AndroidOutIp { get; set; }

        public string FaceInIp { get; set; }

        public string FaceOutIp { get; set; }

        public string FaceInCameraIp { get; set; }

        public string FaceOutCameraIp { get; set; }

        public string MegviiIP { get; set; }
    }
}
