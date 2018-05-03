using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceCore.Model
{
    public class Person
    {
        public int feature_Id { get; set; }

        public double confidence { get; set; }

        public string tag { get; set; }

        public string id { get; set; }
    }
}
