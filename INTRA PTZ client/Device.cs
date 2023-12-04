using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace INTRA_PTZ_client
{
    public class Device
    {
        private MainWindow mainWindow;
        private UDP udp;
        private Route route = new Route();

        private string ip = "10.130.250.197";
        private string mask = "255.255.255.0";
        private int port = 6000;
        private int address = 1;
        private int speed = -1;
        private int acceleration = -1;

        private bool isOnline = false;
        private int answertErrorCount = 0;
        private double currentPan = 0;
        private double currentTilt = 0;
        private double currentZoom = 0;
        private double currentFocus = 0;

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
        private readonly float maxPan = 360;
        private readonly float minTilt = -90;
        private readonly float maxTilt = 90; //45



        public Device(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.udp = new UDP(mainWindow, this);
        }

        public MainWindow MainWindow { get => mainWindow; set => mainWindow = value; }
        public UDP Udp { get => udp; set => udp = value; }
        public Route Route { get => route; set => route = value; }

        public string Ip { get => ip; set => ip = value; }
        public string Mask { get => mask; set => mask = value; }
        public int Port { get => port; set => port = value; }
        public int Address { get => address; set => address = value; }

        public int CurrentStepPan { get => currentStepPan; set => currentStepPan = value; }
        public int CurrentStepTilt { get => currentStepTilt; set => currentStepTilt = value; }

        public float MinPan => minPan;

        public float MaxPan => maxPan;

        public float MinTilt => minTilt;

        public float MaxTilt => maxTilt;

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


        public double GetCurrentZoom()
        {
            return currentZoom;
        }

        public void SetCurrentZoom(byte hh, byte ll)
        {
            //TODO
            currentZoom = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
        }

        public double GetCurrentFocus()
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

        public double GetCurrentPan()
        {
            return currentPan;
        }
        public void SetCurrentPan(byte hh, byte ll)
        {
            currentPan = Math.Round(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * (MaxPan - MinPan) / maxStepPan, 1);
            CurrentStepPan = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
            //System.Diagnostics.Trace.WriteLine("set pan= " + CurrentStepPan);            
        }

        public double GetCurrentTilt()
        {
            return currentTilt;
        }
        public void SetCurrentTilt(byte hh, byte ll)
        {
            currentTilt = Math.Round(BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 }) * (MaxTilt - MinTilt) / maxStepTilt, 1);
            CurrentStepTilt = BitConverter.ToInt32(new byte[] { ll, hh, 0x00, 0x00 });
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
            return "IP: " + ip + ":" + port + "   " + "Address: " + address + "   " + (isOnline ? "Online" : "Offline") + "   Pan:" + currentPan + "   Tilt:" + currentTilt;
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
                //System.Diagnostics.Trace.WriteLine(angle);
                //System.Diagnostics.Trace.WriteLine(float.Parse(angle) * maxStepPan / (maxPan - minPan));

                return (int)Math.Round(float.Parse(angle) * maxStepPan / (MaxPan - MinPan));
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
                return (int)Math.Round(float.Parse(angle) * maxStepTilt / (MaxTilt - MinTilt));
            }
            catch (Exception)
            {
                return -1;
            }
        }


    }
}
