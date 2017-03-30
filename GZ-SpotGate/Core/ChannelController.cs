using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    /// <summary>
    /// 通道控制器
    /// </summary>
    class ChannelController
    {
        private string InGateIp;
        private string outGateIp;
        private string InComIp;
        private string OutComIp;

        private FaceSocket _inFaceSocket = null;
        private FaceSocket _outFaceSocket = null;

        private Request _request = null;

        private IntentType _intentType = IntentType.In;

        public async void Init()
        {
            _request = new Request();

            _inFaceSocket = new FaceSocket("", "", FaceIn);
            await _inFaceSocket.Connect();

            _outFaceSocket = new FaceSocket("", "", FaceOut);
            await _outFaceSocket.Connect();
        }

        public async void Work(CheckIntype checkIntype, string code)
        {
            await _request.CheckIn(checkIntype, code);
            if (_intentType == IntentType.In)
            {
                //进入
            }
            else
            {
                //离开
            }
        }

        public void Stop()
        {
            _inFaceSocket?.Disconnect();
            _outFaceSocket?.Disconnect();
        }

        public bool ContainIp(string ip)
        {
            if (InComIp == ip)
            {
                _intentType = IntentType.In;
                return true;
            }
            else if (OutComIp == ip)
            {
                _intentType = IntentType.Out;
                return true;
            }
            return false;
        }

        private void FaceIn(FaceRecognized face)
        {
            _intentType = IntentType.In;
            var code = face.person.job_number;
            Work(CheckIntype.Face, code);
        }

        private void FaceOut(FaceRecognized face)
        {
            _intentType = IntentType.Out;
            var code = face.person.job_number;
            Work(CheckIntype.Face, code);
        }

        public enum IntentType
        {
            In,
            Out,
        }
    }
}
