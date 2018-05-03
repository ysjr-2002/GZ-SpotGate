using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FaceCore.Model
{
    public class Screen
    {
        public string camera_address { get; set; }

        public object[] allowed_subject_ids { get; set; }

        public string network_switcher_status { get; set; }

        public string box_token { get; set; }

        public string description { get; set; }

        public long box_heartbeat { get; set; }

        public string network_switcher { get; set; }

        public string camera_name { get; set; }

        public string camera_status { get; set; }

        public bool allow_visitor { get; set; }

        public string screen_token { get; set; }

        public object network_switcher_token { get; set; }

        public string box_status { get; set; }

        public bool allow_all_subjects { get; set; }

        public int type { get; set; }

        public int id { get; set; }

        public string camera_position { get; set; }

        public string box_address { get; set; }
    }
}
