using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    /// <summary>
    /// 每日零点清空入园数量
    /// </summary>
    class ResetThread
    {
        private int preday = 0;
        private Thread thread;
        private bool running = false;
        private AutoResetEvent are = new AutoResetEvent(false);

        public void Start()
        {
            running = true;
            preday = DateTime.Now.DayOfYear;
            thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            while (running && !are.WaitOne(30 * 1000))
            {
                if (preday != DateTime.Now.DayOfYear)
                {
                    foreach (var item in Channels.ChannelList)
                    {
                        item.daycount = "0";
                    }
                    Channels.Save();
                    preday = DateTime.Now.DayOfYear;
                }
            }
        }

        public void Stop()
        {
            running = false;
            are.Set();
        }
    }
}
