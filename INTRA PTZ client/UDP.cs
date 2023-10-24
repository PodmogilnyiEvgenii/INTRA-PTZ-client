using System;
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
        private UdpClient udpClient;
        private UdpClient udpServer;

        public UDP(Device device)
        {
            this.mainWindow = device.MainWindow;
            this.device = device;
        }

        public void Connect()
        {
            if (!device.Online)
            {
                try
                {
                    udpClient.Connect(AppOptions.DeviceIp, AppOptions.DevicePort);
                    udpClient.BeginReceive(new AsyncCallback(Received), null);
                    device.Online = true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.ToString());                    
                    device.Online = false;
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                udpClient.Close();
                device.Online = false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());
            }
        }

        public void SendCommand(String command)
        {
            /*try
            {
                if (device.Online)
                {
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(command);
                    //udpClient.Send(sendBytes, sendBytes.Length);

                    udpClient.SendAsync (sendBytes, new IPEndPoint(IPAddress.Parse("192.168.32.235"), 6000));
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.ToString());                
            }*/

            using var sender = new UdpClient();
        }

        private void Received(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, AppOptions.DevicePort);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            //MessageBox.Show(Encoding.UTF8.GetString(received));
            udpClient.BeginReceive(new AsyncCallback(Received), null);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                            new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = Encoding.UTF8.GetString(received)));
        }
    }
}
