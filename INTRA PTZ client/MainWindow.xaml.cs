using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using static INTRA_PTZ_client.Route;
using static INTRA_PTZ_client.Preset;

namespace INTRA_PTZ_client
{
    public partial class MainWindow : Window
    {
        private OptionsWindow optionsWindow;
        private RouteWindow routeWindow;
        private ServiceWindow serviceWindow;
        private PresetWindow presetWindow;
        private Device device;

        public MainWindow()
        {
            InitializeComponent();

            this.device = new Device(this);

            zoomField.IsEnabled = false;
            focusField.IsEnabled = false;

            deviceDataText.Text = device.getStatusString();

            this.Closing += MainWindow_Closing;

            this.optionsWindow = new OptionsWindow(this);
            this.routeWindow = new RouteWindow(this);
            this.serviceWindow = new ServiceWindow(this);
            this.presetWindow = new PresetWindow(this);

            DispatcherTimer refreshTimer = new DispatcherTimer();
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Interval = TimeSpan.FromMilliseconds(1000);
            refreshTimer.Start();

            //Device.Udp.getFirstData();            

            List<Route.RouteTableRow> routeList = new List<Route.RouteTableRow>();
            routeList.Add(new RouteTableRow(1, 0, 0, 0, 60));
            routeList.Add(new RouteTableRow(2, 1, 0, 0, 60));
            routeList.Add(new RouteTableRow(3, 2, 0, 0, 60));
            routeList.Add(new RouteTableRow(4, 0, 0, 0, 60));
            Device.Route.SetRouteList(routeList);

            List<Preset.PresetTableRow> presetList = new List<Preset.PresetTableRow>();
            for (int i = 1; i <= 20; i++)
            {
                presetList.Add(new PresetTableRow(i, 0, 0));
            }
            Device.Preset.SetPresetList(presetList);

            updateTooltips();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            String statusString = device.getStatusString();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            deviceDataText.Text = statusString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            ServiceWindow.deviceDataText.Text = statusString));

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                            routeWindow.deviceDataText.Text = statusString));
        }

        public OptionsWindow OptionsWindow { get => optionsWindow; set => optionsWindow = value; }
        public RouteWindow RouteWindow { get => routeWindow; set => routeWindow = value; }
        public ServiceWindow ServiceWindow { get => serviceWindow; set => serviceWindow = value; }
        public PresetWindow PresetWindow { get => presetWindow; set => presetWindow = value; }
        public Device Device { get => device; set => device = value; }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            device.Udp.Disconnect();
        }

        //menu

        private void ConfigurationLoad_Click(object sender, RoutedEventArgs e)
        {
            //menu

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ECI cfg files (*.cfg)|*.cfg";

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                using (TextReader textReader = new StreamReader(fileName))
                {
                    try
                    {
                        Device device = JsonConvert.DeserializeObject<Device>(textReader.ReadToEnd());
                        device.MainWindow = this;
                        device.Udp=new UDP(this,device);
                        this.device = device;
                        //MessageBox.Show("Конфигурация загружена!");
                    }
                    catch (IOException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Configuration load failed: " + ex.Message);
                    }
                }
            }




        }
        private void ConfigurationSave_Click(object sender, RoutedEventArgs e)
        {
            //menu
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "INTRA PTZ client cfg files (*.cfg) |*.cfg";
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;

                using (TextWriter textWriter = new StreamWriter(fileName))
                {
                    string data = JsonConvert.SerializeObject(device);

                    try
                    {
                        textWriter.Write(data);
                        //MessageBox.Show("Конфигурация сохранена!");


                    }
                    catch (IOException ex)
                    {
                        System.Diagnostics.Trace.WriteLine("Configuration save failed: " + ex.Message);
                    }

                }
            }


        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            //menu
            MessageBox.Show("Здесь могла бы быть ваша реклама!");
            //System.Windows.Application.Current.Shutdown();
        }

        //panel 1

        private void PresetButton_Click(object sender, RoutedEventArgs e)
        {
            PresetWindow.Visibility = Visibility.Visible;
            PresetWindow.Owner = this;
            PresetWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PresetWindow.ShowDialog();
        }
        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow.Visibility = Visibility.Visible;
            OptionsWindow.Owner = this;
            OptionsWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            OptionsWindow.ShowDialog();
        }
        private void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            RouteWindow.Visibility = Visibility.Visible;
            RouteWindow.Owner = this;
            RouteWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            RouteWindow.ShowDialog();
        }
        private void ServiceButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceWindow.Visibility = Visibility.Visible;
            ServiceWindow.Owner = this;
            ServiceWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ServiceWindow.ShowDialog();
        }

        //manual mode   

        private void SetCoordinatesButton_Click(object sender, RoutedEventArgs e)
        {
            float panValue = -1f;
            float tiltValue = -1f;

            try
            {
                panValue = float.Parse(panField.Text);
            }
            catch { }

            if (panValue > device.MaxPan || panValue < device.MinPan) panField.Text = device.GetCurrentPan().ToString();

            try
            {
                tiltValue = float.Parse(tiltField.Text);
            }
            catch { }

            if (tiltValue > device.MaxTilt || tiltValue < 0) tiltField.Text = device.GetCurrentTilt().ToString();

            byte[] pan = BitConverter.GetBytes(Device.panAngleToStep(panField.Text));
            byte[] tilt = BitConverter.GetBytes(Device.tiltAngleToStep(tiltField.Text));

            //byte[] zoom = BitConverter.GetBytes(Device.panAngleToStep(zoomField.Text));
            //byte[] focus = BitConverter.GetBytes(Device.panAngleToStep(focusField.Text));


            if (AppOptions.DEBUG && device.CurrentStepPan == Device.panAngleToStep(panField.Text) && device.CurrentStepTilt == Device.tiltAngleToStep(panField.Text))
            {
                System.Diagnostics.Trace.WriteLine("Same coordinates | pan= " +
                Device.panAngleToStep(panField.Text)
                + " tilt= " +
                Device.tiltAngleToStep(tiltField.Text)
                );
            }

            List<UdpCommand> list = new List<UdpCommand>();
            if (device.CurrentStepPan != Device.panAngleToStep(panField.Text))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (device.CurrentStepTilt != Device.tiltAngleToStep(tiltField.Text))
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }

            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setZoom"), tilt[1], tilt[0]), "Zoom", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setFocus"), tilt[1], tilt[0]), "Focus", AppOptions.UDP_TIMEOUT_SHORT));

            Device.Udp.UdpServices.addTaskToEnd(list);
        }
        private void ManualControlUI(double speed, int dPan, int dTilt)
        {
            List<UdpCommand> list = new List<UdpCommand>();
            if (dPan != 0)
            {
                int resultPan = Device.CurrentStepPan + dPan * AppOptions.SPEED_STEP_PAN[(int)speed];
                if (resultPan < 0) resultPan += Device.GetMaxStepPan();
                if (resultPan > Device.GetMaxStepPan()) resultPan -= Device.GetMaxStepPan();

                byte[] pan = BitConverter.GetBytes(resultPan);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (dTilt != 0)
            {
                int resultTilt = Device.CurrentStepTilt + dTilt * AppOptions.SPEED_STEP_TILT[(int)speed];
                if (resultTilt < 0) resultTilt += Device.GetMaxStepTilt();
                if (resultTilt > Device.GetMaxStepTilt()) resultTilt -= Device.GetMaxStepTilt();

                byte[] tilt = BitConverter.GetBytes(resultTilt);

                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }
            Device.Udp.UdpServices.addTaskToEnd(list);
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 1);
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, 1);
        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 1);
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, 0);
        }
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (device.CurrentStepPan != 0 && device.CurrentStepTilt != 0)
            {
                List<UdpCommand> list = new List<UdpCommand>();
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("makeTest"), 0x00, 0x00), "Done", AppOptions.UDP_TIMEOUT_LONG));
            }
        }
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, 0);
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, -1, -1);
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 0, -1);
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            ManualControlUI(speedSleder.Value, 1, -1);
        }

        private void SpeedPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value < 7) speedSleder.Value = speedSleder.Value + 1;
        }
        private void SpeedMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (speedSleder.Value > 0) speedSleder.Value = speedSleder.Value - 1;
        }

        private void ValidationPanField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            /*
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);*/

            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationTiltField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[-0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationZoomField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }
        private void ValidationFocusField(object sender, TextCompositionEventArgs e)
        {
            //TODO validate min/max
            Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;

        }

        //status bar
        private void Hyperlink_OpenWebConsole(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(/*e.Uri.AbsoluteUri*/"http://" + device.Ip) { UseShellExecute = true });
            e.Handled = true;
        }

        private void moveToPreset(int presetNumber)
        {
            float panValue = Device.Preset.GetPresetList()[presetNumber].Pan;
            float tiltValue = Device.Preset.GetPresetList()[presetNumber].Tilt;

            if (panValue > device.MaxPan || panValue < device.MinPan) Device.Preset.GetPresetList()[presetNumber].Pan = Device.GetCurrentPan();
            if (tiltValue > device.MaxTilt || tiltValue < 0) Device.Preset.GetPresetList()[presetNumber].Tilt = Device.GetCurrentTilt();

            byte[] pan = BitConverter.GetBytes(Device.Preset.GetPresetList()[presetNumber].Pan);
            byte[] tilt = BitConverter.GetBytes(Device.Preset.GetPresetList()[presetNumber].Tilt);

            //byte[] zoom = BitConverter.GetBytes(Device.panAngleToStep(zoomField.Text));
            //byte[] focus = BitConverter.GetBytes(Device.panAngleToStep(focusField.Text));


            if (AppOptions.DEBUG && Device.Preset.GetPresetList()[presetNumber].Pan == Device.GetCurrentPan() && Device.Preset.GetPresetList()[presetNumber].Tilt == Device.GetCurrentTilt())
            {
                System.Diagnostics.Trace.WriteLine("Same coordinates | pan= " +
                Device.panAngleToStep(panField.Text)
                + " tilt= " +
                Device.tiltAngleToStep(tiltField.Text)
                );
            }

            List<UdpCommand> list = new List<UdpCommand>();
            if (Device.Preset.GetPresetList()[presetNumber].Pan != Device.GetCurrentPan())
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setPan"), pan[1], pan[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getPan"), 0x00, 0x00), "Pan", AppOptions.UDP_TIMEOUT_SHORT));
            }

            if (Device.Preset.GetPresetList()[presetNumber].Tilt == Device.GetCurrentTilt())
            {
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setTilt"), tilt[1], tilt[0]), "Done", AppOptions.UDP_TIMEOUT_LONG));
                list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("getTilt"), 0x00, 0x00), "Tilt", AppOptions.UDP_TIMEOUT_SHORT));
            }

            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setZoom"), tilt[1], tilt[0]), "Zoom", AppOptions.UDP_TIMEOUT_SHORT));
            //list.Add(new UdpCommand(PelcoDE.getCommand(Device.Address, 0x00, PelcoDE.getByteCommand("setFocus"), tilt[1], tilt[0]), "Focus", AppOptions.UDP_TIMEOUT_SHORT));

            Device.Udp.UdpServices.addTaskToEnd(list);
        }

        public void updateTooltips()
        {
            List<Button> list = new List<Button>();
            list.Add(preset1);
            list.Add(preset2);
            list.Add(preset3);
            list.Add(preset4);
            list.Add(preset5);
            list.Add(preset6);
            list.Add(preset7);
            list.Add(preset8);
            list.Add(preset9);
            list.Add(preset10);

            for (int i = 0; i < list.Count; i++)
            {
                ToolTip toolTip = new ToolTip();
                toolTip.Content = Device.Preset.getTooltipDesc(i);
                list[i].ToolTip = toolTip;
            }
        }


        private void Preset1_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(0);
        }

        private void Preset2_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(1);
        }

        private void Preset3_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(2);
        }

        private void Preset4_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(3);
        }

        private void Preset5_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(4);
        }

        private void Preset6_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(5);
        }

        private void Preset7_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(6);
        }

        private void Preset8_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(7);
        }

        private void Preset9_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(8);
        }

        private void Preset10_Click(object sender, RoutedEventArgs e)
        {
            moveToPreset(9);
        }


    }
}
