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
            ckbAuto.IsChecked = ConfigProfile.Current.AutoRun == 1;
            txtServer.Text = ConfigProfile.Current.CheckInServerUrl;
            txtOpen.Text = ConfigProfile.Current.OpenGateUrl;
            txtReboot.Text = ConfigProfile.Current.RebootGateUrl;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            ConfigProfile.Current.AutoRun = (ckbAuto.IsChecked.GetValueOrDefault() ? 1 : 0);
            ConfigProfile.Current.CheckInServerUrl = txtServer.Text;
            ConfigProfile.Current.OpenGateUrl = txtOpen.Text;
            ConfigProfile.Current.RebootGateUrl = txtReboot.Text;
            ConfigProfile.Current.Save();
            Common.CMessageBox.Show("保存成功！");
        }
    }
}
