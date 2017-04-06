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
        /// ��ά��
        /// </summary>
        BarCode = 1,
        /// <summary>
        /// ���֤
        /// </summary>
        ID = 2,
        /// <summary>
        /// IC��(�ڲ���Աʹ��)
        /// </summary>
        IC = 3,
        /// <summary>
        /// ����
        /// </summary>
        Face = 4
    }
}