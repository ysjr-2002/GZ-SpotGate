using IPVoice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPVoiceSafe
{
    public partial class FrmMain : Form
    {
        /*Windows 消息定义*/
        public const int USER = 0x0400;
        public const int WM_USER = 0x400;
        public const int WM_MSG_STOP = USER + 101;
        public const int WM_MSG_PAUSE = USER + 102;
        public const int WM_MSG_CONTINUE = USER + 103;

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_MSG_STOP:
                    MessageBox.Show("播放结束 \r\n");
                    break;
                case WM_MSG_PAUSE:
                    MessageBox.Show("播放暂停 \r\n");
                    break;
                case WM_MSG_CONTINUE:
                    MessageBox.Show("播放继续 \r\n");
                    break;
                default:
                    base.DefWndProc(ref m);//调用基类函数处理非自定义消息。 
                    break;
            }
        }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.mp3|*.mp3|*.wav|*.wav";
            var result = ofd.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            var filename = ofd.FileName;
            string ip = "192.168.1.101";
            Voice.Speak(this.Handle, ip, filename);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //LCAudioThrDll.lc_pause(PlayHandle);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //LCAudioThrDll.lc_continue(PlayHandle);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //LCAudioThrDll.lc_stop(PlayHandle);
        }
    }
}
