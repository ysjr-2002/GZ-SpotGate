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
        private List<byte> _barcodeList = new List<byte>();
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
                byte b = 0;
                try
                {
                    while ((b = (byte)_serialPort.ReadByte()) > 0)
                    {
                        if (b == 13)
                        {
                            var barcode = Encoding.UTF8.GetString(_barcodeList.ToArray());
                            _callback?.BeginInvoke(barcode, null, null);
                            Console.WriteLine(barcode);
                            _barcodeList.Clear();
                        }
                        else
                        {
                            _barcodeList.Add(b);
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
            //LogHelper.Info(log, p);
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
