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

        public static void Parse(string xml)
        {
            xml = "<?xml version='1.0' encoding='utf-8' ?>" + xml;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var code = doc.SelectSingleNode("message/errorcode");
            var message = doc.SelectSingleNode("message/errmessage");
            var datetime = doc.SelectSingleNode("message/datetime");
            var nums = doc.SelectSingleNode("message/nums");
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
