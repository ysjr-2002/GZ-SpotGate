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
        public static void Speak(IntPtr ptr, string ip, string filename)
        {
            IntPtr playHandle = IntPtr.Zero;
            try
            {
                var PlayParam = LCAudioThrDll.GetPlayPlayParam(ptr, ip);
                int size = Marshal.SizeOf(PlayParam);
                playHandle = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(PlayParam, playHandle, false);
                var init = LCAudioThrDll.lc_init(filename, playHandle);
                if (init == 0)
                {
                    var playId = LCAudioThrDll.lc_play(playHandle);
                }

                var len = LCAudioThrDll.lc_get_duration(playHandle);
                len = len / 1000;
            }
            finally
            {
                //Marshal.FreeHGlobal(playHandle);
            }
        }
    }
}
