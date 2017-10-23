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
using System.Threading;

namespace GZ_SpotVisual
{
    class HttpSocket
    {
        private WebSocket ws = null;
        private string serverIp = "";
        private Action<string> callback = null;

        private Context _activity = null;
        private const int Reconnect_Interval = 10 * 1000;

        public HttpSocket(Context activity)
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
                var url = "ws://" + serverIp + ":9872/android";
                ws = new WebSocket(url);
                ws.OnOpen += Socket_OnOpen;
                ws.OnError += Socket_OnError;
                ws.OnClose += Socket_OnClose;
                ws.OnMessage += Socket_OnMessage;
                ws.EmitOnPing = true;
                ws.Connect();
            });
        }

        public void SetCallback(Action<string> callback)
        {
            this.callback = callback;
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            MainActivity.handler?.SendEmptyMessage(MainActivity.WEBSOCKET_DATA);
            if (e.IsText)
            {
                var content = e.Data;
                callback?.Invoke(content);
            }
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            Dialog("WebSocket connection error");
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            MainActivity.handler?.SendEmptyMessage(MainActivity.WEBSOCKET_CLOSE);
            Close();
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(Reconnect_Interval);
                Reconnect();
            });
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Dialog("WebSocket connect ok");
            MainActivity.handler?.SendEmptyMessage(MainActivity.WEBSOCKET_OK);
        }

        private void Reconnect()
        {
            Connect(serverIp);
        }

        public void Close()
        {
            if (ws == null)
                return;

            if (ws?.ReadyState == WebSocketState.Open)
                ws?.Close();

            if (ws != null)
            {
                ws.OnOpen -= Socket_OnOpen;
                ws.OnClose -= Socket_OnClose;
                ws.OnError -= Socket_OnError;
                ws.OnMessage -= Socket_OnMessage;
                ws = null;
            }
        }

        private void Dialog(string msg)
        {
            //_activity.RunOnUiThread(() =>
            //{
            //    Toast.MakeText(Application.Context, msg, ToastLength.Short).Show();
            //});
        }
    }
}