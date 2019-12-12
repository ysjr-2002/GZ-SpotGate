using GZSpotGate.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HXMansion.Http
{
    /// <summary>
    /// 启动一个Http服务，供客户调用
    /// </summary>
    public class HttpServer
    {
        bool stop = false;
        Thread thread = null;
        HttpListener listener = null;
        Config config;

        public HttpServer()
        {
            this.config = Config.Instance;
            listener = new HttpListener();
        }

        public string ServerFullPath { get; set; }

        //string temp = "http://ip:10001/api/v1/recognize/";
        public void Start()
        {
            var serverIp = config.LocalIp;
            var port = 10001;
            try
            {
                var recognize = "http://" + serverIp + ":" + port + HttpConstrant.suffix_recognize;
                listener.Prefixes.Add(recognize);
                listener.Start();
                LogHelper.Log("httpserver->" + recognize);
            }
            catch (HttpListenerException ex)
            {
                LogHelper.Log("startup httpserver exception->" + ex.Message);
                return;
            }

            thread = new Thread(Listener);
            thread.Start();
        }

        private void Listener()
        {
            while (!stop)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    new HttpHandler(context).RunAsync();
                }
                catch (Exception ex)
                {
                    if (!stop)
                        LogHelper.Log("process request error->" + ex.Message);
                }
            }
        }

        public void Stop()
        {
            stop = true;
            try
            {
                listener?.Stop();
                if (thread != null)
                    thread.Abort();
            }
            catch (Exception ex)
            {
            }
        }
    }
}



