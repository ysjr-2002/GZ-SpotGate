using GZ_SpotGate.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GZ_SpotGate.WS
{
    class AndroidBehavior : WebSocketBehavior
    {
        private Action<string> action;

        public AndroidBehavior()
        {
            var ip = "";
            if (this.Context != null)
            {
                if (this.Context.UserEndPoint != null)
                {
                    ip = this.Context.UserEndPoint.Address.ToString();
                }
            }
            MyConsole.Current.Log("Pad连接");
        }

        public AndroidBehavior(Action<string> action)
        {
            this.action = action;
            this.EmitOnPing = true;
        }
    }
}
