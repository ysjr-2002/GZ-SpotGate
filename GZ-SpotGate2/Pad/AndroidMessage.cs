using GZSpotGate.Core;
using GZSpotGate.Pad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.WS
{
    class AndroidMessage
    {
        public IDType IDType { get; set; }
        /// <summary>
        /// 验证状态 0:成功 1:失败
        /// </summary>
        public int Status { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public int Delay { get; set; }

        public int DayCount { get; set; }

        public static AndroidMessage GetIDYes(string name, string message = "请入园")
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.ID;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 0;
            temp.Message = message;
            return temp;
        }

        public static AndroidMessage GetIDNotOpen(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.ID;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 1;
            temp.Message = message;
            return temp;
        }

        public static AndroidMessage GetFaceYes(string avatar, string message = "请入园")
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.Face;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 0;
            temp.Avatar = avatar;
            temp.Message = message;
            return temp;
        }

        public static AndroidMessage GetFaceNO(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.Face;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 1;
            temp.Message = message;
            return temp;
        }

        public static AndroidMessage GetQRYes()
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 0;
            temp.Message = "请入园";
            return temp;
        }

        public static AndroidMessage GetQRNO(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 1;
            temp.Message = message;
            return temp;
        }

        public static AndroidMessage GetNoOpen(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.IDType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            temp.Status = 1;
            temp.Message = message;
            return temp;
        }
    }
}
