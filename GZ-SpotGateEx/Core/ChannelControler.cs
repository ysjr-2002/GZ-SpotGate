using GZ_SpotGateEx.Face;
using GZ_SpotGateEx.Model;
using GZ_SpotGateEx.WS;
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

namespace GZ_SpotGateEx.Core
{
    /// <summary>
    /// 通道控制器
    /// </summary>
    class ChannelControler
    {
        private Channel _model;
        private FaceSocket _faceInSocket;
        private FaceSocket _faceOutSocket;

        private Request _request;

        private const int Delay = 3000;
        private const string In_Ok = "欢迎光临,请入园";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "欢迎再次光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";

        private static readonly ILog log = LogManager.GetLogger("ChannelControler");

        public ChannelControler(Channel model)
        {
            _model = model;
        }

        public async Task<bool> Init()
        {
            _request = new Request();

            _faceInSocket = new FaceSocket(_model.FaceInIp, _model.CameraInIp, FaceIn);
            var cameratask1 = _faceInSocket.Connect();

            _faceOutSocket = new FaceSocket(_model.FaceOutIp, _model.CameraOutIp, FaceOut);
            var cameratask2 = _faceOutSocket.Connect();

            await Task.WhenAll(cameratask1, cameratask2);
            var connect1 = cameratask1.Result;
            var connect2 = cameratask2.Result;

            //if (connect1 && connect2)
            //{
            //    MyConsole.Current.Log(string.Format("[{0}]通道初始化成功", _model.No));
            //    return true;
            //}
            //else
            //{
            //    MyConsole.Current.Log(string.Format("[{0}]通道初始化失败", _model.No));
            //    return false;
            //}
            return false;
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
        }

        public void Stop()
        {
            //释放websocket资源
            _faceInSocket?.Disconnect();
            _faceOutSocket?.Disconnect();
        }

        private async void FaceIn(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            //await Check(IntentType.In, IDType.Face, code, name, avatar);
        }

        private async void FaceOut(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            //await Check(IntentType.Out, IDType.Face, code, name, avatar);
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
