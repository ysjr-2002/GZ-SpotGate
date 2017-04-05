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
using System.Threading;
using Java.Net;

namespace GZ_SpotVisual
{
    [Activity(Label = "@string/ApplicationName", Theme = "@style/AppTheme", MainLauncher = true)]
    public class SplashActivity : RootActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            {
                this.RunOnUiThread(new Action(() =>
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    Finish();
                }));
            }));
        }
    }
}