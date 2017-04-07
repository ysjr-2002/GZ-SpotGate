using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class ZKIDReader : IReader
    {
        private static readonly byte[] prefix = new byte[] { 0xAA, 0xAA, 0xAA, 0x96, 0x69 };
        private static readonly int checkInterval = 100;

        private SerialPort _port = null;
        private bool _open = false;

        public bool OpenPort(string portname)
        {
            _port = new SerialPort(portname, 115200, Parity.None, 8, StopBits.One);
            try
            {
                _port.Open();
                _open = true;
                Read();
            }
            catch
            {
                _open = false;
            }
            return _open;
        }


        private Action<string> _callback = null;

        public void SetCallback(Action<string> callback)
        {
            _callback = callback;
        }

        private void Read()
        {
            Task.Run(() =>
            {
                while (_open)
                {
                    var find = FindID();
                    if (find)
                    {
                        var select = SelectID();
                        if (select)
                        {
                            ReadMessage();
                        }
                    }
                    Thread.Sleep(checkInterval);
                }
            });
        }

        private byte getSumCheck(byte[] data)
        {
            byte sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum = (byte)(sum ^ data[i]);
            }
            return sum;
        }

        private bool FindID()
        {
            var len = new byte[] { 0x00, 0x03 };
            var cmd = new byte[] { 0x20, 0x01 };

            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd);

            var sum = getSumCheck(data.ToArray());

            var total = new List<byte>();
            total.AddRange(prefix);
            total.AddRange(data);
            total.Add(sum);

            _port.Write(total.ToArray(), 0, total.Count);

            byte b1 = (byte)_port.ReadByte();
            byte b2 = (byte)_port.ReadByte();
            byte b3 = (byte)_port.ReadByte();
            byte b4 = (byte)_port.ReadByte();
            byte b5 = (byte)_port.ReadByte();

            byte b6 = (byte)_port.ReadByte();
            byte b7 = (byte)_port.ReadByte();

            var i = 0;
            List<byte> list = new List<byte>();
            while (i < b7)
            {
                byte s1 = (byte)_port.ReadByte();
                list.Add(s1);
                i++;
            }

            if (list?[2] == 0x9f)
            {
                return true;
            }
            else if (list[2] == 0x80)
            {
                //寻找身份证失败
                return false;
            }
            else
            {
                return false;
            }
        }

        private string h(byte b)
        {
            return b.ToString("X2");
        }

        private bool SelectID()
        {
            var len = new byte[] { 0x00, 0x03 };
            var cmd = new byte[] { 0x20, 0x02 };

            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd);

            var sum = getSumCheck(data.ToArray());

            var total = new List<byte>();
            total.AddRange(prefix);
            total.AddRange(data);
            total.Add(sum);

            _port.Write(total.ToArray(), 0, total.Count);

            byte b1 = (byte)_port.ReadByte();
            byte b2 = (byte)_port.ReadByte();
            byte b3 = (byte)_port.ReadByte();
            byte b4 = (byte)_port.ReadByte();
            byte b5 = (byte)_port.ReadByte();

            byte b6 = (byte)_port.ReadByte();
            byte b7 = (byte)_port.ReadByte();

            //Console.WriteLine(h(b1) + h(b2) + h(b3) + h(b4) + h(b5) + h(b6) + h(b7));

            var i = 0;
            List<byte> list = new List<byte>();
            while (i < b7)
            {
                byte s1 = (byte)_port.ReadByte();
                list.Add(s1);
                i++;
            }

            if (list[2] == 0x90)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ReadMessage()
        {
            var len = new byte[] { 0x00, 0x03 };
            var cmd = new byte[] { 0x30, 0x01 };

            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd);

            var sum = getSumCheck(data.ToArray());

            var total = new List<byte>();
            total.AddRange(prefix);
            total.AddRange(data);
            total.Add(sum);

            _port.Write(total.ToArray(), 0, total.Count);

            byte b1 = (byte)_port.ReadByte();
            byte b2 = (byte)_port.ReadByte();
            byte b3 = (byte)_port.ReadByte();
            byte b4 = (byte)_port.ReadByte();
            byte b5 = (byte)_port.ReadByte();

            byte b6 = (byte)_port.ReadByte();
            byte b7 = (byte)_port.ReadByte();

            //Console.WriteLine(h(b1) + h(b2) + h(b3) + h(b4) + h(b5) + h(b6) + h(b7));

            var lenByte = new byte[] { b6, b7 };
            Array.Reverse(lenByte);
            var datalen = b6 * 256 + b7; //BitConverter.ToInt16(lenByte, 0);
            var pos = 0;
            var dataBuffer = new byte[datalen];
            var count = datalen;
            while (true)
            {
                var read = _port.Read(dataBuffer, pos, count);
                pos += read;
                count -= read;
                if (pos == datalen)
                {
                    break;
                }
            }
            var list = dataBuffer.ToList();
            if (list[2] == 0x90)
            {
                var txtLen = list[3] * 256 + list[4];
                var picLen = list[5] * 256 + list[6];

                var temp = new byte[txtLen];
                Array.Copy(list.ToArray(), 7, temp, 0, temp.Length);
                var txt = Encoding.UTF8.GetString(temp);
                var un = Encoding.Unicode.GetString(temp);
                var gb = Encoding.Default.GetString(temp);

                var array = un.Split(' ');
                array = array.Where(s => s.Length > 0).ToArray();
                if (array.Length > 2)
                {
                    var idno = getIDNO(array[2]);
                    _callback?.BeginInvoke(idno, null, null);
                    Console.WriteLine(idno);
                }
                //Console.WriteLine(array.Length);
                //array.ToList().ForEach((s) =>
                //{
                //    Console.WriteLine(s);
                //});

                //var picBuffer = new byte[picLen];
                //Array.Copy(list.ToArray(), 7 + temp.Length, picBuffer, 0, picBuffer.Length);

                //MemoryStream ms = new MemoryStream(picBuffer);
                //var bp = Bitmap.FromStream(ms);
                //bp.Save("c:\\zp.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                //ms.Dispose();
                //bp.Dispose();
            }
        }

        private string getIDNO(string str)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(str);
            if (regex.IsMatch(str))
            {
                var no = match.Value;
                return no;
            }
            else
                return string.Empty;
        }

        public bool ClosePort()
        {
            _open = false;
            _port.Close();
            _port = null;
            return true;
        }
    }
}
