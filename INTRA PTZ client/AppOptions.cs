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
        public static readonly bool DEBUG = true;

        public static readonly int UDP_TIMEOUT_SHORT = 1000;
        public static readonly int UDP_TIMEOUT_LONG = 10000;

        public static readonly int UDP_TRY_COMMAND_SEND = 3;

        public static readonly int REQUEST_COORDINATE_TIMEOUT = 2000;

    }
}
