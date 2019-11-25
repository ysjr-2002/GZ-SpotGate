using GZSpotGate.Core;
using LL.SenicSpot.Gate.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        MyUdpComServer udpServer = null;
        private const int qrport = 9876;

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

        private void Onload()
        {
            Util.runWhenStart(Config.Instance.Auto == "1", "GZSpotGate", System.Windows.Forms.Application.ExecutablePath);

            controllers = new List<ChannelController>();
            udpServer = new MyUdpComServer(qrport);
            udpServer.ReceiveAsync();
            udpServer.OnMessageInComming += UdpServer_OnMessageInComming;

            foreach (var channel in Channels.ChannelList)
            {
                var controller = new ChannelController(channel);
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
        }
    }
}
