using GZ_SpotGate.Face;
using GZ_SpotGate.Model;
using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GZ_SpotGate.Core
{
    /// <summary>
    /// 通道控制器
    /// </summary>
    class ChannelController
    {
        private ChannelModel _model;
        private FaceSocket _faceInSocket;
        private FaceSocket _faceOutSocket;

        private WebSocketServer _ws;
        private Request _request;

        private const int Delay = 1500;
        private const string In_Ok = "欢迎光临,请入园";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "欢迎再次光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";

        private static readonly ILog log = LogManager.GetLogger("ChannelController");

        public ChannelController()
        {
        }

        public async Task<bool> Init(ChannelModel model, WebSocketServer ws)
        {
            _ws = ws;
            _model = model;
            _request = new Request();

            //_faceInSocket = new FaceSocket(model.FaceInIp, model.FaceInCameraIp, FaceIn);
            //var connect1 = await _faceInSocket.Connect();

            //_faceOutSocket = new FaceSocket(model.FaceOutIp, model.FaceOutCameraIp, FaceOut);
            //var connect2 = await _faceOutSocket.Connect();

            //if (connect1 && connect2)
            //{
            //    log.Debug(string.Format("[{0}]通道初始化成功", _model.No));
            //    return true;
            //}
            //else
            //{
            //    log.DebugFormat("[{0}]通道初始化失败", _model.No);
            //    return false;
            //}
            return true;
        }

        public async void Report(DataEventArgs data)
        {
            if (data.PersonIn)
            {
                await _request.Calc(this._model.ChannelVirualIp, "Z");
            }
            else
            {
                await _request.Calc(this._model.ChannelVirualIp, "F");
            }
            MyConsole.Current.Log(string.Format("[{0}]通道上报通行人次->", _model.No));
        }

        public async void Work(DataEventArgs args)
        {
            IntentType intentType = IntentType.In;
            IDType checkInType = IDType.IC;

            if (ChannelModel.InReaderPort == args.IPEndPoint.Port || ChannelModel.InIDReaderPort == args.IPEndPoint.Port)
            {
                intentType = IntentType.In;
            }
            else
            {
                intentType = IntentType.Out;
            }

            if (args.QRData)
            {
                checkInType = IDType.BarCode;
            }
            if (args.IDData)
            {
                checkInType = IDType.ID;
            }
            if (args.ICData)
            {
                checkInType = IDType.IC;
            }
            if (args.FaceData)
            {
                checkInType = IDType.Face;
            }
            await Check(intentType, checkInType, args.Data, args.Name);
        }

        private async Task Check(IntentType intentType, IDType checkInType, string uniqueId, string name = "", string avatar = "")
        {
            var listlog = new List<string>();
            listlog.Add(string.Format("[{0}]通道", _model.No));
            if (intentType == IntentType.In)
                listlog.Add("进入");
            else
                listlog.Add("离开");

            if (checkInType == IDType.IC)
                listlog.Add(string.Format("IC={0}", uniqueId));
            else if (checkInType == IDType.ID)
                listlog.Add(string.Format("身份证={0}", uniqueId));
            else if (checkInType == IDType.BarCode)
                listlog.Add(string.Format("二维码={0}", uniqueId));
            else if (checkInType == IDType.Face)
                listlog.Add(string.Format("人脸={0}", uniqueId));

            var content = await _request.CheckIn(this._model.ChannelVirualIp, checkInType, uniqueId);
            //content = new Model.FeedBack();
            //content.code = 100;
            //content.personCount = "1";
            //允许通行
            AndroidMessage am = new AndroidMessage()
            {
                CheckInType = checkInType,
                IntentType = intentType,
                //Avatar = "https://o7rv4xhdy.qnssl.com/@/static/upload/avatar/2017-04-07/741757cb9c5e19f00c8f6ac9a56057d27aab2857.jpg",
                Avatar = avatar,
                Delay = Delay,
                Code = content?.code ?? 0
            };

            byte personCount = content?.personCount.ToByte() ?? 0;
            if (content?.code == 100)
            {
                listlog.Add(string.Format("请通行->{0}人次", personCount));
            }
            else
            {
                //禁止通行
                listlog.Add(content?.message ?? "禁止通行");
            }

            if (intentType == IntentType.In && content?.code == 100)
            {
                //进入-成功
                GateConnectionPool.EnterOpen(this._model.GateComServerIp, personCount);
                am.Line1 = In_Ok;
                am.Line2 = Line2_Ok_Tip;
                _ws.Pass(_model.AndroidInIp, am);
            }
            if (intentType == IntentType.In && content?.code != 100)
            {
                //进入-失败
                am.Line1 = In_Failure;
                am.Line2 = Line2_Failure_Tip;
                _ws.Pass(_model.AndroidInIp, am);
            }
            if (intentType == IntentType.Out && content?.code == 100)
            {
                //离开-成功
                GateConnectionPool.ExitOpen(this._model.GateComServerIp, personCount);
                am.Line1 = Out_Ok;
                am.Line2 = Line2_Ok_Tip;
                _ws.Pass(_model.AndroidOutIp, am);
            }
            if (intentType == IntentType.Out && content?.code != 100)
            {
                //离开-失败
                am.Line1 = Out_Failure;
                am.Line2 = Line2_Failure_Tip;
                _ws.Pass(_model.AndroidOutIp, am);
            }
            MyConsole.Current.Log(listlog.ToArray());
        }

        public void Stop()
        {
            //释放websocket资源
            _faceInSocket?.Disconnect();
            _faceOutSocket?.Disconnect();
        }

        public bool EqualDataServerIp(string ip)
        {
            if (_model.ComServerIp == ip)
            {
                return true;
            }
            return false;
        }

        public bool EqualGateServerIp(string ip)
        {
            if (_model.GateComServerIp == ip)
            {
                return true;
            }
            return false;
        }

        private async void FaceIn(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            await Check(IntentType.In, IDType.Face, code, name, avatar);
        }

        private async void FaceOut(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            await Check(IntentType.Out, IDType.Face, code, name, avatar);
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
