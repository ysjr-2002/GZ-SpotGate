using IPVoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IPVoiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"C:\Users\Shaojie\Desktop\ringin.wav";
            var ip = IPAddress.Parse("192.168.1.10");
            var iplong = BitConverter.ToUInt32(ip.GetAddressBytes(), 0);
            var PlayParam = new PlayParam
            {
                hWnd = IntPtr.Zero,
                Priority = 1,
                IP = iplong,
                SourcType = 0,
                CastMode = 0,
                Volume = 80
            };

            int size = Marshal.SizeOf(PlayParam);
            IntPtr PlayHandle = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(PlayParam, PlayHandle, false);
            var init = LCAudioThrDll.lc_init(filename, PlayHandle);
            if (init == 0)
            {
                var playId = LCAudioThrDll.lc_play(PlayHandle);
            }
            Console.Read();
        }
    }
}
