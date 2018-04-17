using GZ_SpotGate.Face;
using GZ_SpotGate.Model;
using GZ_SpotGate.Tcp;
using GZ_SpotGate.WS;
using IPVoice;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

        private const int Delay = 3000;
        private const string In_Ok = "欢迎光临,请入园";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "欢迎再次光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";

        private string voice_ok = "";
        private string voice_no = "";

        private PlayParam playParam_in;
        private PlayParam playParam_out;

        private IntPtr playHandle_in = IntPtr.Zero;
        private IntPtr playHandle_out = IntPtr.Zero;

        private static readonly ILog log = LogManager.GetLogger("ChannelController");

        public ChannelController()
        {
        }

        public async Task<bool> Init(ChannelModel model, WebSocketServer ws)
        {
            _ws = ws;
            _model = model;
            _request = new Request();


            _faceInSocket = new FaceSocket(model.FaceInIp, model.FaceInCameraIp, FaceIn);
            var cameratask1 = _faceInSocket.Connect();

            _faceOutSocket = new FaceSocket(model.FaceOutIp, model.FaceOutCameraIp, FaceOut);
            var cameratask2 = _faceOutSocket.Connect();

            await Task.WhenAll(cameratask1, cameratask2);
            var connect1 = cameratask1.Result;
            var connect2 = cameratask2.Result;

            if (connect1 && connect2)
            {
                //MyConsole.Current.Log(string.Format("[{0}]通道初始化成功", _model.No));
                return true;
            }
            else
            {
                //MyConsole.Current.Log(string.Format("[{0}]通道初始化失败", _model.No));
                return false;
            }
        }

        public async void Report(DataEventArgs data)
        {
            //if (data.PersonIn)
            //{
            //    await _request.Calc(this._model.ChannelVirualIp, "Z");
            //}
            //else
            //{
            //    await _request.Calc(this._model.ChannelVirualIp, "F");
            //}
            //MyConsole.Current.Log(string.Format("[{0}]通道上报通行人次", _model.No));
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
            if (_model.GateHoleOpen)
            {
                MyConsole.Current.Log(string.Format("[{0}]通道常开模式", _model.No));
                return;
            }

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

            if (string.IsNullOrEmpty(uniqueId))
            {
                listlog.Add("人脸编号为空->" + name);
                MyConsole.Current.Log(listlog.ToArray());
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var content = await _request.CheckIn(this._model.ChannelVirualIp, checkInType, uniqueId);
            sw.Stop();
            //允许通行
            AndroidMessage am = new AndroidMessage()
            {
                CheckInType = checkInType,
                IntentType = intentType,
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
            listlog.Add("验证耗时->" + sw.ElapsedMilliseconds + "ms");
            MyConsole.Current.Log(listlog.ToArray());

            if (intentType == IntentType.In && content?.code == 100)
            {
                //声音
                Voice.Speak(voice_ok, playHandle_in);
                //进入-成功
                GateConnectionPool.EnterOpen(this._model.GateComServerIp, personCount);
                am.Line1 = In_Ok;
                am.Line2 = Line2_Ok_Tip;
                _ws.Pass(_model.AndroidInIp, am);
            }
            if (intentType == IntentType.In && content?.code != 100)
            {
                //声音
                Voice.Speak(voice_no, playHandle_in);
                //进入-失败
                am.Line1 = In_Failure;
                am.Line2 = Line2_Failure_Tip;
                _ws.Pass(_model.AndroidInIp, am);
            }
            if (intentType == IntentType.Out && content?.code == 100)
            {
                //声音
                Voice.Speak(voice_ok, playHandle_out);
                //离开-成功
                GateConnectionPool.ExitOpen(this._model.GateComServerIp, personCount);
                am.Line1 = Out_Ok;
                am.Line2 = Line2_Ok_Tip;
                _ws.Pass(_model.AndroidOutIp, am);
            }
            if (intentType == IntentType.Out && content?.code != 100)
            {
                //声音
                Voice.Speak(voice_no, playHandle_out);
                //离开-失败
                am.Line1 = Out_Failure;
                am.Line2 = Line2_Failure_Tip;
                _ws.Pass(_model.AndroidOutIp, am);
            }
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
