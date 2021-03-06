﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GZ_SpotVisual
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon")]
    public class MainActivity : RootActivity
    {
        private TextView tvWelcome;
        private TextView tvTime;
        private TextView tvCopyright;
        private System.Timers.Timer timer = null;

        private static int Delay = 1000;

        private View vistor = null;
        private TextView tvName;
        private ImageView ivFace;
        private const int p_width = 700;
        private const int p_height = 500;

        public const int WEBSOCKET_OK = 2000;
        public const int WEBSOCKET_CLOSE = 2001;
        public const int WEBSOCKET_ERROR = 2002;
        public const int WEBSOCKET_DATA = 2003;

        public static Handler handler = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.FaceMain);

            tvWelcome = this.FindViewById<TextView>(Resource.Id.tvWelcome);
            tvTime = this.FindViewById<TextView>(Resource.Id.tvTime);
            tvCopyright = this.FindViewById<TextView>(Resource.Id.tvCopyright);
            //vistor = this.FindViewById<LinearLayout>(Resource.Id.alter);
            ivFace = this.FindViewById<ImageView>(Resource.Id.faceImage);
            //tvName = this.FindViewById<TextView>(Resource.Id.tvName);

            var settingTextView = FindViewById<TextView>(Resource.Id.settingTextView);
            settingTextView.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SettingActivity));
                StartActivity(intent);
            };

            tvWelcome.Text = Config.Profile.Welcome;
            var address = IPUtil.GetHostIp();
            var address1 = IPUtil.getIP(this);
            tvCopyright.Text = "终端：" + address + "  版本：V1.0 ";
        }

        private void ShowToast(string info)
        {
            Toast.MakeText(this, info, ToastLength.Short).Show();
        }

        protected override void OnStart()
        {
            base.OnStart();

            //handler = new Handler((message) =>
            //{
            //    switch (message.What)
            //    {
            //        case WEBSOCKET_DATA:
            //            //ShowToast("来数据了");
            //            break;
            //        case WEBSOCKET_OK:
            //            //ShowToast("连接成功");
            //            break;
            //        case WEBSOCKET_CLOSE:
            //            //ShowToast("连接失败");
            //            break;
            //    }
            //});
        }

        protected override void OnPause()
        {
            //转后台后，触发OnPause事件
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            timer?.Stop();
            timer = null;
            StopService(new Intent(this, typeof(WebSocketService)));
            base.OnDestroy();
        }
    }
}

