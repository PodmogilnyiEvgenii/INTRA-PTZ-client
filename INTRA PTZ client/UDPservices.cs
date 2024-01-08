using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows.Documents;
using Timer = System.Timers.Timer;

namespace INTRA_PTZ_client
{
    public class UDPservices
    {
        private Device device;
        private Timer coordinatesTimer = new Timer(AppOptions.REQUEST_COORDINATE_TIMEOUT);
        private Queue<UdpCommand> messageQueue = new Queue<UdpCommand>();
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

        public Queue<UdpCommand> GetMessageQueue()
        {
            return messageQueue;
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
            if (GetMessageQueue().Count <= AppOptions.MAX_COMMAND_IN_QUEUE)
            {
                List<UdpCommand> list = new List<UdpCommand>();
                list.Add(new UdpCommand(PelcoDE.getCommand(device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
                list.Add(new UdpCommand(PelcoDE.getCommand(device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));

                AddTaskToEnd(list);
            }
        }

        private void OnMessageQueue()
        {
            while (true)
            {
                if (GetMessageQueue().Count > 0)
                {  
                    if (!device.GetOnline())
                    {
                        if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Connecting...");                       //TODO

                        if (device.Udp.Connect()) { device.Udp.GetFirstData(); }
                    }

                    if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("\n" + "Start task (last " + (GetMessageQueue().Count - 1) + ")");

                    device.Udp.SendCommand(GetMessageQueue().Dequeue());

                    if (device.GetAnswertErrorCount() >= AppOptions.ERROR_TO_OFFLINE)
                    {
                        device.ResetAnswertErrorCount();
                        device.Udp.UdpServices.CleanQueue();
                        if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("\nDevice offline");
                    }
                }
            }
        }

        public void AddTaskToEnd(List<UdpCommand> task)
        {
            if (GetMessageQueue().Count <= AppOptions.MAX_COMMAND_IN_QUEUE)
            {
                for (int i = 0; i < task.Count; i++)
                {
                    GetMessageQueue().Enqueue(task[i]);
                }
            }
            else
            {
                if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Too many tasks");
            }
        }

        public void AddTaskToBegin(List<UdpCommand> task)
        {
            if (GetMessageQueue().Count <= AppOptions.MAX_COMMAND_IN_QUEUE)
            {
                List<UdpCommand> listOfTasks = new List<UdpCommand>(task);

                while (GetMessageQueue().Count > 0)
                {
                    listOfTasks.Add(GetMessageQueue().Dequeue());
                }

                GetMessageQueue().Clear();

                for (int i = 0; i < listOfTasks.Count; i++)
                {
                    GetMessageQueue().Enqueue(listOfTasks[i]);
                }
            }
            else
            {
                if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Too many tasks");
            }



        }

        public void CleanQueue()
        {
            GetMessageQueue().Clear();
        }
    }
}
