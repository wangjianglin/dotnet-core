using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Lin.Core.Utils
{
    public static class PingPhysicalAddress
    {
        private static object lockBody = new object();
        public static bool PingIp(string IP)
        {
            lock (lockBody)
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(IP);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else if (reply.Status == IPStatus.DestinationHostUnreachable || reply.Status == IPStatus.DestinationPortUnreachable || reply.Status == IPStatus.HardwareError)
                {
                    //return false;
                    throw new Exception("请检查设备的网络设备以及网线或者无线连接情况。");
                }
                else if (reply.Status == IPStatus.DestinationNetworkUnreachable || reply.Status == IPStatus.BadRoute || reply.Status == IPStatus.BadDestination || reply.Status == IPStatus.TimedOut)
                {
                    //return false;
                    throw new Exception("请检查设备间的IP地址。");
                }
                else
                {
                    //return false;
                    throw new Exception("网络连接异常。");
                }
            }
        }
    }
}
