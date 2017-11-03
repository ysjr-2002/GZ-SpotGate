using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using GZ_SpotGate.Model;
using GZ_SpotGate.Tcp;

namespace GZ_SpotGate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainController mc = null;
        const string appname = "SpotGate";
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MyConsole.Current.Init(txtConsole);

            ConfigProfile.Current.ReadConfig();
            Channels.Load();

            bool auto = (ConfigProfile.Current.AutoRun == 1);
            string appPath = System.Windows.Forms.Application.ExecutablePath;
            Util.RunWhenStart(auto, appname, appPath);

            mc = new MainController();
            mc.Start();

            Task.Run(async () =>
            {
                var request = new Request();
                await request.CheckIn("127.0.0.1", IDType.BarCode, "test");
            });
            txtConsole.Focus();

            btnOpen.IsEnabled = true;
            btnClose.IsEnabled = false;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var dialog = MessageBox.Show("确认要退出程序吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialog == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            mc.Dispose();
            MyConsole.Current.Log("系统退出");
            base.OnClosing(e);
        }

        #region 测试
        //private async void btnOpen_Click(object sender, RoutedEventArgs e)
        //{
        //    await Task.Factory.StartNew(() =>
        //    {
        //        var enableChannels = Channels.ChannelList.Where(s => s.IsEnable == true).ToList();
        //        foreach (var c in enableChannels)
        //        {
        //            if (c.GateHoleOpen)
        //            {
        //                GateConnectionPool.EnterHoldOpen(c.GateComServerIp);
        //                Thread.Sleep(100);
        //                GateConnectionPool.ExitHoldOpen(c.GateComServerIp);
        //            }
        //        }
        //    });

        //    btnOpen.IsEnabled = false;
        //    btnClose.IsEnabled = true;
        //}

        //private async void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    await Task.Factory.StartNew(() =>
        //    {
        //        var enableChannels = Channels.ChannelList.Where(s => s.IsEnable == true).ToList();
        //        foreach (var c in enableChannels)
        //        {
        //            if (c.GateHoleOpen)
        //            {
        //                GateConnectionPool.EnterClose(c.GateComServerIp);
        //                Thread.Sleep(100);
        //                GateConnectionPool.ExitClose(c.GateComServerIp);
        //            }
        //        }
        //    });
        //    btnOpen.IsEnabled = true;
        //    btnClose.IsEnabled = false;
        //} 
        #endregion
    }
}
