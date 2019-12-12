using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HXMansion.Http
{
    public class HttpResult
    {
        public int code { get; set; }
        public string msg { get; set; }

        public static byte[] GetOK()
        {
            var result = new HttpResult { code = 0, msg = "" };
            var json = JsonConvert.SerializeObject(result);
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        public static byte[] GetError(string msg)
        {
            var result = new HttpResult { code = 400, msg = msg };
            var json = JsonConvert.SerializeObject(result);
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        public static string GetErrorContent(string msg)
        {
            var result = new HttpResult { code = 400, msg = msg };
            var json = JsonConvert.SerializeObject(result);
            return json;
        }
    }
}
