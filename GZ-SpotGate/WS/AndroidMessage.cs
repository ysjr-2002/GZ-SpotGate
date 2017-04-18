using GZ_SpotGate.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GZ_SpotGate.Core.ChannelController;

namespace GZ_SpotGate.WS
{
    class AndroidMessage
    {
        public IDType CheckInType { get; set; }

        public IntentType IntentType { get; set; }

        public bool Result { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Avatar { get; set; }

        public int Delay { get; set; }

        public int Code { get; set; }
    }
}
