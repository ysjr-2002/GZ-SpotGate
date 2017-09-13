using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GZ_SpotGate.Core;
using log4net;
using System.Net;
using System.Threading;

namespace GZ_SpotGate.Tcp
{
    /// <summary>
    /// 闸机连接
    /// </summary>
    class TcpGateConnection : IGateTcpConnection
    {
        private TcpClient _tcp = null;
        private bool _running = false;
        private NetworkStream _nws = null;
        private IPEndPoint _ipEndPoint = null;
        private Action<DataEventArgs> _callback;
        private Thread _thread = null;
        private static readonly ILog log = LogManager.GetLogger("TcpGateConnection");

        private const byte source_add = 0x01;
        private const byte denst_add = 0x00;

        private int pre_in_count = 0;
        private int pre_out_count = 0;

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        public TcpClient Tcp
        {
            get
            {
                return _tcp;
            }
        }

        public TcpGateConnection(IPEndPoint endPoint, TcpClient tcp)
        {
            _ipEndPoint = endPoint;
            _tcp = tcp;
        }

        private void getPersonCount()
        {
            AskGateState();
            var buffer = Read();
            Parse(buffer, false);
        }

        public void SetCallback(Action<DataEventArgs> act)
        {
            _callback = act;
        }

        public void Start()
        {
            if (_running)
                return;

            _running = true;
            _nws = _tcp.GetStream();

            _thread = new Thread(Work);
            _thread.Start();
        }

        private void Work()
        {
            getPersonCount();
            while (_running)
            {
                try
                {
                    var buffer = Read();
                    if (buffer == null)
                    {
                        break;
                    }
                    Parse(buffer, true);
                }
                catch (Exception ex)
                {
                    log.Fatal("处理数据异常->" + ex.Message);
                }
            }

            _running = false;
            _tcp?.Close();
            _tcp = null;
        }

        private byte[] Read()
        {
            byte[] buffer = new byte[16];
            var pos = 0;
            var count = buffer.Length;
            while (true)
            {
                try
                {
                    var read = _nws.Read(buffer, pos, count);
                    pos += read;
                    count -= read;
                    if (pos == buffer.Length)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    log.Fatal("读取流异常->" + _ipEndPoint.Address.ToString() + " " + ex.Message);
                    return null;
                }
            }
            return buffer;
        }

        public void Parse(byte[] buffer, bool fire)
        {
            var checksum = getCheckSum(buffer);
            if (buffer[3] != 0x12 || checksum != buffer.Last())
                return;

            var incountBytes = new byte[] { 0, buffer[9], buffer[10], buffer[11] };
            var outcountBytes = new byte[] { 0, buffer[12], buffer[13], buffer[14] };

            Array.Reverse(incountBytes);
            Array.Reverse(outcountBytes);

            var incount = BitConverter.ToInt32(incountBytes, 0);
            var outcount = BitConverter.ToInt32(outcountBytes, 0);

            if (pre_in_count != incount && fire)
            {
                DataEventArgs arg = new DataEventArgs
                {
                    IPEndPoint = _ipEndPoint,
                    GateOpen = true,
                    PersonIn = true,
                };
                _callback.BeginInvoke(arg, null, null);
            }
            if (pre_out_count != outcount && fire)
            {
                DataEventArgs arg = new DataEventArgs
                {
                    IPEndPoint = _ipEndPoint,
                    GateOpen = true,
                    PersonIn = false,
                };
                _callback.BeginInvoke(arg, null, null);
            }
            pre_in_count = incount;
            pre_out_count = outcount;
        }

        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="count">值为1时，进向保持</param>
        public void EnterOpen(byte count = 0)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
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
            Send(buffer);
        }

        /// <summary>
        /// 出向开闸
        /// </summary>
        /// <param name="count"></param>
        public void ExitOpen(byte count = 0)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x00, 0x03, count, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        public void ExitHoldOpen()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x00, 0x04, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        /// <summary>
        /// 出向关闸
        /// </summary>
        public void ExitClose()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, 0x04, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        /// <summary>
        /// 查询状态
        /// </summary>
        public void AskGateState()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte data0 = 0xFF;
            //查询闸机状态不支持广播
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, 11, len, data0, 0x00, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        private void Send(byte[] buffer)
        {
            _nws.Write(buffer, 0, buffer.Length);
        }

        private static byte getCheckSum(byte[] data)
        {
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
            StopInternal();
        }

        public void StopAsync()
        {
            Action act = () => { StopInternal(); };
            act.BeginInvoke(null, null);
        }

        private void StopInternal()
        {
            _running = false;
            _tcp?.Close();
            _tcp = null;
            _thread.Join(100);
            _thread = null;
        }
    }
}
