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
        private bool _open = false;
        private string _koalaIp = "";
        private string _cameraIp = "";
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
                var wsurl = string.Format("ws://{0}:9000/video", _koalaIp.Trim());
                //var rtsp = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp?", _cameraIp.Trim());
                var rtsp = HttpUtility.UrlEncode(_cameraIp);
                var url = string.Concat(wsurl, "?url=", rtsp);

                _socket = new WebSocket(url);
                _socket.OnOpen += socket_OnOpen;
                _socket.OnError += socket_OnError;
                _socket.OnClose += socket_OnClose;
                _socket.OnMessage += socket_OnMessage;
                _socket.Connect();
                return _open;
            });
        }

        private void socket_OnMessage(object sender, MessageEventArgs e)
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
                Debug.WriteLine("callback->" + ex.Message);
            }
        }

        private void socket_OnClose(object sender, CloseEventArgs e)
        {
            Debug.WriteLine(string.Format("ws close->koalaIp:{0} cameraIp:{1}", _koalaIp, _cameraIp));
            _open = false;
            if (!_appclose)
            {
                Dispose();
                Thread.Sleep(sleep);
                Connect();
            }
        }

        private void socket_OnError(object sender, ErrorEventArgs e)
        {
            Debug.WriteLine(string.Format("ws error->koalaIp:{0} cameraIp:{1}", _koalaIp, _cameraIp));
            _open = false;
        }

        private void socket_OnOpen(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format("ws open->koalaIp:{0} cameraIp:{1}", _koalaIp, _cameraIp));
            _open = true;
        }

        private void Dispose()
        {
            if (_socket == null)
                return;

            if (_socket.ReadyState == WebSocketState.Open)
                _socket.Close();

            _socket.OnOpen -= socket_OnOpen;
            _socket.OnClose -= socket_OnClose;
            _socket.OnError -= socket_OnError;
            _socket.OnMessage -= socket_OnMessage;
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
