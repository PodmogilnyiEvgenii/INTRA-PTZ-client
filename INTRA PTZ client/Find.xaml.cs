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
    public partial class FindWindow : Window
    {
        public FindWindow()
        {
            InitializeComponent();
        }
        private void FindWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void FindCloseButton_Click(object sender, RoutedEventArgs e)
        {
            findWindow.Visibility = Visibility.Hidden;
        }
    }
}
