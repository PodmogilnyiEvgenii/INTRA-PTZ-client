using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace INTRA_PTZ_client
{
    public partial class PresetWindow : Window
    {
        private MainWindow mainWindow;
        private List<Preset.PresetTableRow> savedPresetList = new List<Preset.PresetTableRow>();

        public PresetWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.Loaded += PresetWindow_Loaded;
            this.IsVisibleChanged += PresetWindow_IsVisibleChanged;
            this.Closing += PresetWindow_Closing;
        }

        private void PresetWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void PresetWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            savedPresetList = mainWindow.Device.Preset.GetPresetList();
            presetTable.ItemsSource = mainWindow.Device.Preset.GetPresetList();
        }
        private void PresetWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void PresetSaveButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.updateTooltips();
            /*
            List<Preset.PresetTableRow> list = mainWindow.Device.Preset.GetPresetList();

            for (int i = 0; i < list.Count; i++)
            {
                System.Diagnostics.Trace.WriteLine(list[i]);
            }
            */
            presetWindow.Visibility = Visibility.Hidden;
        }
        private void PresetCancelButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Device.Preset.SetPresetList(savedPresetList);
            presetWindow.Visibility = Visibility.Hidden;
        }

        private void PresetAddButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Device.Preset.AddRow();
            presetTable.ItemsSource = mainWindow.Device.Preset.GetPresetList();
        }

        private void PresetDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Device.Preset.DeleteRow();
            presetTable.ItemsSource = mainWindow.Device.Preset.GetPresetList();
        }
    }
}
