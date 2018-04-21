using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    class HttpServer
    {
        HttpListener listener;
        Thread thread;
        bool bStop = false;

        public HttpServer()
        {
            listener = new HttpListener();
        }

        public void start()
        {
            string ip = Common.Utility.GetHostIpAddress();
            listener.Prefixes.Add(string.Format(HttpConstrant.url_init, ip));
            listener.Prefixes.Add(string.Format(HttpConstrant.url_verify, ip));
            listener.Prefixes.Add(string.Format(HttpConstrant.url_calccount, ip));
            listener.Prefixes.Add(string.Format(HttpConstrant.url_heartbeat, ip));
            listener.Start();
            thread = new Thread(accept);
            thread.Start();
        }

        private void accept()
        {
            while (!bStop)
            {
                try
                {
                    var context = listener.GetContext();
                    new HttpHandler(context).RunAsync();
                }
                catch (Exception)
                {
                }
            }
        }

        public void Stop()
        {
            bStop = true;
            listener.Stop();
        }
    }
}
