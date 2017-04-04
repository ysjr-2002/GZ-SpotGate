using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace GZ_SpotGate.Face
{
    class FaceSocket
    {
        private string _koalaIp = "";
        private string _cameraIp = "";

        private WebSocket _socket = null;
        private Action<FaceRecognized> _callback = null;
        public FaceSocket(string koalaIp, string cameraIp, Action<FaceRecognized> callback)
        {
            _koalaIp = koalaIp;
            _cameraIp = cameraIp;
            _callback = callback;
        }

        public Task Connect()
        {
            return Task.Factory.StartNew(() =>
            {
                var wsUrl = string.Format("ws://{0}:9000", _koalaIp);
                //var rtspUrl = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp?", cameraIp);
                var rtspUrl = string.Format("rtsp://admin:admin123456@{0}/live1.sdp", _cameraIp);
                var url = string.Concat(wsUrl, "?url=", rtspUrl.UrlEncode());

                _socket = new WebSocket(wsUrl);
                _socket.OnOpen += _socket_OnOpen;
                _socket.OnError += _socket_OnError;
                _socket.OnClose += _socket_OnClose;
                _socket.OnMessage += _socket_OnMessage;
                _socket.EmitOnPing = true;
                _socket.Connect();
            });
        }

        private void _socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var entity = e.Data.Deserlizer<FaceRecognized>();
                if (entity.type == RecognizeState.recognized.ToString())
                {
                    _callback.Invoke(entity);
                }
            }
        }

        private void _socket_OnClose(object sender, CloseEventArgs e)
        {
        }

        private void _socket_OnError(object sender, ErrorEventArgs e)
        {
            Dispose();
            Connect();
        }

        private void _socket_OnOpen(object sender, EventArgs e)
        {
        }

        private void Dispose()
        {
            if (_socket == null)
                return;

            if (_socket.ReadyState == WebSocketState.Open)
                _socket.Close();

            _socket.OnOpen -= _socket_OnOpen;
            _socket.OnClose -= _socket_OnClose;
            _socket.OnError -= _socket_OnError;
            _socket.OnMessage -= _socket_OnMessage;
            _socket = null;
        }

        public void Disconnect()
        {
            _socket?.Close();
        }
    }
}
