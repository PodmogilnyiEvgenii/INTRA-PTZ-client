using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace INTRA_PTZ_client
{
    public class UDPservices
    {
        private Device device;
        private Timer coordinatesTimer = new Timer(AppOptions.REQUEST_COORDINATE_TIMEOUT);
        private Queue<List<UdpCommand>> messageQueue = new Queue<List<UdpCommand>>();
        private bool isTimerOnline = false;

        public Timer CoordinatesTimer { get => coordinatesTimer; set => coordinatesTimer = value; }

        public UDPservices(Device device)
        {
            this.device = device;

            CoordinatesTimer.AutoReset = true;
            CoordinatesTimer.Elapsed += new ElapsedEventHandler(OnCoorditatesTimerEvent);

            Thread queueService = new Thread(new ThreadStart(OnMessageQueue));
            queueService.Start();
        }

        public bool GetIsTimerOnline()
        {
            return isTimerOnline;
        }
        public void SetIsTimerOnline(bool value)
        {
            isTimerOnline = value;

            if (value) CoordinatesTimer.Start(); else CoordinatesTimer.Stop();
        }

        private void OnCoorditatesTimerEvent(Object source, ElapsedEventArgs e)
        {
            if (messageQueue.Count < 3)
            {
                List<UdpCommand> list = new List<UdpCommand>();
                list.Add(new UdpCommand(PelcoDE.getCommand(device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
                list.Add(new UdpCommand(PelcoDE.getCommand(device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
                messageQueue.Enqueue(list);
            }
        }

        private void OnMessageQueue()
        {
            while (true)
            {
                if (messageQueue.Count > 0)
                {
                    if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Start task (last " + (messageQueue.Count - 1) + ")");

                    if (!device.GetOnline())
                    {
                        if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Connecting...");
                        device.Udp.Connect();
                        device.Udp.getFirstData();
                    }

                    List<UdpCommand> commandsList = messageQueue.Dequeue();
                    for (int i = 0; i < commandsList.Count; i++)
                    {
                        device.Udp.SendCommand(commandsList[i]);
                    }
                }
            }
        }

        public void addTask(List<UdpCommand> task)
        {
            messageQueue.Enqueue(task);
        }
    }
}
