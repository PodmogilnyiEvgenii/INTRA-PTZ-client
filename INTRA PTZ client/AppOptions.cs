using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace INTRA_PTZ_client
{
    public static class AppOptions
    {
        private static String deviceIp = "10.130.250.197";
        private static String deviceMask = "255.255.255.0";
        private static int devicePort = 6000;
        private static int deviceAdress = 1;
        private static int deviceSpeed = 10;

        public static string DeviceIp { get => deviceIp; set => deviceIp = value; }
        public static string DeviceMask { get => deviceMask; set => deviceMask = value; }
        public static int DevicePort { get => devicePort; set => devicePort = value; }
        public static int DeviceAdress { get => deviceAdress; set => deviceAdress = value; }
        public static int DeviceSpeed { get => deviceSpeed; set => deviceSpeed = value; }        
    }
}
