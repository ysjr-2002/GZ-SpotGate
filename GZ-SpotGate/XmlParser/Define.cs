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

        public static string GetCheckInXmlContent(IDType type, string uniqueId)
        {
            var idtype = (int)type;
            var content = string.Format(checkin_template, uniqueId, idtype);
            return content;
        }

        public static void ParseXmlContent(string xml, out string uniqueId, out string message, out string datetime, out string nums)
        {
            xml = "<?xml version='1.0' encoding='utf-8' ?>" + xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            uniqueId = doc.SelectSingleNode("message/errorcode")?.InnerText;
            message = doc.SelectSingleNode("message/errmessage")?.InnerText;
            datetime = doc.SelectSingleNode("message/datetime")?.InnerText;
            nums = doc.SelectSingleNode("message/nums")?.InnerText;
        }
    }

    /// <summary>
    /// 号码类型
    /// </summary>
    enum IDType : int
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
