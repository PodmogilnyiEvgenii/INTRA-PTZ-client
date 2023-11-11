using System;
using System.Net.Sockets;
using System.Threading;
using System.Timers;
using System.Windows.Threading;


namespace INTRA_PTZ_client
{
    public class Device
    {
        private MainWindow mainWindow;
        private UDP udp;        

        private string ip = "10.130.250.197";
        private string mask = "255.255.255.0";
        private int port = 6000;
        private int address = 1;
        private int movingSpeed = 1;

        private bool isOnline = false;
        private float currentPan = 0;
        private float currentTilt = 0;
        private float currentZoom = 0;
        private float currentFocus = 0;

        private int maxStepPan = 0;
        private int maxStepTilt = 0;
        private int maxStepZoom = 0;
        private int maxStepFocus = 0;

        private int temperature = 0;

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

        public string Ip { get => ip; set => ip = value; }
        public string Mask { get => mask; set => mask = value; }
        public int Port { get => port; set => port = value; }
        public int Address { get => address; set => address = value; }
        public int MovingSpeed { get => movingSpeed; set => movingSpeed = value; }
               

        public float GetCurrentZoom()
        {
            return currentZoom;
        }

        public void SetCurrentZoom(byte hh, byte ll)
        {
            //TODO
            currentZoom = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public float GetCurrentFocus()
        {
            return currentFocus;
        }

        public void SetCurrentFocus(byte hh, byte ll)
        {
            //TODO
            currentFocus = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public int GetTemperature()
        {
            return temperature;
        }
        public void SetTemperature(byte hh, byte ll)
        {
            //TODO
            temperature = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public float GetCurrentPan()
        {
            return currentPan;
        }
        public void SetCurrentPan(byte hh, byte ll)
        {
            currentPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 })*maxPan/maxStepPan;            
        }

        public float GetCurrentTilt()
        {
            return currentTilt;
        }
        public void SetCurrentTilt(byte hh, byte ll)
        {
            currentTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 })*maxTilt/maxStepTilt;            
        }

        public int GetMaxStepPan()
        {
            return maxStepPan;
        }
        public void SetMaxStepPan(byte hh, byte ll)
        {
            maxStepPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public int GetMaxStepTilt()
        {
            return maxStepTilt;
        }
        public void SetMaxStepTilt(byte hh, byte ll)
        {
            maxStepTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public int GetMaxStepZoom()
        {
            return maxStepZoom;
        }
        public void SetMaxStepZoom(byte hh, byte ll)
        {
            maxStepZoom = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public int GetMaxStepFocus()
        {
            return maxStepFocus;
        }
        public void SetMaxStepFocus(byte hh, byte ll)
        {
            maxStepFocus = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }


        public bool GetOnline()
        {
            return isOnline;
        }
        public void SetOnline(bool value)
        {
            isOnline = value;                        
        }
        

        public String getStatusString()
        {
            return "IP: " + ip + ":" + port + "   " + "Address: " + address + "   " + (isOnline ? "Online" : "Offline") + "   Pan:" + currentPan + "   Tilt:" + currentTilt;
        } 

        public void parseRequest(byte[] request)
        {
            if (request[1] == address)
            {
                SetOnline(true);
                switch (PelcoDE.getDescriptionRequest(request[3]))
                {
                    case "Pan": SetCurrentPan(request[4], request[5]); break;
                    case "Tilt": SetCurrentTilt(request[4], request[5]); break;
                    case "Zoom": SetCurrentZoom(request[4], request[5]); break;
                    case "Focus": SetCurrentFocus(request[4], request[5]); break;

                    case "MaxStepPan": SetMaxStepPan(request[4], request[5]); break;
                    case "MaxStepTilt": SetMaxStepTilt(request[4], request[5]); break;
                    case "MaxStepZoom": SetMaxStepZoom(request[4], request[5]); break;
                    case "MaxStepFocus": SetMaxStepFocus(request[4], request[5]); break;


                    case "Temperature": SetTemperature(request[4], request[5]); break;

                }
            }
            
        }

    }
}
