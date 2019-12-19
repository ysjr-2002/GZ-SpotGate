using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tengli.Core;

namespace LL.SenicSpot.Gate.Model
{
    public class Record
    {
        public const string ID_ImageSource = "/GZ-SpotGate;component/Images/id.png";
        public const string FACE_ImageSource = "/GZ-SpotGate;component/Images/face.png";
        public const string QR_ImageSource = "/GZ-SpotGate;component/Images/qr.png";

        /// <summary>
        /// 通道名称
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// 凭证编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 凭证类型
        /// </summary>
        public IDType IDType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 验证耗时
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 通过时间
        /// </summary>
        public string PassTime { get; set; }
        /// <summary>
        /// 状态码 0:正常 1:失败
        /// </summary>
        public int StatuCode { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string TypeImageSource { get; set; }

        public BitmapSource FaceImage { get; set; }

        public static Record GetQRRecord(string channelname)
        {
            Record record = new Record();
            record.Channel = channelname;
            record.IDType = IDType.BarCode;
            record.PassTime = DateTime.Now.ToStandard();
            record.TypeImageSource = QR_ImageSource;
            record.Time = "0ms";
            return record;
        }

        public static Record GetFacRecord(string channelname)
        {
            Record record = new Record();
            record.Channel = channelname;
            record.IDType = IDType.Face;
            record.PassTime = DateTime.Now.ToStandard();
            record.TypeImageSource = FACE_ImageSource;
            record.Time = "0ms";
            return record;
        }

        public static Record GetIDRecord(string channelname)
        {
            Record record = new Record();
            record.Channel = channelname;
            record.IDType = IDType.ID;
            record.PassTime = DateTime.Now.ToStandard();
            record.TypeImageSource = ID_ImageSource;
            record.Time = "0ms";
            return record;
        }

        public static Record GetError(string name, string messsage)
        {
            Record record = new Record();
            record.Channel = name;
            record.StatuCode = 1;
            record.PassTime = DateTime.Now.ToStandard();
            record.Status = messsage;
            return record;
        }

        public void Output()
        {
            string name = "通道:" + Channel;
            string type = "";
            string code = "编号:" + Code;
            string time = "时间:" + PassTime;
            string verify = "耗时:" + Time;

            if (IDType == IDType.BarCode)
                type = "二维码";
            else if (IDType == IDType.Face)
                type = "人脸";
            if (IDType == IDType.ID)
                type = "身份证";

            type = "方式:" + type;
            LogHelper.Log(name);
            LogHelper.Log(type);
            LogHelper.Log(code);
            LogHelper.Log(time);
            LogHelper.Log(verify);
            if (Status.IsEmpty() == false)
                LogHelper.Log("验票:" + Status);
        }
    }
}
