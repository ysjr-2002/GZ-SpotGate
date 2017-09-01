using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IPVoice
{
    class LCAudioThrDll
    {
        [DllImport("LCAudioThrDll.dll")]
        public extern static int lc_init(string filename, IntPtr ptr);

        [DllImport("LCAudioThrDll.dll")]
        public static extern int lc_play(IntPtr ptr);

        [DllImport("LCAudioThrDll.dll")]
        public static extern int lc_pause(IntPtr Plm);

        [DllImport("LCAudioThrDll.dll")]
        public static extern int lc_continue(IntPtr Plm);

        [DllImport("LCAudioThrDll.dll")]
        public static extern int lc_stop(IntPtr Plm);

        [DllImport("LCAudioThrDll.dll")]
        public static extern int lc_get_playstatus(IntPtr Plm);


        

        public static int LCInit(string filename, IntPtr PlayParam)
        {
            //int len = System.Text.Encoding.Default.GetByteCount(filename);
            //byte[] fn = new byte[len + 1];
            //fn = CodeBytes(filename, len + 1);
            return lc_init(filename, PlayParam);
        }

        public static int LCPlay(IntPtr PlayParam)
        {
            return lc_play(PlayParam);
        }


        //将一个字符串转换成
        public static byte[] CodeBytes(string str, int len)
        {
            if (string.IsNullOrEmpty(str))
            {
                str = string.Empty;
            }

            byte[] result = new byte[len];
            byte[] strBytes = Encoding.Default.GetBytes(str);

            for (int i = 0; i < len; i++)
            {

                if (i < strBytes.Length)
                    result[i] = strBytes[i];
                else
                    result[i] = 0;
            }

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct _PlayParam
    {
        /// <summary>
        /// Windows窗口句柄，如果不为NULL，线程将事件消息发送到此窗口
        /// </summary>
        public UInt32 hWnd;
        /// <summary>
        /// 音频流优先级，0优先级最低，255优先级最高，音频终端优先播放优先级高的音频。
        /// </summary>
        public UInt32 Priority;
        /// <summary>
        /// 组播组号，表示线程向那个组发送音频数据，当播放方式是单播或广播是，此参数无意义。
        /// </summary>
        public UInt32 MultiGroup;
        /// <summary>
        /// 播放方式，单播，组播和广播
        /// </summary>
        public UInt32 CastMode;
        /// <summary>
        /// IP地址，当为播放方式单播是指的是目标IP地址，为组播和广播时，表示使用的本地网络接口
        /// </summary>
        public UInt32 IP;//20110402
        /// <summary>
        /// 音量
        /// </summary>
        public UInt32 Volume;
        /// <summary>
        /// 音调（未使用）
        /// </summary>
        public UInt32 Tone;
        /// <summary>
        /// 高音频率
        /// </summary>
        public UInt32 Treble;
        /// <summary>
        /// 低音频率
        /// </summary>
        public UInt32 Bass;
        /// <summary>
        /// 高音放大因子
        /// </summary>
        public UInt32 Treble_En;
        /// <summary>
        /// 低音放大因子
        /// </summary>
        public UInt32 Bass_En;
        /// <summary>
        /// 音频数据源，0表示数据源为文件，1表示数据源为声卡输入（Line in 或 mic输入）
        /// </summary>
        public UInt32 SourcType;

        /// <summary>
        /// 声卡ID号，仅在SourcType = 1时有效，表示采用哪个声卡的输入。
        /// </summary>
        public UInt32 DeviceID;
        /// <summary>
        /// 混音器名字，表示混音器的那个输入通道最终被采样。不用的声卡有不同的混音器名字。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] MuxName;
        /// <summary>
        /// 通道，采用的通道数
        /// </summary>
        public UInt32 nChannels;
        /// <summary>
        /// 音源为声卡输入时的采用频率
        /// </summary>
        public UInt32 nSamplesPerSec;
        /// <summary>
        /// 音频文件的长度，仅SourcType=0时有效
        /// </summary>
        public UInt32 AudioBufferLength;
        /// <summary>
        /// 音频数据存储地址
        /// </summary>
        public IntPtr AudioBuf;//20110402
        /// <summary>
        /// 私有数据数组，用户不能修改里面的数据。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public UInt32[] PrivateData;
    }
}
