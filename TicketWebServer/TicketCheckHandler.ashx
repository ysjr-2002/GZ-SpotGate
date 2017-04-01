<%@ WebHandler Language="C#" Class="TicketCheckHandler" %>

using System;
using System.Web;
using System.Text;
using System.IO;
using System.Drawing;

public class TicketCheckHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        if (request.ContentLength > 0)
        {
            var stream = request.InputStream;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();
            }
        }

        var sign = request["api_key"];
        var method = request["api_secret"];
        var files = request.Files;


        if (files.Count > 0)
        {
            var na = files.AllKeys[0];
            var ct = files[0].ContentType;
            var fn = files[0].FileName;
            files[0].SaveAs("c:\\client.jpg");
        }

        var responseStr = "<message>"
                   + "<errcode>3100</errcode>"
                  + "<errmessage>验票完成</errmessage>"
                  + "<datetime>2017-4-5</datetime>"
                  + "<nums>1</nums>"
                  + "</message>";

        response.ContentType = "text/plain";
        response.ContentEncoding = Encoding.UTF8;
        response.Write(responseStr);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}