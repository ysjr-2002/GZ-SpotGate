using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using System.Net.Sockets;
using Java.Util;
using Java.Net;
using Android.Util;

namespace GZ_SpotVisual
{
    public static class IPUtil
    {
        public static String getIP(Context context)
        {
            String hostIp = null;
            try
            {
                IEnumeration nis = Java.Net.NetworkInterface.NetworkInterfaces;
                InetAddress ia = null;
                while (nis.HasMoreElements)
                {
                    NetworkInterface ni = (NetworkInterface)nis.NextElement();
                    IEnumeration ias = ni.InetAddresses;
                    while (ias.HasMoreElements)
                    {
                        ia = ias.NextElement() as InetAddress;
                        if (ia is Inet6Address)
                        {
                            continue;// skip ipv6
                        }
                        String ip = ia.HostAddress;
                        if (!"127.0.0.1".Equals(ip))
                        {
                            hostIp = ia.HostAddress;
                            break;
                        }
                    }
                }
            }
            catch (Java.Net.SocketException e)
            {
                Log.Info("IPUtil", "SocketException");
                e.PrintStackTrace();
            }
            return hostIp;
        }

        public static String GetHostIp()
        {
            var hostIp = "";
            try
            {
                var hostname = Dns.GetHostName();
                IPAddress[] ipaddresses = Dns.GetHostAddresses(hostname);
                if (ipaddresses == null)
                    return string.Empty;

                foreach (IPAddress address in ipaddresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if ("127.0.0.1" != address.ToString())
                        {
                            hostIp = address.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return hostIp;
        }
    }
}