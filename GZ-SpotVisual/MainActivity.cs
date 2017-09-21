using Android.App;
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
            vistor = this.FindViewById<LinearLayout>(Resource.Id.alter);
            ivFace = this.FindViewById<ImageView>(Resource.Id.faceImage);
            tvName = this.FindViewById<TextView>(Resource.Id.tvName);

            var settingTextView = FindViewById<TextView>(Resource.Id.settingTextView);
            settingTextView.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SettingActivity));
                StartActivity(intent);
            };
        }

        private void ShowToast(string info)
        {
            Toast.MakeText(this, info, ToastLength.Short).Show();
        }

        public String GetHostIp()
        {
            try
            {
                var hostname = Dns.GetHostName();
                IPAddress[] ipaddresses = Dns.GetHostAddresses(hostname);
                if (ipaddresses == null)
                    return string.Empty;

                foreach (IPAddress address in ipaddresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        return address.ToString();
                }
            }
            catch (Exception e)
            {
            }
            return string.Empty;
        }

        protected override void OnStart()
        {
            base.OnStart();

            tvWelcome.Text = Config.Profile.Welcome;
            var address = GetHostIp();
            tvCopyright.Text = "终端：" + address + "  版本：V1.0 ";

            handler = new Handler((message) =>
            {
                switch (message.What)
                {
                    case WEBSOCKET_OK:
                        ShowToast("连接成功");
                        break;
                    case WEBSOCKET_CLOSE:
                        ShowToast("连接失败");
                        break;
                }
            });
        }

        protected override void OnPause()
        {
            //转后台后，触发OnPause事件
            base.OnPause();
        }

        private void ReceiveFromServer(AndroidMessage am)
        {
            //Bitmap faceImage = null;
            //if (!string.IsNullOrEmpty(am.Avatar))
            //{
            //    var url = am.Avatar;
            //    faceImage = getFaceBitmap(url);
            //}
            //Delay = am.Delay;
            //ShowFace(am, faceImage);
        }

        private void ShowFace(AndroidMessage am, Bitmap faceImage)
        {
            RunOnUiThread(() =>
            {
                var lp = vistor.LayoutParameters;
                lp.Width = p_width;
                lp.Height = p_height;
                vistor.LayoutParameters = lp;

                tvName.Text = am.Line1;
                tvName.SetTextColor(Color.Rgb(255, 106, 00));

                ivFace.SetImageBitmap(faceImage);
                faceImage.Dispose();
                var sa = AnimationUtils.LoadAnimation(this, Resource.Animation.scale);
                sa.AnimationEnd += Sa_AnimationEnd;
                vistor.StartAnimation(sa);
            });
        }

        private void Sa_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            Thread.Sleep(Delay);
            var sa = AnimationUtils.LoadAnimation(this, Resource.Animation.translate);
            vistor.StartAnimation(sa);
        }

        private byte[] DownImage(string url)
        {
            WebClient webclient = new WebClient();
            return webclient.DownloadData(url);
        }

        private Bitmap getFaceBitmap(string url)
        {
            var data = DownImage(url);
            var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
            return bitmap;
        }

        private void StartTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Start();
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Showtime();
        }

        private void Showtime()
        {
            RunOnUiThread(new Action(() =>
            {
                tvTime.Text = DateTime.Now.ToString("HH:mm:ss");
            }));
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

