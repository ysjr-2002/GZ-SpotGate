using FaceCore.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using WebSocketSharp;

namespace FaceCore
{
    public class FaceSocket
    {
        private string _koalaIp = "";
        private string _cameraIp = "";
        private bool _open = false;
        private WebSocket _socket = null;
        private Action<FaceRecognize> _callback = null;
        private const int sleep = 30 * 1000;
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        public FaceSocket(string koalaIp, string camerartsp, Action<FaceRecognize> callback)
        {
            _koalaIp = koalaIp.Trim();
            _cameraIp = camerartsp.Trim();
            _callback = callback;
        }

        /// <summary>
        /// 连接人脸识别服务器
        /// </summary>
        /// <returns>true:成功 false:失败</returns>
        public Task<bool> Connect()
        {
            return Task.Factory.StartNew(() =>
            {
                var url = string.Format("ws://{0}:9000/video", _koalaIp.Trim());
                //var rtsp = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp?", _cameraIp.Trim());
                var rtsp = HttpUtility.UrlEncode(_cameraIp);
                var all = string.Concat(url, "?url=", rtsp);

                _socket = new WebSocket(all);
                _socket.OnOpen += _socket_OnOpen;
                _socket.OnError += _socket_OnError;
                _socket.OnClose += _socket_OnClose;
                _socket.OnMessage += _socket_OnMessage;
                _socket.Connect();
                return _open;
            });
        }

        private void _socket_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    var entity = serializer.Deserialize<FaceRecognize>(e.Data);
                    if (entity.type == RecognizeState.recognized.ToString())
                    {
                        _callback.Invoke(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("回调异常->" + ex.Message);
            }
        }

        private void _socket_OnClose(object sender, CloseEventArgs e)
        {
            Debug.WriteLine("Websocket close->" + _koalaIp);
            _open = false;
            if (!_appclose)
            {
                Dispose();
                Thread.Sleep(sleep);
                Connect();
            }
        }

        private void _socket_OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine("Websocket error->" + _koalaIp);
            _open = false;
        }

        private void _socket_OnOpen(object sender, EventArgs e)
        {
            Debug.WriteLine("Websocket success->" + _koalaIp);
            _open = true;
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

        private bool _appclose = false;
        public void Disconnect()
        {
            _appclose = true;
            _socket?.CloseAsync();
            _socket = null;
        }
    }
}
