using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GZ_SpotGate.Core
{
    class Output
    {
        private static Output _output;

        public static Output Current
        {
            get
            {
                if (_output == null)
                {
                    _output = new Output();
                }
                return _output;
            }
        }

        private Output()
        {
        }

        private TextBox _textbox;
        public void Init(TextBox textbox)
        {
            _textbox = textbox;
        }

        public void Log(string str)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _textbox.AppendText(str);
                _textbox.ScrollToEnd();
            });
        }
    }
}
