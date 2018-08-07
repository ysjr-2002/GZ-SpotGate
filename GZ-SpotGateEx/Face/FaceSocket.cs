using GZ_SpotGateEx.Core;
using GZ_SpotGateEx.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebSocketSharp;

namespace GZ_SpotGateEx.Face
{
    class FaceSocket
    {
        private string _koalaIp = "";
        private string _cameraIp = "";
        private bool _open = false;
        private WebSocket _socket = null;
        private Channel _channel;
        private InOutType _inouttype;
        private Action<FaceRecognized> _callback = null;

        private const int sleep = 30 * 1000;

        public FaceSocket(string koalaIp, string cameraIp, InOutType inouttype, Channel channel, Action<FaceRecognized> callback)
        {
            _koalaIp = koalaIp;
            _cameraIp = cameraIp;
            _callback = callback;
            _inouttype = inouttype;
            _channel = channel;
        }

        public Task<bool> Connect()
        {
            return Task.Factory.StartNew(() =>
            {
                Dispose();

                var url = string.Format("ws://{0}:9000/video", _koalaIp.Trim());
                var rtsp = "";
                if (_inouttype == InOutType.In)
                {
                    if (_channel.CIntype == 1)
                        rtsp = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp", _cameraIp.Trim());
                    else
                        rtsp = string.Format("rtsp://{0}:8080/h264_ulaw.sdp", _cameraIp.Trim());
                }
                if (_inouttype == InOutType.Out)
                {
                    if (_channel.COutype == 1)
                        rtsp = string.Format("rtsp://{0}/user=admin&password=&channel=1&stream=0.sdp", _cameraIp.Trim());
                    else
                        rtsp = string.Format("rtsp://{0}:8080/h264_ulaw.sdp", _cameraIp.Trim());
                }

                rtsp = HttpUtility.UrlEncode(rtsp);
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
            _open = false;
            if (_inouttype == InOutType.In)
                _channel.IsWsIn = false;
            else
                _channel.IsWsOut = false;
            if (!_appclose)
            {
                MyLog.debug("Websocket关闭->" + _koalaIp);
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(sleep);
                    Connect();
                });
            }
        }

        private void _socket_OnError(object sender, ErrorEventArgs e)
        {
            _open = false;
            //MyConsole.Current.Log("Websocket错误->" + _koalaIp);
        }

        private void _socket_OnOpen(object sender, EventArgs e)
        {
            _open = true;
            //MyLog.debug("Websocket成功->" + _koalaIp);
            if (_inouttype == InOutType.In)
                _channel.IsWsIn = true;
            else
                _channel.IsWsOut = true;

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
