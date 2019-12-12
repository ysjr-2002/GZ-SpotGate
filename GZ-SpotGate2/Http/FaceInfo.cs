using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HXMansion.Http
{
    public class PushFace
    {
        public double confidence { get; set; }
        /// <summary>
        /// 是否通过fmp检测
        /// </summary>
        public int fmp_error { get; set; }
        /// <summary>
        /// 0表示可信
        /// </summary>
        public int event_type { get; set; }
        public string timestamp { get; set; }
        [JsonIgnoreAttribute]
        public string photo { get; set; }
        public object age { get; set; }
        public double fmp { get; set; }
        public string screen_token { get; set; }
        public object quality { get; set; }
        /// <summary>
        /// 陌生人为None，员工或访客为实际的subject_id
        /// </summary>
        public string subject_id { get; set; }
    }
}
