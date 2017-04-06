using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
{
    class ChannelModel
    {
        public const int InReaderPort = 1001;

        public const int InIDReaderPort = 1002;

        public const int OutReaderPort = 1003;

        public const int OutIDReaderPort = 1004;
        /// <summary>
        /// 串口服务器Ip
        /// </summary>
        public string ComServerIp { get; set; }
        /// <summary>
        /// 平板入
        /// </summary>
        public string AndroidInIp { get; set; }
        /// <summary>
        /// 平板出
        /// </summary>
        public string AndroidOutIp { get; set; }
        /// <summary>
        /// 考拉入
        /// </summary>
        public string FaceInIp { get; set; }
        /// <summary>
        /// 卡拉出
        /// </summary>
        public string FaceOutIp { get; set; }
        /// <summary>
        /// 入摄像机
        /// </summary>
        public string FaceInCameraIp { get; set; }
        /// <summary>
        /// 出摄像机
        /// </summary>
        public string FaceOutCameraIp { get; set; }
        /// <summary>
        /// 继电器
        /// </summary>
        public string MegviiIP { get; set; }
    }
}
