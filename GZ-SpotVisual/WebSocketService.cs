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
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;

namespace GZ_SpotVisual
{
    [Service]
    class WebSocketService : Service
    {
        HttpSocket hs = null;
        bool isStarted = false;

        bool stop = false;
        UdpClient udp = null;
        IPEndPoint remoteIp = null;
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
                //hs = new HttpSocket(this);
                //hs.SetCallback(ReceiveServer);
                //hs.Connect(Config.Profile.ServerIp);

                udp = new UdpClient(9872, AddressFamily.InterNetwork);
                Task.Factory.StartNew(() =>
                {
                    while (!stop)
                    {
                        try
                        {
                            byte[] buffer = udp.Receive(ref remoteIp);
                            string message = System.Text.Encoding.UTF8.GetString(buffer);
                            ReceiveServer(message);
                        }
                        catch
                        {
                        }
                    }
                });

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

            stop = true;
            udp.Close();

            base.OnDestroy();
        }
    }
}