using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPVoice
{
    public unsafe partial class FrmMain : Form
    {

        /*Windows 消息定义*/
        public const int USER = 0x0400;
        public const int WM_USER = 0x400;
        public const int WM_MSG_STOP = USER + 101;

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_MSG_STOP:
                    MessageBox.Show("播放结束 \r\n");
                    break;
                default:
                    base.DefWndProc(ref m);//调用基类函数处理非自定义消息。 
                    break;
            }
        }

        public FrmMain()
        {
            InitializeComponent();
            PlayParam = LCAudioThrDll.LCPlayGetMem();
        }


        _PlayParam* PlayParam;
        int init = -1;
        int playId = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            var name = ofd.FileName;
            //var ip = IPAddress.Parse("192.168.1.10");
            //var x = Convert.ToUInt32(ip.Address);
            //PlayParam = new _PlayParam
            //{
            //    hWnd = (UInt32)this.Handle,
            //    Priority = 1,
            //    IP = x,
            //    SourcType = 0,
            //    CastMode = 0,
            //    Volume = 80
            //};
            //init = LCAudioThrDll.lc_init(name, ref PlayParam);
            //if (init == 0)
            //{
            //    playId = Voice.lc_play(ref PlayParam);
            //}
            //else
            //{
            //    MessageBox.Show("初始化失败！");
            //}

            PlayParam->hWnd = (UInt32)this.Handle;
            PlayParam->Priority = 80;
            PlayParam->Volume = 80;
            PlayParam->SourcType = 0;
            System.Net.IPAddress ipaddress = System.Net.IPAddress.Parse("192.168.1.10");
            PlayParam->CastMode = 0;//广播模式
            PlayParam->IP = Convert.ToUInt32(ipaddress.Address);

            init = LCAudioThrDll.LCInit(name, PlayParam);
            if (init == 0)
            {
                playId = LCAudioThrDll.LCPlay(PlayParam);
            }
            else
            {
                MessageBox.Show("初始化失败！");
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
