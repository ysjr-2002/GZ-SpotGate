using GZ_SpotGateEx.http;
using System;
using System.Collections.Generic;
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
using Ninject;
using GZ_SpotGateEx.ViewModel;

namespace GZ_SpotGateEx
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.DataContext = MyStandardKernel.Instance.Get<MainViewModel>();
            MyStandardKernel.Instance.Get<MainViewModel>().Container = this.container;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HttpServer server = new HttpServer();
            server.start();
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                return;
            }
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void btnA_Click(object sender, RoutedEventArgs e)
        {
            btnB.IsChecked = false;
            btnC.IsChecked = false;
        }

        private void btnB_Click(object sender, RoutedEventArgs e)
        {
            btnA.IsChecked = false;
            btnC.IsChecked = false;
        }

        private void btnC_Click(object sender, RoutedEventArgs e)
        {
            btnA.IsChecked = false;
            btnB.IsChecked = false;
        }
    }
}
