using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    static class HttpConstrant
    {
        public const string suffix_init = "/api/v1/init/";
        public const string suffix_verify = "/api/v1/verify/";
        public const string suffix_calccount = "/api/v1/calccount/";
        public const string suffix_heartbeat = "/api/v1/heartbeat/";
        /// <summary>
        /// 参数url:ip=?
        /// </summary>
        public static string url_init = "http://{0}:10001" + suffix_init;
        /// <summary>
        /// 参数url:channelno=?&idtype=?&code=?
        /// idtype->
        /// 0:ic
        /// 1:身份证
        /// 2:人脸
        /// 3:二维码
        /// </summary>
        public static string url_verify = "http://{0}:10001" + suffix_verify;

        /// <summary>
        /// 参数url:channelno=?
        /// </summary>
        public static string url_calccount = "http://{0}:10001" + suffix_calccount;

        /// <summary>
        /// 参数url:channelno=?
        /// </summary>
        public static string url_heartbeat = "http://{0}:10001" + suffix_heartbeat;
    }
}
