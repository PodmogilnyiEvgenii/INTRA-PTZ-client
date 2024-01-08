namespace INTRA_PTZ_client
{
    public static class AppOptions
    {
        public static readonly bool DEBUG = true;

        public static readonly int UDP_TIMEOUT_SHORT = 1000;
        public static readonly int UDP_TIMEOUT_LONG = 10000;

        public static readonly int UDP_TRY_COMMAND_SEND = 3;

        public static readonly int REQUEST_COORDINATE_TIMEOUT = 2000;
        public static readonly int ROUTE_START_TIMEOUT = 100;

        public static readonly int[] SPEED_STEP_PAN = { 1, 16, 81, 162, 810, 1620, 4860, 9720 };

        public static readonly int[] SPEED_STEP_TILT = { 1, 8, 40, 81, 405, 810, 2430, 4860 };

        public static readonly int ERROR_TO_OFFLINE = 10;

        public static readonly int REFRESH_STATUS = 200;

        public static readonly int MAX_COMMAND_IN_QUEUE = 20;
    }
}
