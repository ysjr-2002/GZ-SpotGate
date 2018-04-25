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
    class ChannelController
    {
        private Channel channel;
        private string openGateUrl = "";
        private FaceSocket _faceSocket;

        private Request _request;

        private const int Delay = 3000;
        private const string In_Ok = "欢迎光临,请入园";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "欢迎再次光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";

        private IntentType intentType;

        private static readonly ILog log = LogManager.GetLogger("ChannelControler");

        public ChannelController(Channel channel)
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
            openGateUrl = string.Format(ConfigProfile.Current.OpenGateUrl, this.channel.ClientIp);
            _faceSocket = new FaceSocket(channel.FaceIp, channel.CameraIp, FaceIn);
            //var cameratask1 = _faceSocket.Connect();

            if (channel.Inouttype == "0")
                intentType = IntentType.In;
            else
                intentType = IntentType.Out;
            return false;
        }

        public void Stop()
        {
            //释放websocket资源
            _faceSocket?.Disconnect();
        }

        public async void Report()
        {
            Record record = Record.getRecord();
            record.IntentType = this.intentType;
            record.Channel = this.channel.Name;
            record.TypeImageSourceUrl = ImageConstrant.UPLOAD_ImageSource;
            if (intentType == IntentType.In)
            {
                await _request.Calc(this.channel.VirtualIp, "Z");
                record.Status = "入上传人次";
            }
            else
            {
                await _request.Calc(this.channel.VirtualIp, "F");
                record.Status = "出上传人次";
            }
            MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
        }

        public void UpdateHeartbeat()
        {
            this.channel.LastHeartbeat = DateTime.Now.ToStandard();
        }

        public async Task<FeedBack> Check(IDType idType, string uniqueId, string name = "", string avatar = "")
        {
            var listlog = new List<string>();
            listlog.Add(string.Format("[{0}]通道", channel.No));
            Record record = Record.getRecord();
            record.Channel = channel.Name;
            record.IDType = idType;
            record.Code = uniqueId;
            record.Time = "0ms";

            if (string.IsNullOrEmpty(uniqueId))
            {
                record.Status = "人脸编号为空";
                MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
                return null;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var feedback = await _request.CheckIn(this.channel.VirtualIp, idType, uniqueId);
            sw.Stop();
            AndroidMessage am = new AndroidMessage()
            {
                CheckInType = idType,
                IntentType = intentType,
                Avatar = avatar,
                Delay = Delay,
                Code = feedback?.code ?? 0
            };

            record.Status = feedback?.message;
            record.Time = sw.ElapsedMilliseconds + "ms";

            byte personCount = feedback?.personCount.ToByte() ?? 0;
            if (intentType == IntentType.In && feedback?.code == 100)
            {
                //开闸
                am.Line1 = In_Ok;
                am.Line2 = Line2_Ok_Tip;
                Udp.SendToAndroid(channel.PadIp, am);

                if (idType == IDType.Face)
                {
                    var open = await _request.Open(openGateUrl + "0&canIncount=" + personCount);
                }
            }
            if (intentType == IntentType.In && feedback?.code != 100)
            {
                //进入-失败
                am.Line1 = In_Failure;
                am.Line2 = Line2_Failure_Tip;
                Udp.SendToAndroid(channel.PadIp, am);
            }
            if (intentType == IntentType.Out && feedback?.code == 100)
            {
                //离开-成功
                am.Line1 = Out_Ok;
                am.Line2 = Line2_Ok_Tip;
                Udp.SendToAndroid(channel.PadIp, am);
                if (idType == IDType.Face)
                {
                    var open = await _request.Open(openGateUrl + "1&canIncount=" + personCount);
                }
            }
            if (intentType == IntentType.Out && feedback?.code != 100)
            {
                //离开-失败
                am.Line1 = Out_Failure;
                am.Line2 = Line2_Failure_Tip;
                Udp.SendToAndroid(channel.PadIp, am);
            }

            MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
            return feedback;
        }

        private async void FaceIn(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            await Check(IDType.Face, code, name, avatar);
        }
    }
}
