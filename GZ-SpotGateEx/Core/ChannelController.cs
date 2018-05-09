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
        private FaceSocket faceInSocket;
        private FaceSocket faceOutSocket;

        private Request _request;

        private const int Delay = 3000;
        private const string In_Ok = "欢迎光临,请入园";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "欢迎再次光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";

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
            faceInSocket = new FaceSocket(channel.FaceInIp, channel.CameraInIp, OnFaceOutRecognize);
            faceOutSocket = new FaceSocket(channel.FaceOutIp, channel.CameraOutIp, OnFaceOutRecognize);
            var cameratask1 = await faceInSocket.Connect();
            var cameratask2 = await faceOutSocket.Connect();
            Record record = Record.GetInitRecrod();
            record.Channel = this.Channel.Name;
            if (cameratask1 && cameratask2)
            {
                record.StatuCode = 0;
                record.Status = "初始化成功";
            }
            else
            {
                record.Status = "初始化失败";
            }
            MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
            return false;
        }

        public void Stop()
        {
            //释放websocket资源
            faceInSocket?.Disconnect();
            faceOutSocket?.Disconnect();
        }

        public async void Report(InOutType inouttype)
        {
            Record record = Record.getUpLoadRecord();
            record.IntentType = inouttype;
            record.Channel = this.channel.Name;
            if (inouttype == InOutType.In)
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

        public async Task<FeedBack> Check(IDType idType, InOutType inouttype, string uniqueId, string name = "", string avatar = "")
        {
            Record record = Record.getRecord(channel);
            record.IDType = idType;
            record.IntentType = inouttype;
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
            if (feedback == null)
            {
                record.Status = "服务器访问失败,请查看日志";
                record.StatuCode = 1;
            }
            else
            {
                record.Status = feedback.message;
                record.Time = sw.ElapsedMilliseconds + "ms";
                //测试
                feedback.code = 100;
                feedback.personCount = "1";
                AndroidMessage am = new AndroidMessage()
                {
                    CheckInType = idType,
                    IntentType = inouttype,
                    Avatar = avatar,
                    Delay = Delay,
                    Code = feedback.code
                };
                byte personCount = feedback.personCount.ToByte();
                if (inouttype == InOutType.In && feedback.code == 100)
                {
                    //开闸
                    am.Line1 = In_Ok;
                    am.Line2 = Line2_Ok_Tip;
                    Udp.SendToAndroid(channel.PadInIp, am);

                    if (idType == IDType.Face)
                    {
                        var param = string.Format(HttpConstrant.url_client_opentgate, (int)inouttype, personCount);
                        var open = await _request.Open(openGateUrl + param);
                        if (open.code == 0)
                        {
                            record.Status = feedback.message + "\r\n开闸成功";
                        }
                        else
                        {
                            record.Status = feedback.message + "\r\n" + open.message;
                        }
                    }
                }
                if (inouttype == InOutType.In && feedback.code != 100)
                {
                    //进入-失败
                    am.Line1 = In_Failure;
                    am.Line2 = Line2_Failure_Tip;
                    Udp.SendToAndroid(channel.PadInIp, am);
                }
                if (inouttype == InOutType.Out && feedback.code == 100)
                {
                    //离开-成功
                    am.Line1 = Out_Ok;
                    am.Line2 = Line2_Ok_Tip;
                    Udp.SendToAndroid(channel.PadOutIp, am);
                    if (idType == IDType.Face)
                    {
                        var param = string.Format(HttpConstrant.url_client_opentgate, (int)inouttype, personCount);
                        var open = await _request.Open(openGateUrl + param);
                        if (open.code == 0)
                        {
                            record.Status = feedback.message + "\r\n开闸成功";
                        }
                        else
                        {
                            record.Status = feedback.message + "\r\n" + open.message;
                        }
                    }
                }
                if (inouttype == InOutType.Out && feedback.code != 100)
                {
                    //离开-失败
                    am.Line1 = Out_Failure;
                    am.Line2 = Line2_Failure_Tip;
                    Udp.SendToAndroid(channel.PadOutIp, am);
                }
            }
            MyStandardKernel.Instance.Get<MainViewModel>().Append(record);
            return feedback;
        }

        private async void OnFaceInRecognize(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            await Check(IDType.Face, InOutType.In, code, name, avatar);
        }

        private async void OnFaceOutRecognize(FaceRecognized face)
        {
            var name = face.person.name;
            var code = face.person.job_number;
            var avatar = face.person.avatar;
            await Check(IDType.Face, InOutType.Out, code, name, avatar);
        }
    }
}
