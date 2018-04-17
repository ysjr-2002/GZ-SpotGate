using GZ_SpotGateEx.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.http
{
    class HttpHandler
    {
        HttpListenerContext httpContext;

        public HttpHandler(HttpListenerContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public Task RunAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                var request = httpContext.Request;
                var response = httpContext.Response;
                var url = request.Url;
                string rawUrl = request.RawUrl;
                if (rawUrl.StartsWith(HttpConstrant.suffix_init))
                {
                    var clientIp = request.QueryString["ip"];
                    var channel = getChannelByIp(clientIp);
                    var bytes = getInitBytes(channel);
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_verify))
                {
                    var channelno = request.QueryString["channelno"];
                    var idtype = request.QueryString["idtype"];
                    var code = request.QueryString["code"];
                    Console.WriteLine("channelno->" + channelno);
                    Console.WriteLine("idtype->" + idtype);
                    Console.WriteLine("code->" + code);
                    var bytes = getLoginBytes();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_calccount))
                {
                    var channelno = request.QueryString["channelno"];
                    var inouttype = request.QueryString["inouttype"];
                    Console.WriteLine("channelno->" + channelno);
                    Console.WriteLine("inouttype->" + inouttype);
                    var bytes = getLoginBytes();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_heartbeat))
                {
                    var channelno = request.QueryString["channelno"];
                    Console.WriteLine("channelno->" + channelno);
                    var bytes = getHeartbeatBytes();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
            });
        }

        static Channel getChannelByIp(string clientIp)
        {
            Channel channel = Channels.ChannelList.Find(s => s.ClientIp == clientIp);
            return channel;
        }

        private static byte[] getInitBytes(Channel channel)
        {
            InitResult result = null;
            if (channel != null)
            {
                result = new InitResult { code = 0, channelno = channel.No, inhold = channel.HoldIn ? 1 : 0, outhold = channel.HoldOut ? 1 : 0, datetime = DateTime.Now.ToStandard(), shutdowntime = "12:00:00" };
            }
            else
            {
                result = new InitResult { code = -1, message = "通道不存在" };
            }
            string json = JsonConvert.SerializeObject(result);
            return Encoding.UTF8.GetBytes(json);
        }

        private static byte[] getLoginBytes()
        {
            CheckResult back = new CheckResult { code = 0, message = "ok" };
            string json = JsonConvert.SerializeObject(back);
            return Encoding.UTF8.GetBytes(json);
        }

        private static byte[] getHeartbeatBytes()
        {
            HeartbeatResult back = new HeartbeatResult { code = 0, datetime = DateTime.Now.ToString() };
            string json = JsonConvert.SerializeObject(back);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
