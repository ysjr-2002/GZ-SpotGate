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
    class WebServer
    {
        private int _port = 0;
        private WebSocketServer wssv = null;
        private Dictionary<string, AndroidBehavior> androidClients = new Dictionary<string, AndroidBehavior>();
        private static readonly ILog log = LogManager.GetLogger("WebServer");

        public WebServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            wssv = new WebSocketServer(_port);
            wssv.AddWebSocketService<AndroidBehavior>("/android", InitAndroid);
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
            var ip = chat.Context.UserEndPoint.Address.ToString();
            if (androidClients.ContainsKey(ip))
                androidClients[ip] = chat;
            else
                androidClients.Add(ip, chat);

            Debug("接收到客户端连接->" + ip);
            return chat;
        }

        public void Pass(string androidClient, AndroidMessage message)
        {
            if (androidClients.ContainsKey(androidClient))
            {
                var json = Util.toJson(message);
                androidClients[androidClient].Context.WebSocket.Send(json);
                Debug("发送到Android客户端->" + androidClient);
            }
        }

        private void Debug(string content)
        {
            log.Debug(content);
        }

        public void Stop()
        {
            wssv?.Stop();
            foreach (var item in androidClients)
            {
                item.Value.Context.WebSocket.Close();
            }
        }
    }
}
