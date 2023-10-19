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
        private OptionsWindow optionsWindow = new OptionsWindow();
        private RouteWindow routeWindow = new RouteWindow();

        public MainWindow()
        {
            InitializeComponent(); 
            //optionsWindow.Visibility = Visibility.Hidden;           
            


        }

        private void configButton_Click(object sender, RoutedEventArgs e)
        {
            
            optionsWindow.Visibility = Visibility.Visible;
            optionsWindow.Owner = this;
            optionsWindow.ShowDialog();
            
        }

        private void routeButton_Click(object sender, RoutedEventArgs e)
        {
            routeWindow.Visibility = Visibility.Visible;
            routeWindow.Owner = this;
            routeWindow.ShowDialog();
        }


        private void ConfigurationLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConfigurationSave_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        
    }
}
