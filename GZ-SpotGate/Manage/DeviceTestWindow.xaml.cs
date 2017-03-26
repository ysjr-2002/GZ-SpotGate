using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Shapes;

namespace GZ_SpotGate.Manage
{
    /// <summary>
    /// DeviceTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceTestWindow : Window
    {
        private int _dType = 0;
        private SerialPort _port = null;
        private bool _open = false;
        private bool _running = false;
        public DeviceTestWindow()
        {
            InitializeComponent();
            this.Loaded += DeviceTestWindow_Loaded;
        }

        private void DeviceTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            cmbPort.ItemsSource = ports;
            cmbPort.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbPort.Text == "")
            {
                return;
            }
            _dType = cmbType.SelectedIndex;
            if (!_open)
            {
                OpenPort();
                btnOpen.Content = "关闭";
            }
            else
            {
                ClosePort();
                btnOpen.Content = "打开";
            }
        }

        private void OpenPort()
        {
            _port = new SerialPort(cmbPort.Text, 9600, Parity.None, 8, StopBits.None);
            try
            {
                _port.Open();
                _open = true;
            }
            catch
            {
                MessageBox.Show("串口打开失败");
                _open = false;
            }
        }

        private void ClosePort()
        {
            _running = false;
            _port.Close();
            _port = null;
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
    }
}
