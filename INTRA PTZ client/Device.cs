using System;


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

        private int currentStepPan = 0;
        private int currentStepTilt = 0;
        //private int currentStepZoom = 0;
        //private int currentStepFocus = 0;

        private int maxStepPan = 0;
        private int maxStepTilt = 0;
        private int maxStepZoom = 0;
        private int maxStepFocus = 0;

        private int temperature = 0;

        private readonly float minPan = 0;
        private readonly float maxPan = 359.99f;
        private readonly float minTilt = -90;
        private readonly float maxTilt = 90; //45

        public Device(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.udp = new UDP(mainWindow, this);
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
            currentPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * maxPan / maxStepPan;
            currentStepPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public float GetCurrentTilt()
        {
            return currentTilt;
        }
        public void SetCurrentTilt(byte hh, byte ll)
        {
            currentTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * maxTilt / maxStepTilt;
            currentStepTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
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

                    case "Register":                        break; 

                }
            }

        }

        public override string ToString()
        {
            String res = "Pan= " + currentPan + "\n";
            res += "Tilt= " + currentTilt + "\n";
            res += "stepPan= " + currentStepPan + "\n";
            res += "stepTilt= " + currentStepTilt + "\n";
            res += "MaxStepPan= " + maxStepPan + "\n";
            res += "MaxStepTilt= " + maxStepTilt + "\n";
            res += "Temperature= " + temperature + "\n";
            res += "Online= " + isOnline + "\n";

            return res;
        }

        public int panAngleToStep(string angle)
        {
            try
            {
                //System.Diagnostics.Trace.WriteLine(angle);
                //System.Diagnostics.Trace.WriteLine(float.Parse(angle) * maxStepPan / (maxPan - minPan));

                return (int)Math.Round(float.Parse(angle) * maxStepPan / (maxPan - minPan));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public int tiltAngleToStep(string angle)
        {
            try
            {
                return (int)Math.Round(float.Parse(angle) * maxStepTilt / (maxTilt - minTilt));
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
