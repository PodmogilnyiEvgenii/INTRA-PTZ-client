using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace INTRA_PTZ_client
{
    public class UDP
    {
        private MainWindow mainWindow;

        private Device device;
        private UdpClient udpClient = new UdpClient();


        public UDP(Device device)
        {
            this.mainWindow = device.MainWindow;
            this.device = device;
        }

        public void Connect()
        {
            try
            {
                udpClient.Connect(AppOptions.DeviceIp, AppOptions.DevicePort);
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

        public void SendCommand(Byte[] command)
        {
            try
            {
                if (!device.GetOnline())
                {
                    Connect();
                }

                if (device.GetOnline())
                {
                    udpClient.Send(command, command.Length);
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
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, AppOptions.DevicePort);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            //MessageBox.Show(Encoding.UTF8.GetString(received));
            System.Diagnostics.Trace.WriteLine(received.Length);
            System.Diagnostics.Trace.WriteLine(BitConverter.ToString(received));
            udpClient.BeginReceive(new AsyncCallback(Received), null);

            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = BitConverter.ToString(received)));

            
        }
    }
}
