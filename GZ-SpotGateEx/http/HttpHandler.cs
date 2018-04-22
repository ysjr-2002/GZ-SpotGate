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

        public string getInitClientIp(HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                var stream = request.InputStream;
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                InitQuery temp = JsonConvert.DeserializeObject<InitQuery>(content);
                return temp.ip;
            }
            else
            {
                return string.Empty;
            }
        }

        public string getChannelNo(HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                var stream = request.InputStream;
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                ChannelQuery temp = JsonConvert.DeserializeObject<ChannelQuery>(content);
                return temp.channelno;
            }
            else
            {
                return string.Empty;
            }
        }



        public VerifyQuery getVerifyParam(HttpListenerRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                var stream = request.InputStream;
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string content = reader.ReadToEnd();
                VerifyQuery temp = JsonConvert.DeserializeObject<VerifyQuery>(content);
                return temp;
            }
            else
            {
                return null;
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
                    var clientIp = getInitClientIp(request);
                    var channel = getChannelByIp(clientIp);
                    var bytes = getInitBytes(channel);
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_verify))
                {
                    var verify = getVerifyParam(request);
                    var channelno = verify.channelno;
                    var idtype = verify.idtype;
                    var code = verify.code;
                    Console.WriteLine("channelno->" + channelno);
                    Console.WriteLine("idtype->" + idtype);
                    Console.WriteLine("code->" + code);
                    var channelcontroller = MyStandardKernel.Instance.Get<MainViewModel>().getChannelControler(channelno);
                    byte[] bytes = null;
                    if (channelcontroller != null)
                    {
                        var a = (IDType)idtype.ToInt32();
                        var feedback = channelcontroller.Check(a, code).Result;
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
                        bytes = getLoginBytes(-1, "未找到编号对应的通道");
                    }
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_calccount))
                {
                    var channelno = getChannelNo(request);
                    Console.WriteLine("channelno->" + channelno);
                    MyStandardKernel.Instance.Get<MainViewModel>().getChannelControler(channelno)?.Report();
                    var bytes = getLoginBytes();
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
                else if (rawUrl.StartsWith(HttpConstrant.suffix_heartbeat))
                {
                    var channelno = getChannelNo(request);
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
                result = new InitResult { code = 0, channelno = channel.No, holdopen = channel.HoldOpen ? 1 : 0, datetime = DateTime.Now.ToStandard(), shutdowntime = "12:00:00" };
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
