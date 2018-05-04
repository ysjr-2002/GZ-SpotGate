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
        /// <summary>
        /// 通道编号
        /// </summary>
        public string channelno { get; set; }
        /// <summary>
        /// 1:常开 0:否
        /// </summary>
        public int inhold { get; set; }

        public int outhold { get; set; }
        /// <summary>
        /// true:启用 false:否
        /// </summary>
        public bool enableshutdown { get; set; }
        /// <summary>
        /// 关机时间
        /// </summary>
        public string shutdowntime { get; set; }
        /// <summary>
        /// 服务器时间
        /// </summary>
        public string datetime { get; set; }

        public string message { get; set; }

        public InitResult()
        {
            channelno = "";
            datetime = DateTime.Now.ToStandard();
            shutdowntime = "21:00:00";
            enableshutdown = false;
            message = "";
        }
    }
}
