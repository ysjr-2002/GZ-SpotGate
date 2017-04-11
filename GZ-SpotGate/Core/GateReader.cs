using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class GateReader : IReader
    {
        private bool _stop = false;
        private SerialPort _com = null;

        private Action<string> _callback;
        private const int baudRate = 9600;

        private const byte source_add = 0x01;
        /// <summary>
        /// 广播地址
        /// </summary>
        private const byte denst_add = 0x00;

        public bool OpenPort(string portname)
        {
            try
            {
                _com = new SerialPort(portname, baudRate, Parity.None, 8, StopBits.One);
                _com.Open();

                ThreadPool.QueueUserWorkItem(ReadComm);
                return true;
            }
            catch (Exception ex)
            {
                Log("二维码串口打开失败->" + ex.Message);
                return false;
            }
        }

        public void SetCallback(Action<string> callback)
        {
        }

        public bool ClosePort()
        {
            return true;
        }

        private void Log(string content)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
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

        public void ExitOpen(byte count = 0)
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 0x03, count, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        public void ExitClose()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, 00, 0x05, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);
        }

        public void AskGateState()
        {
            byte cid1 = 0x02;
            byte cid2 = 0x00;
            byte len = 0x08;
            byte data0 = 0xFF;
            byte[] buffer = new byte[] { 0xAA, 0x00, source_add, cid1, cid2, denst_add, len, data0, 0x00, 00, 00, 00, 00, 00, 00, 00 };
            var check = getCheckSum(buffer);
            buffer[buffer.Length - 1] = check;
            Send(buffer);

            Parse();
        }

        private void Parse()
        {
            var buffer = Read();
            if (buffer[3] != 0x12)
                return;

            var incount = new byte[] { 0, buffer[9], buffer[10], buffer[11] };
            var outcount = new byte[] { 0, buffer[12], buffer[13], buffer[14] };
        }

        private byte[] Read()
        {
            byte[] buffer = new byte[16];
            var pos = 0;
            var count = buffer.Length;
            while (true)
            {
                var read = _com.Read(buffer, pos, count);
                pos += read;
                count -= read;
                if (pos == count)
                {
                    break;
                }
            }
            return buffer;
        }

        private void Send(byte[] data)
        {
            if (_com != null && _com.IsOpen)
            {
                _com.Write(data, 0, data.Length);
            }
        }

        private byte getCheckSum(byte[] data)
        {
            int sum = 0;
            for (int i = 1; i < data.Length - 1; i++)
            {
                sum += data[i];
            }
            var checkSum = sum & 0x000000FF;
            return (byte)checkSum;
        }

        private void ReadComm(object obj)
        {
        }
    }
}
