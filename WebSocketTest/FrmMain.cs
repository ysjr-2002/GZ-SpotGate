using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WebSocketSharp;
using WindowsFormsApplication1.New;
using WindowsFormsApplication1.Old;

namespace WindowsFormsApplication1
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            if (cmbrtsp.Items.Count > 0)
            {
                cmbrtsp.SelectedIndex = 0;
            }
        }

        WebSocket ws = null;
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请输入Koala Ip地址！");
                return;
            }
            //if (string.IsNullOrEmpty(textBox2.Text) || !textBox2.Text.StartsWith("rtsp:"))
            //{
            //    MessageBox.Show("请输入摄像机rtsp地址！");
            //    return;
            //}

            var url = "ws://" + textBox1.Text + ":9000/video";
            var rtsp = cmbrtsp.Text;
            rtsp = HttpUtility.UrlEncode(rtsp);
            if (radioButton1.Checked)
            {
                var all = string.Concat(url, "?url=", rtsp);
                ws = new WebSocket(all);
            }
            if (radioButton2.Checked)
            {
                var all = string.Concat(url, "?url=", rtsp);
                ws = new WebSocket(all);
            }
            if (radioButton3.Checked)
            {
                WSMethod method = new WSMethod();
                JavaScriptSerializer js = new JavaScriptSerializer();
                var json = js.Serialize(method);
                var all = string.Format(url + "?url={0}&method={1}", rtsp, json);
                ws = new WebSocket(all);
            }
            ws.OnError += Ws_OnError1;
            ws.OnClose += Ws_OnClose;
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += Ws_OnMessage;
            ws.Connect();
        }

        private void Ws_OnError1(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            lblState.Text = "连接错误->" + e.Message + " " + DateTime.Now.ToString("HH:mm:ss");
        }

        int count = 0;
        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                Debug.WriteLine("hz:" + e.Data);
                var js = new JavaScriptSerializer();
                if (radioButton1.Checked)
                {
                    //旧协议
                    var face_old = js.Deserialize<FaceRecognized_Old>(e.Data);
                    if (face_old.type == RecognizeState.recognized.ToString())
                    {
                        showFace(face_old.person.name, face_old.data.face.image);
                    }
                }
                if (radioButton2.Checked)
                {
                    //新协议
                    var face_new = js.Deserialize<FaceRecognized_New>(e.Data);
                    if (face_new.type == RecognizeState.recognized.ToString())
                    {
                        showFace(face_new.person.name, face_new.data.face.image);
                    }
                }
            }
        }

        private void showFace(string name, string base64)
        {
            try
            {
                var buffer = Convert.FromBase64String(base64);
                var ms = new MemoryStream(buffer);
                var image = Image.FromStream(ms);
                count++;
                this.Invoke(new Action(() =>
                {
                    label3.Text = name;//+ "-" + (int)face.data.person.confidence;
                    label4.Text = count.ToString();
                    showFace(image);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("异常->" + ex.Message);
            }
        }

        private void showFace(Image image)
        {
            pictureBox1.Image = image;
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                lblState.Text = "连接成功，请进行人脸识别";
            }));
        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            Invoke(new Action(() =>
            {
                lblState.Text = "连接关闭" + DateTime.Now.ToString("HH:mm:ss");
            }));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            ws?.Close();
            base.OnClosing(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WebSocketSharp.WebSocket ws = new WebSocket("ws://192.168.1.73:8880/VBP_Setting/websocket");
            ws.OnOpen += Ws_OnOpen1;
            ws.OnMessage += Ws_OnMessage1;
            ws.Connect();

        }

        private void Ws_OnMessage1(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void Ws_OnOpen1(object sender, EventArgs e)
        {
        }
    }
}
