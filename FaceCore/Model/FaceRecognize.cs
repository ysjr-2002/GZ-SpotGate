using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceCore.Model
{
    public class FaceRecognize
    {
        public Employee person { get; set; }

        public bool open_door { get; set; }

        public string type { get; set; }

        public Data data { get; set; }

        public string error { get; set; }
    }
}
