using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace GZ_SpotVisual
{
    [Activity(Label = "GZ_SpotVisual", Icon = "@drawable/icon")]
    public class MainActivity : RootActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FaceMain);

            var a = RequestedOrientation;
            RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
        }

        protected override void OnStart()
        {
            base.OnStart();
            //HttpSocket hs = new HttpSocket(this);
            //hs.Connect("", "");
        }
    }
}

