using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using System.Net;
using Android.Views.Animations;
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

        private HttpSocket hs = null;

        private static int Delay = 1000;

        private View vistor = null;
        private TextView tv;
        private TextView tvName;
        private ImageView ivFace;
        private const int p_width = 700;
        private const int p_height = 500;

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
            tv = this.FindViewById<TextView>(Resource.Id.tvWecomeEmp);
            tvName = this.FindViewById<TextView>(Resource.Id.tvName);

            var settingTextView = FindViewById<TextView>(Resource.Id.settingTextView);
            settingTextView.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SettingActivity));
                StartActivity(intent);
            };
        }

        public String getHostIp()
        {
            try
            {
                var name = Dns.GetHostName();
                IPAddress[] ips = Dns.GetHostAddresses(name);
                if (ips == null)
                    return string.Empty;

                foreach (var item in ips)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        return item.ToString();
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

            //Showtime();
            //StartTimer();

            var ip = getHostIp();
            tvCopyright.Text = ip + "-V1.0 ";

            hs = new HttpSocket(this);
            hs.SetCallback(ReceiveServer);
            hs.Connect(Config.Profile.ServerIp);
        }

        protected override void OnPause()
        {
            //转后台后，触发OnPause事件
            base.OnPause();
        }

        private void ReceiveServer(AndroidMessage am)
        {
            Bitmap faceImage = null;
            if (!string.IsNullOrEmpty(am.Avatar))
            {
                var url = am.Avatar;
                faceImage = getFaceBitmap(url);
            }
            Delay = am.Delay;
            ShowFace(am, faceImage);
        }

        private void ShowFace(AndroidMessage am, Bitmap faceImage)
        {
            RunOnUiThread(() =>
            {
                var lp = vistor.LayoutParameters;
                lp.Width = p_width;
                lp.Height = p_height;
                vistor.LayoutParameters = lp;

                //tv.Text = am.Message;

                tvName.Text = am.Message;
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
            base.OnDestroy();
            timer?.Stop();
            hs?.Close();
        }
    }
}

