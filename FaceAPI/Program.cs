using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAPI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());

            //Dictionary<string, string> dicts = new Dictionary<string, string>();
            //dicts.Add("username", "qudian@qq.com");
            //dicts.Add("password", "123456");
            //var cookie = "";
            //var http = new HttpRequest();
            //http.Login(API.login_url, dicts, out cookie);
            //var json = "{\"subject_type\":0,\"entry_date\":1535385600,\"photo_ids\":[1202917],\"name\":\"12\",\"id\":0}";
            //var back = http.PostJson(API.subject_url, "POST", cookie, json);
        }
    }
}
