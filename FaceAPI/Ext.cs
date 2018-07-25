using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace FaceAPI
{
    internal static class Ext
    {
        public static string LinkUrl(this Dictionary<string, string> param)
        {
            var sb = new StringBuilder();
            foreach (var item in param)
            {
                sb.Append(item.Key + "=" + item.Value.UrlEncode() + "&");
            }
            var url = sb.ToString();
            url = url.TrimEnd('&');
            return url;
        }

        public static T Deserialize<T>(this string input)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            return serializer.Deserialize<T>(input);
        }

        public static byte[] FileToByte(this string path)
        {
            if (!File.Exists(path))
                return new byte[] { };

            return File.ReadAllBytes(path);
        }

        public static string UrlEncode(this string content)
        {
            return HttpUtility.UrlEncode(content);
        }

        public static byte[] ToUTF8(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static long ToUnix(this DateTime dt)
        {
            var datetime = new DateTime(1970, 1, 1);
            datetime = TimeZone.CurrentTimeZone.ToLocalTime(datetime);
            var ts = (dt - datetime).TotalSeconds;
            return (long)ts;
        }
    }
}
