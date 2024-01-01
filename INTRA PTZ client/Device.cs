using Newtonsoft.Json;
using System;

namespace INTRA_PTZ_client
{
    [Serializable]
    public class Device
    {
        [JsonIgnore] private MainWindow mainWindow;
        [JsonIgnore] private UDP udp;
        private Route route;
        private Preset preset;

        private string ip = "10.130.250.197";
        private string mask = "255.255.255.0";
        private int port = 6000;
        private int address = 1;
        //private int speed = -1;
        //private int acceleration = -1;

        [JsonIgnore] private bool isOnline = false;
        [JsonIgnore] private int answertErrorCount = 0;
        [JsonIgnore] private float currentPan = 0;
        [JsonIgnore] private float currentTilt = 0;
        [JsonIgnore] private float currentZoom = 0;
        [JsonIgnore] private float currentFocus = 0;

        [JsonIgnore] private int currentStepPan = 0;
        [JsonIgnore] private int currentStepTilt = 0;
        //private int currentStepZoom = 0;
        //private int currentStepFocus = 0;

        [JsonIgnore] private int maxStepPan = 0;
        [JsonIgnore] private int maxStepTilt = 0;
        [JsonIgnore] private int maxStepZoom = 0;
        [JsonIgnore] private int maxStepFocus = 0;

        [JsonIgnore] private int temperature = 0;

        [JsonIgnore] private readonly float minPan = 0;
        [JsonIgnore] private readonly float maxPan = 360;
        [JsonIgnore] private readonly float minTilt = -90;
        [JsonIgnore] private readonly float maxTilt = 90; //45

        public Device(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.udp = new UDP(mainWindow, this);
            if (route == null) this.route = new Route(this);
            if (preset == null) this.preset = new Preset();
        }

        [JsonIgnore] public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        [JsonIgnore] public UDP Udp { get => udp; set => udp = value; }
        public Route Route { get => route; set => route = value; }
        public Preset Preset { get => preset; set => preset = value; }

        public string Ip { get => ip; set => ip = value; }
        public string Mask { get => mask; set => mask = value; }
        public int Port { get => port; set => port = value; }
        public int Address { get => address; set => address = value; }

        [JsonIgnore] public int CurrentStepPan { get => currentStepPan; set => currentStepPan = value; }
        [JsonIgnore] public int CurrentStepTilt { get => currentStepTilt; set => currentStepTilt = value; }

        [JsonIgnore] public float MinPan => minPan;

        [JsonIgnore] public float MaxPan => maxPan;

        [JsonIgnore] public float MinTilt => minTilt;

        [JsonIgnore] public float MaxTilt => maxTilt;

        public int GetAnswertErrorCount()
        {
            return answertErrorCount;
        }
        public void AddAnswertErrorCount()
        {
            answertErrorCount++;
            if (answertErrorCount >= AppOptions.ERROR_TO_OFFLINE)
            {
                udp.UdpServices.CoordinatesTimer.Stop();
            }
        }
        public void ResetAnswertErrorCount()
        {
            answertErrorCount = 0;
        }

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
            currentStepPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            currentPan = (float)panStepToAngle(currentStepPan);

            //Math.Round(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * (MaxPan - MinPan) / maxStepPan, 1);

            //System.Diagnostics.Trace.WriteLine("set pan= " + CurrentStepPan);            
        }

        public float GetCurrentTilt()
        {
            return currentTilt;
        }
        public void SetCurrentTilt(byte hh, byte ll)
        {
            currentStepTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            currentTilt = (float)tiltStepToAngle(currentStepTilt);

            //Math.Round(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * (MaxTilt - MinTilt) / maxStepTilt, 1);

            //System.Diagnostics.Trace.WriteLine("set tilt= " + CurrentStepTilt);            
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

            if (value) udp.UdpServices.SetIsTimerOnline(true);
        }


        public String getStatusString()
        {
            return "IP: " + ip + ":" + port + "   " + "Address: " + address + "   " + (isOnline ? "Online" : "Offline") + "   Pan: " + currentPan + "   Tilt: " + currentTilt;
        }

        public void parseRequest(byte[] request)
        {
            if (request[1] == address)
            {
                SetOnline(true);
                ResetAnswertErrorCount();
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

                    case "Register": break;

                }
            }

        }

        public override string ToString()
        {
            String res = "Pan= " + currentPan + "\n";
            res += "Tilt= " + currentTilt + "\n";
            res += "stepPan= " + CurrentStepPan + "\n";
            res += "stepTilt= " + CurrentStepTilt + "\n";
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
                return (int)Math.Round((float.Parse(angle) - minPan) * maxStepPan / (maxPan - minPan));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public float panStepToAngle(int currentStepPan)
        {
            return currentStepPan / maxStepPan * (maxPan - minPan) + minPan;
        }

        public int tiltAngleToStep(string angle)
        {
            try
            {
                return (int)Math.Round((float.Parse(angle) - MinTilt) * maxStepTilt / (MaxTilt - MinTilt));
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public float tiltStepToAngle(int currentStepTilt)
        {
            return currentStepTilt / maxStepTilt * (maxTilt - minTilt) + minTilt;
        }

    }
}
