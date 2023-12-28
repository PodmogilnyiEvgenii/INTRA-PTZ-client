using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace INTRA_PTZ_client
{
    public partial class OptionsWindow : Window
    {
        private MainWindow mainWindow;

        public OptionsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            this.Loaded += OptionsWindow_Loaded;
            this.IsVisibleChanged += OptionsWindow_IsVisibleChanged;
            this.Closing += OptionsWindow_Closing;
        }

        private void OptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void OptionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private void OptionsWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
            mainWindow.Device.Ip = ip1.Text + "." + ip2.Text + "." + ip3.Text + "." + ip4.Text;
            mainWindow.Device.Mask = mask1.Text + "." + mask2.Text + "." + mask3.Text + "." + mask4.Text;
            mainWindow.Device.Port = int.Parse(port.Text);
            mainWindow.Device.Address = int.Parse(address.Text);

            mainWindow.Device.Udp.getFirstData();

            optionsWindow.Visibility = Visibility.Hidden;

            
        }
        private void OptionsCancelButton_Click(object sender, RoutedEventArgs e)
        {
            optionsWindow.Visibility = Visibility.Hidden;
        }

        private void ValidationField(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
