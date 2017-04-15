using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GZ_SpotGate.Core
{
    static class Util
    {
        public static string LinkUrl(this Dictionary<string, string> param)
        {
            var sb = new StringBuilder();
            foreach (var item in param)
            {
                sb.Append(item.Key + "=" + item.Value + "&");
            }
            var url = sb.ToString();
            url = url.TrimEnd('&');
            return url;
        }

        public static byte[] ToBuffer(this string content)
        {
            return Encoding.UTF8.GetBytes(content);
        }

        public static string ToJson(object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var json = js.Serialize(obj);
            return json;
        }

        public static string HMS(this DateTime dt)
        {
            return dt.ToString("HH:mm:ss");
        }

        public static int ToInt32(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            var n = 0;
            Int32.TryParse(str, out n);
            return n;
        }

        public static byte ToByte(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;

            byte n = 0;
            byte.TryParse(str, out n);
            return n;
        }

        public static T Deserlizer<T>(this string content)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var entity = serializer.Deserialize<T>(content);
            return entity;
        }
    }
}
