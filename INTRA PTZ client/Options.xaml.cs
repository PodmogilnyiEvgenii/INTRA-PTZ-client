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
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }
        private void OptionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            String[] ip = AppOptions.DeviceIp.Split('.');
            ip1.Text = ip[0];
            ip2.Text = ip[1];
            ip3.Text = ip[2];
            ip4.Text = ip[3];

            ip = AppOptions.DeviceMask.Split('.');
            mask1.Text = ip[0];
            mask2.Text = ip[1];
            mask3.Text = ip[2]; 
            mask4.Text = ip[3];

            port.Text=AppOptions.DevicePort.ToString();  
        }

        private void OptionsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            optionsWindow.Visibility = Visibility.Hidden;

        }

        private void OptionsCancelButton_Click(object sender, RoutedEventArgs e)
        {
            optionsWindow.Visibility = Visibility.Hidden;
        }


    }
}
