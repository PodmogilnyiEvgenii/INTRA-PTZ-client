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
        private OptionsWindow optionsWindow=new OptionsWindow();
        private RouteWindow routeWindow = new RouteWindow();
        private ServiceWindow serviceWindow = new ServiceWindow();
        private FindWindow findWindow = new FindWindow();

        public MainWindow()
        {            
            InitializeComponent();
            address.Text = AppOptions.DeviceAdress.ToString();
            speed.Text = AppOptions.DeviceSpeed.ToString();                
        }

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
            optionsWindow.Visibility = Visibility.Visible;
            optionsWindow.Owner = this;
            optionsWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;            
            optionsWindow.ShowDialog();
        }

        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            routeWindow.Visibility = Visibility.Visible;
            routeWindow.Owner = this;
            routeWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;            
            routeWindow.ShowDialog();
        }

        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            serviceWindow.Visibility = Visibility.Visible;
            serviceWindow.Owner = this;
            serviceWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;            
            serviceWindow.ShowDialog();
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            findWindow.Visibility = Visibility.Visible;
            findWindow.Owner = this;
            findWindow.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;            
            findWindow.ShowDialog();
        }

        //manual mode



    }
}
