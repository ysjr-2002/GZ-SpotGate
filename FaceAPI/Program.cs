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
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FrmMain());
            var task = new API().Login("tengli@megvii.com", "123456");
            var login = task.Result;
            if (login)
            {
                Console.WriteLine("login ok");
            }
            else
            {
                Console.WriteLine("login error");
            }
            Console.Read();
        }
    }
}
