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
            bool auto = (ConfigProfile.Current.AutoRun == 1);
            string appPath = System.Windows.Forms.Application.ExecutablePath;
            Util.RunWhenStart(auto, appname, appPath);

            MyConsole.Current.Init(txtConsole);

            mc = new MainController();
            mc.Start();

            Task.Run(async () =>
            {
                var request = new Request();
                await request.CheckIn("127.0.0.1", IDType.BarCode, "test");
            });
            txtConsole.Focus();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var dialog = MessageBox.Show("确认要退出程序吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialog == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
            MyConsole.Current.Log("系统退出");
            mc.Dispose();
            base.OnClosing(e);
        }
    }
}
