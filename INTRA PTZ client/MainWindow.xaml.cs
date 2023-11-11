using System;
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

            device = new Device(this);

            //speed.Text = device.MovingSpeed.ToString();
            deviceDataText.Text = device.getStatusString();
            isOnline.IsChecked = device.Udp.GetIsTimerOnline();

            optionsWindow = new OptionsWindow(this);
            routeWindow = new RouteWindow();
            serviceWindow = new ServiceWindow(this);
            findWindow = new FindWindow();

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = TimeSpan.FromMilliseconds(1000);
            refreshTimer.Start();

        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            deviceDataText.Text = device.getStatusString()));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            ServiceWindow.deviceDataText.Text = device.getStatusString()));
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
        private void isOnline_Click(object sender, RoutedEventArgs e)
        {
            device.Udp.SetIsTimerOnline(!device.Udp.GetIsTimerOnline());
            isOnline.IsChecked = device.Udp.GetIsTimerOnline();
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

        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SpeedPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value < 8) speedSleder.Value = speedSleder.Value + 1;
        }

        private void SpeedMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value > 1) speedSleder.Value = speedSleder.Value - 1;
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
