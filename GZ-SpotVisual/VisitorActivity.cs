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
using System.Net;
using Android.Graphics;
using System.Threading.Tasks;

namespace GZ_SpotVisual
{
    [Activity(Label = "@string/ApplicationName")]
    public class VisitorActivity : RootActivity
    {
        TextView tvWelcome;
        TextView tvState;
        ImageView faceImage;
        Bitmap facebm;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.visitor);

            faceImage = FindViewById<ImageView>(Resource.Id.faceImage);
            tvWelcome = FindViewById<TextView>(Resource.Id.tvWelcome);
            tvState = FindViewById<TextView>(Resource.Id.tvState);
        }

        protected override void OnStart()
        {
            base.OnStart();

            var content = Intent.GetStringExtra("am");
            AndroidMessage am = null;
            try
            {
                am = Newtonsoft.Json.JsonConvert.DeserializeObject<AndroidMessage>(content);
                if (am?.Code == 100)
                {
                    if (am.CheckInType == CheckIntype.Face)
                        ShowFace(am.Avatar);
                    else
                    {
                        faceImage.SetImageResource(Resource.Drawable.yes);
                    }
                }
                else
                {
                    faceImage.SetImageResource(Resource.Drawable.no);
                }
                tvWelcome.Text = am?.Line1;
                tvState.Text = am?.Line2;
            }
            catch { }

            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(am?.Delay ?? 1000);
                StartActivity(typeof(MainActivity));
                Finish();
            });
        }

        private byte[] DownImage(string url)
        {
            WebClient webclient = new WebClient();
            return webclient.DownloadData(url);
        }

        private async void ShowFace(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            facebm = await getFaceBitmap(url);
            faceImage.SetImageBitmap(facebm);
        }

        private Task<Bitmap> getFaceBitmap(string url)
        {
            return Task.Factory.StartNew(() =>
            {
                var data = DownImage(url);
                var bitmap = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                return bitmap;
            });
        }

        protected override void OnDestroy()
        {
            facebm?.Recycle();
            facebm?.Dispose();
            facebm = null;
            GC.Collect();
            base.OnDestroy();
        }
    }
}