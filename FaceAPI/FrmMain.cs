using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceAPI
{
    public partial class FrmMain : Form
    {
        API api = null;
        bool login = false;
        public FrmMain()
        {
            InitializeComponent();
            var filename = "account.txt";
            if (File.Exists(filename))
            {
                var lines = File.ReadAllLines(filename);
                comboBox1.Items.AddRange(lines);
                comboBox1.SelectedIndex = 1;
            }

            if (cmbHost.Items.Count > 0)
            {
                cmbHost.SelectedIndex = 0;
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                tip("请输入账号和密码");
                return;
            }
            if (cmbHost.Text.Contains("v2"))
                API.root = "https://" + cmbHost.Text;
            else
                API.root = "http://" + cmbHost.Text;
            api = new API();
            login = await api.Login(comboBox1.Text, textBox2.Text);
            if (login)
            {
                tip("登录成功");
            }
            else
            {
                tip("登录失败");
            }
        }

        int photo_id = -1;
        string avatarurl = "";
        private async void button2_Click(object sender, EventArgs e)
        {
            if (!login)
                return;

            if (txtPhoto.Text.IsEmpty())
            {
                tip("请选择图像");
                return;
            }

            var upload = await api.UpdatePhoto(txtPhoto.Text);
            if (upload != null && upload.code == 0)
            {
                photo_id = upload.data.id;
                //var avatar = await api.UpdateAvatar(txtPhoto.Text);
                //avatarurl = avatar.data.url;
                tip("符合识别要求");
            }
            else
            {
                tip("请查看文档对应的错误码->" + upload?.code);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            txtPhoto.Text = OpenFileDialog();
        }

        private string OpenFileDialog()
        {
            var filter = "Jpg文件|*.jpg|Png文件|*.png";
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = filter;
            dialog.Title = "选择文件";
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
                return dialog.FileName;
            else
                return string.Empty;
        }

        int serverId = 0;
        private async void button3_Click(object sender, EventArgs e)
        {
            if (!login)
                return;

            if (photo_id == 0)
            {
                tip("请上传人像");
                return;
            }

            await Task.Factory.StartNew(async () =>
            {
                var i = 0;
                //for (int i = 1; i <= 2000; i++)
                //{
                var upload = await api.UpdatePhoto(txtPhoto.Text);
                if (upload != null && upload.code == 0)
                {
                    photo_id = upload.data.id;
                }
                serverId = await api.CreateSubjectWithPhotos(txtName.Text, i.ToString(), avatarurl, new int[] { photo_id }, false);
                if (serverId > 0)
                    tip("创建用户->" + serverId);
                //}
            });
            //serverId = await api.CreateSubjectWithPhotos(txtName.Text, txtJobNumber.Text, avatarurl, new int[] { photo_id });
            //if (serverId > 0)
            //{
            //    tip("创建用户成功");
            //}
            //else
            //{
            //    tip("创建用户失败");
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!login)
                return;

            if (api.DeleteSubject(serverId))
            {
                tip("删除成功");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var list = (api.GetSubject()).Deserialize<SubjectList>();
            foreach (var item in list.data)
            {
                Console.WriteLine(item.id + " " + item.name + " " + item.job_number + " " + item.avatar);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
        }

        private async void button7_Click(object sender, EventArgs e)
        {
            //var i = 0;
            //var kk = api.CreateVisitor("访客" + i, txtPhoto.Text);
            //Console.WriteLine(kk);
            var serverId = await api.CreateSubjectWithPhotos(txtName.Text, txtJobNumber.Text, "", new int[] { photo_id }, true);
            if (serverId > 0)
            {
                tip("创建访客->" + serverId);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //user.name = "杨绍杰";
            //user.department = "华尊";
            //user.title = "研发工程师";
            //user.phone = "15914112520";
            ////user.email = "ysjr-2002@163.com";
            //user.photo_ids = new int[] { user.photos.First().id };
            //api.UpdateUser(user);
        }

        SubjectData user = null;
        private void button9_Click(object sender, EventArgs e)
        {
            //user = api.GetUser(8).data;
            var tt = api.GetEx("/subject1/abc");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var start = Convert.ToInt32(textBox1.Text);
            for (int i = start; i < 9999999; i++)
            {
                Stopwatch sw = Stopwatch.StartNew();
                api.DeleteSubject(i);
                sw.Stop();
                Console.WriteLine("time->" + sw.ElapsedMilliseconds);
            }
        }

        private void tip(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var content = api.Recognize(txtPhoto.Text);
            richTextBox1.Text = content;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            txtAvatar.Text = OpenFileDialog();
        }

        private async void button13_Click(object sender, EventArgs e)
        {
            var data = await api.UpdateAvatar(serverId, txtAvatar.Text);
            if (data.code == 0)
            {
                var url = data.data.url;
            }
        }

        private async void button14_Click(object sender, EventArgs e)
        {
            var content = await api.CheckIn(7, txtPhoto.Text);
            richTextBox1.Text = content;
            var result = content.Deserialize<error>();
        }
    }
}
