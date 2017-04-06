using GZ_SpotGate.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    internal class TcpIDConnection : ITcpConnection
    {
        private string _ipAddress = "";
        private bool _running = false;
        private Thread _thread = null;
        private TcpClient _tcp = null;
        private NetworkStream _nws = null;
        private Action<DataEventArgs> _callback;

        private const int CHECK_INTERVAL = 100;
        private static readonly byte[] prefix = new byte[] { 0xAA, 0xAA, 0xAA, 0x96, 0x69 };

        public TcpClient Tcp
        {
            get
            {
                return _tcp;
            }
        }

        public bool Running
        {
            get
            {
                return _running;
            }
        }

        public TcpIDConnection(string ipAddress, TcpClient tcp)
        {
            _ipAddress = ipAddress;
            _tcp = tcp;
            _nws = tcp.GetStream();
        }

        public void Start()
        {
            if (_running)
                return;

            _running = true;
            _thread = new Thread(Work);
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _nws?.Close();
            _thread.Join(1000);
            _thread = null;
        }

        public void SetCallback(Action<DataEventArgs> callback)
        {
            _callback = callback;
        }

        private void Work()
        {
            while (_running)
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
                Thread.Sleep(CHECK_INTERVAL);
            }
        }

        private static byte GetXOR(byte[] data)
        {
            byte xor = 0;
            for (int i = 0; i < data.Length; i++)
            {
                xor = (byte)(xor ^ data[i]);
            }
            return xor;
        }

        private bool FindID()
        {
            var len = new byte[] { 0x00, 0x03 };
            var cmd = new byte[] { 0x20, 0x01 };

            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd);

            var xor = GetXOR(data.ToArray());
            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(xor);

            try
            {
                _nws.Write(package.ToArray(), 0, package.Count);

                byte b1 = (byte)_nws.ReadByte();
                byte b2 = (byte)_nws.ReadByte();
                byte b3 = (byte)_nws.ReadByte();
                byte b4 = (byte)_nws.ReadByte();
                byte b5 = (byte)_nws.ReadByte();

                byte b6 = (byte)_nws.ReadByte();
                byte b7 = (byte)_nws.ReadByte();

                var i = 0;
                List<byte> recBuffer = new List<byte>();
                while (i < b7)
                {
                    byte b = (byte)_nws.ReadByte();
                    recBuffer.Add(b);
                    i++;
                }

                if (recBuffer[2] == 0x9f)
                {
                    return true;
                }
                else if (recBuffer[2] == 0x80)
                {
                    //寻找身份证失败
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SelectID()
        {
            var len = new byte[] { 0x00, 0x03 };
            var cmd = new byte[] { 0x20, 0x02 };

            var data = new List<byte>();
            data.AddRange(len);
            data.AddRange(cmd);

            var sum = GetXOR(data.ToArray());

            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(sum);
            _nws.Write(package.ToArray(), 0, package.Count);

            byte b1 = (byte)_nws.ReadByte();
            byte b2 = (byte)_nws.ReadByte();
            byte b3 = (byte)_nws.ReadByte();
            byte b4 = (byte)_nws.ReadByte();
            byte b5 = (byte)_nws.ReadByte();

            byte b6 = (byte)_nws.ReadByte();
            byte b7 = (byte)_nws.ReadByte();

            var i = 0;
            List<byte> recBuffer = new List<byte>();
            while (i < b7)
            {
                byte b = (byte)_nws.ReadByte();
                recBuffer.Add(b);
                i++;
            }
            if (recBuffer[2] == 0x90)
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

            var xor = GetXOR(data.ToArray());
            var package = new List<byte>();
            package.AddRange(prefix);
            package.AddRange(data);
            package.Add(xor);
            _nws.Write(package.ToArray(), 0, package.Count);

            byte b1 = (byte)_nws.ReadByte();
            byte b2 = (byte)_nws.ReadByte();
            byte b3 = (byte)_nws.ReadByte();
            byte b4 = (byte)_nws.ReadByte();
            byte b5 = (byte)_nws.ReadByte();

            byte b6 = (byte)_nws.ReadByte();
            byte b7 = (byte)_nws.ReadByte();

            var lenByte = new byte[] { b6, b7 };
            Array.Reverse(lenByte);
            var datalen = b6 * 256 + b7; //BitConverter.ToInt16(lenByte, 0);
            var pos = 0;
            var recBuffer = new byte[datalen];
            var size = datalen;
            while (true)
            {
                var read = _nws.Read(recBuffer, pos, size);
                pos += read;
                size -= read;
                if (pos == datalen)
                {
                    break;
                }
            }
            var list = recBuffer.ToList();
            if (list[2] == 0x90)
            {
                var txtLen = list[3] * 256 + list[4];
                var picLen = list[5] * 256 + list[6];

                var temp = new byte[txtLen];
                //数据区从第7为开始
                Array.Copy(list.ToArray(), 7, temp, 0, temp.Length);
                var txt = Encoding.UTF8.GetString(temp);
                var un = Encoding.Unicode.GetString(temp);
                var gb = Encoding.Default.GetString(temp);

                var array = un.Split(' ');
                array = array.Where(s => s.Length > 0).ToArray();
                if (array.Length > 2)
                {
                    var idno = getIDNO(array[2]);
                    DataEventArgs args = new DataEventArgs
                    {
                        Name = "",
                        Data = idno,
                        Ip = _ipAddress,
                        IDData = true
                    };
                    _callback?.BeginInvoke(args, null, null);
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
    }
}
