using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGate.Tcp
{
    /// <summary>
    /// 身份证读取
    /// </summary>
    internal class TcpIDConnection : ITcpConnection
    {
        private bool _running = false;
        private Thread _thread = null;
        private TcpClient _tcp = null;
        private NetworkStream _nws = null;
        private Action<DataEventArgs> _callback;

        private const int CHECK_INTERVAL = 100;
        private const int READ_TIME_OUT = 5000;
        private static readonly byte[] prefix = new byte[] { 0xAA, 0xAA, 0xAA, 0x96, 0x69 };

        private static ILog log = LogManager.GetLogger("TcpIDConnection");

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

        private IPEndPoint _ipEndPoint;

        public TcpIDConnection(IPEndPoint endPoint, TcpClient tcp)
        {
            _ipEndPoint = endPoint;
            _tcp = tcp;
            _nws = tcp.GetStream();
            _nws.ReadTimeout = READ_TIME_OUT;
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
            try
            {
                _running = false;
                _nws?.Close();
                _nws = null;
                _tcp?.Close();
                _tcp = null;
                Thread.Sleep(50);
                _thread = null;
            }
            catch (Exception ex)
            {
            }
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
                if (find == 1)
                {
                    var select = SelectID();
                    if (select)
                    {
                        ReadMessage();
                    }
                }
                else if (find == 3 || find == 4)
                {
                    Stop();
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

        private int FindID()
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
            }
            catch (Exception ex)
            {
                log.Debug(_ipEndPoint + "发送寻卡指令异常->" + ex.Message);
                return 4;
            }
            try
            {
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
                    return 1;
                }
                else if (recBuffer[2] == 0x80)
                {
                    //寻找身份证失败
                    return 2;
                }
                else
                {
                    return 2;
                }
            }
            catch (System.IO.IOException ex)
            {
                //读取超时
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.GetType().FullName == "System.Net.Sockets.SocketException")
                    {
                        return 2;
                    }
                }
                return 3;
            }
            catch (Exception ex)
            {
                MyConsole.Current.Log("读取数据异常->" + ex.Message);
                return 3;
            }
        }

        private byte readtimeout()
        {
            return 1;
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
            try
            {
                _nws.Write(package.ToArray(), 0, package.Count);
            }
            catch (Exception ex)
            {
                return false;
            }

            try
            {
                byte b1 = (byte)_nws.ReadByte();
                byte b2 = (byte)_nws.ReadByte();
                byte b3 = (byte)_nws.ReadByte();
                byte b4 = (byte)_nws.ReadByte();
                if (b1 == 65 && b2 == 65 && b3 == 65 && b4 == 65)
                {
                    //心跳
                    return false;
                }
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
            catch (System.IO.IOException ex)
            {
                //读取超时
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.GetType().FullName == "System.Net.Sockets.SocketException")
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MyConsole.Current.Log("读取数据异常->" + ex.Message);
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
            try
            {
                _nws.Write(package.ToArray(), 0, package.Count);
            }
            catch
            {
                return;
            }

            try
            {
                byte b1 = (byte)_nws.ReadByte();
                byte b2 = (byte)_nws.ReadByte();
                byte b3 = (byte)_nws.ReadByte();
                byte b4 = (byte)_nws.ReadByte();
                byte b5 = (byte)_nws.ReadByte();

                byte b6 = (byte)_nws.ReadByte();
                byte b7 = (byte)_nws.ReadByte();

                var lenByte = new byte[] { b6, b7 };
                Array.Reverse(lenByte);
                var datalen = b6 * 256 + b7;
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
                            Name = array[0],
                            Data = idno,
                            IPEndPoint = _ipEndPoint,
                            IDData = true
                        };
                        _callback?.BeginInvoke(args, null, null);
                    }
                }
            }
            catch
            {
            }
        }

        private string getIDNO(string str)
        {
            //Regex regex = new Regex(@"\d+");
            //Regex regex = new Regex(@"\d+(X|x)?");
            Regex regex = new Regex(@"\d+(X)?", RegexOptions.IgnoreCase);
            Match match = regex.Match(str);
            if (regex.IsMatch(str))
            {
                var no = match.Value;
                return no;
            }
            else
                return string.Empty;
        }

        private void Test()
        {
            //var picBuffer = new byte[picLen];
            //Array.Copy(list.ToArray(), 7 + temp.Length, picBuffer, 0, picBuffer.Length);
            //MemoryStream ms = new MemoryStream(picBuffer);
            //var bp = Bitmap.FromStream(ms);
            //bp.Save("c:\\zp.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            //ms.Dispose();
            //bp.Dispose();
        }

        private void ResolveData(byte[] bytes)
        {
            byte[] photo_bs = new byte[1024];
            //prefix 5
            //len    2
            //status 3
            //txtlen 2
            //piclen 4
            //txtlen 256
            //总计   270
            Array.Copy(bytes, 270, photo_bs, 0, photo_bs.Length);
            //photo_bs = getPhotoBytes(photo_bs);
            byte[] name_bs = new byte[30];
            byte[] sex_bs = new byte[2];
            byte[] nation_bs = new byte[4];
            byte[] time_bs = new byte[16];
            byte[] address_bs = new byte[70];
            byte[] id_bs = new byte[36];
            byte[] office_bs = new byte[30];
            byte[] start_bs = new byte[16];
            byte[] stop_bs = new byte[16];
            byte[] newaddress_bs = new byte[36];

            //prefix 5
            //len    2
            //status 3
            //txtlen 2
            //piclen 4
            //总计   14
            Array.Copy(bytes, 14, name_bs, 0, 30);
            Array.Copy(bytes, 44, sex_bs, 0, 2);
            Array.Copy(bytes, 46, nation_bs, 0, 4);
            Array.Copy(bytes, 50, time_bs, 0, 16);
            Array.Copy(bytes, 66, address_bs, 0, 70);
            Array.Copy(bytes, 136, id_bs, 0, 36);
            Array.Copy(bytes, 172, office_bs, 0, 30);
            Array.Copy(bytes, 202, start_bs, 0, 16);
            Array.Copy(bytes, 218, stop_bs, 0, 16);
            Array.Copy(bytes, 234, newaddress_bs, 0, 36);
        }

        private string BufferToString(byte[] bytes)
        {
            var str = Encoding.UTF8.GetString(bytes);
            str = str.TrimEnd('\0');
            return str;
        }

        private byte[] getDataBytes(byte[] data)
        {
            byte b = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 2 == 0)
                {
                    b = data[i];
                    data[i] = data[i + 1];
                }
                else
                {
                    data[i] = b;
                }
            }
            return data;
        }
    }
}
