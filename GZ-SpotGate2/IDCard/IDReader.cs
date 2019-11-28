using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZSpotGate.IDCard
{
    /// <summary>
    /// 如果收不到数据，是主板设置没有主动上报数据
    /// </summary>
    class IDReader
    {
        private UdpClient udp;
        private bool bRun = false;
        private string remoteIp;
        private IPEndPoint remotePoint;
        private Action<IDModel> OnReadCallback;
        private Thread thread;
        private const int port = 1002;

        public IDReader(string remoteIp)
        {
            this.remoteIp = remoteIp;
            remotePoint = new IPEndPoint(IPAddress.Parse(remoteIp), port);
            udp = new UdpClient();
            udp.Client.ReceiveBufferSize = 4096;
            udp.Client.ReceiveTimeout = 1000;
        }

        public void SetDataCallback(Action<IDModel> callback)
        {
            this.OnReadCallback = callback;
        }

        public void Run()
        {
            thread = new Thread(Start);
            thread.Start();
        }

        private void Start()
        {
            bRun = true;
            var ret = false;
            while (bRun)
            {
                ret = FindID();
                if (ret)
                {
                    ret = SelectID();
                    if (ret)
                    {
                        ReadID();
                    }
                }
                Thread.Sleep(1 * 100);
            }
        }

        public void ReadID()
        {
            byte[] receive = null;
            try
            {
                var senddata = IDPackage.getReadIDPackage();
                udp.Send(senddata, senddata.Length, remotePoint);
                IPEndPoint epSender = null;
                var list = new List<byte>();
                var pack1 = udp.Receive(ref epSender);
                //串口服务器对包进行了限制单包只能1024字节
                var pack2 = udp.Receive(ref epSender);
                var a = pack1.Length + pack2.Length;
                list.AddRange(pack1);
                list.AddRange(pack2);
                receive = list.ToArray();
                //5+2
                Debug.WriteLine("hz:data len=" + pack1.Length + " " + pack2.Length);
                if (receive.Length >= 7)
                {
                    var hex = receive.ToHex();
                    var len = receive[5] * 256 + receive[6];
                    //去除最后效验
                    byte[] buffers = new byte[len - 1];
                    Array.Copy(receive, 7, buffers, 0, buffers.Length);
                    if (buffers[2] == 0x90)
                    {
                        //读取成功
                        var txtlen = buffers[3] * 256 + buffers[4];
                        var piclen = buffers[5] * 256 + buffers[6];

                        var idmsgbuffer = new byte[txtlen];
                        var picbuffer = new byte[piclen];
                        Array.Copy(buffers, 7, idmsgbuffer, 0, idmsgbuffer.Length);
                        Array.Copy(buffers, 7 + txtlen, picbuffer, 0, picbuffer.Length);

                        IDModel idmodel = new IDModel();
                        IDPackage.ParseMessage(idmsgbuffer, idmodel);
                        IDPackage.SetPicBuffer(picbuffer);
                        IDPhotoHelper.Save(idmodel.IDCard, picbuffer);
                        OnReadCallback?.Invoke(idmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("hz:" + ex.Message);
            }
        }

        private bool FindID()
        {
            try
            {
                var data = IDPackage.getFindPackage();
                udp.Send(data, data.Length, remotePoint);
                IPEndPoint epSender = null;
                var receive = udp.Receive(ref epSender);
                ////5 + 2 + 3 + 4 + 1;
                if (receive.Length == 15)
                {
                    if (receive[9] == 0x9F)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    //Debug.WriteLine("hz:error FindID1->" + this.remoteIp + " " + ex.Message);
                }
                return false;
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("hz:error FindID2->" + this.remoteIp + " " + ex.Message);
                return false;
            }
        }

        private bool SelectID()
        {
            try
            {
                var data = IDPackage.getSelectPackage();
                udp.Send(data, data.Length, remotePoint);
                IPEndPoint epSender = null;
                var receive = udp.Receive(ref epSender);
                //5 + 2 + 3 + 8 + 1;
                if (receive.Length >= 10)
                {
                    if (receive[9] == 0x90)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Close()
        {
            bRun = false;
            if (thread != null)
            {
                thread.Abort();
            }
        }
    }
}
