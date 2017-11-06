using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.WS
{
    /**
    * 命名空间: GZ_SpotGate.WS
    *
    * 功 能： N/A
    * 类 名： UdpClient
    *
    * Ver         变更日期                            负责人 
    * ───────────────────────────────────
    * V0.01        2017/11/6 18:17:55                             杨绍杰 
    *
    * Copyright (c) 2017 Harzone Corporation. All rights reserved.
    *┌──────────────────────────────────┐
    *│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
    *│　版权所有：华尊科股份有限公司 　　　　　　　　　　　　　　│
    *└──────────────────────────────────┘
*/
    class Udp
    {
        public static void send(string androidip, string json)
        {
            UdpClient udp = new UdpClient();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
            IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse(androidip), 9872);
            udp.Send(data, data.Length, remoteIp);
        }
    }
}
