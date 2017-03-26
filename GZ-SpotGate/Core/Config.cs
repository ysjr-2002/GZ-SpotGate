using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGate.Core
{
    class Config
    {
        private static Config _config = new Config();

        public static Config Current
        {
            get
            {
                return _config;
            }
        }

        public void Read()
        {

        }
    }
}
