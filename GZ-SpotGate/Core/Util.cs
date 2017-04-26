using log4net;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace GZ_SpotGate.Core
{
    static class Util
    {
        private static readonly ILog log = LogManager.GetLogger("Util");

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

        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static string SubElement(this XElement element, string name)
        {
            return element.Element(name).Value;
        }

        public static string ToJson(object obj)
        {
            if (obj == null)
                return string.Empty;

            JavaScriptSerializer js = new JavaScriptSerializer();
            var json = js.Serialize(obj);
            return json;
        }

        public static T Deserlizer<T>(this string content)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var entity = serializer.Deserialize<T>(content);
            return entity;
        }


        //64 SoftWare\Wow6432Node\Microsoft\Windows\CurrentVersion\\Run
        const string keyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        //[RegistryPermission(SecurityAction.PermitOnly, Read=keyName, Write = keyName)]
        public static bool RunWhenStart(bool started, string exeName, string path)
        {
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(keyName, true);//打开注册表子项
                if (key == null)
                {
                    //如果该项不存在，则创建该子项
                    key = Registry.LocalMachine.CreateSubKey(keyName);
                }
            }
            catch (Exception ex)
            {
                log.Debug("设置开机启动失败：" + ex.Message);
                return false;
            }
            if (started == true)
            {
                try
                {
                    key.SetValue(exeName, path);//设置为开机启动
                    key.Close();
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    key.DeleteValue(exeName);//取消开机启动
                    key.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
