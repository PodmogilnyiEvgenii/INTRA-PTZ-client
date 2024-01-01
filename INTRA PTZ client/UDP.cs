using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Threading;


namespace INTRA_PTZ_client
{
    public class UDP
    {
        private MainWindow mainWindow;
        private Device device;
        private UdpClient udpClient = new UdpClient();
        private UDPservices udpServices;
        private string lastAnswerString = "";

        public Device Device { get => device; set => device = value; }
        public UDPservices UdpServices { get => udpServices; set => udpServices = value; }

        public UDP(MainWindow mainWindow, Device device)
        {
            this.mainWindow = mainWindow;
            this.device = device;
            this.udpServices = new UDPservices(device);
        }

        public bool Connect()
        {
            try
            {
                udpClient.Connect(Device.Ip, Device.Port);
                udpClient.BeginReceive(new AsyncCallback(Received), null);
                Device.SetOnline(true);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
                Device.SetOnline(false);
                return false;
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

        public void GetFirstData()
        {
            if (Device.GetMaxStepPan() == 0 || Device.GetMaxStepTilt() == 0 || Device.GetMaxStepZoom() == 0 || Device.GetMaxStepFocus() == 0)
            {

                List<UdpCommand> list = new List<UdpCommand>();
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00), "MaxStepFocus", AppOptions.UDP_TIMEOUT_SHORT));
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
                
                udpServices.AddTaskToBegin(list);
                //TODO
                /*
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0D), "Register", AppOptions.UDP_TIMEOUT_SHORT)); //слева
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0E), "Register", AppOptions.UDP_TIMEOUT_SHORT)); //справа
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0F), "Register", AppOptions.UDP_TIMEOUT_SHORT)); //снизу
                list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x10), "Register", AppOptions.UDP_TIMEOUT_SHORT)); //сверху
                */
                
            }
        }
        public void SendCommand(UdpCommand udpCommand)
        {
            try
            {
                if (Device.GetOnline())
                {
                    lastAnswerString = "";
                    int count = 0;
                    TimeSpan timeout;

                    do
                    {
                        udpClient.Send(udpCommand.Request, udpCommand.Request.Length);
                        DateTime start = DateTime.Now;

                        if (AppOptions.DEBUG) System.Diagnostics.Trace.WriteLine("\n" + "=> | " + BitConverter.ToString(udpCommand.Request));

                        do
                        {
                            Task.WaitAll(new Task[] { Task.Delay(50) });
                            timeout = DateTime.Now - start;

                        } while (!lastAnswerString.Equals(udpCommand.AnswerString) && timeout.TotalMilliseconds < udpCommand.Timeout);

                        count++;

                        if (AppOptions.DEBUG)
                        {
                            if (lastAnswerString.Equals(udpCommand.AnswerString))
                            {
                                System.Diagnostics.Trace.WriteLine("Answer in " + timeout.TotalMilliseconds + " mc | count " + count);
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine("No answer in " + timeout.TotalMilliseconds + " mc | count " + count);
                            }
                        }

                    } while (lastAnswerString != udpCommand.AnswerString && count < AppOptions.UDP_TRY_COMMAND_SEND);

                    if (timeout.TotalMilliseconds > udpCommand.Timeout)
                    {
                        Device.SetOnline(false);
                        device.AddAnswertErrorCount();
                    }

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
            /*System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = BitConverter.ToString(received)));*/
        }
        
    }

}
