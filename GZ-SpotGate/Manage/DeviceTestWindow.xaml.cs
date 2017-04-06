using BJ_Benz.Code;
using GZ_SpotGate.Udp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GZ_SpotGate.Manage
{
    /// <summary>
    /// DeviceTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceTestWindow : Window
    {
        private int _dType = 0;

        public DeviceTestWindow()
        {
            InitializeComponent();
            this.Loaded += DeviceTestWindow_Loaded;
        }

        private void DeviceTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            cmbPort.ItemsSource = ports;
            cmbPort.SelectedIndex = 1;
        }

        private IReader reader = null;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPort.Text == "")
            {
                return;
            }

            if (cmbType.SelectedIndex < 3)
            {
                reader = new WGReader();
            }
            else
            {
                reader = new ZKIDReader();
            }
            reader.SetCallback(OnGetIDNO);
            var _open = reader.OpenPort(cmbPort.Text);
            try
            {
                _open = true;
                btnOpen.IsEnabled = false;
            }
            catch
            {
                MessageBox.Show("串口打开失败");
                _open = false;
            }
        }

        private void ReadPort()
        {
            if (_dType == 0)
            {

            }
            else if (_dType == 1)
            {

            }
            else if (_dType == 2)
            {

            }
            else if (_dType == 3)
            {

            }
        }

        //二维码
        private void BarCode(byte[] data)
        {

        }

        //身份证
        private void IDCard(byte[] data)
        {

        }

        //IC卡
        private void ICCard(byte[] data)
        {

        }

        //门票码
        private void Ticket(byte[] data)
        {

        }

        private void OnGetIDNO(string no)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblContent.Content = no;
            });
        }

        private void btnOpen1_Click(object sender, RoutedEventArgs e)
        {
            MegviiGate gate = new MegviiGate();
            gate.In(txtIp.Text);
            gate.Dispose();
        }

        private void btnOpen2_Click(object sender, RoutedEventArgs e)
        {
            MegviiGate gate = new MegviiGate();
            gate.Out(txtIp.Text);
            gate.Dispose();
        }

        private void btnOpenX_Click(object sender, RoutedEventArgs e)
        {
            Tcp.TcpComServer tcp = new Tcp.TcpComServer(9871);
            tcp.OnMessageInComming += Tcp_OnMessageInComming;
            tcp.Start();
            btnOpenX.IsEnabled = false;
        }

        private void Tcp_OnMessageInComming(object sender, DataEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if( e.IDData)
                {
                    lblContent.Content = e.Data;
                }
            });
        }

        private void btnOpenU_Click(object sender, RoutedEventArgs e)
        {
            Udp.UdpComServer udp = new Udp.UdpComServer(9870);
            udp.OnMessageInComming += Udp_OnMessageInComming;
            udp.Start();
        }

        private void Udp_OnMessageInComming(object sender, DataEventArgs e)
        {
        }
    }
}
