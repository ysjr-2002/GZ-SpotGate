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
        private Request _request = null;
        public ChannelController(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }

        public void Start()
        {
            faceSocket = new FaceSocket(Channel, OnFaceRecognize);
            faceSocket.Connect();

            idreader = new IDReader(Channel.comserver);
            idreader.Run();
            idreader.SetDataCallback(OnReaderID);

            _request = new Request();
        }

        internal async void OnReaderID(IDModel model)
        {
            await Check(IntentType.In, IDType.ID, model.IDCard, model.Name);
        }

        internal async void OnFaceRecognize(FaceRecognized face)
        {
            await Check(IntentType.In, IDType.Face, face.person.job_number, face.person.name, face.person.avatar);
        }

        internal async void OnQRCode(string barCode)
        {
            await Check(IntentType.In, IDType.BarCode, barCode);
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
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var content = await _request.CheckIn(this.Channel.ChannelVirualIp, checkInType, uniqueId);
            sw.Stop();

            AndroidMessage am = new AndroidMessage();
            if (checkInType == IDType.Face)
            {
                am = AndroidMessage.GetFaceYes(avatar);
                record = Record.GetFacRecord(Channel.name);
            }
            if (checkInType == IDType.ID)
            {
                am = AndroidMessage.GetIDYes(name);
                record = Record.GetIDRecord(Channel.name);
            }
            if (checkInType == IDType.BarCode)
            {
                am = AndroidMessage.GetQRYes();
                record = Record.GetQRRecord(Channel.name);
            }
            record.Time = sw.ElapsedMilliseconds + "ms";

            byte personCount = content?.personCount.ToByte() ?? 0;
            if (content?.code == 100)
            {
                //listlog.Add(string.Format("请通行->{0}人次", personCount));
                record.StatuCode = 0;
            }
            else
            {
                //禁止通行
                record.StatuCode = 1;
                record.PostMessage = content.message;
            }

            if (intentType == IntentType.In && content?.code == 100)
            {
                GateHelper.Open(Channel.comserver);
                PadHelper.SendToPad(Channel.pad, am);
            }
            if (intentType == IntentType.In && content?.code != 100)
            {
                am.Status = 1;
                PadHelper.SendToPad(Channel.pad, am);
            }

            LogHelper.Append(record);
        }
    }
}
