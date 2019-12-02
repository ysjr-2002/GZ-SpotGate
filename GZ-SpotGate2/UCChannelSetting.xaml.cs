using GZSpotGate.Core;
using GZSpotGate.ViewModel;
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
using Tengli.Core;

namespace GZSpotGate
{
    /// <summary>
    /// UcChannelSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCChannelSetting : UserControl
    {
        public UCChannelSetting()
        {
            InitializeComponent();
            this.Loaded += UCChannelSetting_Loaded;
        }

        private void UCChannelSetting_Loaded(object sender, RoutedEventArgs e)
        {
            lbChannels.ItemsSource = Channels.ChannelList;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Channels.Save();
            MessageBox.Show("配置保存成功，重启生效！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnOpengate_Click(object sender, RoutedEventArgs e)
        {
            var gateIp = ((FrameworkElement)sender).Tag.ToString();
            if (gateIp != null)
            {
                //var open = GateHelper.Open(gateIp);
                //if (open)
                //{
                //    MessageBox.Show("开闸成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                //else
                //{
                //    MessageBox.Show("开闸失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                //}
                MainWindowViewModel.Instance.Open(gateIp);
            }
        }
    }
}
