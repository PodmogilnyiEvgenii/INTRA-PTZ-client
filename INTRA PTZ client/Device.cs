using System;
using System.Collections.Generic;
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
        private int pan = 0;
        private int tilt = 0;

        public Device(MainWindow mainWindow)
        {   
            this.mainWindow = mainWindow;
            Udp = new UDP(this);
        }

        public bool GetOnline()
        {
            return online;
        }

        public void SetOnline(bool value)
        {
            online = value;
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() => mainWindow.ServiceWindow.isOnline.Text = value ? "online" : "offline"));
        }
        
        public int Pan { get => pan; set => pan = value; }
        public int Tilt { get => tilt; set => tilt = value; }
        public UDP Udp { get => udp; set => udp = value; }
        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
    }
}
