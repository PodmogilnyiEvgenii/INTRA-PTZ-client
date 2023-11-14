using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;


namespace INTRA_PTZ_client
{
    public class UDP
    {
        private MainWindow mainWindow;
        private Device device;
        private UdpClient udpClient = new UdpClient();
        private UDPservices udpServices;
        private string lastAnswerString="";

        public Device Device { get => device; set => device = value; }
        public UDPservices UdpServices { get => udpServices; set => udpServices = value; }

        public UDP(MainWindow mainWindow, Device device)
        {
            this.mainWindow = mainWindow;
            this.device = device;
            this.udpServices = new UDPservices(device);

            if (device == null) System.Diagnostics.Trace.WriteLine("device null");
        }

        public void Connect()
        {
            try
            {
                udpClient.Connect(Device.Ip, Device.Port);
                udpClient.BeginReceive(new AsyncCallback(Received), null);
                Device.SetOnline(true);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                Device.SetOnline(false);
            }
        }

        public void Disconnect()
        {
            try
            {
                udpClient.Close();
                Device.SetOnline(false);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
            }
        }

        public void getFirstData()
        {
            if (Device.GetMaxStepPan() == 0 || Device.GetMaxStepTilt() == 0 || Device.GetMaxStepZoom() == 0 || Device.GetMaxStepFocus() == 0)
            {
                SendCommand(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00), "MaxStepFocus", 1000));
            }
        }

        public void SendCommand(UdpCommand udpCommand)
        {
            try
            {
                if (Device.GetOnline())
                {
                    lastAnswerString = "";
                    udpClient.Send(udpCommand.Request, udpCommand.Request.Length);

                    if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("\n" + "=> | " + BitConverter.ToString(udpCommand.Request));

                    DateTime start = DateTime.Now;
                    TimeSpan timeout;

                    do
                    {
                        Task.WaitAll(new Task[] { Task.Delay(50) });
                        timeout = DateTime.Now - start;
                        //System.Diagnostics.Trace.WriteLine(timeout.TotalMilliseconds);
                        //System.Diagnostics.Trace.WriteLine(udpCommand.Timeout);

                    } while (lastAnswerString != udpCommand.AnswerString && timeout.TotalMilliseconds < udpCommand.Timeout);

                    if (timeout.TotalMilliseconds > udpCommand.Timeout) Device.SetOnline(false);

                    if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("Answer in " + timeout.TotalMilliseconds);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("Send command but not online");
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                Device.SetOnline(false);
            }
        }

        private void Received(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, Device.Port);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            lastAnswerString = PelcoDE.getDescriptionRequest(received[3]);
            if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("<= | " + PelcoDE.getDescriptionRequest(received[3]) + " | " + BitConverter.ToString(received));

            Device.parseRequest(received);

            udpClient.BeginReceive(new AsyncCallback(Received), null);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = BitConverter.ToString(received)));


        }

        public void SendCommandOld(byte[] command)
        {
            try
            {
                if (!Device.GetOnline())
                {
                    Connect();
                    /*if (device.GetMaxStepPan() == 0 || device.GetMaxStepTilt() == 0 || device.GetMaxStepZoom() == 0 || device.GetMaxStepFocus() == 0)
                    {
                        SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00));
                        Task.WaitAll(new Task[] { Task.Delay(500) });
                    }*/
                }

                if (Device.GetOnline())
                {

                    udpClient.Send(command, command.Length);
                    Device.SetOnline(false);

                    if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("=> | " + BitConverter.ToString(command));

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                Device.SetOnline(false);
            }
        }
    }

}
