using GZSpotGate.Core;
using GZSpotGate.ViewModel;
using LL.SenicSpot.Gate.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tengli.Core;

namespace HXMansion.Http
{
    public class HttpHandler
    {
        readonly HttpListenerContext httpContext;
        HttpListenerRequest request;
        HttpListenerResponse response;

        public HttpHandler(HttpListenerContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public Task RunAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                request = httpContext.Request;
                response = httpContext.Response;
                var rawUrl = request.RawUrl;
                response.StatusCode = 200;
                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;
                response.AddHeader("Access-Control-Allow-Methods", "OPTIONS,POST,GET");
                response.AddHeader("Access-Control-Allow-Headers", "X-Requested-With,accept, origin, content-type");
                response.AddHeader("Access-Control-Allow-Origin", "*");
                var ip = httpContext.Request.RemoteEndPoint.Address.ToString();
                var bytes = HttpResult.GetError("未注册的请求");
                if (request.HttpMethod == HttpMethod.Post.ToString())
                {
                    if (rawUrl.StartsWith(HttpConstrant.suffix_recognize))
                    {
                        //人脸识别
                        rlsb();
                    }
                }
                else
                {
                    response.OutputStream.Write(bytes, 0, bytes.Length);
                    response.Close();
                }
            });
        }

        /// <summary>
        /// 人脸识别
        /// </summary>
        private void rlsb()
        {
            var stream = request.InputStream;
            var reader = new StreamReader(stream, Encoding.UTF8);
            var content = reader.ReadToEnd();
            reader.Close();
            var subjectId = 0;
            PushFace pushFace = null;
            try
            {
                pushFace = JsonConvert.DeserializeObject<PushFace>(content);
                if (pushFace == null || pushFace.subject_id.ToInt32() <= 0) //subject NULL,-1,""
                {
                    LogHelper.AppendWXLog("未识别 subjectId=" + pushFace?.subject_id);
                    return;
                }
                var screen = KoalaCore.KoalaHelper.Instance.ScreenList.FirstOrDefault(s => s.screen_token == pushFace.screen_token);
                if (screen == null)
                {
                    var record = Record.GetError("", "token未绑定:" + pushFace.screen_token);
                    LogHelper.Append(record);
                    return;
                }
                var channel = Channels.ChannelList.FirstOrDefault(s => s.camera == screen.camera);
                if (channel == null)
                {
                    var record = Record.GetError("", "camera地址错误:" + screen.camera);
                    LogHelper.Append(record);
                    return;
                }

                subjectId = pushFace.subject_id.ToInt32();
                var subject = GetSubject(subjectId);
                if (subject == null)
                {
                    LogHelper.AppendWXLog("获取用户信息失败");
                    return;
                }
                var isOk = FaceCache.IsOutInterval(subjectId);
                if (isOk)
                {
                    var controller = MainWindowViewModel.Instance.controllers.First(s => s.Channel.camera == channel.camera);
                    if (controller != null)
                    {
                        controller.OnFaceRecognize(new GZSpotGate.Face.FaceRecognized
                        {
                            person = new GZSpotGate.Face.Employee
                            {
                                avatar = "",
                                name = subject.name,
                                description = subject.description
                            }
                        });
                    }
                }
                else
                {
                    var record = Record.GetError(channel.name, Config.Instance.Interval + "秒内同一人脸");
                    LogHelper.Append(record);
                }
            }
            catch (Exception ex)
            {
                var data = JsonConvert.SerializeObject(pushFace);
                LogHelper.AppendWXLog("query subject error->" + ex.Message);
                LogHelper.AppendWXLog("query subject error->" + data);
            }
            finally
            {
                response.Close();
            }
        }

        private KoalaCore.Subject GetSubject(int subjectId)
        {
            var index = 1;
            var flag = true;
            var url = KoalaCore.KoalaConstrant.subject_url + "/" + subjectId;
            var content = KoalaCore.KoalaHelper.Instance.Get(url).Trim();
            while ((content.IsEmpty() || content.StartsWith("<!")) && index < 4)
            {
                LogHelper.AppendWXLog($"try->{index}");
                //session失效
                flag = KoalaCore.KoalaHelper.Instance.Login().Result;
                if (flag)
                {
                    content = KoalaCore.KoalaHelper.Instance.Get(url).Trim();
                    break;
                }
                index++;
            }
            if (flag)
            {
                var response = JsonConvert.DeserializeObject<KoalaCore.KoalaResponse<KoalaCore.Subject>>(content);
                return response.data;
            }
            else
            {
                return null;
            }
        }
    }
}
