using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    class GateUdpComServer
    {
        private IPEndPoint gateRemotePoint = null;
        private bool bInit = false;
        private const byte source_add = 0x01;
        private byte denst_add = 0x00;

        private int pre_in_count = 0;
        private int pre_out_count = 0;
        private UdpClient udp = null;
        private static readonly string TAG = "gate:";

        private readonly static Dictionary<int, string> gatestate = new Dictionary<int, string>()
        {
            { 0, "0---闸机已关闭(空闲)" },
            { 1, "1---闸机已关闭(火警)" },
            { 2, "2---闸机已关闭(掉电)" },
            { 3, "3---闸机已打开(空闲)" },
            { 4, "4---闸机已打开(火警)" },
            { 5, "5---闸机已打开(手动)" },
            { 6, "6---闸机已打开(进向)" },
            { 7, "7---闸机已打开(出向)" },
            { 8, "8---进向关闸中" },
            { 9, "9---出向关闸中" },
            { 0x0A, "0x0A---进向关闸中(防夹有人)" },
            { 0x0B, "0x0B---出向关闸中(防夹有人)" },
            { 0x0C, "0x0C---进向开闸中" },
            { 0x0D, "0x0D---出向开闸中" },
            { 0x0E, "0x0E---进向手推开闸中" },
            { 0x0F, "0x0F---出向手推开闸中" },
            { 0x5F, "0x5F---滞留告警" },
            { 0x60, "0x60---闯入告警" },
            { 0x61, "0x61---逆向告警" },
            { 0x62, "0x62---尾随告警" },
            { 0x63, "0x63---外部告警" },
            { 0xFF, "0xFF---开机自检中" },
        };

        public GateUdpComServer(Channel channel)
        {
            try
            {
                this.Channel = channel;
                var port = 0;
                port = 10000 + channel.no.ToInt32();
                udp = new UdpClient(port);
                gateRemotePoint = new IPEndPoint(IPAddress.Parse(channel.comserver), 1004);
                bInit = true;
                LogHelper.Log(TAG + channel.name + " start");
            }
            catch (Exception ex)
            {
                LogHelper.Log(TAG + "constructor error, deviceId:" + channel.no);
                LogHelper.Log(TAG + "constructor error:" + ex.Message);
            }
        }

        public Channel Channel
        {
            get;
            set;
        }

        public void Start()
        {
            if (!bInit)
                return;

            BeginReceive();
        }

        private void BeginReceive()
        {
            udp.BeginReceive(EndReceive, null);
        }

        private void EndReceive(IAsyncResult ir)
        {
            if (udp.Client != null)
            {
                IPEndPoint remote = null;
                try
                {
                    byte[] recBuffer = udp.EndReceive(ir, ref remote);
                    Parse(recBuffer, true);
                    BeginReceive();
                }
                catch
                {
                }
            }
        }

        public void Parse(byte[] buffer, bool fire)
        {
            if (buffer == null)
                return;

            var checksum = getCheckSum(buffer);
            if (/*buffer[3] != 0x12 ||*/ checksum != buffer.Last())
            {
                //LogHelper.Log("gate package error,len=" + buffer.Length);
                //LogHelper.Log("gate package error,hex=" + buffer.ToHex());
                return;
            }

            if (buffer.Length < 16)
            {
                //LogHelper.Log("gate package length error,len=" + buffer.Length);
                return;
            }

            if (buffer[7] == 0)
            {
                //空闲
                LogHelper.Log(TAG + gateRemotePoint + " 00 idle add:" + buffer[2]);
            }
            else
            {
                //非空闲
                LogHelper.Log(TAG + gateRemotePoint + " " + buffer[7].ToHex() + " recognize");
            }

            var state = gatestate[buffer[7]];

            var incountBytes = new byte[] { 0, buffer[9], buffer[10], buffer[11] };
            var outcountBytes = new byte[] { 0, buffer[12], buffer[13], buffer[14] };

            Array.Reverse(incountBytes);
            Array.Reverse(outcountBytes);

            var incount = BitConverter.ToInt32(incountBytes, 0);
            var outcount = BitConverter.ToInt32(outcountBytes, 0);

            LogHelper.Log(TAG + gateRemotePoint + $" prein:{pre_in_count} curin:{incount} preout:{pre_out_count} curout:{outcount}");

            if ((pre_in_count != incount && incount > 0) && fire)
            {
                pre_in_count = incount;

                _ = new Request().Calc(this.Channel.ChannelVirualIp, "Z");
            }
            if ((pre_out_count != outcount && outcount > 0) && fire)
            {
                //离开
                pre_out_count = outcount;

            }
        }

        Stack<string> stackInId = new Stack<string>();
        Stack<string> stackOutId = new Stack<string>();
        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="count">值为1时，进向保持</param>
        public bool EnterOpen(byte count)
        {
            LogHelper.Log(TAG + gateRemotePoint + " In open");
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            return Send(buffer, false, false);
        }

        /// <summary>
        /// 进向关闸
        /// </summary>
        public void EnterClose()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, 02, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer, false, false);
        }

        /// <summary>
        /// 出向开闸
        /// </summary>
        /// <param name="count"></param>
        /// <param name="busId"></param>
        /// <param name="openModel"></param>
        public bool ExitOpen(byte count)
        {
            LogHelper.Log(TAG + gateRemotePoint + " Out open");
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x00, 0x03, count, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            return Send(buffer, false, false);
        }

        /// <summary>
        /// 进向开闸保持
        /// </summary>
        public void EnterHoldOpen()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, 01, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer, false, false);
        }

        /// <summary>
        /// 出向开闸保持
        /// </summary>
        public void ExitHoldOpen()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x00, 0x04, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer, false, false);
        }

        /// <summary>
        /// 出向关闸
        /// </summary>
        public void ExitClose()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, 0x05, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer, false, false);
        }

        private bool Send(byte[] buffer, bool parse, bool fire = true)
        {
            if (buffer == null)
                return false;

            try
            {
                udp.Send(buffer, buffer.Length, gateRemotePoint);
                LogHelper.Log(TAG + gateRemotePoint.ToString() + " send:" + buffer.ToHex());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static byte getCheckSum(byte[] data)
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

        public void Stop()
        {
            bInit = false;
            if (udp != null)
            {
                udp.Close();
            }
        }
    }
}
