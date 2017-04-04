using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GZ_SpotVisual
{
    class Config
    {
        private const char spliter = '$';
        private const string faceroot = "facevisual";
        private const string config = "config.txt";
        private const string log = "log.txt";
        public static Profile Profile { get; set; }

        static Config()
        {
            Profile = new Profile();
        }

        public static void Log(string content)
        {
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.Path;
            var dir = System.IO.Path.Combine(sdCardPath, faceroot);
            var filePath = System.IO.Path.Combine(dir, log);
            content = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss") + "->" + content + System.Environment.NewLine;
            System.IO.File.AppendAllText(filePath, content, Encoding.UTF8);
        }

        public static void ReadProfile()
        {
            var root = Android.OS.Environment.ExternalStorageDirectory.Path;
            var dir = System.IO.Path.Combine(root, faceroot);
            var filePath = System.IO.Path.Combine(dir, config);
            if (System.IO.File.Exists(filePath))
            {
                var content = System.IO.File.ReadAllText(filePath, Encoding.UTF8);
                var array = content.Split(spliter);
                Profile.ServerIp = array[0];
                Profile.CameraMain = array[1];
                Profile.CameraSub = array[2];
                Profile.Welcome1 = array[3];
                Profile.Welcome2 = array[4];
                Profile.Delay = Convert.ToInt32(array[5]);
            }
        }

        public static void SaveProfile()
        {
            var root = Android.OS.Environment.ExternalStorageDirectory.Path;
            var dir = System.IO.Path.Combine(root, faceroot);
            var sub = new System.IO.DirectoryInfo(dir);
            if (!sub.Exists)
                sub.Create();

            var filePath = System.IO.Path.Combine(dir, config);
            var content = string.Concat(Profile.ServerIp, spliter,
                Profile.CameraMain, spliter,
                 Profile.CameraSub, spliter,
                  Profile.Welcome1, spliter,
                  Profile.Welcome2, spliter,
                Profile.Delay);

            System.IO.File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }

    public class Profile
    {
        public Profile()
        {
            CameraMain = "192.168.1.2";
            CameraSub = "192.168.1.3";
            Delay = 1000;
            Welcome1 = "»¶Ó­¹âÁÙ";
            Welcome2 = "»¶Ó­Äú";
        }

        public string ServerIp { get; set; }
        public string CameraMain { get; set; }
        public string CameraSub { get; set; }
        public string Welcome1 { get; set; }
        public string Welcome2 { get; set; }
        public int Delay { get; set; }
    }
}