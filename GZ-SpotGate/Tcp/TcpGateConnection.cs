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
    class TcpGateConnection : IGateTcpConnection
    {
        private TcpClient _tcp = null;
        private bool _running = false;
        private NetworkStream _nws = null;
        private IPEndPoint _ipEndPoint = null;
        private Action<DataEventArgs> _callback;
        private Thread _thread = null;
        private static readonly ILog log = LogManager.GetLogger("TcpConnection");

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

        public void SetCallback(Action<DataEventArgs> act)
        {
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
            while (_running)
            {
                try
                {
                    var buffer = Read();
                    Parse(buffer);
                }
                catch (Exception)
                {
                }
            }
        }

        private byte[] Read()
        {
            byte[] buffer = new byte[16];
            var pos = 0;
            var count = buffer.Length;
            while (true)
            {
                var read = _nws.Read(buffer, pos, count);
                pos += read;
                count -= read;
                if (pos == count)
                {
                    break;
                }
            }
            return buffer;
        }

        public void Parse(byte[] buffer)
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

            if (pre_in_count != incount)
            {
                DataEventArgs arg = new DataEventArgs
                {
                    IPEndPoint = _ipEndPoint,
                    GateOpen = true,
                    PersonIn = true,
                };
                _callback.BeginInvoke(arg, null, null);
            }
            if (pre_out_count != outcount)
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

        public void EnterOpen(byte count)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            _nws.Write(buffer, 0, buffer.Length);
        }

        public void ExitOpen(byte count)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x03, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
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
            _running = false;
            _nws.Close();
            _thread.Join(1000);
            _thread = null;
        }
    }
}
