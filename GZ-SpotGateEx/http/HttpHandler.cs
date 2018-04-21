using GZ_SpotGateEx.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using GZ_SpotGateEx.ViewModel;
using GZ_SpotGateEx.Model;
using System.IO;

namespace GZ_SpotGateEx.http
{
    class HttpHandler
    {
        HttpListenerContext httpContext;

        public HttpHandler(HttpListenerContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public string getInputStream(Stream stream)
        {
            if (stream != null)
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                return content;
            }
            else
            {
                return string.Empty;
            }
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
                    var content = getInputStream(request.InputStream);
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
                    var inouttype = request.QueryString["inouttype"];
                    var code = request.QueryString["code"];
                    Console.WriteLine("channelno->" + channelno);
                    Console.WriteLine("idtype->" + idtype);
                    Console.WriteLine("inouttype->" + inouttype);
                    Console.WriteLine("code->" + code);
                    var channelcontroller = MyStandardKernel.Instance.Get<MainViewModel>().getChannelControler(channelno);
                    byte[] bytes = null;
                    if (channelcontroller != null)
                    {
                        var a = (IDType)idtype.ToInt32();
                        var b = (IntentType)inouttype.ToInt32();
                        var feedback = channelcontroller.Check(b, a, code).Result;
                        if (feedback?.code == 100)
                        {
                            bytes = getLoginBytes();
                        }
                        else
                        {
                            bytes = getLoginBytes(feedback.code, feedback.message, feedback.personCount.ToInt32());
                        }
                    }
                    else
                    {
                        bytes = getLoginBytes(-1, "为找到编号对应的通道");
                    }
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_calccount))
                {
                    var channelno = request.QueryString["channelno"];
                    var inouttype = request.QueryString["inouttype"];
                    Console.WriteLine("channelno->" + channelno);
                    Console.WriteLine("inouttype->" + inouttype);
                    MyStandardKernel.Instance.Get<MainViewModel>().getChannelControler(channelno)?.Report(inouttype);
                    var bytes = getLoginBytes();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_heartbeat))
                {
                    var channelno = request.QueryString["channelno"];
                    Console.WriteLine("channelno->" + channelno);
                    //更新心跳
                    MyStandardKernel.Instance.Get<MainViewModel>().getChannelControler(channelno)?.UpdateHeartbeat();
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

        private static byte[] getLoginBytes(int code = 0, string message = "ok", int personcount = 0)
        {
            CheckResult back = new CheckResult { code = code, message = message, entrycount = personcount };
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
