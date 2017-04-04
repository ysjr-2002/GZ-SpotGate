using GZ_SpotGate.Face;
using GZ_SpotGate.WS;
using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    /// <summary>
    /// 通道控制器
    /// </summary>
    class ChannelController
    {
        private ChannelModel _model = null;

        private FaceSocket _inFaceSocket = null;
        private FaceSocket _outFaceSocket = null;

        private Request _request = null;

        private IntentType _intentType = IntentType.In;

        private WebServer _ws;
        private MegviiGate _megvii;

        public async void Init(ChannelModel model)
        {
            _request = new Request();

            _inFaceSocket = new FaceSocket(model.FaceInIp, model.FaceInCameraIp, FaceIn);
            await _inFaceSocket.Connect();

            _outFaceSocket = new FaceSocket(model.FaceOutIp, model.FaceOutCameraIp, FaceOut);
            await _outFaceSocket.Connect();
        }

        public async void Work(CheckIntype checkIntype, string code)
        {
            await _request.CheckIn(checkIntype, code);
            if (_intentType == IntentType.In)
            {
                //进入
                _ws.Pass(_model.AndroidInIp, null);
            }
            else
            {
                //离开
                _ws.Pass(_model.AndroidOutIp, null);
            }
        }

        public void Stop()
        {
            _inFaceSocket?.Disconnect();
            _outFaceSocket?.Disconnect();
        }

        public bool ContainIp(string ip)
        {
            //if (_model.ComServerIp == ip)
            //{
            //    _intentType = IntentType.In;
            //    return true;
            //}
            //else if (_model.ComOutIp == ip)
            //{
            //    _intentType = IntentType.Out;
            //    return true;
            //}
            return false;
        }

        private void FaceIn(FaceRecognized face)
        {
            _intentType = IntentType.In;
            var code = face.person.job_number;
            Work(CheckIntype.Face, code);
        }

        private void FaceOut(FaceRecognized face)
        {
            _intentType = IntentType.Out;
            var code = face.person.job_number;
            Work(CheckIntype.Face, code);
        }

        public void OpenGate()
        {

        }

        public enum IntentType
        {
            In,
            Out,
        }
    }
}
