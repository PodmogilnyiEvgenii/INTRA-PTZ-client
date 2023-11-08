﻿using System;
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
        private readonly bool DEBUG = true;


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

        public void SendCommand(byte[] command)
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
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, AppOptions.DevicePort);
            byte[] received = udpClient.EndReceive(res, ref RemoteIpEndPoint);

            //System.Diagnostics.Trace.WriteLine(received.Length);
            if (DEBUG) System.Diagnostics.Trace.WriteLine("<= | " + PelcoDE.getDescriptionRequest(received[3]) + " | " + BitConverter.ToString(received)+ "\n");

            device.parseRequest(received);

            udpClient.BeginReceive(new AsyncCallback(Received), null);
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => mainWindow.ServiceWindow.answerTextBox.Text = BitConverter.ToString(received)));


        }
    }
}
