using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            address.Text = AppOptions.DeviceAdress.ToString();
            speed.Text = AppOptions.DeviceSpeed.ToString();

            optionsWindow = new OptionsWindow();
            routeWindow = new RouteWindow();
            serviceWindow = new ServiceWindow(this);
            findWindow = new FindWindow();

            device = new Device(this);

            System.Diagnostics.Trace.WriteLine("Start");              
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
        }

        //manual mode



    }
}
