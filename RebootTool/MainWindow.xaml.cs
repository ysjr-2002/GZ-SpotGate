using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using WXCX.Gate;
using System.ComponentModel;

namespace RebootTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string datafile = "reboot.ini";
        string time = "";
        private System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            datafile = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, datafile);
            time = System.IO.File.ReadAllText(datafile);
            AutoRun();
            StartTimer();

            var hour = time.Substring(0, 2);
            var minute = time.Substring(2, 2);
            var second = time.Substring(4, 2);
            txtHour.Text = hour;
            txtMinute.Text = minute;
            txtSecond.Text = second;
        }

        private void StartTimer()
        {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        static void AutoRun()
        {
            var auto = ConfigurationManager.AppSettings["auto"] == "1";
            Util.runWhenStart(auto, "RebootTool", System.Windows.Forms.Application.ExecutablePath);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var curTime = DateTime.Now.ToString("HHmmss");
            if (curTime == time)
            {
                RebootMachineAPI.ShuwDown();
            }
            else
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var total = string.Concat(txtHour.Text, txtMinute.Text, txtSecond.Text);
            if (total.Length != 6)
            {
                Warn("时间格式不正确！");
                return;
            }
            System.IO.File.WriteAllText(datafile, total);
            Warn("设置成功！");
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer?.Stop();
            base.OnClosing(e);
        }
    }
}
