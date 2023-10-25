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
        private MainWindow mainWindow;

        public ServiceWindow(MainWindow mainWindow)
        {
            InitializeComponent(); 
            this.mainWindow = mainWindow;
        }

        private void ServiceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            isOnline.Text = mainWindow.Device.GetOnline() ? "online" : "offline";
            ipOptionsText.Text = AppOptions.DeviceIp + ":" + AppOptions.DevicePort + "  AD="+AppOptions.DeviceAdress;
            request1.Text = AppOptions.DeviceAdress.ToString();

        }

        private void ServiceWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void RequestSendButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Device.Udp.Connect();
            //mainWindow.Device.Udp.SendCommand("123456789");
            //Byte[] sendBytes = Encoding.ASCII.GetBytes(command);

            if (mainWindow.Device.GetOnline())
            {
                isOnline.Text = "online";
                mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(AppOptions.DeviceAdress, 147, 99, 99, 99));
                //mainWindow.Device.Udp.SendCommand(PelcoDE.getCommand(AppOptions.DeviceAdress, 0, 8, 0, 1));
            }
            else
            {
                isOnline.Text = "offline";
            } 
        }

        private void ServiceCloseButton_Click(object sender, RoutedEventArgs e)
        {
            serviceWindow.Visibility = Visibility.Hidden;
        }

        internal void ShowAnswer (String answer)
        {
            answerTextBox.Text= answer;
        }
    }
}
