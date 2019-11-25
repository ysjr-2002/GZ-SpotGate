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
using System.Web.Script.Serialization;

namespace FaceAPI
{
    class HttpRequest
    {
        private const int timeout = 5000;

        public bool Login(string url, Dictionary<string, string> parms, out string cookie)
        {
            try
            {
                var buffer = parms.LinkUrl().ToUTF8();
                var wr = (HttpWebRequest)WebRequest.Create(url);
                wr.Timeout = timeout;
                wr.ContentType = "application/x-www-form-urlencoded";
                wr.Method = "POST";
                wr.UserAgent = "Koala Admin";
                wr.ContentLength = buffer.Length;

                var requeststream = wr.GetRequestStream();
                requeststream.Write(buffer, 0, buffer.Length);
                requeststream.Close();

                var response = wr.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream, System.Text.Encoding.UTF8);
                var content = sr.ReadToEnd();
                var headers = response.Headers;
                cookie = headers["Set-Cookie"];
                if (!cookie.IsEmpty())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                cookie = "";
                return false;
            }
        }

        private static HttpWebRequest PostImage(string url, string boundary)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.Timeout = timeout;
            wr.KeepAlive = false;
            return wr;
        }

        public string PostPhoto(string url, string pname, byte[] data, string cookie, Dictionary<string, string> param)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)PostImage(url, boundary);
            if (!cookie.IsEmpty())
                request.Headers["cookie"] = cookie;

            WebResponse response = null;
            StringBuilder sb = new StringBuilder();
            try
            {
                var rs = request.GetRequestStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in param.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, param[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }

                //文件开始
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                //图片
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n";
                headerTemplate += "Content-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, pname, "image.jpg", "application/octet-stream");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                rs.Write(data, 0, data.Length);
                //文件结束
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    var content = sr.ReadToEnd();
                    sr.Close();
                    return content;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Post(string url, byte[] data, Dictionary<string, string> param)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)PostImage(url, boundary);
            WebResponse response = null;

            StringBuilder sb = new StringBuilder();
            try
            {
                var rs = request.GetRequestStream();
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in param.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, param[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }

                //文件开始
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                //图片
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "image", "image.jpg", "text/plain");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);
                rs.Write(data, 0, data.Length);
                //文件结束
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                response = request.GetResponse();
                if (response != null)
                {
                    var responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream);
                    var content = sr.ReadToEnd();
                    sr.Close();
                    return content;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string PostJson(string url, string method, string cookie, object instance)
        {
            //JavaScriptSerializer js = new JavaScriptSerializer();
            var jsonData = JsonConvert.SerializeObject(instance);
            Console.WriteLine(jsonData);
            var data = jsonData.ToUTF8();
            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = method;
            request.Timeout = timeout;
            request.ContentLength = data.Length;
            if (!cookie.IsEmpty())
                request.Headers["cookie"] = cookie;
            var responseStr = "";
            try
            {
                using (var rs = request.GetRequestStream())
                {
                    rs.Write(data, 0, data.Length);
                }
                var response = request.GetResponse();
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = stream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
            }
            return responseStr;
        }

        public string PostJson(string url, string method, string cookie, string jsonData)
        {
            var data = jsonData.ToUTF8();
            WebRequest request = WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = method;
            request.Timeout = timeout;
            request.ContentLength = data.Length;
            if (!cookie.IsEmpty())
                request.Headers["cookie"] = cookie;
            var responseStr = "";
            try
            {
                using (var rs = request.GetRequestStream())
                {
                    rs.Write(data, 0, data.Length);
                }
                var response = request.GetResponse();
                using (var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = stream.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
            }
            return responseStr;
        }

        public string Get(string url, string cookie)
        {
            var wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Timeout = 5000;
            wr.Method = "GET";
            if (!cookie.IsEmpty())
                wr.Headers.Add("cookie", cookie);

            try
            {
                var response = wr.GetResponse();
                var stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
                var content = sr.ReadToEnd();
                return content;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string Delete(string url, string cookie)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "DELETE";
                if (!cookie.IsEmpty())
                    request.Headers["cookie"] = cookie;

                var response = request.GetResponse();
                var content = "";
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                    Console.WriteLine("delete->" + content);
                }
                return content;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string PostDelete(string url, string cookie, string data)
        {
            try
            {
                var request = WebRequest.Create(url);
                request.Timeout = timeout;
                request.Method = "DELETE";
                if (!cookie.IsEmpty())
                    request.Headers["cookie"] = cookie;

                var bytes = System.Text.Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;
                var stream = request.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();

                var response = request.GetResponse();
                var content = "";
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    content = reader.ReadToEnd();
                    Console.WriteLine("delete->" + content);
                }
                return content;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
