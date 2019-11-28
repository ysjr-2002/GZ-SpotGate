using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.IDCard
{
    class IDPhotoHelper
    {
        [DllImport("libwlt2bmp.dll")]
        public static extern int wlt2bmp(byte[] bytes);

        [DllImport("libwlt2bmp.dll")]
        public static extern int wlt2bmpBuffer(byte[] bytes, IntPtr ptr, int len);

        public static void Save(string cardno, byte[] wltbuffer)
        {
            var path = "zp";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            int ret = wlt2bmp(wltbuffer);
            var len = (102 * 3 + 2) * 126 + 54;
            var bmpbuffer = new byte[len];
            var ptr = Marshal.AllocHGlobal(bmpbuffer.Length);
            ret = wlt2bmpBuffer(wltbuffer, ptr, bmpbuffer.Length);
            Marshal.Copy(ptr, bmpbuffer, 0, bmpbuffer.Length);
            var ms = new MemoryStream(bmpbuffer);
            var bitmap = System.Drawing.Image.FromStream(ms);
            bitmap.Save(path + "\\" + cardno + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }
    }
}
