using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GZ_SpotGateEx.Model
{
    class Record
    {
        public string Channel { get; set; }
        public string BarCode { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string CheckTime { get; set; }
        public int StatuCode { get; set; }
        public string TypeImageSourceUrl { get; set; }
    }
}
