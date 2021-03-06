﻿using GZ_SpotGate.Core;
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
using System.Threading;
using log4net;

namespace GZ_SpotGate
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
