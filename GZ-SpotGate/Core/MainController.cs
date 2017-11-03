using GZ_SpotGate.Core;
using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private System.Timers.Timer timer = new System.Timers.Timer();

        public MainController()
        {
        }

        public void Start()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

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

            MyConsole.Current.Log("系统启动");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var curTime = DateTime.Now.ToString("HHmmss");
            if (curTime == ConfigProfile.Current.AutoRestartTime)
            {
                log.Info("执行自动重启->" + curTime);
                timer.Stop();
                restart();
            }
            else
            {
                Debug.WriteLine("hz:" + curTime);
            }
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
            _tcpServer?.Stop();
            _webServer?.Stop();
            foreach (var channel in _channels)
            {
                //关闭websocket
                channel.Stop();
            }
            GateConnectionPool.Dispose();
        }

        private void restart()
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string strPath = exePath.Replace(".vshost", "");
            Process.Start(strPath, "0");
            Environment.Exit(0);//关闭当前进程
        }
    }
}
