using GZ_SpotGateEx.Core;
using GZ_SpotGateEx.ViewModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GZ_SpotGateEx
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static ILog log = LogManager.GetLogger("App");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bnew = false;
            var appname = System.Windows.Forms.Application.ProductName;
            var mutex = new Mutex(true, appname, out bnew);
            if (bnew)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 12;
                InitIOC();
                ConfigProfile.Current.ReadConfig();
                Channels.Load();
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                var window = new MainWindow();
                window.ShowDialog();
                mutex.WaitOne();
            }
            else
            {
                MessageBox.Show("系统已运行！");
                Application.Current.Shutdown();
            }
        }
        static void InitIOC()
        {
            MyStandardKernel.Instance.Bind<MainViewModel>().ToSelf().InSingletonScope();
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var msg = "UnhandledException发生异常->" + e.ExceptionObject;
            log.Fatal(msg);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var msg = "DispatcherUnhandledException发生异常->" + e.Exception.StackTrace;
            log.Fatal(msg);
        }
    }
}
