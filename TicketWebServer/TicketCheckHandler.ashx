﻿<%@ WebHandler Language="C#" Class="TicketCheckHandler" %>

using System;
using System.Web;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web.Script;
public class TicketCheckHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var request = context.Request;
        var response = context.Response;

        if (request.ContentLength > 0)
        {
            var a = context.Request.Form["doorip"];
            var b = context.Request["doorip"];
            var c = context.Request["barcode"];
            var t = context.Request["type"];
            var stream = request.InputStream;
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();
                System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var obj = js.Deserialize(content, typeof(requestParam));
            }
        }

        var responseStr = "";
        response.ContentType = "application/json";
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

//public class requestParam
//{
//    public requestParam()
//    {
//        doorip = "172.21.4.31";
//        barcode = "2017041000018";
//        type = "P";
//    }
//    public string doorip { get; set; }
//    public string barcode { get; set; }
//    public string type { get; set; }
//}