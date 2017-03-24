using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static byte[] ToData(this string content)
        {
            return System.Text.Encoding.UTF8.GetBytes(content);
        }
    }
}
