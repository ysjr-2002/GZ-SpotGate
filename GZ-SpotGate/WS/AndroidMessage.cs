﻿using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.WS
{
    class AndroidMessage
    {
        public IDType CheckInType { get; set; }

        public bool Result { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
    }
}
