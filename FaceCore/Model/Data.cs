using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceCore.Model
{
    public class Data
    {
        public string status { get; set; }

        public Attr attr { get; set; }

        public long track { get; set; }

        public long timestamp { get; set; }

        public Face face { get; set; }

        public Person person { get; set; }

        public double quality { get; set; }
    }
}
