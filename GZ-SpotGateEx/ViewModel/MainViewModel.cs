using Common;
using Common.NotifyBase;
using GZ_SpotGateEx.Core;
using GZ_SpotGateEx.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private static ImageSource QR_ImageSource = new BitmapImage(new Uri("/GZ_SpotGateEx;component/Images/qr.png", UriKind.Relative));

        private static ImageSource FACE_ImageSource = new BitmapImage(new Uri("/GZ_SpotGateEx;component/Images/face.png", UriKind.Relative));

        private List<ChannelControler> controlers = new List<ChannelControler>();

        public int TabSelecteIndex
        {
            get { return this.GetValue(s => s.TabSelecteIndex); }
            set { this.SetValue(s => s.TabSelecteIndex, value); }
        }

        public MainViewModel(StackPanel container)
        {
            this.container = container;
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

            //Task.Factory.StartNew(() =>
            //{
            //    long temp = 0;
            //    var error = "";
            //    var result = WXVerifyService.Check(url, "67890", out temp, out error);
            //});
        }

        static void AutoRun()
        {
            //var auto = Config.Current.AutoRun == "1";
            //Util.runWhenStart(auto, "DHServer", System.Windows.Forms.Application.ExecutablePath);
        }

        private void Append(Record data)
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
    }
}
