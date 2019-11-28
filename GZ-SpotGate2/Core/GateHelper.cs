using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    public class GateHelper
    {
        private const byte source_add = 0x01;
        private const byte denst_add = 0x00;
        private const int port = 1004;

        private static UdpClient udp = new UdpClient();

        public static bool Open(string gateIp)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte count = 0x00;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;

            var remotePoint = new IPEndPoint(IPAddress.Parse(gateIp), port);
            udp.Client.ReceiveTimeout = 1 * 1000;
            udp.Send(buffer, buffer.Length, remotePoint);
            try
            {
                IPEndPoint epSender = null;
                var receive = udp.Receive(ref epSender);
                if (receive.Length != 16)
                    return false;

                var crc = getCheckSum(receive);
                if (crc == receive.Last())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("hz:gate open not back->" + ex.Message);
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    LogHelper.Log("开闸失败:" + gateIp);
                }
                return false;
            }
        }

        private static byte getCheckSum(byte[] data)
        {
            if (data == null)
                return 0;

            int sum = 0;
            for (int i = 1; i < data.Length - 1; i++)
            {
                sum += data[i];
            }
            var checkSum = sum & 0x000000FF;
            return (byte)checkSum;
        }
    }
}
