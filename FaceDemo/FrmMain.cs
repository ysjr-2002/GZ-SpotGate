using FaceCore;
using FaceCore.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FaceDemo
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        FaceCore.FaceSocket socket = null;
        private void button1_Click(object sender, EventArgs e)
        {
            socket = new FaceSocket(this.textBox1.Text.Trim(), this.textBox2.Text.Trim(), OnFaceRecognize);
            var result = socket.Connect().Result;
            if (result)
            {
                btnConnect.Enabled = false;
            }
        }

        private void OnFaceRecognize(FaceRecognize face)
        {
            showFace(face.person.name, face.data.face.image);
        }

        private void showFace(string name, string base64)
        {
            try
            {
                var buffer = Convert.FromBase64String(base64);
                var stream = new MemoryStream(buffer);
                var image = Image.FromStream(stream);
                this.Invoke(new Action(() =>
                {
                    lblname.Text = name;
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (socket != null)
            {
                socket.Disconnect();
                socket = null;
                btnConnect.Enabled = true;
            }
        }
    }
}
