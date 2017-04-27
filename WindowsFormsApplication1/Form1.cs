using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
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
            //var all = "ws://192.168.1.116:9872/android";
            MessageBox.Show("连接地址->" + all);
            ws = new WebSocket(all);
            ws.OnError += Ws_OnError1;
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
        }

        private void Ws_OnError1(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            lblState.Text = "连接错误";
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                var js = new JavaScriptSerializer();
                var obj = js.Deserialize<FaceRecognized>(e.Data);
                if (obj.type == RecognizeState.recognized.ToString())
                {
                    try
                    {
                        var base64 = obj.data.face.image;
                        var buffer = Convert.FromBase64String(base64);
                        var ms = new MemoryStream(buffer);
                        var image = Image.FromStream(ms);
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() => { showFace(image); }));
                        }
                        else
                        {
                            showFace(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("异常->" + ex.Message);
                    }
                }
            }
        }

        private void showFace(Image image)
        {
            pictureBox1.Image = image;
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            lblState.Text = "连接成功，请进行人脸识别";
            pictureBox1.ImageLocation = "https://o7rv4xhdy.qnssl.com/@/static/upload/avatar/2017-04-07/741757cb9c5e19f00c8f6ac9a56057d27aab2857.jpg";
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            lblState.Text = "连接关闭";
        }
    }
}
