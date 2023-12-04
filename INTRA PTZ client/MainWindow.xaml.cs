using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace INTRA_PTZ_client
{
    public partial class MainWindow : Window
    {
        private OptionsWindow optionsWindow;
        private RouteWindow routeWindow;
        private ServiceWindow serviceWindow;
        private FindWindow findWindow;
        private Device device;

        public MainWindow()
        {
            InitializeComponent();

            this.device = new Device(this);

            zoomField.IsEnabled = false;
            focusField.IsEnabled = false;

            deviceDataText.Text = device.getStatusString();
            
            this.Closing += MainWindow_Closing;

            this.optionsWindow = new OptionsWindow(this);
            this.routeWindow = new RouteWindow(this);
            this.serviceWindow = new ServiceWindow(this);
            this.findWindow = new FindWindow();

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = TimeSpan.FromMilliseconds(1000);
            refreshTimer.Start();

        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            String statusString = device.getStatusString();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            deviceDataText.Text = statusString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            ServiceWindow.deviceDataText.Text = statusString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            routeWindow.deviceDataText.Text = statusString));
        }

        public OptionsWindow OptionsWindow { get => optionsWindow; set => optionsWindow = value; }
        public RouteWindow RouteWindow { get => routeWindow; set => routeWindow = value; }
        public ServiceWindow ServiceWindow { get => serviceWindow; set => serviceWindow = value; }
        public FindWindow FindWindow { get => findWindow; set => findWindow = value; }
        public Device Device { get => device; set => device = value; }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            device.Udp.Disconnect();
        }

        //menu

        private void ConfigurationLoad_Click(object sender, RoutedEventArgs e)
        {
            //menu
        }

        private void ConfigurationSave_Click(object sender, RoutedEventArgs e)
        {
            //menu
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //menu
            System.Windows.Application.Current.Shutdown();
        }        

        //panel 1

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow.Visibility = Visibility.Visible;
            OptionsWindow.Owner = this;
            OptionsWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            OptionsWindow.ShowDialog();
        }

        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            RouteWindow.Visibility = Visibility.Visible;
            RouteWindow.Owner = this;
            RouteWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            RouteWindow.ShowDialog();
        }

        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceWindow.Visibility = Visibility.Visible;
            ServiceWindow.Owner = this;
            ServiceWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            ServiceWindow.ShowDialog();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            FindWindow.Visibility = Visibility.Visible;
            FindWindow.Owner = this;
            FindWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            FindWindow.ShowDialog();

            //MessageBox.Show(device.getStatusString());
            //device.refreshStatus();
            //device.Udp.SendCommand(PelcoDE.getCommand(device.Address, 0x00, PelcoDE.getByteCommand("getAllCoordinates"), 0x00, 0x00));
        }

        //manual mode   

        private void SetCoordinatesButton_Click(object sender, RoutedEventArgs e)
        {
            int panValue = -1;
            int tiltValue = -1;

            try
            {
                panValue = int.Parse(panField.Text);
            }
            catch { }

            if (panValue > device.MaxPan || panValue < device.MinPan) panField.Text = device.GetCurrentPan().ToString();

            try
            {
               tiltValue = int.Parse(tiltField.Text);
            }
            catch { }

            if (tiltValue > device.MaxTilt || tiltValue < 0) tiltField.Text = device.GetCurrentTilt().ToString();

            byte[] pan = BitConverter.GetBytes(Device.panAngleToStep(panField.Text));
            byte[] tilt = BitConverter.GetBytes(Device.tiltAngleToStep(tiltField.Text));

            //byte[] zoom = BitConverter.GetBytes(Device.panAngleToStep(zoomField.Text));
            //byte[] focus = BitConverter.GetBytes(Device.panAngleToStep(focusField.Text));


            if (AppOptions.DEBUG && device.CurrentStepPan == Device.panAngleToStep(panField.Text) && device.CurrentStepTilt == Device.tiltAngleToStep(panField.Text))
            {
                System.Diagnostics.Trace.WriteLine("Same coordinates | pan= " +
                Device.panAngleToStep(panField.Text)
                + " tilt= " +
                Device.tiltAngleToStep(tiltField.Text)
                );
            }

            List<UdpCommand> list = new List<UdpCommand>();
            if (device.CurrentStepPan != Device.panAngleToStep(panField.Text))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (device.CurrentStepTilt != Device.tiltAngleToStep(tiltField.Text))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }

            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setZoom"), tilt[1], tilt[0]), "Zoom", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setFocus"), tilt[1], tilt[0]), "Focus", AppOptions.UDP_TIMEOUT_SHORT));

            Device.Udp.UdpServices.addTaskToEnd(list);
        }

        private void ManualControlUI(double speed, int dPan, int dTilt)
        {
            List<UdpCommand> list = new List<UdpCommand>();
            if (dPan != 0)
            {
                int resultPan = Device.CurrentStepPan + dPan * AppOptions.SPEED_STEP_PAN[(int)speed];
                if (resultPan < 0) resultPan += Device.GetMaxStepPan();
                if (resultPan > Device.GetMaxStepPan()) resultPan -= Device.GetMaxStepPan();

                byte[] pan = BitConverter.GetBytes(resultPan);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (dTilt != 0)
            {
                int resultTilt = Device.CurrentStepTilt + dTilt * AppOptions.SPEED_STEP_TILT[(int)speed];
                if (resultTilt < 0) resultTilt += Device.GetMaxStepTilt();
                if (resultTilt > Device.GetMaxStepTilt()) resultTilt -= Device.GetMaxStepTilt();

                byte[] tilt = BitConverter.GetBytes(resultTilt);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }
            Device.Udp.UdpServices.addTaskToEnd(list);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 1);
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, 1);
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 1);
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 0);
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (device.CurrentStepPan != 0 && device.CurrentStepTilt != 0)
            {
                List<UdpCommand> list = new List<UdpCommand>();
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("makeTest"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
            }
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 0);
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, -1);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, -1);
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, -1);
        }

        private void SpeedPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value < 7) speedSleder.Value = speedSleder.Value + 1;
        }

        private void SpeedMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value > 0) speedSleder.Value = speedSleder.Value - 1;
        }

        private void ValidationPanField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            /*
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);*/

            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void ValidationTiltField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void ValidationZoomField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void ValidationFocusField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;

        }

        //status bar
        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }


    }
}
