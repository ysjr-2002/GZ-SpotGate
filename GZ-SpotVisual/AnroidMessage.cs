using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GZ_SpotVisual
{
    class AndroidMessage
    {
        public CheckIntype CheckInType { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
    }

    enum CheckIntype : int
    {
        /// <summary>
        /// 二维码
        /// </summary>
        BarCode = 1,
        /// <summary>
        /// 身份证
        /// </summary>
        ID = 2,
        /// <summary>
        /// IC卡(内部人员使用)
        /// </summary>
        IC = 3,
        /// <summary>
        /// 人脸
        /// </summary>
        Face = 4
    }
}