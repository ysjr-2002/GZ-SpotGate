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
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FaceMain);
        }
    }
}

