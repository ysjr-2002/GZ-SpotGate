using GZ_SpotGate.Core;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebSocketSharp.Server;

namespace GZ_SpotGate.WS
{
    /// <summary>
    /// 启动一个Websocket服务，接收android平台的连接
    /// 并将消息发送到指定的平台
    /// </summary>
    class WebSocketServer
    {
        private int _port = 0;
        private WebSocketSharp.Server.WebSocketServer wssv = null;
        private static readonly ILog log = LogManager.GetLogger("WebServer");

        private const string SERVICE_PATH = "/android";

        public WebSocketServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            wssv = new WebSocketSharp.Server.WebSocketServer(_port);
            wssv.Log.Level = WebSocketSharp.LogLevel.Fatal;
            wssv.AddWebSocketService<AndroidBehavior>(SERVICE_PATH, InitAndroid);
            wssv.Start();
            if (wssv.IsListening)
            {
                Debug("Web server start,port->" + wssv.Port);
                foreach (var path in wssv.WebSocketServices.Paths)
                {
                    Debug(path);
                }
            }
        }

        private AndroidBehavior InitAndroid()
        {
            AndroidBehavior chat = new AndroidBehavior();
            return chat;
        }

        public void Pass(string androidClient, AndroidMessage message)
        {
            if (message == null)
                return;

            if (wssv == null)
                return;

            WebSocketServiceHost host = null;
            if (wssv.WebSocketServices.TryGetServiceHost(SERVICE_PATH, out host))
            {
                MyConsole.Current.Log("发送至平板->" + androidClient);
                MyConsole.Current.Log("连接平板数量->" + host.Sessions.IDs.Count());
                //MyConsole.Current.Log("已连接平板数量2->" + host.Sessions.ActiveIDs.Count());
                //foreach (var sID in host.Sessions.IDs)
                //{
                //    var remoteIp = host.Sessions[sID].Context.UserEndPoint.Address.ToString();
                //    MyConsole.Current.Log("UnActive已连接平板->" + remoteIp);
                //}

                try
                {
                    foreach (var sID in host.Sessions.IDs)
                    {
                        var webSocketContext = host.Sessions[sID].Context;
                        if (webSocketContext != null)
                        {
                            var remoteIp = webSocketContext.UserEndPoint.Address.ToString();
                            MyConsole.Current.Log("已连接平板->" + remoteIp);
                            if (remoteIp == androidClient && host.Sessions[sID].State == WebSocketSharp.WebSocketState.Open)
                            {
                                var json = Util.ToJson(message);
                                webSocketContext.WebSocket.Send(json);
                                MyConsole.Current.Log("Android发送成功");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Fatal("发送数据异常->" + ex.StackTrace);
                }
            }
        }

        private static void send(string ip, WebSocketServiceHost host)
        {
            List<string> list = new List<string>();
            foreach (var sID in host.Sessions.ActiveIDs)
            {
                var remoteIp = host.Sessions[sID].Context.UserEndPoint.Address.ToString();
                if (remoteIp == ip)
                {
                    list.Add(sID);
                }
            }
        }

        private void Debug(string content)
        {
            MyConsole.Current.Log(content);
        }

        public void Stop()
        {
            wssv?.Stop();
        }
    }
}
