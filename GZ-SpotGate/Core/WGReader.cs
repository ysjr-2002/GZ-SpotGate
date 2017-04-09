using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BJ_Benz.Code
{
    class WGReader : IReader
    {
        private bool _stop = false;
        private SerialPort _serialPort = null;

        private Action<string> _callback;
        private const int baudRate = 9600;

        public bool OpenPort(string portname)
        {
            try
            {
                _serialPort = new SerialPort(portname, baudRate, Parity.None, 8, StopBits.One);
                _serialPort.Open();

                ThreadPool.QueueUserWorkItem(ReadComm);
                return true;
            }
            catch (Exception ex)
            {
                Log("二维码串口打开失败->" + ex.Message);
                return false;
            }
        }

        public void ReadComm(object obj)
        {
            while (!_stop)
            {
                try
                {
                    byte b = 0;
                    List<byte> buffer = new List<byte>();
                    while ((b = (byte)_serialPort.ReadByte()) > 0)
                    {
                        if (b == 13)
                        {
                            var array = buffer.ToArray();
                            var len = buffer.Count;
                            var code = "";
                            var prefix = Encoding.UTF8.GetString(array, 0, 2);
                            if (prefix == "qr")
                            {
                                code = Encoding.UTF8.GetString(array, 2, len - 2);
                            }
                            else
                            {
                                code = BitConverter.ToInt32(array, 2).ToString();
                            }
                            _callback?.BeginInvoke(code, null, null);
                            buffer.Clear();
                        }
                        else
                        {
                            buffer.Add(b);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("关闭串口->" + ex.Message);
                }
            }
        }

        private void Log(string log, params string[] p)
        {
            Console.WriteLine(log);
        }

        public bool ClosePort()
        {
            _stop = true;
            if (_serialPort != null)
                _serialPort.Close();

            return true;
        }

        public void SetCallback(Action<string> callback)
        {
            _callback = callback;
        }
    }
}
