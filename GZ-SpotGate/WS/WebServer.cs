using GZ_SpotGate.Udp;
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
    class WebServer
    {
        private int _port = 0;
        private WebSocketServer wssv = null;
        private static readonly ILog log = LogManager.GetLogger("WebServer");

        private const string SERVICE_PATH = "/android";

        public WebServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            wssv = new WebSocketServer(_port);
            wssv.AddWebSocketService<AndroidBehavior>(SERVICE_PATH, InitAndroid);
            wssv.Start();
            if (wssv.IsListening)
            {
                Debug("Web server start");
                var log = string.Format("Listening on port {0}, and providing WebSocket services:", wssv.Port.ToString());
                Debug(log);
                foreach (var path in wssv.WebSocketServices.Paths)
                {
                    Debug(path);
                }
            }
        }

        private AndroidBehavior InitAndroid()
        {
            AndroidBehavior chat = new AndroidBehavior();
            Debug("接收到客户端连接->");
            return chat;
        }

        public void Pass(string androidClient, AndroidMessage message)
        {
            WebSocketServiceHost host = null;
            if (wssv.WebSocketServices.TryGetServiceHost(SERVICE_PATH, out host))
            {
                foreach (var sID in host.Sessions.ActiveIDs)
                {
                    var userIp = host.Sessions[sID].Context.UserEndPoint.Address.ToString();
                    if (userIp == androidClient)
                    {
                        var json = Util.toJson(message);
                        host.Sessions[sID].Context.WebSocket.Send(json);
                        break;
                    }
                }
            }
        }

        private void Debug(string content)
        {
            log.Debug(content);
        }

        public void Stop()
        {
            wssv?.Stop();
        }
    }
}
