using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GZ_SpotGateEx
{
    class MyStandardKernel : Ninject.StandardKernel
    {
        static StandardKernel sk = new StandardKernel();

        private MyStandardKernel()
        {
            sk.Load(Assembly.GetExecutingAssembly());
        }

        public static StandardKernel Instance
        {
            get
            {
                return sk;
            }
        }
    }
}
