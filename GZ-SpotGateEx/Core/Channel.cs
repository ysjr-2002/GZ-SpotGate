using Common.NotifyBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.Core
{
    class Channel : PropertyNotifyObject
    {
        public string No
        {
            get { return this.GetValue(s => s.No); }
            set { this.SetValue(s => s.No, value); }
        }

        public string Name
        {
            get { return this.GetValue(s => s.Name); }
            set { this.SetValue(s => s.Name, value); }
        }

        public string ClientIp
        {
            get { return this.GetValue(s => s.ClientIp); }
            set { this.SetValue(s => s.ClientIp, value); }
        }

        /// <summary>
        /// 通道虚拟Ip
        /// </summary>
        public string VirtualIp
        {
            get { return this.GetValue(s => s.VirtualIp); }
            set { this.SetValue(s => s.VirtualIp, value); }
        }
        /// <summary>
        /// 平板入
        /// </summary>
        public string PadIp
        {
            get { return this.GetValue(s => s.PadIp); }
            set { this.SetValue(s => s.PadIp, value); }
        }
       
        /// <summary>
        /// 考拉
        /// </summary>
        public string FaceIp
        {
            get { return this.GetValue(s => s.FaceIp); }
            set { this.SetValue(s => s.FaceIp, value); }
        }
        
        /// <summary>
        /// 摄像机
        /// </summary>
        public string CameraIp
        {
            get { return this.GetValue(s => s.CameraIp); }
            set { this.SetValue(s => s.CameraIp, value); }
        }

        /// <summary>
        /// 保持常开
        /// </summary>
        public string Inouttype
        {
            get { return this.GetValue(s => s.Inouttype); }
            set { this.SetValue(s => s.Inouttype, value); }
        }

        /// <summary>
        /// 保持常开
        /// </summary>
        public bool HoldOpen
        {
            get { return this.GetValue(s => s.HoldOpen); }
            set { this.SetValue(s => s.HoldOpen, value); }
        }

        /// <summary>
        /// 最近心跳
        /// </summary>
        public string LastHeartbeat
        {
            get { return this.GetValue(s => s.LastHeartbeat); }
            set { this.SetValue(s => s.LastHeartbeat, value); }
        }

        public bool IsTimeOut
        {
            get { return this.GetValue(s => s.IsTimeOut); }
            set { this.SetValue(s => s.IsTimeOut, value); }
        }
    }
}
