using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace INTRA_PTZ_client
{
    public partial class PresetWindow : Window
    {
        private Device device;
        private List<Preset.PresetTableRow> savedPresetList = new List<Preset.PresetTableRow>();

        public PresetWindow(Device device)
        {
            InitializeComponent();
            this.device = device;
            this.Loaded += PresetWindow_Loaded;
            this.IsVisibleChanged += PresetWindow_IsVisibleChanged;
            this.Closing += PresetWindow_Closing;
        }

        private void PresetWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void PresetWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            savedPresetList = device.Preset.GetPresetList();
            presetTable.ItemsSource = device.Preset.GetPresetList();
        }
        private void PresetWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void PresetSaveButton_Click(object sender, RoutedEventArgs e)
        {
            device.MainWindow.updateTooltips();
            /*
            List<Preset.PresetTableRow> list = device.Preset.GetPresetList();

            for (int i = 0; i < list.Count; i++)
            {
                System.Diagnostics.Trace.WriteLine(list[i]);
            }
            */
            presetWindow.Visibility = Visibility.Hidden;
        }
        private void PresetCancelButton_Click(object sender, RoutedEventArgs e)
        {
            device.Preset.SetPresetList(savedPresetList);
            presetWindow.Visibility = Visibility.Hidden;
        }

        private void PresetAddButton_Click(object sender, RoutedEventArgs e)
        {
            device.Preset.AddRow();
            presetTable.ItemsSource = device.Preset.GetPresetList();
        }

        private void PresetDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            device.Preset.DeleteRow();
            presetTable.ItemsSource = device.Preset.GetPresetList();
        }

        private void TableMenuMoveToPreset_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine(presetTable.SelectedIndex);
            device.MainWindow.MoveToPreset(presetTable.SelectedIndex);
        }
    }
}
