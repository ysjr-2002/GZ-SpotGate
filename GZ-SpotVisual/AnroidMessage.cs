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

        public IntentType IntentType { get; set; }

        public string Message { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public int Delay { get; set; }
    }

    enum CheckIntype : int
    {
        IC,
        ID,
        Face,
        BarCode,
    }

    /// <summary>
    /// “‚Õº¿‡–Õ
    /// </summary>
    enum IntentType
    {
        In,
        Out,
    }
}