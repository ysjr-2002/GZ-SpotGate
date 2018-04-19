using GZ_SpotGateEx.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GZ_SpotGateEx.Core
{
    internal class Channels
    {
        private static readonly ILog log = LogManager.GetLogger("Channels");
        static string filename = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "channels.xml");
        private Channels()
        {
        }

        static Channels()
        {
            ChannelList = new List<Channel>();
        }

        public static List<Channel> ChannelList
        {
            get;
            private set;
        }

        public static void Load()
        {
            var root = XElement.Load(filename);
            foreach (var item in root.Elements("channel"))
            {
                var no = item.Element("no").Value;
                if (item.SubElement("no").IsEmpty() ||
                    item.SubElement("channelvirtualip").IsEmpty() ||
                    item.SubElement("androidInip").IsEmpty() ||
                    item.SubElement("androidOutip").IsEmpty() ||
                    item.SubElement("faceInip").IsEmpty() ||
                    item.SubElement("faceOutip").IsEmpty() ||
                    item.SubElement("faceInCameraip").IsEmpty() ||
                    item.SubElement("faceOutCameraip").IsEmpty())
                {
                    log.DebugFormat("{0}通道信息配置不完整", no);
                    continue;
                }

                Channel cm = new Channel
                {
                    No = no,
                    Name = item.Element("name").Value,
                    ChannelVirualIp = item.Element("channelvirtualip").Value,
                    ClientIp = item.Element("clientip").Value,
                    PadInIp = item.Element("androidInip").Value,
                    PadOutIp = item.Element("androidOutip").Value,
                    FaceInIp = item.Element("faceInip").Value,
                    FaceOutIp = item.Element("faceOutip").Value,
                    CameraInIp = item.Element("faceInCameraip").Value,
                    CameraOutIp = item.Element("faceOutCameraip").Value,
                    IsEnable = item.Element("enable").Value.ToInt32() == 1,
                    HoldIn = EValue(item, "holdopen").ToInt32() == 1
                };
                ChannelList.Add(cm);
            }
        }

        public static string EValue(XElement e, string name)
        {
            try
            {
                return e.Element(name).Value;
            }
            catch
            {
                //MyConsole.Current.Log("读取项[" + name + "]错误");
                return string.Empty;
            }
        }
    }
}
