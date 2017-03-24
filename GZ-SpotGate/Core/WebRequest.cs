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
        public async Task<string> CheckIn(CheckIntype type, string code)
        {
            var content = await doRequest(type, code);
            return content;
        }

        private async Task<string> doRequest(CheckIntype type, string code)
        {
            var url = "http://localhost:12840/TicketCheckHandler.ashx";
            var dict = new Dictionary<string, string>();
            dict.Add("sign", "1");
            dict.Add("method", "ticketcheckinface");

            var content = Define.GetCheckInContent(type, code);
            var contentBuffer = content.ToData();

            url = string.Concat(url, "?", dict.LinkUrl());
            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = contentBuffer.Length;

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
    }
}
