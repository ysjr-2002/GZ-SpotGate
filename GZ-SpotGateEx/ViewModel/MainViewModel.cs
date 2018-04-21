using Common;
using Common.NotifyBase;
using GZ_SpotGateEx.Core;
using GZ_SpotGateEx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GZ_SpotGateEx.ViewModel
{
    class MainViewModel : PropertyNotifyObject
    {
        StackPanel container;

        public ICommand LoadedCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand MaxCommand { get; set; }
        public ICommand SwitchCommand { get; set; }

        private const int MAX_COUNT = 100;

        private List<ChannelControler> controlers = new List<ChannelControler>();

        public int TabSelecteIndex
        {
            get { return this.GetValue(s => s.TabSelecteIndex); }
            set { this.SetValue(s => s.TabSelecteIndex, value); }
        }

        public StackPanel Container
        {
            get
            {
                return this.container;
            }
            set
            {
                this.container = value;
            }
        }

        public MainViewModel()
        {
            InitCommand();
        }

        private void InitCommand()
        {
            LoadedCommand = new DelegateCommand(WindowLoad);
            CloseCommand = new DelegateCommand(WindowClose);
            MaxCommand = new DelegateCommand(Max);
            SwitchCommand = new DelegateCommand<string>(Switch);
        }

        private void Switch(string tag)
        {
            if (tag == "0")
                TabSelecteIndex = 0;
            if (tag == "1")
                TabSelecteIndex = 1;
            if (tag == "2")
                TabSelecteIndex = 2;
        }

        private void WindowLoad()
        {
            foreach (var item in Channels.ChannelList)
            {
                ChannelControler controller = new ChannelControler(item);
                controller.Init();
                controlers.Add(controller);
            }

            AutoRun();
            CheckHeartBeat();

            //Task.Factory.StartNew(() =>
            //{
            //    long temp = 0;
            //    var error = "";
            //    var result = WXVerifyService.Check(url, "67890", out temp, out error);
            //});
        }

        static void AutoRun()
        {
            var auto = ConfigProfile.Current.AutoRun == 1;
            Util.RunWhenStart(auto, "GZ_GateSpot", System.Windows.Forms.Application.ExecutablePath);
        }

        public ChannelControler getChannelControler(string no)
        {
            return controlers.FirstOrDefault(s => s.Channel.No == no);
        }

        static object sync = new object();
        public void Append(Record data)
        {
            lock (sync)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (container.Children.Count >= MAX_COUNT)
                        {
                            container.Children.Clear();
                        }
                        ItemControl item = new ItemControl();
                        item.DataContext = data;
                        container.Children.Insert(0, item);
                    }
                    catch
                    {

                    }
                });
            }
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
            Environment.Exit(Environment.ExitCode);
        }

        CancellationTokenSource cts = new CancellationTokenSource();
        private void CheckHeartBeat()
        {
            Task.Factory.StartNew(() =>
            {
                while (cts.IsCancellationRequested == false)
                {
                    foreach (var item in Channels.ChannelList)
                    {
                        var ts = DateTime.Now - item.LastHeartbeat.ToDateTime();
                        if (ts.TotalSeconds > 10)
                        {
                            item.IsTimeOut = true;
                        }
                        else
                        {
                            item.IsTimeOut = false;
                        }
                    }
                    Thread.Sleep(5 * 1000);
                }
            });
        }

        public void Dispose()
        {
            cts.Cancel();
            foreach (var item in controlers)
            {
                item.Stop();
            }
        }
    }
}
