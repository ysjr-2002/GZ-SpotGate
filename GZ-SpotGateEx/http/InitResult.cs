using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    class InitResult
    {
        public int code { get; set; }

        public string channelno { get; set; }

        public int inhold { get; set; }

        public int outhold { get; set; }

        public string shutdowntime { get; set; }

        public string datetime { get; set; }

        public string message { get; set; }

        public InitResult()
        {
            datetime = DateTime.Now.ToStandard();
            message = "";
        }
    }
}
