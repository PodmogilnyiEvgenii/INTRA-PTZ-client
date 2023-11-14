using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace INTRA_PTZ_client
{
    public partial class ServiceWindow : Window
    {
        private MainWindow mainWindow;

        public ServiceWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void ServiceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            deviceDataText.Text = mainWindow.Device.getStatusString(); 
        }

        private void ServiceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void RequestSendButton_Click(object sender, RoutedEventArgs e)
        {
            /*mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00));
            Task.WaitAll(new Task[] { Task.Delay(500) });
            mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getMaxPan"), 0x00, 0x00));
            Task.WaitAll(new Task[] { Task.Delay(500) });
            mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00));
            Task.WaitAll(new Task[] { Task.Delay(500) });
            mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getMaxTilt"), 0x00, 0x00));
            */
            mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("set"), 0x00, 0x00));            
        }

        private void RequestSendButton1_Click(object sender, RoutedEventArgs e)
        {           
            mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x00, 0x01));   //0         
        }

        private void RequestSendButton2_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x38, 0xF4));   //90
        }

        private void RequestSendButton3_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x71, 0xE8));   //180
            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTemperature"), 0x00, 0x00));

            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTemperature"), 0x00, 0x00), "Temperature", AppOptions.UDP_TIMEOUT_SHORT));
            
            mainWindow.Device.Udp.UdpServices.addTask(list);           



            //System.Diagnostics.Trace.WriteLine(mainWindow.Device.ToString());
        }

        private void GetAllCoordinats_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getAllCoordinates"), 0x00, 0x00));
            mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllCoordinates"), 0x00, 0x00));
        }

        private void GetAllMaxCoordinats_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Addres, 0x00, PelcoDE.getByteCommand("getAllMaxCoordinates"), 0x00, 0x00));
            mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00));
        }


        private void ServiceCloseButton_Click(object sender, RoutedEventArgs e)
        {
            serviceWindow.Visibility = Visibility.Hidden;
        }

        internal void ShowAnswer(string answer)
        {
            answerTextBox.Text = answer;
        }  
        

        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://"+ mainWindow.Device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
