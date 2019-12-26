using GZSpotGate.Face;
using GZSpotGate.IDCard;
using GZSpotGate.Pad;
using GZSpotGate.WS;
using LL.SenicSpot.Gate.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    class ChannelController
    {
        private FaceSocket faceSocket = null;
        public IDReader idreader { get; private set; }
        private Request request = null;
        private GateUdpComServer gateServer = null;
        private const string In_Ok = "欢迎光临";
        private const string In_Failure = "请重新验证";

        private const string Out_Ok = "谢谢光临";
        private const string Out_Failure = "请重新验证";

        private const string Line2_Ok_Tip = "验证成功";
        private const string Line2_Failure_Tip = "验证失败";
        public ChannelController(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }

        public void Start()
        {
            if (Channel.comserver.IsEmpty())
                return;

            faceSocket = new FaceSocket(Channel, OnFaceRecognize);
            faceSocket.Connect();

            idreader = new IDReader(Channel.comserver);
            idreader.Run();
            idreader.SetDataCallback(OnReaderID);

            gateServer = new GateUdpComServer(Channel);
            gateServer.Start();

            request = new Request();
        }

        internal async void OnQRCode(string barCode)
        {
            await Check(IntentType.In, IDType.BarCode, barCode);
        }

        internal async void OnReaderID(IDModel model)
        {
            await Check(IntentType.In, IDType.ID, model.IDCard, model.Name);
        }

        internal async void OnFaceRecognize(FaceRecognized face)
        {
            var avatar = "";
            if (face.person.avatar.StartsWith("http") == false)
                avatar = "http://" + Config.Instance.FaceServer + face.person.avatar;
            else
                avatar = face.person.avatar;

            LogHelper.Log("avatar:" + avatar);
            await Check(IntentType.In, IDType.Face, face.person.description, face.person.name, avatar);
        }

        private async Task Check(IntentType intentType, IDType checkInType, string uniqueId, string name = "", string avatar = "")
        {
            Record record = null;
            //var error = SecurityHelper.IsAuth();
            //if (!error.IsEmpty())
            //{
            //    record = Record.GetError(Channel.name, error);
            //    LogHelper.Append(record);
            //    return;
            //}

            if (uniqueId.IsEmpty())
            {
                record = Record.GetError(Channel.name, "票号为空");
                if (checkInType == IDType.BarCode)
                    record.TypeImageSource = Record.QR_ImageSource;
                if (checkInType == IDType.ID)
                    record.TypeImageSource = Record.ID_ImageSource;
                if (checkInType == IDType.Face)
                    record.TypeImageSource = Record.FACE_ImageSource;
                LogHelper.Append(record);

                var temp = new AndroidMessage()
                {
                    Line1 = "票号为空",
                    Line2 = "",
                    CheckInType = checkInType,
                    IntentType = InOutType.In,
                    Delay = Config.Instance.PadDelay * 1000,
                    Avatar = ""
                };
                PadHelper.SendToPad(Channel.pad, temp);
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            var content = await request.CheckIn(this.Channel.ChannelVirualIp, checkInType, uniqueId);
            sw.Stop();

            AndroidMessage am = new AndroidMessage
            {
                CheckInType = checkInType,
                IntentType = InOutType.In,
                Delay = Config.Instance.PadDelay * 1000,
                Code = content.code,
                Avatar = avatar
            };

            if (checkInType == IDType.Face)
            {
                record = Record.GetFacRecord(Channel.name);
                record.Code = $"姓名:{name} 号码:{uniqueId}";
            }
            if (checkInType == IDType.ID)
            {
                record = Record.GetIDRecord(Channel.name);
                record.Code = $"姓名:{name} 号码:{uniqueId}";
            }
            if (checkInType == IDType.BarCode)
            {
                record = Record.GetQRRecord(Channel.name);
                record.Code = uniqueId;
            }

            record.Time = sw.ElapsedMilliseconds + "ms";
            byte personCount = content?.personCount.ToByte() ?? 0;
            if (content?.code == 100)
            {
                //listlog.Add(string.Format("请通行->{0}人次", personCount));
            }

            if (name.Length > 0)
            {
                name = name.Substring(0, 1).PadRight(name.Length, '*');
            }

            if (intentType == IntentType.In && content?.code == 100)
            {
                record.StatuCode = 0;
                record.Status = content?.message;
                Channel.daycount = (Channel.daycount.ToInt32() + 1).ToString();
                Channels.Save();
                //GateHelper.Open(Channel.comserver);

                am.Line1 = In_Ok + " " + name;
                am.Line2 = Line2_Ok_Tip;
                am.DayCount = Channel.daycount.ToInt32();

                gateServer.EnterOpen(0);
            }
            if (intentType == IntentType.In && content?.code != 100)
            {
                //禁止通行
                record.StatuCode = 1;
                record.Status = content.message;

                am.Line1 = Line2_Failure_Tip + " " + name;
                am.Line2 = content.message;
                am.DayCount = Channel.daycount.ToInt32();
            }
            PadHelper.SendToPad(Channel.pad, am);
            LogHelper.Append(record);
        }

        public void Open()
        {
            this.gateServer.EnterOpen(0);
        }

        public void Dispose()
        {
            idreader?.Close();
            gateServer?.Stop();
            faceSocket?.Disconnect();
        }
    }
}
