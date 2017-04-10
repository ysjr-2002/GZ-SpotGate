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
            //����AndroidManifest.xmlȨ��, Receiver_Boot_Completed
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                Intent newIntent = new Intent(context, typeof(SplashActivity));
                newIntent.AddFlags(ActivityFlags.NewTask);  //�����Ǳ����
                context.StartActivity(newIntent);
            }
        }
    }
}