using GZ_SpotGate.Face;
using GZ_SpotGate.WS;
using GZ_SpotGate.XmlParser;
using log4net;
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

        private WebSocketServer _ws;
        private MegviiGate _megvii;
        private Request _request = null;

        private static readonly ILog log = LogManager.GetLogger("ChannelController");

        public async Task<bool> Init(ChannelModel model, WebSocketServer ws, MegviiGate megvii)
        {
            _ws = ws;
            _model = model;
            _megvii = megvii;
            _request = new Request();

            _inFaceSocket = new FaceSocket(model.FaceInIp, model.FaceInCameraIp, FaceIn);
            var connect1 = await _inFaceSocket.Connect();

            _outFaceSocket = new FaceSocket(model.FaceOutIp, model.FaceOutCameraIp, FaceOut);
            var connect2 = await _outFaceSocket.Connect();

            if (connect1 && connect2)
                return true;
            else
            {
                log.DebugFormat("初始化失败->{0}", _model.No);
                return false;
            }
        }

        public async void Work(DataEventArgs args)
        {
            IntentType intentType = IntentType.In;
            IDType checkInType = IDType.IC;

            if (ChannelModel.InReaderPort == args.IPEndPoint.Port || ChannelModel.InIDReaderPort == args.IPEndPoint.Port)
                intentType = IntentType.In;
            else
                intentType = IntentType.Out;

            if (args.QRData)
                checkInType = IDType.BarCode;
            if (args.IDData)
                checkInType = IDType.ID;
            if (args.ICData)
                checkInType = IDType.IC;

            await Check(intentType, checkInType, args.Data);
        }

        private async Task Check(IntentType intentType, IDType checkInType, string uniqueId, string name = "", string avatar = "")
        {
            var content = await _request.CheckIn(checkInType, uniqueId);

            string code, errmessage, datetime, nums;
            Define.ParseXmlContent(content, out code, out errmessage, out datetime, out nums);

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
            await Check(IntentType.In, IDType.Face, code);
        }

        private async void FaceOut(FaceRecognized face)
        {
            var code = face.person.job_number;
            await Check(IntentType.Out, IDType.Face, code);
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
