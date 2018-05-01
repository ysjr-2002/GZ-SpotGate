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
using GZ_SpotGateEx.Core;

namespace GZ_SpotGateEx
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
            stackpanelchannel.Children.Clear();

            foreach (var channel in Channels.ChannelList)
            {
                UCChannel ucchannel = new UCChannel();
                ucchannel.DataContext = channel;
                stackpanelchannel.Children.Add(ucchannel);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var bRet = Channels.Save();
            if (bRet)
            {
                MessageBox.Show("保存成功！", "提示");
            }
        }
    }
}
