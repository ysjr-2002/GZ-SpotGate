using GZ_SpotGateEx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    /// <summary>
    /// 上报通行人次
    /// </summary>
    class CalcQuery
    {
        public string channelno { get; set; }

        public InOutType inouttype { get; set; }
    }
}
