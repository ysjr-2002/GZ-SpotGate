﻿using GZ_SpotGate.Face;
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
        private FaceSocket _inFaceSocket;
        private FaceSocket _outFaceSocket;

        private WebSocketServer _ws;
        private Request _request;
        private TextBox _output;

        private const int Delay = 1000;
        private const string In_Welcome = "欢迎光临";
        private const string Out_Welcome = "欢迎再次光临";

        private static readonly ILog log = LogManager.GetLogger("ChannelController");

        public ChannelController(TextBox output)
        {
            _output = output;
        }

        public async Task<bool> Init(ChannelModel model, WebSocketServer ws)
        {
            _ws = ws;
            _model = model;
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

        private static string prefix
        {
            get
            {
                return DateTime.Now.HMS() + "->";
            }
        }

        public async void Report(DataEventArgs data)
        {
            if (data.PersonIn)
                await _request.Calc(this._model.ChannelVirualIp, "Z");
            else
                await _request.Calc(this._model.ChannelVirualIp, "F");
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
            await Check(intentType, checkInType, args.Data);
        }

        private async Task Check(IntentType intentType, IDType checkInType, string uniqueId, string name = "", string avatar = "")
        {
            var sb = new StringBuilder();
            sb.Append(prefix + "通道" + _model.No + "\n");
            if (intentType == IntentType.In)
            {
                sb.Append(prefix + "进入 \n");
            }
            else
            {
                sb.Append(prefix + "离开 \n");
            }

            if (checkInType == IDType.IC)
                sb.Append(string.Format(prefix + "身份证={0} \n", uniqueId));
            if (checkInType == IDType.ID)
                sb.Append(string.Format(prefix + "IC={0} \n", uniqueId));
            if (checkInType == IDType.BarCode)
                sb.Append(string.Format(prefix + "二维码={0} \n", uniqueId));
            if (checkInType == IDType.Face)
                sb.Append(string.Format(prefix + "Face={0} \n", uniqueId));

            var content = await _request.CheckIn(this._model.ChannelVirualIp, checkInType, uniqueId);
            content.code = 100;
            if (content?.code == 100)
            {
                //允许通行
                AndroidMessage am = new AndroidMessage()
                {
                    CheckInType = checkInType,
                    IntentType = intentType,
                    Avatar = "https://o7rv4xhdy.qnssl.com/@/static/upload/avatar/2017-04-07/741757cb9c5e19f00c8f6ac9a56057d27aab2857.jpg",
                    Name = name,
                    Delay = Delay
                };
                byte personCount = content.personCount.ToByte();
                if (intentType == IntentType.In)
                {
                    //进入
                    GateConnectionPool.EnterOpen(this._model.GateComServerIp, personCount);
                    am.Message = In_Welcome;
                    _ws.Pass(_model.AndroidInIp, am);
                }
                else
                {
                    //离开
                    GateConnectionPool.ExitOpen(this._model.GateComServerIp, personCount);
                    am.Message = Out_Welcome;
                    _ws.Pass(_model.AndroidOutIp, am);
                }
                sb.Append(string.Format(prefix + "请通行 {0}人次\n", personCount));
            }
            else
            {
                //禁止通行
                sb.Append(prefix + "禁止通行 \n");
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                _output?.AppendText(sb.ToString());
            });
        }

        public void Stop()
        {
            //释放websocket资源
            _inFaceSocket?.Disconnect();
            _outFaceSocket?.Disconnect();
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
