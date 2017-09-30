using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class WSMethod
    {
        public WSMethod()
        {
            snapshot = true;
            attribute = true;
        }

        public bool snapshot { get; set; }
        public bool attribute { get; set; }
        //public snapshot_config snapshot_config { get; set; }
    }

    class snapshot_config
    {
        public bool disabled { get; set; }
    }
}
