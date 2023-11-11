using System;
using System.Diagnostics;
using System.Windows;
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

            speed.Text = device.MovingSpeed.ToString();
            deviceDataText.Text = device.getStatusString();
            isOnline.IsChecked = device.Udp.GetIsTimerOnline();

            optionsWindow = new OptionsWindow(this);
            routeWindow = new RouteWindow();
            serviceWindow = new ServiceWindow(this);
            findWindow = new FindWindow();

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval=TimeSpan.FromMilliseconds(1000);
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
            //123
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




        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }

        private void isOnline_Click(object sender, RoutedEventArgs e)
        {
            device.Udp.SetIsTimerOnline(!device.Udp.GetIsTimerOnline());
            isOnline.IsChecked = device.Udp.GetIsTimerOnline();
        }
    }
}
