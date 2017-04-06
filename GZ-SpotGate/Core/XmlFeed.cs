using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GZ_SpotGate.Core
{
    public class feed
    {
        [XmlElement("errcode")]
        public message message { get; set; }
    }

    public class message
    {
        [XmlElement("errcode")]
        public int errcode { get; set; }
        [XmlElement("errmessage")]
        public string errmessage { get; set; }
        [XmlElement("datetime")]
        public string datetime { get; set; }
        [XmlElement("nums")]
        public int nums { get; set; }
    }
}
