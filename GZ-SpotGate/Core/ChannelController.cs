using GZ_SpotGate.Face;
using GZ_SpotGate.WS;
using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Udp
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
        private WebServer _ws;
        private MegviiGate _megvii;

        public async void Init(ChannelModel model, WebServer ws, MegviiGate megvii)
        {
            _ws = ws;
            _megvii = megvii;
            _request = new Request();

            _inFaceSocket = new FaceSocket(model.FaceInIp, model.FaceInCameraIp, FaceIn);
            await _inFaceSocket.Connect();

            _outFaceSocket = new FaceSocket(model.FaceOutIp, model.FaceOutCameraIp, FaceOut);
            await _outFaceSocket.Connect();
        }

        public async void Work(DataEventArgs args)
        {
            IntentType intentType = IntentType.In;
            CheckIntype checkInType = CheckIntype.IC;

            if (ChannelModel.InReaderPort == args.IPEndPoint.Port || ChannelModel.InIDReaderPort == args.IPEndPoint.Port)
                intentType = IntentType.In;
            else
                intentType = IntentType.Out;

            if (args.ICData)
                checkInType = CheckIntype.IC;
            if (args.IDData)
                checkInType = CheckIntype.ID;
            if (args.QRData)
                checkInType = CheckIntype.BarCode;

            await Check(intentType, checkInType, args.Data);
        }

        private async Task Check(IntentType intentType, CheckIntype checkInType, string uniqueId, string name = "", string avatar = "")
        {
            var content = await _request.CheckIn(checkInType, uniqueId);

            string code, errmessage, datetime, nums;
            Define.Parse(content, out code, out errmessage, out datetime, out nums);

            AndroidMessage am = new AndroidMessage()
            {
                CheckInType = checkInType,
                Avatar = avatar,
                Name = name,
                Message = ""
            };

            if (intentType == IntentType.In)
            {
                //进入
                _ws.Pass(_model.AndroidInIp, am);
            }
            else
            {
                //离开
                _ws.Pass(_model.AndroidOutIp, am);
            }
        }

        public void Stop()
        {
            //释放websocket资源
            _inFaceSocket?.Disconnect();
            _outFaceSocket?.Disconnect();
        }

        public bool ContainIp(string ip)
        {
            if (_model.ComServerIp == ip)
            {
                return true;
            }
            return false;
        }

        private async void FaceIn(FaceRecognized face)
        {
            var code = face.person.job_number;
            await Check(IntentType.In, CheckIntype.Face, code);
        }

        private async void FaceOut(FaceRecognized face)
        {
            var code = face.person.job_number;
            await Check(IntentType.Out, CheckIntype.Face, code);
        }

        /// <summary>
        /// 意图类型
        /// </summary>
        public enum IntentType
        {
            In,
            Out,
        }
    }
}
