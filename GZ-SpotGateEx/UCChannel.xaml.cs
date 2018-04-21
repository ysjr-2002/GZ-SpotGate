using GZ_SpotGateEx.Core;
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

namespace GZ_SpotGateEx
{
    /// <summary>
    /// UCChannel.xaml 的交互逻辑
    /// </summary>
    public partial class UCChannel : UserControl
    {
        public UCChannel()
        {
            InitializeComponent();
        }

        private async void btnReboot_Click(object sender, RoutedEventArgs e)
        {
            var dialog = Common.CMessageBox.Confirm("确认重启主机？");
            if (dialog == System.Windows.Forms.DialogResult.Yes)
            {
                var channel = this.DataContext as Channel;
                var request = new Request();
                var url = string.Format(ConfigProfile.Current.RebootGateUrl, channel.ClientIp);
                var result = await request.Open(url);
            }
        }
    }
}
