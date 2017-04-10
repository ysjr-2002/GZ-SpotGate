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
    //[BroadcastReceiver]
    [IntentFilter(new string[1] { Intent.ActionBootCompleted })]
    class BootReceiver : BroadcastReceiver
    {
        protected BootReceiver(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            //配置AndroidManifest.xml权限, Receiver_Boot_Completed
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                Intent newIntent = new Intent(context, typeof(SplashActivity));
                newIntent.AddFlags(ActivityFlags.NewTask);  //这个标记必须加
                context.StartActivity(newIntent);
            }
        }
    }
}