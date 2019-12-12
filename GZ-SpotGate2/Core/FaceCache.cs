using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZSpotGate.Core
{
    public static class FaceCache
    {
        private static Dictionary<int, DateTime> cache = new Dictionary<int, DateTime>();

        public static bool IsOutInterval(int koalaId)
        {
            if (cache.ContainsKey(koalaId))
            {
                var time = cache[koalaId];
                var ts = DateTime.Now - time;
                if (ts.TotalSeconds > Config.Instance.Interval)
                {
                    cache[koalaId] = DateTime.Now;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                cache.Add(koalaId, DateTime.Now);
                return true;
            }
        }
    }
}
