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
            //ckbAuto.IsChecked = Config.Current.AutoRun == "1";
            //txtServer.Text = Config.Current.VerifyServer;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Config.Current.AutoRun = (ckbAuto.IsChecked.GetValueOrDefault() ? "1" : "0");
            //Config.Current.VerifyServer = txtServer.Text;
            //Config.Current.Save();
        }
    }
}
