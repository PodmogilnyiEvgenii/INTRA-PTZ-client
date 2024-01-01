using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            this.Loaded += ServiceWindow_Loaded;
            this.IsVisibleChanged += ServiceWindow_IsVisibleChanged;
            this.Closing += ServiceWindow_Closing;            
        }

        private void ServiceWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void ServiceWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            deviceDataText.Text = mainWindow.Device.getStatusString();
            minPanTextBox.Text = mainWindow.Device.MinPan.ToString();
            maxPanTextBox.Text = mainWindow.Device.MaxPan.ToString();
            maxPanStepTextBox.Text = mainWindow.Device.GetMaxStepPan().ToString();

            minTiltTextBox.Text = mainWindow.Device.MinTilt.ToString();
            maxTiltTextBox.Text = mainWindow.Device.MaxTilt.ToString();
            maxTiltStepTextBox.Text = mainWindow.Device.GetMaxStepTilt().ToString();
        }
        private void ServiceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void RequestSendButton1_Click(object sender, RoutedEventArgs e)
        {
            //mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x00, 0x01));   //0

            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x06), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x09), "Register", AppOptions.UDP_TIMEOUT_SHORT));

            /*list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0D), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0E), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0F), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x10), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x12), "Register", AppOptions.UDP_TIMEOUT_SHORT));*/

            mainWindow.Device.Udp.UdpServices.AddTaskToEnd(list);

        }

        private void RequestSendButton2_Click(object sender, RoutedEventArgs e)
        {
            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x06, 0x01), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x09, 0x01), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0xFE, 0x00), "", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x09, 0x01), "", AppOptions.UDP_TIMEOUT_SHORT));

            mainWindow.Device.Udp.UdpServices.AddTaskToEnd(list);

            //mainWindow.Device.Udp.SendCommandOld(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x38, 0xF4));   //90
        }

        private void RequestSendButton3_Click(object sender, RoutedEventArgs e)
        {
            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x06, 0x05), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x09, 0x3F), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0xFE, 0x00), "", AppOptions.UDP_TIMEOUT_SHORT));

            mainWindow.Device.Udp.UdpServices.AddTaskToEnd(list);



            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x71, 0xE8));   //180
            //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTemperature"), 0x00, 0x00));
            /*
            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getTemperature"), 0x00, 0x00), "Temperature", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getVoltage"), 0x00, 0x00), "Voltage", AppOptions.UDP_TIMEOUT_SHORT));
            mainWindow.Device.Udp.UdpServices.addTask(list);           
            */


            //System.Diagnostics.Trace.WriteLine(mainWindow.Device.ToString());
        }

        private void ServiceCloseButton_Click(object sender, RoutedEventArgs e)
        {
            serviceWindow.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + mainWindow.Device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }

        private void getBasicOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getAllMaxStepCoordinates"), 0x00, 0x00), "MaxStepFocus", AppOptions.UDP_TIMEOUT_SHORT));

            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0D), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0E), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x0F), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x10), "Register", AppOptions.UDP_TIMEOUT_SHORT));

            mainWindow.Device.Udp.UdpServices.AddTaskToEnd(list);

            Task.WaitAll(new Task[] { Task.Delay(1500) });

            minPanTextBox.Text = mainWindow.Device.MinPan.ToString();
            maxPanTextBox.Text = mainWindow.Device.MaxPan.ToString();
            maxPanStepTextBox.Text = mainWindow.Device.GetMaxStepPan().ToString();

            minTiltTextBox.Text = mainWindow.Device.MinTilt.ToString();
            maxTiltTextBox.Text = mainWindow.Device.MaxTilt.ToString();
            maxTiltStepTextBox.Text = mainWindow.Device.GetMaxStepTilt().ToString();
        }

        private void setParametrs_Click(object sender, RoutedEventArgs e)
        {
            int speedValue = -1;
            int accelerationValue = -1;

            try
            {
                speedValue = int.Parse(speedTextBox.Text);
            }
            catch { }

            if (speedValue > 63 || speedValue < 1) speedTextBox.Text = "1";

            try
            {
                accelerationValue = int.Parse(accelerationTextBox.Text);
            }
            catch { }

            if (accelerationValue > 5 || accelerationValue < 0) accelerationTextBox.Text = "1";

            //byte[] speed = BitConverter.GetBytes(int.Parse(speedTextBox.Text));
            //byte[] acceleration = BitConverter.GetBytes(int.Parse(accelerationTextBox.Text));


            List<UdpCommand> list = new List<UdpCommand>();
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x09, /*speed[0]*/(byte) speedValue), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0x06, /*acceleration[0]*/(byte) accelerationValue), "", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("setRegister"), 0xFE, 0x00), "", AppOptions.UDP_TIMEOUT_SHORT));

            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x09), "Register", AppOptions.UDP_TIMEOUT_SHORT));
            list.Add(new UdpCommand(PelcoDE.getCommand(mainWindow.Device.Address, 0x00, PelcoDE.getByteCommand("getRegister"), 0x00, 0x06), "Register", AppOptions.UDP_TIMEOUT_SHORT));


            mainWindow.Device.Udp.UdpServices.AddTaskToEnd(list);
        }

        private void ValidationSpeedField(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ValidationAccelerationField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
