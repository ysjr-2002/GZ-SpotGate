using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            api = new API();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            login = await api.Login(textBox1.Text, textBox2.Text);
            if (login)
            {
                MessageBox.Show("登录成功");
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
                MessageBox.Show("请选择图像");
                return;
            }

            var upload = await api.UpdatePhoto(txtPhoto.Text);
            if (upload != null && upload.code == 0)
            {
                photo_id = upload.data.id;
                //var avatar = await api.UpdateAvatar(txtPhoto.Text);
                //avatarurl = avatar.data.url;
                MessageBox.Show("符合识别要求");
            }
            else
            {
                MessageBox.Show("请查看文档对应的错误码->" + upload?.code);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            txtPhoto.Text = OpenFileDialog();
        }

        private string OpenFileDialog()
        {
            var filter = "Jpg文件|*.jpg|Png文件|*.png";
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
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
                MessageBox.Show("请上传人像");
                return;
            }

            serverId = await api.CreateSubjectWithPhotos(txtName.Text, txtJobNumber.Text, avatarurl, new string[] { photo_id.ToString() });
            if (serverId > 0)
            {
                MessageBox.Show("创建用户成功");
            }
            else
            {
                MessageBox.Show("创建用户失败");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!login)
                return;

            if (api.DeleteSubject(serverId))
            {
                MessageBox.Show("删除成功");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var list = (api.GetSubject()).Deserialize<SubjectList>();
            foreach (var item in list.data)
            {
                Console.WriteLine(item.name + " " + item.job_number + " " + item.avatar);
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
        }

        private void button7_Click(object sender, EventArgs e)
        {
            api.CreateVisitor(txtPhoto.Text);
        }
    }
}
