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
using System.Windows.Shapes;

namespace INTRA_PTZ_client
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class RouteWindow : Window
    {
        public RouteWindow()
        {
            InitializeComponent();
        }

        private void routeSave_Click(object sender, RoutedEventArgs e)
        {
            routeWindow.Visibility = Visibility.Hidden;
        }
        private void routeCancel_Click(object sender, RoutedEventArgs e)
        {
            routeWindow.Visibility = Visibility.Hidden;
        }
    }
}
