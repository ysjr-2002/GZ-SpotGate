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
        static XElement root = null;
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
            root = XElement.Load(filename);
            foreach (var item in root.Elements("channel"))
            {
                var no = item.Element("no").Value;
                if (no.IsEmpty() ||
                    item.SubElement("name").IsEmpty() ||
                    item.SubElement("channelvirtualip").IsEmpty() ||
                    item.SubElement("clientip").IsEmpty() ||
                    item.SubElement("androidInip").IsEmpty() ||
                    item.SubElement("androidOutip").IsEmpty() ||
                    item.SubElement("faceInip").IsEmpty() ||
                    item.SubElement("faceOutip").IsEmpty() ||
                    item.SubElement("faceInCameraip").IsEmpty())
                {
                    log.DebugFormat("{0}通道信息配置不完整", no);
                    continue;
                }

                Channel cm = new Channel
                {
                    No = no,
                    Name = item.Element("name").Value,
                    VirtualIp = item.Element("channelvirtualip").Value,
                    ClientIp = item.Element("clientip").Value,
                    PadInIp = item.Element("androidInip").Value,
                    PadOutIp = item.Element("androidOutip").Value,
                    FaceInIp = item.Element("faceInip").Value,
                    FaceOutIp = item.Element("faceOutip").Value,
                    CameraInIp = item.Element("faceInCameraip").Value,
                    CameraOutIp = item.Element("faceOutCameraip").Value,
                    InHold = EValue(item, "inhold").ToInt32(),
                    OutHold = EValue(item, "outhold").ToInt32(),
                    LastHeartbeat = DateTime.Now.ToStandard()
                };
                ChannelList.Add(cm);
            }
        }

        public static bool Save()
        {
            try
            {
                foreach (var item in root.Elements("channel"))
                {
                    var no = item.Element("no").Value;
                    var channel = Channels.ChannelList.Find(s => s.No == no);
                    item.Element("name").Value = channel.Name;
                    item.Element("inhold").Value = channel.InHold.ToString();
                    item.Element("outhold").Value = channel.OutHold.ToString();
                }
                root.Save(filename);
                return true;
            }
            catch (Exception ex)
            {
                MyLog.debug("保存配置->" + ex.StackTrace);
                return false;
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
                MyLog.debug("读取项[" + name + "]错误");
                return string.Empty;
            }
        }
    }
}
