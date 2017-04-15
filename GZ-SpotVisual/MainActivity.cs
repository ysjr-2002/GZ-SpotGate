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
    [Activity(Label = "GZ_SpotVisual", Icon = "@drawable/icon")]
    public class MainActivity : RootActivity
    {
        private TextView tvWelcome;
        private TextView tvCopyright;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FaceMain);
            //RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            tvWelcome = this.FindViewById<TextView>(Resource.Id.tvWelcome);
            tvCopyright = this.FindViewById<TextView>(Resource.Id.tvCopyright);
            vistor = this.FindViewById<LinearLayout>(Resource.Id.alter);
            ivFace = this.FindViewById<ImageView>(Resource.Id.faceImage);
            tv = this.FindViewById<TextView>(Resource.Id.tvWecomeEmp);
            tvName = this.FindViewById<TextView>(Resource.Id.tvName);

            tvWelcome.Text = "欢迎光临";
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

            var ip = getHostIp();
            tvCopyright.Text = ip + "-V1.0";

            HttpSocket hs = new HttpSocket(this);
            hs.SetCallback(ReceiveServer);
            hs.Connect("192.168.0.4");
        }

        protected override void OnPause()
        {
            //转后台后，触发OnPause事件
            base.OnPause();
        }

        private void ReceiveServer(AndroidMessage am)
        {
            var name = "";
            Bitmap faceImage = null;

            name = am.Name;
            if (!string.IsNullOrEmpty(am.Avatar))
            {
                var url = am.Avatar;
                faceImage = getFaceBitmap(url);
            }
            ShowFace(name, faceImage);
        }

        private View vistor = null;
        private TextView tv;
        private TextView tvName;
        private ImageView ivFace;
        private const int p_width = 500;
        private const int p_height = 600;

        private void ShowFace(string name, Bitmap faceImage)
        {
            RunOnUiThread(() =>
            {
                var lp = vistor.LayoutParameters;
                lp.Width = p_width;
                lp.Height = p_height;
                vistor.LayoutParameters = lp;
                tvName.Text = name;
                tvName.SetTextColor(Color.Rgb(255, 106, 00));
                tv.Text = Config.Profile.Welcome2;
                ivFace.SetImageBitmap(faceImage);
                faceImage.Dispose();
                var sa = AnimationUtils.LoadAnimation(this, Resource.Animation.scale);
                sa.AnimationEnd += Sa_AnimationEnd;
                vistor.StartAnimation(sa);
            });
        }

        private void Sa_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            Thread.Sleep(Config.Profile.Delay);
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
    }
}

