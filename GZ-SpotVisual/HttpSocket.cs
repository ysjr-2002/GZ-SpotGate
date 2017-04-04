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
        private string koalaIp = "";
        private WebSocket socket = null;
        private Action<AndroidMessage> callback = null;

        private Activity _activity = null;
        public HttpSocket(Activity activity)
        {
            _activity = activity;
        }

        public Task Connect(string koalaIp, string cameraIp)
        {
            return Task.Factory.StartNew(() =>
            {
                this.koalaIp = koalaIp;
                var wsUrl = string.Format("ws://{0}:9000", koalaIp);
                //var rtspUrl = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp?", cameraIp);
                var rtspUrl = string.Format("rtsp://admin:admin123456@{0}/live1.sdp", cameraIp);
                var url = string.Concat(wsUrl, "?url=", rtspUrl.UrlEncode());
                socket = new WebSocket(url);
                socket.OnOpen += Socket_OnOpen;
                socket.OnError += Socket_OnError;
                socket.OnClose += Socket_OnClose;
                socket.OnMessage += Socket_OnMessage;
                socket.EmitOnPing = true;
                socket.Connect();
            });
        }

        public void Close()
        {
            socket?.Close();
        }

        public void SetCallback(Action<AndroidMessage> callback)
        {
            this.callback = callback;
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var entity = JsonConvert.DeserializeObject<AndroidMessage>(e.Data);
                callback?.Invoke(entity);
            }
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            Config.Log(koalaIp + " Websocket error");
            Dialog("WebSocket connection error");
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            Config.Log(koalaIp + " Websocket close");
            Dialog("WebSocket connection close");
        }

        private void Socket_OnOpen(object sender, EventArgs e)
        {
            Dialog("WebSocket connect ok");
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