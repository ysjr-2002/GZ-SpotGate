using GZSpotGate;
using LL.SenicSpot.Gate.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace System
{
    class LogHelper
    {
        static readonly int MAX_COUNT = 100;
        static readonly ILog log = LogManager.GetLogger("Gate");

        public static StackPanel sprecord;

        public static void Log(string content)
        {
            log.Debug(content);
        }

        public static void AppendWXLog(string content)
        {
            log.Debug(content);
        }

        public static void Append(Record data)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    if (sprecord.Children.Count >= MAX_COUNT)
                    {
                        sprecord.Children.Clear();
                    }

                    ItemControl item = new ItemControl();
                    item.DataContext = data;
                    sprecord.Children.Insert(0, item);
                    data.Output();
                }
                catch
                {
                }
            });
        }
    }
}
