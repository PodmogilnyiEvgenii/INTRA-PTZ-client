using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Timer = System.Timers.Timer;

namespace INTRA_PTZ_client
{
    public class UDP
    {
        private MainWindow mainWindow;

        private Device device;
        private UdpClient udpClient = new UdpClient();
        private readonly bool DEBUG = true;

        private Timer coordinatesTimer;
        private bool isTimerOnline = false;


        public Timer CoordinatesTimer { get => coordinatesTimer; set => coordinatesTimer = value; }

        public bool GetIsTimerOnline()
        {
            return isTimerOnline;
        }

        public void SetIsTimerOnline(bool value)
        {
            isTimerOnline = value;

            if (value) CoordinatesTimer.Start(); else CoordinatesTimer.Stop();
        }

        public UDP(Device device)
        {
            this.mainWindow = device.MainWindow;
            this.device = device;

            CoordinatesTimer = new Timer(1000);
            CoordinatesTimer.AutoReset = true;
            CoordinatesTimer.Elapsed += new ElapsedEventHandler(OnCoorditatesTimerEvent);
        }

        private void OnCoorditatesTimerEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                
                //SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllCoordinates"), 0x00, 0x00));

                SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00));
                Thread.Sleep(500);
                SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00));
                

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        public void Connect()
        {
            try
            {
                udpClient.Connect(device.Ip, device.Port);
                udpClient.BeginReceive(new AsyncCallback(Received), null);
                device.SetOnline(true);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                device.SetOnline(false);
            }
        }

        public void Disconnect()
        {
            try
            {
                udpClient.Close();
                device.SetOnline(false);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
            }
        }

        public void SendCommand(byte[] command)
        {
            try
            {
                if (!device.GetOnline())
                {
                    Connect();
                    if (device.GetMaxStepPan()==0 || device.GetMaxStepTilt()== 0 || device.GetMaxStepZoom() == 0 || device.GetMaxStepFocus() == 0)
                    {
                        SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00));
                        Task.WaitAll(new Task[] { Task.Delay(500) });
                    }
                }

                if (device.GetOnline())
                {

                    udpClient.Send(command, command.Length);
                    device.SetOnline(false);

                    if (DEBUG) System.Diagnostics.Trace.WriteLine("=> | " + BitConverter.ToString(command));

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                device.SetOnline(false);
            }
        }

        private void Received(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, device.Port);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            if (DEBUG) System.Diagnostics.Trace.WriteLine("<= | " + PelcoDE.getDescriptionRequest(received[3]) + " | " + BitConverter.ToString(received) + "\n");

            device.parseRequest(received);

            udpClient.BeginReceive(new AsyncCallback(Received), null);

            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = BitConverter.ToString(received)));


        }
    }
}
