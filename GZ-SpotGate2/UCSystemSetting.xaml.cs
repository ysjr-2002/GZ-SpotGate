using GZSpotGate.Core;
using GZSpotGate.Pad;
using GZSpotGate.WS;
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

namespace GZSpotGate
{
    /// <summary>
    /// UCSystemSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCSystemSetting : UserControl
    {
        public UCSystemSetting()
        {
            InitializeComponent();
            this.Loaded += UCSystemSetting_Loaded;
        }


        private void UCSystemSetting_Loaded(object sender, RoutedEventArgs e)
        {
            ckbAuto.IsChecked = Config.Instance.Auto == "1";
            txtpwServer.Text = Config.Instance.PWServer;
            txtfaceserver.Text = Config.Instance.FaceServer;
            txtdelay.Text = Config.Instance.PadDelay.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.Auto = (ckbAuto.IsChecked.GetValueOrDefault() ? "1" : "0");
            Config.Instance.PWServer = txtpwServer.Text;
            Config.Instance.FaceServer = txtfaceserver.Text;
            Config.Instance.PadDelay = txtdelay.Text.ToInt32();

            Config.Instance.Save();
            MessageBox.Show("配置保存成功，重启生效！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            var avatar = "http://pic-bucket.nosdn.127.net/photo/0003/2018-11-05/DVRRHAA100AJ0003NOS.jpg";
            AndroidMessage am = AndroidMessage.GetFaceYes(avatar);
            PadHelper.SendToPad(Channels.ChannelList.First().pad, am);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            AndroidMessage am = AndroidMessage.GetIDYes("杨绍杰");
            PadHelper.SendToPad(Channels.ChannelList.First().pad, am);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            AndroidMessage am = AndroidMessage.GetQRYes();
            PadHelper.SendToPad(Channels.ChannelList.First().pad, am);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            AndroidMessage am = AndroidMessage.GetFaceNO("验证失败");
            PadHelper.SendToPad(Channels.ChannelList.First().pad, am);
        }
    }
}
