using GZ_SpotGateEx.Core;
using GZ_SpotGateEx.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GZ_SpotGateEx.Model
{
    class Record
    {
        private IDType idType;
        public string Channel { get; set; }
        public IDType IDType
        {
            get { return idType; }
            set
            {
                idType = value;
                if (idType == IDType.IC)
                    this.TypeImageSourceUrl = ImageConstrant.IC_ImageSource;
                else if (idType == IDType.ID)
                    this.TypeImageSourceUrl = ImageConstrant.ID_ImageSource;
                else if (idType == IDType.BarCode)
                    this.TypeImageSourceUrl = ImageConstrant.QR_ImageSource;
                else if (idType == IDType.Face)
                    this.TypeImageSourceUrl = ImageConstrant.FACE_ImageSource;
            }
        }
        public InOutType IntentType { get; set; }
        /// <summary>
        /// 凭证编号
        /// </summary>
        public string Code { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string PassTime { get; set; }
        public int StatuCode { get; set; }
        public string TypeImageSourceUrl { get; set; }

        private Record()
        {
        }

        public static Record GetInitRecrod()
        {
            Record record = new Record();
            record.IDType = IDType.Init;
            record.PassTime = DateTime.Now.ToStandard();
            return record;
        }

        public static Record getUpLoadRecord()
        {
            Record record = new Record();
            record.IDType = IDType.Upload;
            record.PassTime = DateTime.Now.ToStandard();
            record.Time = "0ms";
            return record;
        }

        public static Record getRecord(Channel channel)
        {
            Record record = new Record();
            record.Channel = channel.Name;
            record.PassTime = DateTime.Now.ToStandard();
            record.Time = "0ms";
            return record;
        }

        public void Output()
        {
            string name = "通道:" + Channel;
            string action = IntentType == InOutType.In ? "进入" : "离开";
            string type = "";
            string code = "编号:" + Code;
            string time = "时间:" + PassTime;
            string verify = "耗时:" + Time;

            if (IDType == IDType.BarCode)
                type = "二维码";
            else if (IDType == IDType.ID)
                type = "身份证";
            else if (IDType == IDType.IC)
                type = "IC卡";
            else if (IDType == IDType.Face)
                type = "人脸";
            else if (IDType == IDType.Upload)
                type = "计数";
            else if (IDType == IDType.Init)
            {
                MyLog.debug(Status);
                return;
            }

            MyLog.debug(name);
            MyLog.debug(action);
            MyLog.debug(type);
            MyLog.debug(code);
            MyLog.debug(time);
            MyLog.debug(verify);
        }
    }
}
