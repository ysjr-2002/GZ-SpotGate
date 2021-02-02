using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace WS
{
    class Program
    {
        static WebSocket ws = null;
        static void Main(string[] args)
        {
            var lines = System.IO.File.ReadAllLines("cfg.txt");
            var rtsp = lines[1];
            var encode = System.Web.HttpUtility.UrlEncode(rtsp);
            var url = lines[0] + "?url=" + encode;
            ws = new WebSocket(url);
            ws.OnOpen += Ws_OnOpen;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
            Console.WriteLine("start");
            Console.Read();
        }

        private static void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("close:" + e.Reason);
        }

        private static void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("error:" + e.Message);
        }

        private static void Ws_OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("open:");
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            var content = e.Data;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(content);
            if (data != null)
            {
                Console.WriteLine("response type:" + data.type);
                if (data.type == "recognized")
                {
                    System.IO.File.WriteAllText("data.json", e.Data);
                }
            }
            else
            {
                Console.WriteLine("response is null");
            }
        }
    }

    class Response
    {
        public string type;
    }

    class Person
    {
        public string name;
    }
}
