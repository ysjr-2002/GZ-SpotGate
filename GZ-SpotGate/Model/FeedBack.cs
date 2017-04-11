using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Model
{
    public class FeedBack
    {
        public int code { get; set; }

        public string message { get; set; }

        public string sound { get; set; }

        public string personCount { get; set; }

        public string personOnceCount { get; set; }

        public string direction { get; set; }

        public string contentType { get; set; }
    }
}
