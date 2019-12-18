using GZSpotGate.Core;
using GZSpotGate.IDCard;
using HXMansion.Http;
using LL.SenicSpot.Gate.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GZSpotGate.ViewModel
{
    class MainWindowViewModel : PropertyNotifyObject
    {
        public ICommand LoadedCommand { get; set; }
        public ICommand AuthCommand { get; set; }
        public ICommand MinCommand { get; set; }
        public ICommand MaxCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand SwitchCommand { get; set; }

        public static MainWindowViewModel Instance = new MainWindowViewModel();

        QRUdpComServer udpServer = null;

        private HttpServer httpServer;

        public MainWindowViewModel()
        {
            InitCommand();
            TabSelectedIndex = 0;
        }

        public int TabSelectedIndex
        {
            get { return this.GetValue(s => s.TabSelectedIndex); }
            set { this.SetValue(s => s.TabSelectedIndex, value); }
        }

        public string MyTitle
        {
            get { return this.GetValue(s => s.MyTitle); }
            set { this.SetValue(s => s.MyTitle, value); }
        }

        public SettingViewModel SettingViewModel
        {
            get { return this.GetValue(s => s.SettingViewModel); }
            set { this.SetValue(s => s.SettingViewModel, value); }
        }

        public List<ChannelController> controllers;

        private void InitCommand()
        {
            LoadedCommand = new DelegateCommand(Onload);
            AuthCommand = new DelegateCommand(Auth);
            MinCommand = new DelegateCommand(Min);
            MaxCommand = new DelegateCommand(Max);
            CloseCommand = new DelegateCommand(WindowClose);
            SwitchCommand = new DelegateCommand<string>(Switch);
            SettingViewModel = new SettingViewModel();
        }

        private void Switch(string tag)
        {
            TabSelectedIndex = tag.ToInt32();
        }

        private async void Onload()
        {
            LogHelper.Log("start up...");
            Config.Instance.Read();
            Channels.Load();
            Util.runWhenStart(Config.Instance.Auto == "1", "GZSpotGate", System.Windows.Forms.Application.ExecutablePath);
            KoalaCore.KoalaConstrant.Init(Config.Instance.FaceServer);
            var login = await KoalaCore.KoalaHelper.Instance.Login(Config.Instance.Account, Config.Instance.Pwd);
            MyTitle += (login ? "天河潭道闸控制软件-Koala登录成功" : "天河潭道闸控制软件-Koala登录失败");
            if (login)
            {
                KoalaCore.KoalaHelper.Instance.GetScreenList();
                foreach (var item in KoalaCore.KoalaHelper.Instance.ScreenList)
                {
                    Debug.WriteLine("hz:camera:" + item.camera + " token:" + item.screen_token);
                }
            }

            //httpServer = new HttpServer();
            //httpServer.Start();

            controllers = new List<ChannelController>();
            udpServer = new QRUdpComServer();
            udpServer.ReceiveAsync();
            udpServer.OnMessageInComming += UdpServer_OnMessageInComming;

            foreach (var channel in Channels.ChannelList)
            {
                var controller = new ChannelController(channel);
                controller.Start();
                controllers.Add(controller);
            }
        }

        private void UdpServer_OnMessageInComming(object sender, DataEventArgs e)
        {
            var controller = controllers.FirstOrDefault(s => s.Channel.comserver == e.readerIp);
            if (controller == null)
            {
                return;
            }
            if (e.QRData)
                controller.OnQRCode(e.Data);
        }

        private void Auth()
        {
            var window = new WPFControlLibrary.AuthViewWindow();
            window.ShowDialog();
        }

        private void Min()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        internal void Open(string comIp)
        {
            var item = this.controllers.FirstOrDefault(s => s.Channel.comserver == comIp);
            if (item == null || comIp.IsEmpty())
            {
                MsgBox.Warning("Ip不存在");
                return;
            }

            item.Open();
        }

        private void Max()
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Normal)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void WindowClose()
        {
            Application.Current.MainWindow.Close();
        }

        public new void Dispose()
        {
            udpServer?.Stop();
            httpServer?.Stop();
            foreach (var controller in controllers)
            {
                controller.Dispose();
            }
        }
    }
}
