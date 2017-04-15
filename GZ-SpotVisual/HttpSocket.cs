using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GZ_SpotVisual
{
    class HttpSocket
    {
        private string serverIp = "";
        private WebSocket ws = null;
        private Action<AndroidMessage> callback = null;

        private Activity _activity = null;
        public HttpSocket(Activity activity)
        {
            _activity = activity;
        }

        public HttpSocket()
        {
        }

        public Task Connect(string serverIp)
        {
            this.serverIp = serverIp;
            return Task.Factory.StartNew(() =>
            {
               var url = "ws://"+ serverIp + ":9872/android";
                ws = new WebSocket(url);
                ws.OnOpen += Socket_OnOpen;
                ws.OnError += Socket_OnError;
                ws.OnClose += Socket_OnClose;
                ws.OnMessage += Socket_OnMessage;
                ws.EmitOnPing = true;
                ws.Connect();
            });
        }

        public void SetCallback(Action<AndroidMessage> callback)
        {
            this.callback = callback;
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                Console.WriteLine(e.Data);
                var entity = JsonConvert.DeserializeObject<AndroidMessage>(e.Data);
                callback?.Invoke(entity);
            }
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            //Config.Log(koalaIp + " Websocket error");
            Dialog("WebSocket connection error");
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            //Config.Log(koalaIp + " Websocket close");
            Reconnect();
            //Dialog("WebSocket connection close");
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Dialog("WebSocket connect ok");
        }

        private void Reconnect()
        {
            Close();
            Connect(serverIp);
        }

        public void Close()
        {
            if (ws == null)
                return;

            if (ws.ReadyState == WebSocketState.Open)
                ws.Close();

            ws.OnOpen -= Socket_OnOpen;
            ws.OnClose -= Socket_OnClose;
            ws.OnError -= Socket_OnError;
            ws.OnMessage -= Socket_OnMessage;
            ws = null;
        }

        private void Dialog(string msg)
        {
            _activity.RunOnUiThread(() =>
            {
                Toast.MakeText(Application.Context, msg, ToastLength.Short).Show();
            });
        }
    }
}