using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GZ_SpotGate.Core
{
    internal class Channels
    {
        private static ILog log = LogManager.GetLogger("Channels");
        private Channels()
        {
        }

        static Channels()
        {
            ChannelList = new List<ChannelModel>();
        }

        public static List<ChannelModel> ChannelList
        {
            get;
            private set;
        }

        public static void Load()
        {
            var filename = "channels.xml";
            var root = XElement.Load(filename);
            foreach (var item in root.Elements("channel"))
            {
                var no = item.Element("no").Value;
                if (item.SubElement("no").IsEmpty() ||
                    item.SubElement("channelvirtualip").IsEmpty() ||
                    item.SubElement("comserverip").IsEmpty() ||
                    item.SubElement("androidInip").IsEmpty() ||
                    item.SubElement("androidOutip").IsEmpty() ||
                    item.SubElement("faceInip").IsEmpty() ||
                    item.SubElement("faceOutip").IsEmpty() ||
                    item.SubElement("faceInCameraip").IsEmpty() ||
                    item.SubElement("faceOutCameraip").IsEmpty() ||
                    item.SubElement("gatecomserverip").IsEmpty())
                {
                    log.DebugFormat("{0}通道信息配置不完整", no);
                    continue;
                }

                ChannelModel cm = new ChannelModel
                {
                    No = no,
                    ChannelVirualIp = item.Element("channelvirtualip").Value,
                    ComServerIp = item.Element("comserverip").Value,
                    AndroidInIp = item.Element("androidInip").Value,
                    AndroidOutIp = item.Element("androidOutip").Value,
                    FaceInIp = item.Element("faceInip").Value,
                    FaceOutIp = item.Element("faceOutip").Value,
                    FaceInCameraIp = item.Element("faceInCameraip").Value,
                    FaceOutCameraIp = item.Element("faceOutCameraip").Value,
                    GateComServerIp = item.Element("gatecomserverip").Value,
                };
                ChannelList.Add(cm);
            }
        }

        public static string EValue(XElement e, string name)
        {
            return e.Element(name).Value;
        }
    }
}
