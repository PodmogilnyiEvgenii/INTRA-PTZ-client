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
    public partial class ServiceWindow : Window
    {
        public ServiceWindow()
        {
            InitializeComponent();
        }
        private void ServiceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void RequestSendButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ServiceCloseButton_Click(object sender, RoutedEventArgs e)
        {
            serviceWindow.Visibility = Visibility.Hidden;
        }
    }
}
