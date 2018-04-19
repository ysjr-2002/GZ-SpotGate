using Ninject;
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
using GZ_SpotGateEx.ViewModel;
using GZ_SpotGateEx.http;

namespace GZ_SpotGateEx.Core
{
    /// <summary>
    /// 通道控制器
    /// </summary>
    class ChannelControler
    {
        private Channel channel;
        private string openGateUrl = "";
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

        public ChannelControler(Channel channel)
        {
            this.channel = channel;
        }

        public Channel Channel
        {
            get
            {
                return channel;
            }
        }

        public async Task<bool> Init()
        {
            _request = new Request();
            openGateUrl = string.Format(ConfigProfile.Current.ClientOpenGateUrl, this.channel.ClientIp);
            //_faceInSocket = new FaceSocket(channel.FaceInIp, channel.CameraInIp, FaceIn);
            //var cameratask1 = _faceInSocket.Connect();

            //_faceOutSocket = new FaceSocket(channel.FaceOutIp, channel.CameraOutIp, FaceOut);
            //var cameratask2 = _faceOutSocket.Connect();

            //await Task.WhenAll(cameratask1, cameratask2);
            //var connect1 = cameratask1.Result;
            //var connect2 = cameratask2.Result;

            //if (connect1 && connect2)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return false;
        }

        public async void Report(DataEventArgs data)
        {
            if (data.PersonIn)
            {
                await _request.Calc(this.channel.ChannelVirualIp, "Z");
            }
            else
            {
                await _request.Calc(this.channel.ChannelVirualIp, "F");
            }
        }

        public void Stop()
        {
            //释放websocket资源
            _faceInSocket?.Disconnect();
            _faceOutSocket?.Disconnect();
        }

        public async void Report(string inouttype)
        {
            if (inouttype == "0")
            {
                await _request.Calc(this.channel.ChannelVirualIp, "Z");
            }
            else
            {
                await _request.Calc(this.channel.ChannelVirualIp, "F");
            }
        }

        public async Task<FeedBack> Check(IntentType intentType, IDType idType, string uniqueId, string name = "", string avatar = "")
        {
            var listlog = new List<string>();
            listlog.Add(string.Format("[{0}]通道", channel.No));
            Record record = new Record();
            if (intentType == IntentType.In)
                listlog.Add("进入");
            else
                listlog.Add("离开");

            record.Channel = channel.Name;
            if (idType == IDType.IC)
                record.TypeImageSourceUrl = ImageConstrant.QR_ImageSource;
            else if (idType == IDType.ID)
                record.TypeImageSourceUrl = ImageConstrant.QR_ImageSource;
            else if (idType == IDType.BarCode)
                record.TypeImageSourceUrl = ImageConstrant.QR_ImageSource;
            else if (idType == IDType.Face)
                record.TypeImageSourceUrl = ImageConstrant.FACE_ImageSource;

            if (string.IsNullOrEmpty(uniqueId))
            {
                listlog.Add("人脸编号为空->" + name);
                //MyConsole.Current.Log(listlog.ToArray());
                return null;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var feedback = await _request.CheckIn(this.channel.ChannelVirualIp, idType, uniqueId);
            sw.Stop();
            //允许通行
            AndroidMessage am = new AndroidMessage()
            {
                CheckInType = idType,
                IntentType = intentType,
                Avatar = avatar,
                Delay = Delay,
                Code = feedback?.code ?? 0
            };

            record.BarCode = uniqueId;
            record.Status = feedback?.message;
            record.CheckTime = DateTime.Now.ToStandard();

            byte personCount = feedback?.personCount.ToByte() ?? 0;
            if (feedback?.code == 100)
            {
                listlog.Add(string.Format("请通行->{0}人次", personCount));
            }
            else
            {
                //禁止通行
                listlog.Add(feedback?.message ?? "禁止通行");
            }

            listlog.Add("验证耗时->" + sw.ElapsedMilliseconds + "ms");

            if (intentType == IntentType.In && feedback?.code == 100)
            {
                //开闸
                am.Line1 = In_Ok;
                am.Line2 = Line2_Ok_Tip;
                Udp.SendToAndroid(channel.PadInIp, am);

                if (idType == IDType.Face)
                {
                    //入
                    var open = await _request.Open(openGateUrl + "0");
                }
            }
            if (intentType == IntentType.In && feedback?.code != 100)
            {
                //进入-失败
                am.Line1 = In_Failure;
                am.Line2 = Line2_Failure_Tip;
                Udp.SendToAndroid(channel.PadInIp, am);
            }
            if (intentType == IntentType.Out && feedback?.code == 100)
            {
                //离开-成功
                am.Line1 = Out_Ok;
                am.Line2 = Line2_Ok_Tip;
                Udp.SendToAndroid(channel.PadOutIp, am);

                if (idType == IDType.Face)
                {
                    //出
                    var open = await _request.Open(openGateUrl + "1");
                }
            }
            if (intentType == IntentType.Out && feedback?.code != 100)
            {
                //声音
                //离开-失败
                am.Line1 = Out_Failure;
                am.Line2 = Line2_Failure_Tip;
                Udp.SendToAndroid(channel.PadOutIp, am);
            }

            MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
            return feedback;
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
    }
}
