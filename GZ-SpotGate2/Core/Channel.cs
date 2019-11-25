using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    class Channel : PropertyNotifyObject
    {
        public string no
        {
            get { return this.GetValue(s => s.no); }
            set { this.SetValue(s => s.no, value); }
        }

        public string name
        {
            get { return this.GetValue(s => s.name); }
            set { this.SetValue(s => s.name, value); }
        }

        public string ChannelVirualIp
        {
            get { return this.GetValue(s => s.ChannelVirualIp); }
            set { this.SetValue(s => s.ChannelVirualIp, value); }
        }

        public string comserver
        {
            get { return this.GetValue(s => s.comserver); }
            set { this.SetValue(s => s.comserver, value); }
        }

        public string faceserver
        {
            get { return this.GetValue(s => s.faceserver); }
            set { this.SetValue(s => s.faceserver, value); }
        }

        public string pad
        {
            get { return this.GetValue(s => s.pad); }
            set { this.SetValue(s => s.pad, value); }
        }

        public string camera
        {
            get { return this.GetValue(s => s.camera); }
            set { this.SetValue(s => s.camera, value); }
        }

        public bool wsok
        {
            get { return this.GetValue(s => s.wsok); }
            set { this.SetValue(s => s.wsok, value); }
        }
    }
}
