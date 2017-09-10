using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Old
{
    public class Rect
    {
        public int top { get; set; }

        public int right { get; set; }

        public int bottom { get; set; }

        public int left { get; set; }
    }

    public class Face
    {
        public string image { get; set; }

        public Rect rect { get; set; }
    }

    public class DataPerson
    {
        public int feature_Id { get; set; }

        public double confidence { get; set; }

        public string tag { get; set; }

        public string id { get; set; }
    }

    public class Data
    {
        public string status { get; set; }

        public attr attr { get; set; }

        public long track { get; set; }

        public long timestamp { get; set; }

        public Face face { get; set; }

        public DataPerson person { get; set; }

        public double quality { get; set; }
    }

    public class attr
    {
        public double age { get; set; }

        public double male { get; set; }

        public double female { get; set; }
    }

    public class Person
    {
        public string src { get; set; }

        public string remark { get; set; }

        public int subject_type { get; set; }

        public string description { get; set; }

        public string title { get; set; }

        public long timestamp { get; set; }

        public int start_time { get; set; }

        public string avatar { get; set; }

        public string job_number { get; set; }

        public object birthday { get; set; }

        public object entry_date { get; set; }

        public string department { get; set; }

        public int end_time { get; set; }

        public int id { get; set; }

        public string name { get; set; }
    }

    public class FaceRecognized_Old
    {
        public Person person { get; set; }

        public bool open_door { get; set; }

        public string type { get; set; }

        public Data data { get; set; }

        public string error { get; set; }
    }
}
