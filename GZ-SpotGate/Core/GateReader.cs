﻿using System;
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

        private int pre_in_count = 0;
        private int pre_out_count = 0;

        public bool OpenPort(string portname)
        {
            try
            {
                _com = new SerialPort(portname, baudRate, Parity.None, 8, StopBits.One);
                _com.Open();

                //获取闸机人次信息
                getPersonCount();

                ThreadPool.QueueUserWorkItem(ReadComm);
                return true;
            }
            catch (Exception ex)
            {
                Log("二维码串口打开失败->" + ex.Message);
                return false;
            }
        }

        private void getPersonCount()
        {
            AskGateState();
            var buffer = Read();
            Parse(buffer, false);
        }

        public void SetCallback(Action<string> callback)
        {
            this._callback = callback;
        }

        public bool ClosePort()
        {
            _stop = true;
            if (_com != null && _com.IsOpen)
            {
                _com.Close();
            }
            return true;
        }

        private void Log(string content)
        {
            Console.WriteLine(content);
        }

        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="count">值为1时，是进向保持</param>
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
                _callback("入变化=>入次:" + incount + "  出次:" + outcount);
            }
            if (pre_out_count != outcount && fire)
            {
                _callback("出变化=>入次:" + incount + "  出次:" + outcount);
            }
            pre_in_count = incount;
            pre_out_count = outcount;
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
                    var read = _com.Read(buffer, pos, count);
                    pos += read;
                    count -= read;
                    if (pos == buffer.Length)
                    {
                        break;
                    }
                }
                catch (Exception)
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
                var sb = new StringBuilder();
                foreach (var b in data)
                {
                    sb.Append(b.ToString("X2") + " ");
                }
                Console.WriteLine(sb.ToString());
                _com.Write(data, 0, data.Length);
            }
        }

        public byte getCheckSum(byte[] data)
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
            while (!_stop)
            {
                var buffer = Read();
                Parse(buffer, true);
            }
        }
    }
}
