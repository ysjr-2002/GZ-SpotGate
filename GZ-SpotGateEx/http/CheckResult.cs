﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    class CheckResult
    {
        public int code { get; set; }

        public string message { get; set; }

        public int entrycount { get; set; }

        public CheckResult()
        {
            message = "";
            entrycount = 1;
        }
    }
}
