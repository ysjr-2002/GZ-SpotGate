using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using WebSocketSharp;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebSocket ws = null;
        private void button1_Click(object sender, EventArgs e)
        {
            var url = "ws://" + textBox1.Text + ":9000/video";
            var rtsp = textBox2.Text;
            rtsp = HttpUtility.UrlEncode(rtsp);
            var all = string.Concat(url, "?url=", rtsp);
            MessageBox.Show("连接地址->" + all);
            ws = new WebSocket(all);
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                MessageBox.Show("收到数据");
            }
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            MessageBox.Show("请刷脸");
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            MessageBox.Show("Close");
        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show("Error");
        }
    }
}
