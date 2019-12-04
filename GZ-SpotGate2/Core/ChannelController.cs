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
            await Check(IntentType.In, IDType.Face, face.person.job_number, face.person.name, face.person.avatar);
        }

        private async Task Check(IntentType intentType, IDType checkInType, string uniqueId, string name = "", string avatar = "")
        {
            Record record = null;
            var error = SecurityHelper.IsAuth();
            if (!error.IsEmpty())
            {
                record = Record.GetError(Channel.name, error);
                LogHelper.Append(record);
                return;
            }

            if (uniqueId.IsEmpty())
            {
                record = Record.GetError(Channel.name, "编号为空");
                LogHelper.Append(record);
                return;
            }
            Stopwatch sw = Stopwatch.StartNew();
            var content = await request.CheckIn(this.Channel.ChannelVirualIp, checkInType, uniqueId);
            content.code = 100;
            sw.Stop();

            AndroidMessage am = new AndroidMessage();
            if (checkInType == IDType.Face)
            {
                am = AndroidMessage.GetFaceYes(avatar);
                record = Record.GetFacRecord(Channel.name);
                record.Code = $"姓名:{name} 号码:{uniqueId}";
            }
            if (checkInType == IDType.ID)
            {
                am = AndroidMessage.GetIDYes(name);
                record = Record.GetIDRecord(Channel.name);
                record.Code = $"姓名:{name} 号码:{uniqueId}";
            }
            if (checkInType == IDType.BarCode)
            {
                am = AndroidMessage.GetQRYes();
                record = Record.GetQRRecord(Channel.name);
                record.Code = uniqueId;
            }

            record.Time = sw.ElapsedMilliseconds + "ms";
            Debug.WriteLine("hz:step4");
            byte personCount = content?.personCount.ToByte() ?? 0;
            if (content?.code == 100)
            {
                //listlog.Add(string.Format("请通行->{0}人次", personCount));
            }

            if (intentType == IntentType.In && content?.code == 100)
            {
                record.StatuCode = 0;
                record.PostMessage = content?.message;
                Channel.daycount = (Channel.daycount.ToInt32() + 1).ToString();
                Channels.Save();
                //GateHelper.Open(Channel.comserver);

                am.DayCount = Channel.daycount.ToInt32();
                //PadHelper.SendToPad(Channel.pad, am);

                gateServer.EnterOpen(0);
            }
            if (intentType == IntentType.In && content?.code != 100)
            {
                //禁止通行
                record.StatuCode = 1;
                record.PostMessage = content.message;

                am.Status = 1;
                am.DayCount = Channel.daycount.ToInt32();

                //PadHelper.SendToPad(Channel.pad, am);
            }

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
