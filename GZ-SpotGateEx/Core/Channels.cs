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
        static string filename = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "config.xml");
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
                    item.SubElement("virtualip").IsEmpty() ||
                    item.SubElement("padip").IsEmpty() ||
                    item.SubElement("clientip").IsEmpty() ||
                    item.SubElement("face").IsEmpty() ||
                    item.SubElement("camera").IsEmpty() ||
                    item.SubElement("holdopen").IsEmpty() ||
                    item.SubElement("inouttype").IsEmpty())
                {
                    log.DebugFormat("{0}通道信息配置不完整", no);
                    continue;
                }

                Channel cm = new Channel
                {
                    No = no,
                    Name = item.Element("name").Value,
                    VirtualIp = item.Element("virtualip").Value,
                    ClientIp = item.Element("clientip").Value,
                    PadIp = item.Element("padip").Value,
                    FaceIp = item.Element("face").Value,
                    CameraIp = item.Element("camera").Value,
                    HoldOpen = EValue(item, "holdopen").ToInt32() == 1,
                    Inouttype = item.Element("inouttype").Value,
                    LastHeartbeat = DateTime.Now.ToStandard()
                };
                ChannelList.Add(cm);
            }
        }

        public static void Save()
        {

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
