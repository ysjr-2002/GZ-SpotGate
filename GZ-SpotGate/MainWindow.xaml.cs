using GZ_SpotGate.Udp;
using GZ_SpotGate.XmlParser;
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

namespace GZ_SpotGate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainController _main = null;
        public MainWindow()
        {
            InitializeComponent();
            _main = new MainController();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _main.Start();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new Request();
            var content = await request.CheckIn(CheckIntype.BarCode, "123456");
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("获取数据失败");
                return;
            }
            string code, message, datetime, nums;
            Define.Parse(content, out code, out message, out datetime, out nums);
        }

        private async void Test()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            await test();
            Debug.WriteLine("hz:are you ok?");
            btnawait.Content = "execute ok";
            List<string> list = new List<string>();
            list.AsParallel();
        }

        private Task test()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    Debug.WriteLine("hz:write");
            //});
            var task = new Task(() =>
            {
                Debug.WriteLine("hz:execute");
            });
            return task;
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            var task1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("task1");
            });

            var task2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("task2");
            });

            var task3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);
                Console.WriteLine("task3");
            });
            //Task.WaitAll(task1, task2, task3);
            await Task.WhenAll(task1, task2, task3);
            Console.WriteLine("task over");
        }

        private UdpComServer server = null;
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            server = new UdpComServer(9876);
            server.Start();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
        }
    }
}
