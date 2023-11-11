using System;
using System.Windows;

namespace INTRA_PTZ_client
{
    public partial class OptionsWindow : Window
    {
        private MainWindow mainWindow;

        public OptionsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }
        private void OptionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            String[] ip = mainWindow.Device.Ip.Split('.');
            ip1.Text = ip[0];
            ip2.Text = ip[1];
            ip3.Text = ip[2];
            ip4.Text = ip[3];

            ip = mainWindow.Device.Mask.Split('.');
            mask1.Text = ip[0];
            mask2.Text = ip[1];
            mask3.Text = ip[2];
            mask4.Text = ip[3];

            port.Text = mainWindow.Device.Port.ToString();
            address.Text = mainWindow.Device.Address.ToString();
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
