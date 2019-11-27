﻿using GZSpotGate.WS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Pad
{
    class PadHelper
    {
        private const int port = 6000;
        /// <summary>
        /// 发送信息至pad
        /// </summary>
        /// <param name="padIp"></param>
        /// <param name="message"></param>
        public static void SendToPad(string padIp, AndroidMessage message)
        {
            if (message == null)
                return;

            var json = JsonConvert.SerializeObject(message);
            var buffers = json.ToUtf8();

            UdpClient udp = new UdpClient();

            IPAddress ipAddress = IPAddress.Parse(padIp);
            udp.Send(buffers, buffers.Length, new IPEndPoint(ipAddress, port));
        }
    }
}