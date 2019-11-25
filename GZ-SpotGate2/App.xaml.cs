﻿using GZSpotGate.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GZSpotGate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var bnew = false;
            var appname = System.Windows.Forms.Application.ProductName;
            var mutex = new Mutex(true, appname, out bnew);
            if (bnew)
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 64;
                Config.Instance.Read();
                Channels.Load();
                var window = new MainWindow();
                Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
                System.Windows.Application.Current.MainWindow = window;
                window.ShowDialog();
            }
            else
            {
                MsgBox.Warning("系统已运行！");
                Environment.Exit(Environment.ExitCode);
            }
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var msg = e.Exception.Message;
            MsgBox.Warning(msg);
            e.Handled = true;
        }
    }
}
