using GZ_SpotGate.Core;
using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GZ_SpotGate.Core
{
    /// <summary>
    /// DEBUG
    /// INFO
    /// WARN
    /// ERROR
    /// FATAL
    /// </summary>
    class MainController
    {
        private static readonly ILog log = LogManager.GetLogger("MainController");

        private WebSocketServer _webServer = null;
        private TcpComServer _tcpServer = null;

        private List<ChannelController> _channels = new List<ChannelController>();

        public MainController()
        {

        }

        public void Start()
        {
            _tcpServer = new TcpComServer(ConfigProfile.Current.TcpComListenPort);
            _tcpServer.OnMessageInComming += ComServer_OnMessageInComming;
            _tcpServer.Start();

            _webServer = new WebSocketServer(ConfigProfile.Current.WebSocketListenPort);
            _webServer.Start();

            var enableChannels = Channels.ChannelList.Where(s => s.IsEnable == true).ToList();
            foreach (var c in enableChannels)
            {
                ChannelController cc = new ChannelController();
                cc.Init(c, _webServer);
                _channels.Add(cc);
            }

            //timer.Interval = 1000;
            //timer.Elapsed += delegate { RestartApp(); };
            //timer.Start();
            MyConsole.Current.Log("系统启动");

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        _webServer.Pass("192.168.2.175", new AndroidMessage
            //        {
            //            CheckInType = Model.IDType.ID,
            //            Line1 = "欢迎光临,请入园",
            //            Line2 = "验证成功",
            //            Delay = 3000,
            //            Code = 100
            //        });
            //        Thread.Sleep(5000);
            //    }
            //});
        }

        private void ComServer_OnMessageInComming(object sender, DataEventArgs e)
        {
            var epSendIp = e.IPEndPoint.Address.ToString();
            ChannelController channlController = null;
            foreach (var channel in _channels)
            {
                if (e.GateOpen)
                {
                    if (channel.EqualGateServerIp(epSendIp))
                    {
                        channlController = channel;
                        break;
                    }
                }
                else
                {
                    if (channel.EqualDataServerIp(epSendIp))
                    {
                        channlController = channel;
                        break;
                    }
                }
            }

            if (e.GateOpen)
            {
                //开闸上报
                channlController?.Report(e);
            }
            else
            {
                channlController?.Work(e);
            }
        }

        public void Dispose()
        {
            timer.Stop();
            _tcpServer.Stop();
            _webServer.Stop();
            foreach (var channel in _channels)
            {
                //关闭websocket
                channel.Stop();
            }
            GateConnectionPool.Dispose();
        }

        System.Timers.Timer timer = new System.Timers.Timer();
        private void RestartApp()
        {
            var now = DateTime.Now.ToString("HH:mm:ss");
            if (now == "18:11:00")
            {
                System.Windows.Forms.Application.Restart();
            }
        }
    }
}
