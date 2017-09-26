using GZ_SpotGate.Core;
using IPVoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IPVoice
{
    /// <summary>
    /// 播放网络声音
    /// </summary>
    public static class Voice
    {
        public static void Speak(string filename, IntPtr playHandle)
        {
            try
            {
                var init = LCAudioThrDll.lc_init(filename, playHandle);
                if (init == 0)
                {
                    var playId = LCAudioThrDll.lc_play(playHandle);
                }
            }
            catch (Exception ex)
            {
                MyConsole.Current.Log("语音播放失败->" + ex.Message);
            }
        }
    }
}
