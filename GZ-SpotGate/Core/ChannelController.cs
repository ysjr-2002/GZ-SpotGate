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

        public void Init()
        {
            _request = new Request();
        }

        public void Work()
        {

        }

        public void Stop()
        {

        }
    }
}
