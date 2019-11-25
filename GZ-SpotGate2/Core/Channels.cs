using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GZSpotGate.Core
{
    internal class Channels
    {
        private static readonly ILog log = LogManager.GetLogger("Channels");
        static XElement xelement = null;
        static string filepath = "";
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
            filepath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "channels.xml");
            xelement = XElement.Load(filepath);
            foreach (var item in xelement.Elements("channel"))
            {
                var no = item.Element("no").Value;
                Channel cm = new Channel
                {
                    no = no,
                    name = item.Element("name").Value,
                    ChannelVirualIp = item.Element("channelvirtualip").Value,
                    comserver = item.Element("comserver").Value,
                    faceserver = item.Element("faceserver").Value,
                    camera = item.Element("camera").Value,
                    pad = item.Element("pad").Value,
                };
                ChannelList.Add(cm);
            }
        }

        public static void Save()
        {
            foreach (var item in xelement.Elements("channel"))
            {
                var no = item.Element("no").Value;
                var channel = ChannelList.Find(s => s.no == no);
                item.Element("name").Value = channel.name;
                item.Element("channelvirtualip").Value = channel.ChannelVirualIp;
                item.Element("faceserver").Value = channel.faceserver;
                item.Element("camera").Value = channel.camera;
                item.Element("pad").Value = channel.pad;
                item.Element("comserver").Value = channel.comserver;
            }
            xelement.Save(filepath);
        }

        public static string EValue(XElement e, string name)
        {
            try
            {
                return e.Element(name).Value;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
