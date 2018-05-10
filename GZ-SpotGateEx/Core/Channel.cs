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
        public string PadInIp
        {
            get { return this.GetValue(s => s.PadInIp); }
            set { this.SetValue(s => s.PadInIp, value); }
        }

        /// <summary>
        /// 平板出
        /// </summary>
        public string PadOutIp
        {
            get { return this.GetValue(s => s.PadOutIp); }
            set { this.SetValue(s => s.PadOutIp, value); }
        }

        /// <summary>
        /// 考拉入
        /// </summary>
        public string FaceInIp
        {
            get { return this.GetValue(s => s.FaceInIp); }
            set { this.SetValue(s => s.FaceInIp, value); }
        }

        /// <summary>
        /// 考拉出
        /// </summary>
        public string FaceOutIp
        {
            get { return this.GetValue(s => s.FaceOutIp); }
            set { this.SetValue(s => s.FaceOutIp, value); }
        }

        /// <summary>
        /// 摄像机入
        /// </summary>
        public string CameraInIp
        {
            get { return this.GetValue(s => s.CameraInIp); }
            set { this.SetValue(s => s.CameraInIp, value); }
        }

        /// <summary>
        /// 摄像机入
        /// </summary>
        public string CameraOutIp
        {
            get { return this.GetValue(s => s.CameraOutIp); }
            set { this.SetValue(s => s.CameraOutIp, value); }
        }

        /// <summary>
        /// 保持常开(入)
        /// </summary>
        public int InHold
        {
            get { return this.GetValue(s => s.InHold); }
            set { this.SetValue(s => s.InHold, value); }
        }

        /// <summary>
        /// 保持常开(出)
        /// </summary>
        public int OutHold
        {
            get { return this.GetValue(s => s.OutHold); }
            set { this.SetValue(s => s.OutHold, value); }
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

        public bool IsWsIn
        {
            get { return this.GetValue(s => s.IsWsIn); }
            set { this.SetValue(s => s.IsWsIn, value); }
        }

        public bool IsWsOut
        {
            get { return this.GetValue(s => s.IsWsOut); }
            set { this.SetValue(s => s.IsWsOut, value); }
        }
    }
}
