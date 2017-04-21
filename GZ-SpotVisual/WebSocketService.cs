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
    [Service]
    class WebSocketService : Service
    {
        HttpSocket hs = null;
        bool isStarted = false;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if (isStarted)
            {

            }
            else
            {
                //Æô¶¯·þÎñ
                hs = new HttpSocket(this);
                hs.SetCallback(ReceiveServer);
                hs.Connect(Config.Profile.ServerIp);
                isStarted = true;
            }
            return base.OnStartCommand(intent, flags, startId);
        }

        private void ReceiveServer(string jsonMessage)
        {
            Intent intent = new Intent(this, typeof(VisitorActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.PutExtra("am", jsonMessage);
            StartActivity(intent);            
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            hs?.Close();
            hs = null;
            base.OnDestroy();
        }
    }
}