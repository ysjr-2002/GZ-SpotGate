using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx.Core
{
    class MyLog
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger("GATE");
        public static void debug(string content)
        {
            log.Debug(content);
        }
    }
}
