﻿using GZ_SpotGateEx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    class VerifyQuery
    {
        public string channelno { get; set; }

        public IDType idtype { get; set; }

        public InOutType inouttype { get; set; }

        public string code { get; set; }
    }
}
