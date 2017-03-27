using GZ_SpotGate.Core;
using GZ_SpotGate.XmlParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var request = new Request();
            var content = await request.CheckIn(XmlParser.CheckIntype.BarCode, "123456");
            if (string.IsNullOrEmpty(content))
            {
                MessageBox.Show("获取数据失败");
                return;
            }
            Define.Parse(content);
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

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }
    }
}
