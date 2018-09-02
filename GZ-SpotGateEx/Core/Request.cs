using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using GZ_SpotGateEx.Model;
using log4net;
using System.Diagnostics;
using System.Threading;
using System.IO;
using GZ_SpotGateEx.http;
using Newtonsoft.Json;

namespace GZ_SpotGateEx.Core
{
    internal class Request
    {
        private static readonly ILog log = LogManager.GetLogger("Request");

        public async Task<FeedBack> CheckIn(string doorIp, IDType type, string code)
        {
            var url = ConfigProfile.Current.CheckInServerUrl + "?do=ticketface";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("doorip", doorIp);
            dict.Add("barcode", code);
            dict.Add("type", getTypeName(type));
            var postData = dict.LinkUrl();
            var content = await doRequest(url, postData);
            var feedback = content.Deserlizer<FeedBack>();
            return feedback;
        }

        public async Task<FaceOpenResult> Open(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                Debug.WriteLine("hz:url->" + url);
                Debug.WriteLine("hz:open gate result->" + content);
                if (content.IsEmpty())
                {
                    return new FaceOpenResult { code = -1, message = "返回值为空" };
                }
                else
                {
                    return JsonConvert.DeserializeObject<FaceOpenResult>(content);
                }
            }
            catch (Exception ex)
            {
                return new FaceOpenResult { code = -1, message = ex.Message };
            }
        }

        public async Task<string> Calc(string doorIp, string direction = "Z", string code = "900")
        {
            var url = ConfigProfile.Current.CheckInServerUrl + "?do=calccount";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("doorip", doorIp);
            dict.Add("code", code);
            dict.Add("direction", direction);
            var postData = dict.LinkUrl();
            var content = await doRequest(url, postData);
            return content;
        }

        private async Task<string> doRequest(string url, string postData)
        {
            var contentBuffer = postData.ToBuffer();
            WebRequest request = WebRequest.Create(url);
            request.Timeout = 5000;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = contentBuffer.Length;
            try
            {
                var requestStream = await request.GetRequestStreamAsync();
                await requestStream.WriteAsync(contentBuffer, 0, contentBuffer.Length);
                requestStream.Close();

                var response = await request.GetResponseAsync();
                var responseStr = "";
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
                    {
                        responseStr = await reader.ReadToEndAsync();
                    }
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                log.Fatal("请求服务异常->" + ex.Message);
                return string.Empty;
            }
            finally
            {
            }
        }

        static string getTypeName(IDType type)
        {
            if (type == IDType.BarCode)
                return "T";
            else if (type == IDType.IC)
                return "IC";
            else if (type == IDType.Face)
                return "P";
            else if (type == IDType.ID)
                return "I";

            throw new ArgumentException("凭证类型参数异常");
        }
    }
}
