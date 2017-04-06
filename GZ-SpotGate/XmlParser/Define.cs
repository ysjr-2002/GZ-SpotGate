using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GZ_SpotGate.XmlParser
{
    class Define
    {
        private const string checkin_template = "<checkin><uniqueid>{0}</uniqueid><idtype>{1}</idtype></checkin>";
        public static string GetCheckInContent(CheckIntype type, string code)
        {
            var idtype = (int)type;
            var content = string.Format(checkin_template, code, idtype);
            return content;
        }

        public static void Parse(string xml, out string code, out string message, out string datetime, out string nums)
        {
            xml = "<?xml version='1.0' encoding='utf-8' ?>" + xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            code = doc.SelectSingleNode("message/errorcode")?.InnerText;
            message = doc.SelectSingleNode("message/errmessage")?.InnerText;
            datetime = doc.SelectSingleNode("message/datetime")?.InnerText;
            nums = doc.SelectSingleNode("message/nums")?.InnerText;
        }
    }

    enum CheckIntype : int
    {
        /// <summary>
        /// 二维码
        /// </summary>
        BarCode = 1,
        /// <summary>
        /// 身份证
        /// </summary>
        ID = 2,
        /// <summary>
        /// IC卡(内部人员使用)
        /// </summary>
        IC = 3,
        /// <summary>
        /// 人脸
        /// </summary>
        Face = 4
    }
}
