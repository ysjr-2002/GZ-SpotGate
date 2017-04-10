using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using GZ_SpotGate.XmlParser;

namespace GZ_SpotGate.Core
{
    class Request
    {
        public async Task<string> CheckIn(IDType type, string code)
        {
            var content = await doRequest(type, code);
            return content;
        }

        private async Task<string> doRequest(IDType type, string code)
        {
            var url = "http://220.197.187.4:8000/HarewareService/gatecheck.ashx?do=ticketface";
            //var url = "http://localhost:12840/TicketCheckHandler.ashx";

            requestParam rp = new requestParam();
            var json = Util.toJson(rp);

            var contentBuffer = json.ToBuffer();
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
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
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public class requestParam
    {
        public requestParam()
        {
            doorip = "172.21.4.31";
            barcode = "2017041000018";
            type = "P";
        }

        public string doorip { get; set; }

        public string barcode { get; set; }

        public string type { get; set; }
    }

    public class FeedBack
    {
        public string code { get; set; }

        public string message { get; set; }

        public string sound { get; set; }

        public string personCount { get; set; }

        public string personOnceCount { get; set; }

        public string direction { get; set; }

        public string contentType { get; set; }
    }
}
