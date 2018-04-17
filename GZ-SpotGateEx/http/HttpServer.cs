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
            listener.Prefixes.Add(HttpConstrant.url_init);
            listener.Prefixes.Add(HttpConstrant.url_verify);
            listener.Prefixes.Add(HttpConstrant.url_calccount);
            listener.Prefixes.Add(HttpConstrant.url_heartbeat);
        }

        public void start()
        {
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
