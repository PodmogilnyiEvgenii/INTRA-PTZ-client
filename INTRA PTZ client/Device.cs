using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace INTRA_PTZ_client
{
    public class Device
    {
        private MainWindow mainWindow;
        private UDP udp;

        private bool online = false;
        private int currentPan = 0;
        private int currentTilt = 0;
        private int maxStepPan = 0;
        private int maxStepTilt = 0;

        private readonly int minPan = 0;
        private readonly int maxPan = 360;
        private readonly int minTilt = -90;
        private readonly int maxTilt = 45;

        public Device(MainWindow mainWindow)
        {
            this.MainWindow = mainWindow;
            Udp = new UDP(this);
        }

        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        public UDP Udp { get => udp; set => udp = value; }
        public int MinPan => minPan;
        public int MaxPan => maxPan;
        public int MinTilt => minTilt;
        public int MaxTilt => maxTilt;

        public int GetCurrentPan()
        {
            return currentPan;
        }

        public void SetCurrentPan(byte hh, byte ll)
        {
            currentPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            //System.Diagnostics.Trace.WriteLine(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }));
        }

        public int GetCurrentTilt()
        {
            return currentTilt;
        }

        public void SetCurrentTilt(byte hh, byte ll)
        {
            currentTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            //System.Diagnostics.Trace.WriteLine(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }));
        }

        public int GetMaxStepPan()
        {
            return maxStepPan;
        }

        public void SetMaxStepPan(byte hh, byte ll)
        {
            maxStepPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            //System.Diagnostics.Trace.WriteLine(BitConverter.ToInt32(new byte[] {ll, hh,0x00, 0x00 }));

        }

        public int GetMaxStepTilt()
        {
            return maxStepTilt;
        }

        public void SetMaxStepTilt(byte hh, byte ll)
        {
            maxStepTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            //System.Diagnostics.Trace.WriteLine(BitConverter.ToInt32(new byte[] {ll, hh,0x00, 0x00 }));
        }


        public bool GetOnline()
        {
            return online;
        }

        public void SetOnline(bool value)
        {
            online = value;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() => MainWindow.ServiceWindow.isOnline.Text = value ? "online" : "offline"));
        }

        public void parseRequest(byte[] request)
        {
            if (request[1] == AppOptions.DeviceAdress)
            {
                //System.Diagnostics.Trace.WriteLine("OK address");
                switch (PelcoDE.getDescriptionRequest(request[3]))
                {
                    case "Pan": SetCurrentPan(request[4], request[5]); break;
                    case "Tilt": SetCurrentTilt(request[4], request[5]); break;
                    case "MaxStepPan": SetMaxStepPan(request[4], request[5]); break;
                    case "MaxStepTilt": SetMaxStepTilt(request[4], request[5]); break;
                }
            }
        }

    }
}
