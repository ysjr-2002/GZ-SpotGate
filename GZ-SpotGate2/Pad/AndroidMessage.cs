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
        public IDType CheckInType { get; set; }

        public InOutType IntentType { get; set; }

        public bool Result { get; set; }

        public string Line1 { get; set; }

        public string Line2 { get; set; }

        public string Avatar { get; set; }

        public int Delay { get; set; }

        public int Code { get; set; }

        public int DayCount { get; set; }

        public static AndroidMessage GetIDYes(string name, string message = "请入园")
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.ID;
            temp.Delay = Config.Instance.PadDelay;
           
            return temp;
        }

        public static AndroidMessage GetIDNotOpen(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.ID;
            temp.Delay = Config.Instance.PadDelay;
          
            return temp;
        }

        public static AndroidMessage GetFaceYes(string avatar, string message = "请入园")
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.Face;
            temp.Delay = Config.Instance.PadDelay;
            temp.Avatar = avatar;
            return temp;
        }

        public static AndroidMessage GetFaceNO(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.Face;
            temp.Delay = Config.Instance.PadDelay;
            return temp;
        }

        public static AndroidMessage GetQRYes()
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            return temp;
        }

        public static AndroidMessage GetQRNO(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            return temp;
        }

        public static AndroidMessage GetNoOpen(string message)
        {
            AndroidMessage temp = new AndroidMessage();
            temp.CheckInType = IDType.BarCode;
            temp.Delay = Config.Instance.PadDelay;
            return temp;
        }
    }
}
