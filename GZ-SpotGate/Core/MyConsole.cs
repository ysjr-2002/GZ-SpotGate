using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GZ_SpotGate.Core
{
    class MyConsole
    {
        private static MyConsole _output;

        private static object sync = new object();

        public static MyConsole Current
        {
            get
            {
                if (_output == null)
                {
                    lock (sync)
                    {
                        if (_output == null)
                        {
                            _output = new MyConsole();
                        }
                    }
                }
                return _output;
            }
        }

        private MyConsole()
        {
        }

        private TextBox _textbox;
        public void Init(TextBox textbox)
        {
            _textbox = textbox;
        }

        private string prefix
        {
            get
            {
                return DateTime.Now.HMS() + "->";
            }
        }

        public void Log(string log)
        {
            Log(new[] { log });
        }

        public void Log(string[] lines)
        {
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.Append(prefix + line);
            }

            lock (sync)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _textbox.AppendText(sb.ToString());
                    _textbox.ScrollToEnd();
                });
            };
        }
    }
}
