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
        /// ip=?
        /// </summary>
        public static string url_init = "http://192.168.2.165:10000" + suffix_init;
        /// <summary>
        /// channelno=?&idtype=?&code=?
        /// </summary>
        public static string url_verify = "http://192.168.2.165:10000" + suffix_verify;

        /// <summary>
        /// channelno=?&inouttype=0
        /// </summary>
        public static string url_calccount = "http://192.168.2.165:10000" + suffix_calccount;

        /// <summary>
        /// channelno=?
        /// </summary>
        public static string url_heartbeat = "http://192.168.2.165:10000" + suffix_heartbeat;
    }
}
