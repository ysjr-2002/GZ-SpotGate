using GZ_SpotGate.Core;
using GZ_SpotGate.Manage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using System.IO;
using GZ_SpotGate.Core;

namespace GZ_SpotGate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ConfigProfile.Current.ReadConfig();
            Channels.Load();
            //var window = new MainWindow();
            //window.ShowDialog();
            base.OnStartup(e);

            //MainController mc = new MainController();
            //mc.Start();
        }
    }
}
