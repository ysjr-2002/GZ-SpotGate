using BJ_Benz.Code;
using GZ_SpotGate.Core;
using GZ_SpotGate.Model;
using GZ_SpotGate.WS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
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
using GZ_SpotGate.Tcp;
using System.ComponentModel;

namespace GZ_SpotGate.Manage
{
    /// <summary>
    /// DeviceTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceTestWindow : Window
    {
        private int _dType = 0;
        private IReader reader = null;
        private GateReader gr = new GateReader();
        private WebSocketServer ws = null;
        private TcpComServer tcpServer = null;

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

            cmbPort1.ItemsSource = ports;
            cmbPort1.SelectedIndex = 1;

            ws = new WebSocketServer(ConfigProfile.Current.WebSocketListenPort);
            ws.Start();

            tcpServer = new TcpComServer(ConfigProfile.Current.TcpComListenPort);
            tcpServer.Start();

            MyConsole.Current.Init(tbResult);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            ws?.Stop();
            tcpServer?.Stop();
        }

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

        private void OnGetIDNO(string no)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblContent.Content = no;
            });
        }

        /// <summary>
        /// 打开闸机串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen1_Click(object sender, RoutedEventArgs e)
        {
            if (!gr.OpenPort(cmbPort1.Text))
            {
                MessageBox.Show("打开失败！");
                return;
            }
            gr.SetCallback((s) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    tbResult.AppendText(s + "\n");
                });
            });
        }

        private void btnOpenX_Click(object sender, RoutedEventArgs e)
        {
            btnOpenX.IsEnabled = false;
        }

        private void Tcp_OnMessageInComming(object sender, DataEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (e.IDData)
                {
                    lblContent.Content = e.Data;
                }
                if (e.ICData)
                {
                    lblContent.Content = e.Data;
                }
                if (e.QRData)
                {
                    lblContent.Content = e.Data;
                }
            });
        }

        private void btnStopTcpServer_Click(object sender, RoutedEventArgs e)
        {
            tcpServer?.Stop();
            btnOpenX.IsEnabled = true;
        }

        /// <summary>
        /// 进向开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            gr.EnterOpen((byte)(txtOpencount.Text.ToInt32()));
        }
        /// <summary>
        /// 进向持久开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnterHold_click(object sender, RoutedEventArgs e)
        {
            gr.EnterOpen(1);
        }
        /// <summary>
        /// 进向关闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnterClose_click(object sender, RoutedEventArgs e)
        {
            gr.EnterClose();
        }
        /// <summary>
        /// 出向开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            gr.ExitOpen((byte)(txtOpencount.Text.ToInt32()));
        }
        /// <summary>
        /// 出向持久
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExitHold_click(object sender, RoutedEventArgs e)
        {
            gr.ExitHoldOpen();
        }
        /// <summary>
        /// 出向关闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExitClose_click(object sender, RoutedEventArgs e)
        {
            gr.EnterClose();
        }

        private async void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            var request = new Request();
            var json = await request.CheckIn("172.21.4.31", IDType.Face, "2017041000003");
            if (json?.code != 100)
            {
                MessageBox.Show("异常->" + json?.code);
            }
            else
            {
                MessageBox.Show("验证成功");
            }
        }

        private async void btnPersonCalc_Click(object sender, RoutedEventArgs e)
        {
            var request = new Request();
            var c = await request.Calc("172.21.4.31");
            var json = c.Deserlizer<FeedBack>();
            if (json?.code != 100)
            {
                MessageBox.Show("异常->" + json?.code);
            }
            else
            {
                MessageBox.Show("上报成功");
            }
        }

        private void btnAsk_click(object sender, RoutedEventArgs e)
        {
            gr.AskGateState();
        }

        private async void btnChannel_Click(object sender, RoutedEventArgs e)
        {
            var c1 = new ChannelController();
            var code = "1";
            DataEventArgs arg = null;
            if (sender == btnInOK)
            {
                arg = new DataEventArgs
                {
                    Data = code,
                    QRData = true,
                    IPEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.2"), 1001)
                };
            }
            if (sender == btnInError)
            {
                arg = new DataEventArgs
                {
                    Data = code,
                    QRData = true,
                    IPEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.2"), 1001)
                };
            }
            if (sender == btnOutOk)
            {
                arg = new DataEventArgs
                {
                    Data = code,
                    QRData = true,
                    IPEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.2"), 1003)
                };
            }
            if (sender == btnOutError)
            {
                arg = new DataEventArgs
                {
                    Data = code,
                    QRData = true,
                    IPEndPoint = new System.Net.IPEndPoint(IPAddress.Parse("192.168.1.2"), 1003)
                };
            }
            await c1.Init(Channels.ChannelList.First(), ws);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    c1.Work(arg);
                    break;
                    Thread.Sleep(3000);
                }
            });
        }
    }
}
